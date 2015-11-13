/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/11/13
 * 时间: 10:45
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
	/// Description of RoomEvent.
	/// </summary>
	public static class RoomEvent
	{
		public static void server_OnServerInfo(this RoomServer roomServer, Server server, bool isClose)
		{
			//向旧客户端发送消息
		}

		public static void server_OnRoomStart(this RoomServer roomServer, Server server, string name)
		{
		}

		public static void server_OnRoomCreate(this RoomServer roomServer, Server server, GameConfig config)
		{
		}

		public static void server_OnRoomClose(this RoomServer roomServer, Server server, string name)
		{
		}

		public static void server_OnPlayerLeave(this RoomServer roomServer, Server server, string name, string room)
		{
		}

		public static void server_OnPlayerJoin(this RoomServer roomServer, Server server, string name, string room)
		{
		}
	}
}
