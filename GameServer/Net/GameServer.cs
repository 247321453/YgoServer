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
				Api.Init(Config.Path, Config.ScriptFolder, Config.CardCDB);
				BanlistManager.Init(Config.BanlistFile);
				MsgSystem.Init(Config.File_ServerMsgs);
				WinInfo.Init(Config.WinDbName);
				RoomManager.init();
				m_listener = new AsyncTcpListener<GameSession>(IPAddress.Any, Config.ServerPort);
				m_listener.OnConnect    += new AsyncTcpListener<GameSession>.ConnectEventHandler(Listener_OnConnect);
				m_listener.OnReceive    += new AsyncTcpListener<GameSession>.ReceiveEventHandler(Listener_OnReceive);
				m_listener.OnDisconnect += new AsyncTcpListener<GameSession>.DisconnectEventHandler(Listener_OnDisconnect);
				m_listener.Start();
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
				ThreadPool.QueueUserWorkItem(Client.Tag.OnReceive);
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
