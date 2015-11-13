/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/11/12
 * 时间: 11:24
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Net;
using System.Collections.Generic;
using AsyncServer;

namespace YGOCore
{
	/// <summary>
	/// 房间服务端
	/// </summary>
	public class RoomServer
	{
		#region member
		public bool IsListening{get;private set;}
		public readonly List<Server> Servers=new List<Server>();
		public readonly List<Session> Clients=new List<Session>();
		private AsyncTcpListener<Session> m_listener;
		private string[] Configs;
		private string ServerExe;
		private int Port;
		public RoomServer(int port=18910,string serverExe="GameServer.exe",string[] configs=null)
		{
			Port = port;
			ServerExe = serverExe;
			Configs = configs;
		}
		#endregion
		
		#region start/stop
		public bool Start(){
			if(IsListening) return true;
			IsListening = true;
			if(Configs!=null){
				foreach(string config in Configs){
					Server server=new Server(ServerExe, config);
					server.OnPlayerJoin += new OnPlayerJoinEvent(this.server_OnPlayerJoin);
					server.OnPlayerLeave+=new OnPlayerLeaveEvent(this.server_OnPlayerLeave);
					server.OnRoomClose+=new OnRoomCloseEvent(this.server_OnRoomClose);
					server.OnRoomCreate+=new OnRoomCreateEvent(this.server_OnRoomCreate);
					server.OnRoomStart+=new OnRoomStartEvent(this.server_OnRoomStart);
					server.OnServerInfo+=new OnServerInfoEvent(this.server_OnServerInfo);
					Servers.Add(server);
					server.Start();
				}
			}else{
				Logger.Error("no configs");
			}
			try{
				if(m_listener == null){
					m_listener = new AsyncTcpListener<Session>(IPAddress.Any, Port);
					m_listener.OnConnect +=new AsyncTcpListener<Session>.ConnectEventHandler(Listener_OnConnect);
					m_listener.OnDisconnect +=new AsyncTcpListener<Session>.DisconnectEventHandler(Listener_OnDisconnect);
					m_listener.OnReceive += new AsyncTcpListener<Session>.ReceiveEventHandler(Listener_OnReceive);
				}
				m_listener.Start();
			}catch(Exception e){
				Logger.Error(e);
				Stop();
			}
			return IsListening;
		}
		public void Stop(){
			//Server.Close();
			if(!IsListening) return;
			IsListening = false;
			lock(Servers){
				foreach(Server server in Servers){
					server.Close();
				}
			}
		}
		#endregion
		
		#region listener
		void Listener_OnDisconnect(Connection<Session> Client)
		{
			if(Client.Tag!=null){
				lock(Clients){
					Clients.Remove(Client.Tag);
				}
				Client.Tag.Close();
				Client.Tag = null;
			}
		}
		void Listener_OnReceive(Connection<Session> Client)
		{
			if(Client.Tag!=null){
				Client.Tag.OnRecevice();
			}
		}
		
		void Listener_OnConnect(Connection<Session> Client)
		{
			Session session= new Session(Client);
			lock(Clients){
				Clients.Add(session);
			}
		}
		#endregion

	}
}
