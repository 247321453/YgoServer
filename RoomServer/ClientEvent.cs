/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/4
 * 时间: 20:17
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using OcgWrapper.Enums;
using System.Collections.Generic;
using AsyncServer;

namespace YGOCore
{
	public static class ClientEvent
	{
		#region 发给玩家
		public static void OnServerInfo(this RoomServer RoomServer, Server server){
			
		}
		public static void OnRoomCreate(this RoomServer RoomServer, Server server, RoomInfo info){
			
		}
		public static void OnRoomClose(this RoomServer RoomServer, Server server, string name){

		}
		public static void OnRoomStart(this RoomServer RoomServer, Server server, string name){

		}
		public static void OnPlayerJoin(this RoomServer RoomServer, Server server, string player,string room){

		}
		public static void OnPlayerLeave(this RoomServer RoomServer, Server server, string player,string room){

		}
		#endregion
		
		
		static readonly EventHandler<ushort, Session, PacketReader> EventHandler =new EventHandler<ushort, Session, PacketReader>();
		
		static ClientEvent()
		{
			EventHandler.Register((ushort)CtosMessage.RoomJoin, OnRoomJoin);
			EventHandler.Register((ushort)CtosMessage.Roomleave, OnRoomleave);
			EventHandler.Register((ushort)CtosMessage.OutRoomChat, OnOutRoomChat);
		}
		
		public static void Handler(this Session client, List<PacketReader> packets){
			foreach(PacketReader packet in packets){
				//			Parse(player, packet);
				ushort id = (ushort)packet.ReadByte();
				if(CtosMessage.IsDefined(typeof(CtosMessage), (byte)id)){
					Logger.Debug((CtosMessage)id);
				}else{
					Logger.Debug("client unknown :"+id);
				}
				EventHandler.Do(id, client, packet);
				packet.Close();
			}
		}
		public static void OnRoomJoin(this Session client, PacketReader reader){
			client.Close();
		}
		public static void OnRoomleave(this Session client, PacketReader reader){
			//发送列表
		}
		public static void OnOutRoomChat(this Session client, PacketReader reader){
			//大厅聊天
		}
	}
}
