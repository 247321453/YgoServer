/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/4
 * 时间: 21:28
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using AsyncServer;
using YGOCore.Game;

namespace YGOCore
{
	/// <summary>
	/// Description of ServerSession.
	/// </summary>
	public class ServerSession
	{
		public readonly  List<RoomInfo> Rooms = new List<RoomInfo>();
		public readonly SortedList<string, PlayerStatu> Players=new SortedList<string, PlayerStatu>();
		Connection<ServerSession> Client;
		
		public string Name;
		public string Desc;
		public int port;
		public ServerSession(Connection<ServerSession> client)
		{
			Client = client;
		}
	}
}
