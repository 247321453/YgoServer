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
using OcgWrapper.Enums;
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
		public ServerConfig Config{get;private set;}
		public bool IsListening{get; private set;}
		public bool UseApi{get; private set;}
		#endregion
		
		public GameServer(ServerConfig config)
		{
			Config = config;
		}
		
		#region socket
		public bool Start()
		{
			if(IsListening) return false;
			try
			{
				string script = Tool.Combine(Config.Path, "script");
				string cdb = Tool.Combine(Config.Path, "cards.cdb");
				string windb = Tool.Combine(Config.Path, "win.db");
				string lflist= Tool.Combine(Config.Path, "lflist.conf");
				string namelist = Tool.Combine(Program.Config.Path, "namelist.txt");
				string msgfile = Tool.Combine(Program.Config.Path, "server_msg.txt");
				//	Logger.Debug("script:"+script);
				//	Logger.Debug("windb:"+windb);
				//	Logger.Debug("lflist:"+lflist);
				Api.Init(Config.Path, script, cdb);
				BanlistManager.Init(lflist);
				WinInfo.Init(windb);
				RoomManager.init(namelist);
				Messages.Init(msgfile);
				if(Config.ApiPort > 0){
					Logger.Info("Connecting api server.");
					if(!ServerApi.Init(Config.ApiPort)){
						Logger.Warn("connect api server fail.");
					}else{
						Logger.Info("Connect api server ok");
					}
				}
				m_listener = new AsyncTcpListener<GameSession>(IPAddress.Any, Config.ServerPort);
				m_listener.OnConnect    += new AsyncTcpListener<GameSession>.ConnectEventHandler(Listener_OnConnect);
				m_listener.OnReceive    += new AsyncTcpListener<GameSession>.ReceiveEventHandler(Listener_OnReceive);
				m_listener.OnDisconnect += new AsyncTcpListener<GameSession>.DisconnectEventHandler(Listener_OnDisconnect);
				m_listener.Start();
				ServerApi.OnServerInfo(this);
				IsListening = true;
			}
			catch (SocketException)
			{
				Logger.Warn("The " + Config.ServerPort + " port is currently in use.");
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
				m_listener.Stop();
			}
		}
		#endregion
		
		#region listener

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
				Client.Tag.OnReceive(null);
				//ThreadPool.QueueUserWorkItem(Client.Tag.OnReceive);
			}
		}
		
		void Listener_OnConnect(Connection<GameSession> Client)
		{
			if(Client!=null){
				//绑定关系
				GameSession session = new GameSession(Client, Config.ClientVersion,Config.Timeout);
				Client.isAsync = Config.AsyncMode;
				Client.Tag = session;
			}
		}
		#endregion
		
	}
}
