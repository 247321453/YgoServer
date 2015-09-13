using System;
using System.Net;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using YGOCore.Game;
using OcgWrapper;
using OcgWrapper.Enums;
using YGOCore.Plugin;

namespace YGOCore
{
	public class Server
	{
		public bool IsListening { get; private set; }
		private TcpListener m_listener;
		private List<GameClient> m_clients;
		private Mutex m_mutexClients;
		private static List<string> banNames=new List<string>();
		
		private static List<WinInfo> WinInfos=new List<WinInfo>();
		private static Mutex MutexWinInfo=new Mutex();
		
		private bool isClosing=false;
		/// <summary>
		/// 定时保存游戏记录
		/// </summary>
		private System.Timers.Timer WinSaveTimer;
		private System.Timers.Timer CloseTimer;
		private System.Timers.Timer ServerMsgTimer;
		private MyHttpServer ApiServer;
		delegate void ThreadDelagate(object obj);
		public Server()
		{
			m_clients = new List<GameClient>();
			m_mutexClients=new Mutex();
			//int time=Program.Config.SaveRecordTime<=0?1*60*1000:Program.Config.SaveRecordTime*60*1000;
			//每30秒记录游戏结果记录
			WinSaveTimer = new System.Timers.Timer(15*1000);
			WinSaveTimer.AutoReset=true;
			WinSaveTimer.Enabled=true;
			WinSaveTimer.Elapsed+=new System.Timers.ElapsedEventHandler(WinSaveTimer_Elapsed);
			ServerMsgTimer= new System.Timers.Timer(15*60*1000);
			ServerMsgTimer.AutoReset=true;
			ServerMsgTimer.Enabled=true;
			ServerMsgTimer.Elapsed+=new System.Timers.ElapsedEventHandler(ServerMsgTimer_Elapsed);
			
			ApiServer=new MyHttpServer(this, Program.Config.ApiPort);
		}
		
		public string getRoomJson(bool hasLock=true,bool hasStart=false){
			List<RoomInfo> list=GameManager.getRoomInfos(hasLock, hasStart);
			return Tool.ToJson(list);
		}
		public int getRoomCount(){
			return GameManager.getRoomInfos(true, true).Count;
		}
		
		public int getPlayerCount(){
			List<RoomInfo> list=GameManager.getRoomInfos();
			int count=0;
			foreach(RoomInfo info in list){
				count+=info.Count;
			}
			return count;
		}
		private void ServerMsgTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if(MsgSystem.Msgs == null || MsgSystem.Msgs.Count<=1){
				return;
			}
			ThreadPool.QueueUserWorkItem(new WaitCallback(
				delegate(object o)
				{
					Say(GameManager.getMessage(MsgSystem.getNextMessage(), PlayerType.Yellow));
				}
			));
			
		}
		private void WinSaveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if(!Program.Config.RecordWin){
				return;
			}
			List<WinInfo> tmpList=new List<WinInfo>();
			MutexWinInfo.WaitOne();
			tmpList.AddRange(WinInfos);
			WinInfos.Clear();
			MutexWinInfo.ReleaseMutex();
			int count=tmpList.Count;
			if(count>0){
				string[] sqls=new string[count];
				for(int i=0;i<count;i++){
					sqls[i]=tmpList[i].getSQL();
				}
				ThreadPool.QueueUserWorkItem(new WaitCallback(
					delegate(object obj)
					{
						//string[] sqls = (string[])obj;
						SQLiteTool.Command(Program.Config.WinDbName, sqls);
						Logger.WriteLine("save wins record:"+sqls.Length);
					}
				));
			}
		}
		
		public void Reload(){
			Program.Config.Load();
			Api.Init(Program.Config.Path, Program.Config.ScriptFolder, Program.Config.CardCDB);
			BanlistManager.Init(Program.Config.BanlistFile);
			MsgSystem.Init(Program.Config.File_ServerMsgs);
			WinInfo.Init(Program.Config.WinDbName);
			banNames.Clear();
			if(File.Exists(Program.Config.File_BanAccont)){
				string[] lines = File.ReadAllLines(Program.Config.File_BanAccont);
				foreach(string line in lines){
					if(string.IsNullOrEmpty(line)||line.StartsWith("#")){
						continue;
					}
					string name=line.Trim();
					if(!banNames.Contains(name)){
						banNames.Add(name);
					}
				}
			}
		}
		public bool Start(int port = 0)
		{
			try
			{
				Api.Init(Program.Config.Path, Program.Config.ScriptFolder, Program.Config.CardCDB);
				BanlistManager.Init(Program.Config.BanlistFile);
				//DecksManager.Init();
				MsgSystem.Init(Program.Config.File_ServerMsgs);
				WinInfo.Init(Program.Config.WinDbName);
				if(File.Exists(Program.Config.File_BanAccont)){
					string[] lines = File.ReadAllLines(Program.Config.File_BanAccont);
					foreach(string line in lines){
						if(string.IsNullOrEmpty(line)||line.StartsWith("#")){
							continue;
						}
						string name=line.Trim();
						if(!banNames.Contains(name)){
							banNames.Add(name);
						}
					}
				}
				try{
					Directory.CreateDirectory(Program.Config.replayFolder);
				}catch(IOException){
					
				}
				IsListening = true;
				m_listener = new TcpListener(IPAddress.Any, port == 0 ? Program.Config.ServerPort : port);
				m_listener.Start();
				Thread acceptThread=new Thread(new ThreadStart(AcceptClient));
				acceptThread.IsBackground=true;
				acceptThread.Start();
				WinSaveTimer.Start();
				ApiServer.Start();
				
			}
			catch (SocketException)
			{
				Logger.WriteError("The " + (port == 0 ? Program.Config.ServerPort : port) + " port is currently in use.");
				return false;
			}
			catch (Exception e)
			{
				Logger.WriteError(e);
				return false;
			}

			Logger.WriteLine("Listening on port " + (port == 0 ? Program.Config.ServerPort : port));
			if(BanlistManager.Banlists!=null && BanlistManager.Banlists.Count>0){
				Logger.WriteLine("Banlist = "+BanlistManager.Banlists[0].Name);
			}
			return true;
		}

		public void Stop()
		{
			if (IsListening)
			{
				m_listener.Stop();
				IsListening = false;

				foreach (GameClient client in m_clients)
					client.Close();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns>false=被禁止</returns>
		public static bool CheckPlayerBan(string name){
			if(string.IsNullOrEmpty(name)){
				return false;
			}
			name = name.Split('$')[0];
			if(Program.Config.BanMode==0){
				return true;
			}
			else if(Program.Config.BanMode==1){
				return !banNames.Contains(name);
			}
			else{
				return banNames.Contains(name);
			}
		}
		
		private void AcceptClient(){
			while(IsListening){
				GameClient client=null;
				try{
					TcpClient tcpClient=m_listener.AcceptTcpClient();
					client=new GameClient(tcpClient);
				}catch(Exception){
					client=null;
				}
				m_mutexClients.WaitOne();
				m_clients.Add(client);
				m_mutexClients.ReleaseMutex();
				Thread.Sleep(1);
			}
		}
		
		public void Process()
		{
			//在游戏的，则以房间为单位
			//接收到 游戏房间内任意一个玩家的发送内容，则在房间内传播
			GameManager.HandleRooms();

			//大量玩家连接服务端，可能耗时
			//while (IsListening && m_listener.Pending())
			//	m_clients.Add(new GameClient(m_listener.AcceptTcpClient()));

			//不在游戏房间的玩家
			List<GameClient> _clients= new List<GameClient>();
			m_mutexClients.WaitOne();
			_clients.AddRange(m_clients);
			m_mutexClients.ReleaseMutex();
			foreach (GameClient client in _clients)
			{
				client.Tick();
				if (!client.IsConnected || client.InGame()){
					//断开，或者在游戏,一定时间内没有加入房间
					//	Logger.WriteLine("player logout");
					m_mutexClients.WaitOne();
					m_clients.Remove(client);
					m_mutexClients.ReleaseMutex();
				}
			}
		}
		
		public static int onLogin(string name,string pass){
			int uid=-1;
			//Logger.WriteLine(name+"$"+pass+" is login");
			try{
				string dpass=Tool.GetMd5(pass);
				if(name.StartsWith("[AI]")){
					return (dpass==Tool.GetMd5(Program.Config.AIPass))?1:-1;
				}else{
					string result=Tool.PostHtmlContentByUrl(Program.Config.LoginUrl,
					                                        "username="+name.Replace("&","")+"&password="+dpass,
					                                        10*1000
					                                       );
					uid=int.Parse(result.Trim().Replace("\"",""));
				}
			}catch(Exception){
				
			}
			return uid;
		}
		
		public List<string> Say(GameServerPacket msg, string name=null){
			if(msg==null){
				return new List<string>();
			}
			List<string> names=GameManager.SendMessage(msg, name);
			List<GameClient> _clients= new List<GameClient>();
			m_mutexClients.WaitOne();
			_clients.AddRange(m_clients);
			m_mutexClients.ReleaseMutex();
			foreach (GameClient client in _clients)
			{
				if(client!=null && client.Player!=null){
					if(!names.Contains(client.Player.Name)){
						if(!string.IsNullOrEmpty(name)){
							if(client.Player.Name != name){
								continue;
							}
						}
						names.Add(client.Player.Name);
						client.Player.Send(msg);
					}
				}
			}
			return names;
		}
		
		public static void onLogout(string name){
			
		}
		public static void onWin(string room,int mode, int win,int reason,string replay,string[] names, int[] uids,bool force){
			MutexWinInfo.WaitOne();
			WinInfos.Add(new WinInfo(room, win, reason,replay, names, uids, force));
			MutexWinInfo.ReleaseMutex();
		}
		
		public void CacelCloseDealyed(){
			isClosing=false;
			CloseTimer.Stop();
		}
		
		public void CloseDealyed(){
			if(isClosing){
				return;
			}
			isClosing=true;
			//TODO 延时关闭
			GameManager.SendWarringMessage(Messages.MSG_CLOSE);
			GameManager.SendErrorMessage(Messages.MSG_CLOSE);
			if(CloseTimer==null){
				CloseTimer = new System.Timers.Timer(3*60*1000);
				CloseTimer.AutoReset=false;
				CloseTimer.Enabled=true;
				CloseTimer.Elapsed+=new System.Timers.ElapsedEventHandler(CloseTimer_Elapsed);
			}
			CloseTimer.Start();
		}
		
		private void CloseTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if(isClosing){
				Stop();
			}
		}

	}
}
