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
		public static void OnSendServerInfo(this RoomServer roomServer,Session session){
			using(PacketWriter writer = new PacketWriter(2)){
				writer.Write((byte)RoomMessage.Info);
				writer.Write(roomServer.GetChatPort());
				Server srv = roomServer.GetMinServer();
				session.ServerInfo = srv;
				if(srv!=null){
					writer.Write(srv.Port);
					writer.Write(srv.NeedAuth);
				}else{
					writer.Write((byte)0);
					writer.Write((byte)0);
				}
				writer.Use();
				session.Client.SendPackage(writer.Content);
			}
		}
		
		public static void OnServerClose(this RoomServer roomServer,Server server)
		{
			using(PacketWriter writer = new PacketWriter(2)){
				writer.Write((byte)RoomMessage.ServerClose);
				writer.Write(server.Port);
				Server srv = roomServer.GetMinServer();
				if(srv!=null){
					writer.Write(srv.Port);
					writer.Write(srv.NeedAuth);
				}else{
					writer.Write((byte)0);
					writer.Write((byte)0);
				}
				//session.ServerInfo = srv;
				writer.Use();
				roomServer.SendAll(writer.Content);
			}
		}


		#region room list
		public static void OnRoomList(this RoomServer roomServer,Session session,bool nolock=false,bool nostart = false){
			Logger.Debug("roomlist");
			lock(roomServer.Servers){
				foreach(Server srv in roomServer.Servers){
					using(PacketWriter wrtier=new PacketWriter(20)){
						wrtier.Write((byte)RoomMessage.RoomList);
						wrtier.Write(srv.Port);
						wrtier.Write(srv.NeedAuth);
						lock(srv.Rooms){
							wrtier.Write(srv.Rooms.Count);
							foreach(GameConfig config in srv.Rooms.Values){
								wrtier.WriteUnicode(config.Name, 20);
								wrtier.WriteUnicode(config.BanList, 20);
								wrtier.WriteUnicode(config.RoomString, 20);
							}
						}
						wrtier.Use();
						session.Client.SendPackage(wrtier.Content, false);
					}
				}
			}
			session.Client.PeekSend();
		}
		#endregion
		
		#region msg
		public static void OnChatMessage(this RoomServer roomServer,string name, string toname, string msg){
			using(PacketWriter writer = new PacketWriter(2)){
				writer.Write((byte)RoomMessage.Chat);
				writer.WriteUnicode(name, 20);
				writer.WriteUnicode(toname, 20);
				writer.WriteUnicode(msg, msg.Length+1);
				writer.Use();
				roomServer.SendAll(writer.Content, true, true);
			}
		}
		public static void SendError(this Session session, string err){
			if(session.Client==null) return;
			using(PacketWriter writer=new PacketWriter(2)){
				writer.Write((byte)RoomMessage.Error);
				writer.WriteUnicode(err, err.Length+1);
				writer.Use();
				session.Client.SendPackage(writer.Content);
			}
		}
		
		private static void SendAll(this RoomServer roomServer,byte[] data,bool isNow = true,bool Force=false){
			lock(roomServer.Clients){
				foreach(Session client in roomServer.Clients.Values){
					if(Force || !client.IsPause){
						client.Client.SendPackage(data, isNow);
					}
				}
			}
		}
		
		#endregion
		
		#region room
		public static void server_OnRoomCreate(this RoomServer roomServer, Server server, string name,string banlist,string gameinfo)
		{
			using(PacketWriter writer = new PacketWriter(2)){
				writer.Write((byte)RoomMessage.RoomCreate);
				writer.Write(server.Port);
				writer.Write(server.NeedAuth);
				writer.WriteUnicode(name, 20);
				writer.WriteUnicode(banlist, 20);
				writer.WriteUnicode(gameinfo, gameinfo.Length+1);
				writer.Use();
				roomServer.SendAll(writer.Content);
			}
		}

		public static void server_OnRoomStart(this RoomServer roomServer, Server server, string name)
		{
			using(PacketWriter writer = new PacketWriter(2)){
				writer.Write((byte)RoomMessage.RoomStart);
				writer.Write(server.Port);
				writer.WriteUnicode(name, 20);
				writer.Use();
				roomServer.SendAll(writer.Content);
			}
		}

		
		public static void server_OnRoomClose(this RoomServer roomServer, Server server, string name)
		{
			using(PacketWriter writer = new PacketWriter(2)){
				writer.Write((byte)RoomMessage.RoomClose);
				writer.Write(server.Port);
				writer.WriteUnicode(name, 20);
				writer.Use();
				roomServer.SendAll(writer.Content);
			}
		}
		#endregion
		
		#region player
		public static void OnPlayerList(this Session client){
			lock(client.Server.Clients){
				using(PacketWriter writer = new PacketWriter(2)){
					writer.Write((byte)RoomMessage.PlayerList);
					writer.Write(client.Server.Clients.Count);
					foreach(Session session in client.Server.Clients.Values){
						writer.Write(session.ServerInfo==null?0:session.ServerInfo.Port);
						writer.WriteUnicode(session.Name, 20);
						writer.WriteUnicode(session.RoomName, 20);
					}
					writer.Use();
					client.Server.SendAll(writer.Content);
				}
			}
		}
		public static void server_OnPlayerLeave(this RoomServer roomServer, Server server, string name, string room)
		{
			if(string.IsNullOrEmpty(name))return ;
			lock(roomServer.Clients){
				if(roomServer.Clients.ContainsKey(name)){
					Session player = roomServer.Clients[name];
					player.RoomName = null;
				}
			}
			using(PacketWriter writer = new PacketWriter(2)){
				writer.Write((byte)RoomMessage.PlayerLeave);
				writer.Write(server.Port);
				writer.WriteUnicode(name, 20);
				writer.WriteUnicode(room, 20);
				writer.Use();
				roomServer.SendAll(writer.Content);
			}
		}

		public static void server_OnPlayerJoin(this RoomServer roomServer, Server server, string name, string room)
		{
			if(string.IsNullOrEmpty(name))return ;
			lock(roomServer.Clients){
				if(roomServer.Clients.ContainsKey(name)){
					Session player = roomServer.Clients[name];
					player.RoomName = room;
				}
			}
			using(PacketWriter writer = new PacketWriter(2)){
				writer.Write((byte)RoomMessage.PlayerEnter);
				writer.Write(server.Port);
				writer.WriteUnicode(name, 20);
				writer.WriteUnicode(room, 20);
				writer.Use();
				roomServer.SendAll(writer.Content);
			}
		}
		#endregion
	}
}
