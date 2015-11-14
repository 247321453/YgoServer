﻿/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/8
 * 时间: 9:27
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using AsyncServer;
using System.Windows.Forms;
using YGOCore.Game;
using System.Collections.Generic;
using YGOCore;

namespace GameClient
{
	/// <summary>
	/// Description of ClientEvent.
	/// </summary>
	public static class ClientEvent
	{
		#region handler
		static readonly EventHandler<ushort, Client, PacketReader> EventHandler = new EventHandler<ushort, Client, PacketReader>();
		
		static ClientEvent(){
			RegisterEvents();
		}
		static void RegisterEvents(){
			EventHandler.Register((ushort)RoomMessage.Error, OnError);
			EventHandler.Register((ushort)RoomMessage.Info, OnServerInfo);
			EventHandler.Register((ushort)RoomMessage.Chat, OnClientChat);
			EventHandler.Register((ushort)RoomMessage.RoomCreate, OnRoomCreate);
			EventHandler.Register((ushort)RoomMessage.RoomStart, OnRoomStart);
			EventHandler.Register((ushort)RoomMessage.RoomClose, OnRoomClose);
			EventHandler.Register((ushort)RoomMessage.RoomList, OnRoomList);
			EventHandler.Register((ushort)RoomMessage.PlayerEnter, OnPlayerEnter);
			EventHandler.Register((ushort)RoomMessage.PlayerLeave, OnPlayerLeave);
			EventHandler.Register((ushort)RoomMessage.ServerClose, OnServerClose);
		}
		public static void Handler(Client client, List<PacketReader> packets){
			if(packets.Count==0) return;
			
			foreach(PacketReader packet in packets){
				//			Parse(player, packet);
				ushort id = packet.ReadByte();
				EventHandler.Do(id, client, packet);
				packet.Close();
			}
		}
		#endregion
		
		#region msg
		private static void OnError(Client client, PacketReader reader){
			//错误
			string err = reader.ReadUnicode(256);
			MessageBox.Show(err);
			if(!client.IsLogin){
				try{
					client.Close();
				}catch{}
			}
		}
		private static void OnServerInfo(Client client, PacketReader reader){
			//服务器信息
			Program.Config.ChatPort = reader.ReadInt32(); 
			Program.Config.DuelPort = reader.ReadInt32();
			MessageBox.Show(Program.Config.ChatPort+":"+Program.Config.DuelPort);
			client.OnLoginOk();
		}
		private static void OnClientChat(Client client, PacketReader reader){
			//大厅聊天
			string name = reader.ReadUnicode(20);
			string toname = reader.ReadUnicode(20);
			string msg = reader.ReadUnicode(256);
			client.ServerChat(name, toname, msg);
		}
		#endregion
		
		#region room
		private static void OnRoomCreate(Client client, PacketReader reader){
			//房间创建
			int port = reader.ReadInt32();
			string room = reader.ReadUnicode(20);
			string banlist = reader.ReadUnicode(20);
			string info = reader.ReadUnicode(40);
			GameConfig2 config = new GameConfig2();
			config.Parse(info);
			config.Name = room;
			config.DeulPort = port;
			config.BanList = banlist;
			client.ServerRoomCreate(config);
		}
		private static void OnRoomStart(Client client, PacketReader reader){
			int port = reader.ReadInt32();
			string room = reader.ReadUnicode(20);
			client.ServerRoomStart(port, room);
		}
		private static void OnRoomClose(Client client, PacketReader reader){
			int port = reader.ReadInt32();
			string room = reader.ReadUnicode(20);
			client.ServerRoomClose(port, room);
		}
		private static void OnRoomList(Client client, PacketReader reader){
			int port = reader.ReadInt32();
			int count = reader.ReadInt32();
			List<GameConfig2> configs=new List<GameConfig2>();
			for(int i=0;i<count;i++){
				string name = reader.ReadUnicode(20);
				string banlist = reader.ReadUnicode(20);
				string info = reader.ReadUnicode(20);
				GameConfig2 config =new GameConfig2();
				config.Parse(info);
				config.Name = name;
				config.BanList = banlist;
				config.DeulPort = port;
				configs.Add(config);
			}
			MessageBox.Show("roomlist:"+configs.Count);
			client.ServerRoomList(configs);
		}
		private static void OnPlayerEnter(Client client, PacketReader reader){
			int port = reader.ReadInt32();
			string name = reader.ReadUnicode(20);
			string room = reader.ReadUnicode(20);
			client.ServerPlayerEnter(port, name, room);
		}
		private static void OnPlayerLeave(Client client, PacketReader reader){
			int port = reader.ReadInt32();
			string name = reader.ReadUnicode(20);
			string room = reader.ReadUnicode(20);
			client.ServerPlayerLeave(port, name, room);
		}
		private static void OnServerClose(Client client, PacketReader reader){
			int port = reader.ReadInt32();
			int nport=reader.ReadInt32();
			Program.Config.DuelPort = nport;
			client.ServerClose(port);
		}
		#endregion
		
	}
}
