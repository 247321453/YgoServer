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
using System.Collections.Generic;

namespace YGOCore
{

	/// <summary>
	/// 登记服务端信息
	/// 玩家状态
	/// 房间列表
	/// </summary>
	public static class LocalEvent
	{
		public static void Local_OnTimeout(this RoomServer server,Connection<ServerSession> timeoutConnection, double time)
		{
			server.Local_OnDisconnect(timeoutConnection);
		}

		public static void Local_OnDisconnect(this RoomServer server,Connection<ServerSession> Client)
		{
			lock(server.LocalClients){
				server.LocalClients.Remove(Client);
			}
		}

		public static void Local_OnConnect(this RoomServer server,Connection<ServerSession> Client)
		{
			ServerSession session = new ServerSession(Client);
			Client.Tag = session;
			lock(server.LocalClients){
				server.LocalClients.Add(Client);
			}
		}

		public static void Local_OnReceive(this RoomServer server,Connection<ServerSession> Client)
		{
			/////保存
			//服务端信息
			///////
			//转发
			//玩家状态
			//房间状态
		}
	}
}
