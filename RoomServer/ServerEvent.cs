/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/5
 * 时间: 20:58
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using YGOCore.Net;
using System.Collections.Generic;
using OcgWrapper.Enums;
using AsyncServer;

namespace YGOCore
{
	/// <summary>
	/// Description of ServerEvent.
	/// </summary>
	public static class ServerEvent
	{
		static readonly EventHandler<ushort, Server, PacketReader> EventHandler =new EventHandler<ushort, Server, PacketReader>();
		static ServerEvent()
		{
			EventHandler.Register((ushort)StocMessage.ServerInfo, OnServerInfo);
			EventHandler.Register((ushort)StocMessage.RoomCreate, OnRoomCreate);
			EventHandler.Register((ushort)StocMessage.RoomClose, OnRoomClose);
			EventHandler.Register((ushort)StocMessage.RoomStart, OnRoomStart);
			EventHandler.Register((ushort)StocMessage.PlayerJoin, OnPlayerJoin);
			EventHandler.Register((ushort)StocMessage.PlayerLeave, OnPlayerLeave);
		}
		
		public static void Handler(this Server server, List<PacketReader> packets){
			foreach(PacketReader packet in packets){
				//			Parse(player, packet);
				ushort id = (ushort)packet.ReadByte();
				if(StocMessage.IsDefined(typeof(StocMessage), (byte)id)){
					Logger.Debug((StocMessage)id);
				}else{
					Logger.Debug("server unknown :"+id);
				}
				EventHandler.Do(id, server, packet);
				packet.Close();
			}
		}
		
		public static void OnServerInfo(this Server server, PacketReader reader){
			server.Port = reader.ReadInt32();
			server.Name = reader.ReadUnicode(20);
			server.Desc = reader.ReadUnicode(256);
			server.Version = reader.ReadInt32();
			server.RoomServer.OnServerInfo(server);
		}
		public static void OnRoomCreate(this Server server, PacketReader reader){
			RoomInfo info =new RoomInfo();
			info.Name = reader.ReadUnicode(40);
			lock(server.Rooms){
				if(server.Rooms.ContainsKey(info.Name)){
					//房间存在
					return;
				}
			}
			info.BanList = reader.ReadUnicode(40);
			info.Rule = reader.ReadUInt16();
			info.Mode = reader.ReadUInt16();
			info.EnablePriority = reader.ReadBoolean();
			info.NoCheckDeck  = reader.ReadBoolean();
			info.NoShuffleDeck = reader.ReadBoolean();
			info.StartLp = reader.ReadInt32();
			info.StartHand = reader.ReadInt32();
			info.DrawCount = reader.ReadInt32();
			info.GameTimer = reader.ReadInt32();
			lock(server.Rooms){
				if(!server.Rooms.ContainsKey(info.Name)){
					//房间创建
					server.Rooms.Add(info.Name, info);
					server.RoomServer.OnRoomCreate(server, info);
				}
			}
		}
		public static void OnRoomClose(this Server server, PacketReader reader){
			string room = reader.ReadUnicode(40);
			lock(server.Rooms){
				if(server.Rooms.ContainsKey(room)){
					//房间关闭
					server.Rooms.Remove(room);
					server.RoomServer.OnRoomClose(server, room);
				}
			}
		}
		public static void OnRoomStart(this Server server, PacketReader reader){
			string room = reader.ReadUnicode(40);
			lock(server.Rooms){
				if(server.Rooms.ContainsKey(room)){
					//房间开始
					server.Rooms[room].IsStart = true;
					server.RoomServer.OnRoomStart(server, room);
				}
			}
		}
		public static void OnPlayerJoin(this Server server, PacketReader reader){
			string player = reader.ReadUnicode(20);
			string room = reader.ReadUnicode(40);
			lock(server.Rooms){
				if(server.Rooms.ContainsKey(room)){
					RoomInfo info = server.Rooms[room];
					if(!info.Players.Contains(player)){
						//发送玩家加入
						info.Players.Add(player);
						server.RoomServer.OnPlayerJoin(server, player, room);
					}
				}
			}
		}
		public static void OnPlayerLeave(this Server server, PacketReader reader){
			string player = reader.ReadUnicode(20);
			string room = reader.ReadUnicode(40);
			lock(server.Rooms){
				if(server.Rooms.ContainsKey(room)){
					RoomInfo info = server.Rooms[room];
					if(info.Players.Contains(player)){
						//发送玩家离开
						info.Players.Remove(player);
						server.RoomServer.OnPlayerLeave(server, player, room);
					}
				}
			}
		}
	}
}
