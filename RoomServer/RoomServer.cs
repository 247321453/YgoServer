/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/4
 * 时间: 20:09
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using AsyncServer;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace YGOCore
{
	/// <summary>
	/// Description of RoomServer.
	/// </summary>
	public class RoomServer
	{
		const int DEFAULT = 8920;
		const int DEFAULT_LOCAL = 18920;
		/// <summary>
		/// 玩家
		/// </summary>
		public readonly List<Connection<Session>> m_clients=new List<Connection<Session>>();
		/// <summary>
		/// 服务端
		/// </summary>
		public readonly List<Connection<Session>> LocalClients=new List<Connection<Session>>();
		/// <summary>
		/// 对外
		/// </summary>
		AsyncTcpListener<Session> m_listener;
		/// <summary>
		/// 本地
		/// </summary>
		AsyncTcpListener<Session> m_localListenr;
		
		bool isListening = false;
		public RoomServer(int port = DEFAULT,int localport=DEFAULT_LOCAL)
		{
			if(port==0){
				port = DEFAULT;
			}
			if(localport == 0){
				localport = DEFAULT_LOCAL;
			}
			Console.WriteLine("外部端口:"+port+"，本地端口:"+localport);
			m_listener = new AsyncTcpListener<Session>(IPAddress.Any, 0 ,0);
			m_listener.OnReceive += new AsyncTcpListener<Session>.ReceiveEventHandler(this.Client_OnReceive);
			m_listener.OnConnect+=new AsyncTcpListener<Session>.ConnectEventHandler(this.Client_OnConnect);
			m_listener.OnDisconnect+=new AsyncTcpListener<Session>.DisconnectEventHandler(this.Client_OnDisconnect);
			m_listener.OnTimeout +=new AsyncTcpListener<Session>.TimeoutEventHandler(this.Client_OnTimeout);
			
			m_localListenr = new AsyncTcpListener<Session>(IPAddress.Parse("127.0.0.1"), localport, 0,0);
			m_localListenr.OnReceive += new AsyncTcpListener<Session>.ReceiveEventHandler(this.Local_OnReceive);
			m_localListenr.OnConnect+=new AsyncTcpListener<Session>.ConnectEventHandler(this.Local_OnConnect);
			m_localListenr.OnDisconnect +=new AsyncTcpListener<Session>.DisconnectEventHandler(this.Local_OnDisconnect);
			m_localListenr.OnTimeout+=new AsyncTcpListener<Session>.TimeoutEventHandler(this.Local_OnTimeout);
		}
		
		public void Start(){
			if(isListening) return;
			isListening = true;
			m_listener.Start();
			m_localListenr.Start();
		}
	}
}
