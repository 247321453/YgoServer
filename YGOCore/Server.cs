using System;
using System.Net;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using YGOCore.Game;
using OcgWrapper;

namespace YGOCore
{
	public class Server
	{
		public bool IsListening { get; private set; }
		private TcpListener m_listener;
		private List<GameClient> m_clients;
		private Mutex m_mutexClients;
		
		private static List<WinInfo> WinInfos=new List<WinInfo>();
		private static Mutex MutexWinInfo=new Mutex();
		
		private bool isClosing=false;
		/// <summary>
		/// 定时保存游戏记录
		/// </summary>
		private System.Timers.Timer WinSaveTimer;
		private System.Timers.Timer CloseTimer;
		private ApiHttpServer ApiServer;
		
		public Server()
		{
			m_clients = new List<GameClient>();
			m_mutexClients=new Mutex();
			int time=Program.Config.SaveRecordTime<=0?1*60*1000:Program.Config.SaveRecordTime*60*1000;
			WinSaveTimer = new System.Timers.Timer(time);
			WinSaveTimer.AutoReset=true;
			WinSaveTimer.Enabled=true;
			WinSaveTimer.Elapsed+=new System.Timers.ElapsedEventHandler(WinSaveTimer_Elapsed);
			ApiServer=new ApiHttpServer(this, "127.0.0.1", Program.Config.ApiPort);
		}
		
		public string getRoomJson(){
			List<RoomInfo> list=GameManager.getRoomInfos();
			return Tool.ToJson(list);
//			string json="[";
//			foreach(RoomInfo info in list){
//				json+=info.toJson()+",";
//			}
//			if(list.Count>0){
//				json=json.Substring(0,json.Length-1);
//			}
//			json+="]";
//			return json;
		}
		public int getRoomCount(){
			return GameManager.getRoomInfos().Count;
		}
		
		public int getPlayerCount(){
			List<RoomInfo> list=GameManager.getRoomInfos();
			int count=0;
			foreach(RoomInfo info in list){
				count+=info.Count;
			}
			return count;
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
				SQLiteTool.Command(Program.Config.WinDbName, sqls);
				Logger.WriteLine("save wins record:"+tmpList.Count);
			}
		}

		public bool Start(int port = 0)
		{
			try
			{
				Api.Init(Program.Config.Path, Program.Config.ScriptFolder, Program.Config.CardCDB);
				BanlistManager.Init(Program.Config.BanlistFile);
				//DecksManager.Init();
				SQLiteTool.Init(Program.Config.WinDbName, WinInfo.SQL_Table);
				try{
					Directory.CreateDirectory(Program.Config.replayFolder);
				}catch(IOException){
					
				}
				m_listener = new TcpListener(IPAddress.Any, port == 0 ? Program.Config.ServerPort : port);
				Thread acceptThread=new Thread(new ThreadStart(AcceptClient));
				acceptThread.IsBackground=true;
				acceptThread.Start();
				WinSaveTimer.Start();
				ApiServer.Start();
				IsListening = true;
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
		
		private void AcceptClient(){
			m_listener.Start();
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
			List<GameClient> toRemove = new List<GameClient>();
			List<GameClient> _clients= new List<GameClient>();
			m_mutexClients.WaitOne();
			_clients.AddRange(m_clients);
			m_mutexClients.ReleaseMutex();
			foreach (GameClient client in _clients)
			{
				client.Tick();
				if (!client.IsConnected || client.InGame()){
					//断开，或者在游戏
					toRemove.Add(client);
				}
			}
			m_mutexClients.WaitOne();
			while (toRemove.Count > 0)
			{
				m_clients.Remove(toRemove[0]);
				toRemove.RemoveAt(0);
			}
			m_mutexClients.ReleaseMutex();
		}
		
		public static bool onLogin(string name,string pass){
			//Logger.WriteLine(name+"$"+pass+" is login");
			try{
				string dpass=Tool.GetMd5(pass);
				string result=Tool.PostHtmlContentByUrl(Program.Config.LoginUrl,
				                                        "username="+name.Replace("&","")+"&password="+dpass,
				                                        10*1000
				                                       );
				if(result.IndexOf("-")<0){
					return true;
				}
			}catch(Exception){
				
			}
			return false;
		}
		
		public static void onWin(string room,int win,int reason,string replay,string pl0,string pl1,string pl2,string pl3,bool force){
			MutexWinInfo.WaitOne();
			WinInfos.Add(new WinInfo(room, win, reason,replay, pl0,pl1,pl2,pl3,force));
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
			GameManager.SendWarringMessage("Server will close after 5 minute.");
			GameManager.SendErrorMessage("Server will close after 5 minute.");
			if(CloseTimer==null){
				CloseTimer = new System.Timers.Timer(5*60*1000);
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
