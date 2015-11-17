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
using YGOCore.Game;

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
			EventHandler.Register((ushort)RoomMessage.Info, 		OnInfo);
			EventHandler.Register((ushort)RoomMessage.RoomCreate, 	OnRoomCreate);
			EventHandler.Register((ushort)RoomMessage.RoomStart, 	OnRoomStart);
			EventHandler.Register((ushort)RoomMessage.RoomClose, 	OnRoomClose);
			EventHandler.Register((ushort)RoomMessage.PlayerLeave, 	OnPlayerLeave);
			EventHandler.Register((ushort)RoomMessage.PlayerEnter, 	OnPlayerEnter);
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
		private static void OnInfo(DuelServer server, PacketReader packet){
			int Port = packet.ReadInt32();
			bool NeedAuth = packet.ReadBoolean();
			server.Init(Port, NeedAuth);
			Logger.Info("duel server port="+Port+",needauth="+NeedAuth);
		}
		private static void OnRoomCreate(DuelServer server, PacketReader packet){
			string name = packet.ReadUnicode(20);
			string info = packet.ReadUnicode(20);
			string banlist=packet.ReadUnicode(50);
			GameConfig config=new GameConfig();
			config.Parse(info);
			config.Name=name;
			config.BanList=banlist;
			Logger.Debug("OnRoomCreate:"+server.Port+","+name);
			lock(server.Rooms){
				if(!server.Rooms.ContainsKey(name)){
					server.Rooms.Add(name, config);
				}else{
					Logger.Warn("same room:"+name+" form "+server.Port);
				}
			}
			if(server.Server!=null)
				server.Server.server_OnRoomCreate(server, name, banlist, info);
		}
		private static void OnRoomStart(DuelServer server, PacketReader packet){
			string name = packet.ReadUnicode(20);
			Logger.Debug("OnRoomStart:"+server.Port+","+name);
			lock(server.Rooms){
				if(server.Rooms.ContainsKey(name)){
					server.Rooms[name].IsStart = true;
				}else{
					Logger.Warn("no start room:"+name+" form "+server.Port);
				}
			}
			if(server.Server!=null)
				server.Server.server_OnRoomStart(server, name);
		}
		private static void OnRoomClose(DuelServer server, PacketReader packet){
			string name = packet.ReadUnicode(20);
			Logger.Debug("OnRoomClose:"+server.Port+","+name);
			lock(server.Rooms){
				if(server.Rooms.ContainsKey(name)){
					server.Rooms.Remove(name);
				}else{
					Logger.Warn("no close room:"+name+" form "+server.Port);
				}
			}
			if(server.Server!=null)
				server.Server.server_OnRoomClose(server, name);
		}
		private static void OnPlayerLeave(DuelServer server, PacketReader packet){
			Logger.Debug("OnPlayerLeave:"+server.Port);
			string name = packet.ReadUnicode(20);
			string room = packet.ReadUnicode(20);
			if(server.Server!=null)
				server.Server.server_OnPlayerLeave(server, name, room);
		}
		private static void OnPlayerEnter(DuelServer server, PacketReader packet){
			Logger.Debug("OnPlayerEnter:"+server.Port);
			string name = packet.ReadUnicode(20);
			string room = packet.ReadUnicode(20);
			if(server.Server!=null)
				server.Server.server_OnPlayerJoin(server, name, room);
		}
	}
}
