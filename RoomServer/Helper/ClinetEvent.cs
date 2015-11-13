/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/11/13
 * 时间: 11:20
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using AsyncServer;
using System.Collections.Generic;

namespace YGOCore
{
	/// <summary>
	/// Description of ClinetEvent.
	/// </summary>
	public static class ClinetEvent
	{
		#region 消息匹配
		static readonly EventHandler<ushort, Session, PacketReader> EventHandler = new EventHandler<ushort, Session, PacketReader>();
		
		static ClinetEvent(){
			RegisterEvents();
		}
		static void RegisterEvents(){
			//EventHandler.Register((ushort)RoomMessage.Error, 	OnError);
			//EventHandler.Register((ushort)RoomMessage.RoomList,	OnRoomList);
			EventHandler.Register((ushort)RoomMessage.Info,	OnInfo);
		//	EventHandler.Register((ushort)RoomMessage.Chat,	OnChat);
			//EventHandler.Register((ushort)RoomMessage.SystemChat,	OnSystemChat);
			//EventHandler.Register((ushort)RoomMessage.RoomCreate,	OnRoomCreate);
			//EventHandler.Register((ushort)RoomMessage.RoomStart,	OnRoomStart);
			//EventHandler.Register((ushort)RoomMessage.RoomClose,	OnRoomClose);
		}
		public static void Handler(Session session, List<PacketReader> packets){
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
		//登录
		private static void OnInfo(Session session, PacketReader packet){
			
		}
		private static void OnChat(Session session, PacketReader packet){
			
		}
	}
}
