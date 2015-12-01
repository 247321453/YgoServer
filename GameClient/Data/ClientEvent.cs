/*
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
using System.IO;

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
			EventHandler.Register((ushort)RoomMessage.PlayerList,OnPlayerList);
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
			Program.Config.NeedAuth = reader.ReadBoolean();
			#if DEBUG
			MessageBox.Show(Program.Config.ChatPort+":"+Program.Config.DuelPort);
			#endif
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
			bool needauth = reader.ReadBoolean();
			string room = reader.ReadUnicode(20);
			string banlist = reader.ReadUnicode(20);
			string info = reader.ReadUnicode(40);
			GameConfig2 config = new GameConfig2();
			config.Parse(info);
			config.Name = room;
			config.NeedAuth = needauth;
			config.DeulPort = port;
			config.BanList = banlist;
			
			client.ServerRoomCreate(config);
		}
		private static void OnRoomStart(Client client, PacketReader reader){
			int port = reader.ReadInt32();
			string room = reader.ReadUnicode(20);
			client.ServerRoomStart(new RoomInfo(port, room));
		}
		private static void OnRoomClose(Client client, PacketReader reader){
			int port = reader.ReadInt32();
			string room = reader.ReadUnicode(20);
			client.ServerRoomClose(new RoomInfo(port, room));
		}
		private static void OnRoomList(Client client, PacketReader reader){
			int count = reader.ReadInt32();
			List<GameConfig2> configs=new List<GameConfig2>();
			for(int i=0;i<count;i++){
                int port = reader.ReadInt32();
                bool needauth = reader.ReadBoolean();
                string name = reader.ReadUnicode(20);
				string banlist = reader.ReadUnicode(20);
				string info = reader.ReadUnicode(20);
                bool start = reader.ReadBoolean();
                GameConfig2 config =new GameConfig2();
				config.Parse(info);
				config.Name = name;
				config.BanList = banlist;
				config.DeulPort = port;
				config.NeedAuth = needauth;
                config.IsStart = start;
				configs.Add(config);
			}
			client.ServerRoomList(configs);
		}
		private static void OnPlayerList(Client client, PacketReader reader){
			int count = reader.ReadInt32();
			List<PlayerInfo> players=new List<PlayerInfo>();
			for(int i=0;i<count;i++){
				int port = reader.ReadInt32();
				string name = reader.ReadUnicode(20);
				string room = reader.ReadUnicode(20);
				PlayerInfo player=new PlayerInfo();
				player.Name = name;
				player.Room=new RoomInfo(room, port);
				players.Add(player);
			}
			client.ServerPlayerList(players);
		}
		private static void OnPlayerEnter(Client client, PacketReader reader){
			int port = reader.ReadInt32();
			string name = reader.ReadUnicode(20);
			string room = reader.ReadUnicode(20);
			PlayerInfo player=new PlayerInfo();
			player.Name = name;
			player.Room=new RoomInfo(room, port);
			client.ServerPlayerEnter(player);
		}
		private static void OnPlayerLeave(Client client, PacketReader reader){
			int port = reader.ReadInt32();
			string name = reader.ReadUnicode(20);
			string room = reader.ReadUnicode(20);
			PlayerInfo player=new PlayerInfo();
			player.Name = name;
			player.Room=new RoomInfo(room, port);
			client.ServerPlayerLeave(player);
		}
		private static void OnServerClose(Client client, PacketReader reader){
			int port = reader.ReadInt32();
			int nport=reader.ReadInt32();
			Program.Config.DuelPort = nport;
			Program.Config.NeedAuth = reader.ReadBoolean();
			client.ServerClose(port);
		}
		#endregion
		
	}
}
