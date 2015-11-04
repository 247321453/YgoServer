/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/4
 * 时间: 20:17
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using AsyncServer;

namespace YGOCore
{

	public static class ClientEvent
	{

		public static void Client_OnTimeout(this RoomServer server,Connection<Session> timeoutConnection, double time)
		{
			server.Client_OnDisconnect(timeoutConnection);
		}

		public static void Client_OnDisconnect(this RoomServer server,Connection<Session> Client)
		{
			lock(server.m_clients){
				server.m_clients.Remove(Client);
			}
		}

		public static void Client_OnConnect(this RoomServer server,Connection<Session> Client)
		{
			Session session = new Session(Client);
			Client.Tag = session;
			lock(server.m_clients){
				server.m_clients.Add(Client);
			}
		}

		public static void Client_OnReceive(this RoomServer server,Connection<Session> Client)
		{
			throw new NotImplementedException();
		}
	}
}
