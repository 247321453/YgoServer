/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/11/17
 * 时间: 16:59
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using AsyncServer;
using System.Collections.Generic;

namespace YGOCore
{
	/// <summary>
	/// Description of ServerEvent.
	/// </summary>
	public static class ServerEvent
	{
		#region 消息匹配
		static readonly EventHandler<ushort, DuelServer, PacketReader> EventHandler = new EventHandler<ushort, DuelServer, PacketReader>();
		
		static ServerEvent(){
			RegisterEvents();
		}
		static void RegisterEvents(){
			EventHandler.Register((ushort)RoomMessage.Info, 	OnError);
			EventHandler.Register((ushort)RoomMessage.RoomCreate, 	OnError);
			EventHandler.Register((ushort)RoomMessage.RoomStart, 	OnError);
			EventHandler.Register((ushort)RoomMessage.RoomClose, 	OnError);
			EventHandler.Register((ushort)RoomMessage.Info, 	OnError);
			//EventHandler.Register((ushort)RoomMessage.SystemChat,	OnSystemChat);
			//EventHandler.Register((ushort)RoomMessage.RoomCreate,	OnRoomCreate);
			//EventHandler.Register((ushort)RoomMessage.RoomStart,	OnRoomStart);
			//EventHandler.Register((ushort)RoomMessage.RoomClose,	OnRoomClose);
		}
		public static void Handler(DuelServer session, List<PacketReader> packets){
			foreach(PacketReader packet in packets){
				//			Parse(player, packet);
				ushort id = packet.ReadByte();
				//if(RoomMessage.IsDefined(typeof(RoomMessage), id)){
				EventHandler.Do(id, session, packet);
				packet.Close();
				//}
			}
		}
		#endregion
	}
}
