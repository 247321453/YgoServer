/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/11/5
 * 时间: 17:01
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

using AsyncServer;

namespace YGOCore
{
	/// <summary>
	/// Description of Server.
	/// </summary>
	public class RoomServer
	{
		AsyncTcpListener<Server> ServerListener;
		AsyncTcpListener<Session> ClientListener;
		public bool isStarted = false;
		readonly List<Connection<Server>> Servers=new List<Connection<Server>>();
		readonly List<Connection<Session>> Clients=new List<Connection<Session>>();
		
		public RoomServer(int localPort=18910, int port=9910)
		{
			ServerListener = new AsyncTcpListener<Server>(IPAddress.Parse("127.0.0.1"), localPort);
			ServerListener.OnConnect += new AsyncTcpListener<Server>.ConnectEventHandler(ServerListener_OnConnect);
			ServerListener.OnDisconnect+=new AsyncTcpListener<Server>.DisconnectEventHandler(ServerListener_OnDisconnect);
			ServerListener.OnReceive+=new AsyncTcpListener<Server>.ReceiveEventHandler(ServerListener_OnReceive);
			ServerListener.OnTimeout+=new AsyncTcpListener<Server>.TimeoutEventHandler(ServerListener_OnTimeout);
			
			ClientListener = new AsyncTcpListener<Session>(IPAddress.Any, port);
			ClientListener.OnConnect += new AsyncTcpListener<Session>.ConnectEventHandler(Client_OnConnect);
			ClientListener.OnDisconnect+=new AsyncTcpListener<Session>.DisconnectEventHandler(Client_OnDisconnect);
			ClientListener.OnReceive+=new AsyncTcpListener<Session>.ReceiveEventHandler(Client_OnReceive);
			ClientListener.OnTimeout+=new AsyncTcpListener<Session>.TimeoutEventHandler(Client_OnTimeout);
			
			Logger.Info("local:"+localPort+", client port:"+port);
		}

		#region Client
		public void Client_OnTimeout(Connection<Session> timeoutConnection, double time)
		{
			Client_OnDisconnect(timeoutConnection);
		}

		public void Client_OnDisconnect(Connection<Session> Client)
		{
			lock(Clients){
				if(Clients.Contains(Client)){
					Client.Close();
					Clients.Remove(Client);
				}
			}
		}

		public void Client_OnConnect(Connection<Session> Client)
		{
			lock(Clients){
				if(Clients.Contains(Client)){
					Session server=new Session(this, Client);
					Client.Tag = server;
					Clients.Add(Client);
				}
			}
		}

		public void Client_OnReceive(Connection<Session> Client)
		{
			if(Client.Tag!=null)
				Client.Tag.OnReceive();
		}
		#endregion
		
		#region Server
		void ServerListener_OnTimeout(Connection<Server> timeoutConnection, double time)
		{
			ServerListener_OnDisconnect(timeoutConnection);
		}

		void ServerListener_OnReceive(Connection<Server> Client)
		{
			if(Client.Tag!=null)
				Client.Tag.OnReceive();
			else{
				Logger.Warn("Client.Tag == null");
			}
		}

		void ServerListener_OnDisconnect(Connection<Server> Client)
		{
			lock(Servers){
				if(Servers.Contains(Client)){
					Client.Close();
					Servers.Remove(Client);
				}
			}
		}

		void ServerListener_OnConnect(Connection<Server> Client)
		{
			lock(Servers){
				if(!Servers.Contains(Client)){
					Server server=new Server(this, Client);
					Client.Tag = server;
					Servers.Add(Client);
				}
			}
		}
		#endregion
		
		public void Start(){
			if(isStarted) return;
			isStarted = true;
			ServerListener.Start();
			ClientListener.Start();
		}
	}
}
