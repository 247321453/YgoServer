/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/11/12
 * 时间: 13:30
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using YGOCore.Game;
using AsyncServer;
using YGOCore;
using YGOCore.Net;

namespace YGOCore
{
	/// <summary>
	/// Description of ServerApi.
	/// </summary>
	public static class ServerApi
	{
		public static void OnServerInfo(GameServer server){
			ServerConfig Config = server.Config;
			using(PacketWriter writer=new PacketWriter(2)){
				writer.Write((byte)RoomMessage.Info);
				writer.Write(Config.ServerPort);
				writer.Write(Config.isNeedAuth);
				writer.Use();
				Send(writer.Content);
			}
		}
		public static void OnRoomCreate(GameRoom room){
			if(room==null||room.Config==null){
				return;
			}
			using(PacketWriter writer=new PacketWriter(2)){
				writer.Write((byte)RoomMessage.RoomCreate);
				writer.Write(room.Name, 20);
				writer.Write(room.Config.Name, 20);
				writer.Write(room.Config.BanList);
				writer.Use();
				Send(writer.Content);
			}
		}
		
		public static void OnRoomStart(GameRoom room){
			using(PacketWriter writer=new PacketWriter(2)){
				writer.Write((byte)RoomMessage.RoomStart);
				writer.Write(room.Config.Name, 20);
				writer.Use();
				Send(writer.Content);
			}
		}
		
		public static void OnRoomClose(GameRoom room){
			using(PacketWriter writer=new PacketWriter(2)){
				writer.Write((byte)RoomMessage.RoomClose);
				writer.Write(room.Config.Name, 20);
				writer.Use();
				Send(writer.Content);
			}
		}

		public static void OnPlayerLeave(GameSession player, GameRoom room){
			using(PacketWriter writer=new PacketWriter(2)){
				writer.Write((byte)RoomMessage.PlayerLeave);
				writer.Write(player.Name, 20);
				writer.Write(room.Config.Name, 20);
				writer.Use();
				Send(writer.Content);
			}
		}
		
		public static void OnPlayerEnter(GameSession player, GameRoom room){
			using(PacketWriter writer=new PacketWriter(2)){
				writer.Write((byte)RoomMessage.PlayerEnter);
				writer.Write(player.Name, 20);
				writer.Write(room.Config.Name, 20);
				writer.Use();
				Send(writer.Content);
			}
			string tip = Messages.RandomMessage();
			if(!string.IsNullOrEmpty(tip))
				player.ServerMessage(Messages.RandomMessage());
		}
		private static void Send(byte[] data){
			if(Client!=null&& Client.Connected){
				Client.AsyncSend(data);
			}
		}
		
		static AsyncClient Client;
		public static bool Init(int port){
			Client = new AsyncClient();
			return Client.Connect("127.0.0.1", port);
		}
	}
}
