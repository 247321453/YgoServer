/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/10/31
 * 时间: 15:45
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Net;
using System.Collections.Generic;
using AsyncServer;
using OcgWrapper;
using System.Net.Sockets;
using YGOCore.Game;
using System.IO;
using System.Threading;

namespace YGOCore.Net
{
	public class GameServer
	{
		#region member
		private AsyncTcpListener<GameSession> m_listener;
		private static List<string> banNames=new List<string>();
		public ServerConfig Config{get;private set;}
		public readonly SortedList<string, RoomInfo> Rooms = new SortedList<string, RoomInfo>(32);
		public readonly Queue<WinInfo> WinInfos=new Queue<WinInfo>();
		private System.Timers.Timer WinSaveTimer;
		private RoomServer roomServer;
		private TcpClient roomClient;
		public TcpClient LocalClient {get{return roomClient;}}
		public bool IsListening;
		#endregion
		
		public GameServer(ServerConfig config)
		{
			Config = config;
		}
		
		#region socket
		public bool Start()
		{
			try
			{
				Api.Init(Config.Path, Config.ScriptFolder, Config.CardCDB);
				BanlistManager.Init(Config.BanlistFile);
				MsgSystem.Init(Config.File_ServerMsgs);
				WinInfo.Init(Config.WinDbName);
				ReadBanNames();
				if(Config.RecordWin){
					SatrtWinTimer();
				}
				m_listener = new AsyncTcpListener<GameSession>(IPAddress.Any, Config.ServerPort);
				m_listener.OnConnect    += new AsyncTcpListener<GameSession>.ConnectEventHandler(Listener_OnConnect);
				m_listener.OnReceive    += new AsyncTcpListener<GameSession>.ReceiveEventHandler(Listener_OnReceive);
				m_listener.OnTimeout    += new AsyncTcpListener<GameSession>.TimeoutEventHandler(Listener_OnTimeout);
				m_listener.OnDisconnect += new AsyncTcpListener<GameSession>.DisconnectEventHandler(Listener_OnDisconnect);
				m_listener.Start();
				if(Config.ApiIsLocal){
					if(roomClient==null){
						roomClient = new TcpClient();
						try{
							roomClient.Client.Connect(IPAddress.Parse("127.0.0.1"), Config.ApiPort);
						}catch(Exception){
						}
					}
				}else{
					if(roomServer==null){
						roomServer = new RoomServer(this, Config.ApiPort);
					}
					roomServer.Start();
				}
				IsListening = true;
			}
			catch (SocketException)
			{
				Logger.Error("The " + Config.ServerPort + " port is currently in use.");
				return false;
			}
			catch (Exception e)
			{
				Logger.Error(e);
				return false;
			}
			Logger.Info("Listening on port " + Config.ServerPort);
			return true;
		}
		/// <summary>
		/// 停止
		/// </summary>
		public void Stop(){
			if(IsListening){
				IsListening = false;
				if(roomClient != null ){
					try{
						if(roomClient.Connected){
							roomClient.Close();
						}
					}catch(Exception){
						
					}
				}
				m_listener.Stop();
				if(Config.RecordWin){
					WinSaveTimer.Close();
				}
				WinSaveTimer_Elapsed(null, null);
			}
		}
		#endregion
		
		#region listener
		void Listener_OnTimeout(Connection<GameSession> Client, double time)
		{
			if(Client!=null){
				if(Client.Tag!=null){
					Client.Tag.Close();
				}
				m_listener.DisconnectClient(Client);
			}
		}

		void Listener_OnDisconnect(Connection<GameSession> Client)
		{
			if(Client!=null){
				if(Client.Tag!=null){
					Client.Tag.Close();
				}
				m_listener.DisconnectClient(Client);
			}
		}
		void Listener_OnReceive(Connection<GameSession> Client)
		{
			if(Client!=null && Client.Tag!=null){
				//处理接收数据
				ThreadPool.QueueUserWorkItem(Client.Tag.OnReceive);
			}
		}
		
		void Listener_OnConnect(Connection<GameSession> Client)
		{
			if(Client!=null){
				//绑定关系
				GameSession session = new GameSession(this, Client);
				Client.Tag = session;
			}
		}
		#endregion
		
		#region public method
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns>false=被禁止</returns>
		public bool CheckPlayerBan(string name){
			if(string.IsNullOrEmpty(name)){
				return false;
			}
			name = name.Split('$')[0];
			if(Config.BanMode==0){
				return true;
			}
			else if(Config.BanMode==1){
				return !banNames.Contains(name);
			}
			else{
				return banNames.Contains(name);
			}
		}
		#endregion
		
		#region private method
		private void ReadBanNames()
		{
			if(File.Exists(Config.File_BanAccont)){
				string[] lines = File.ReadAllLines(Config.File_BanAccont);
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
		#endregion
		
		#region 比赛记录
		private void SatrtWinTimer(){
			//30s保存一次结果
			WinSaveTimer = new System.Timers.Timer(30*1000);
			WinSaveTimer.AutoReset=true;
			WinSaveTimer.Enabled=true;
			WinSaveTimer.Elapsed+=new System.Timers.ElapsedEventHandler(WinSaveTimer_Elapsed);
			WinSaveTimer.Start();
		}
		private void WinSaveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if(!Config.RecordWin){
				return;
			}
			string[] sqls=null;
			lock(WinInfos){
				if(WinInfos.Count==0) return;
				sqls=new string[WinInfos.Count];
				int i=0;
				foreach(WinInfo info in WinInfos){
					sqls[i++]=info.GetSQL();
				}
			}
			ThreadPool.QueueUserWorkItem(new WaitCallback(
				delegate(object obj)
				{
					//string[] sqls = (string[])obj;
					SQLiteTool.Command(Config.WinDbName, sqls);
					Logger.Debug("save wins record:"+sqls.Length);
				}
			));
		}
		#endregion
	}
}
