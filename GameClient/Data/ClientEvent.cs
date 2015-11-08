/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/8
 * 时间: 9:27
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using AsyncServer;
using OcgWrapper.Enums;
using System.Windows.Forms;
using GameClient.Data;
using YGOCore.Game;
using System.Collections.Generic;

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
			EventHandler.Register((ushort)StocMessage.HsPlayerEnter, OnError);
			EventHandler.Register((ushort)StocMessage.Chat, OnServerMsg);
			EventHandler.Register((ushort)StocMessage.ServerInfo, OnServerInfo);
			EventHandler.Register((ushort)StocMessage.ClientChat, OnClientChat);
			EventHandler.Register((ushort)StocMessage.RoomCreate, OnRoomCreate);
			EventHandler.Register((ushort)StocMessage.RoomStart, OnRoomStart);
			EventHandler.Register((ushort)StocMessage.RoomClose, OnRoomClose);
			EventHandler.Register((ushort)StocMessage.RoomList, OnRoomList);
		}
		public static void Handler(Client client, List<PacketReader> packets){
			if(packets.Count==0) return;
			
			foreach(PacketReader packet in packets){
				//			Parse(player, packet);
				StocMessage msg = Byte2Ctos(packet.ReadByte());
				EventHandler.Do((ushort)msg, client, packet);
				packet.Close();
			}
		}
		public static StocMessage Byte2Ctos(byte ctos)
		{
			if(StocMessage.IsDefined(typeof(StocMessage), ctos)){
				return (StocMessage)ctos;
			}
			return StocMessage.Unknown;
		}
		#endregion
		
		#region msg
		private static void OnError(Client client, PacketReader reader){
			//错误
			string err = reader.ReadUnicode(20);
			MessageBox.Show(err);
			if(!client.IsLogin){
				try{
					client.Close();
				}catch{}
			}
		}
		private static void OnServerMsg(Client client, PacketReader reader){
			//服务器消息
			int type = (int)reader.ReadInt16();
			string msg = reader.ReadUnicode(256);
			client.ServerChat(null, null, msg);
		}
		private static void OnServerInfo(Client client, PacketReader reader){
			//服务器信息
			ServerInfo info =new ServerInfo();
			info.Name = reader.ReadUnicode(20);
			info.Host = reader.ReadUnicode(20);
			info.Port = reader.ReadInt32();
			info.NeedAuth = reader.ReadBoolean();
			info.Token = reader.ReadUnicode(20);
			info.Desc = reader.ReadUnicode(256);
			client.OnServerInfo(info);
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
			GameConfig config =new GameConfig();
			config.Name = reader.ReadUnicode(20);
			config.BanList = reader.ReadUnicode(20);
			config.LfList = reader.ReadInt16();
			config.Rule = reader.ReadInt16();
			config.Mode = reader.ReadInt16();
			config.EnablePriority = reader.ReadBoolean();
			config.NoCheckDeck = reader.ReadBoolean();
			config.NoShuffleDeck = reader.ReadBoolean();
			config.StartLp = reader.ReadInt32();
			config.StartHand = reader.ReadInt16();
			config.DrawCount= reader.ReadInt16();
			config.GameTimer = reader.ReadInt32();
			config.IsStart = reader.ReadBoolean();
			client.OnServerRoomCreate(config);
		}
		private static void OnRoomStart(Client client, PacketReader reader){
			string room = reader.ReadUnicode(20);
			client.OnServerRoomStart(room);
		}
		private static void OnRoomClose(Client client, PacketReader reader){
			string room = reader.ReadUnicode(20);
			client.OnServerRoomClose(room);
		}
		private static void OnRoomList(Client client, PacketReader reader){
			int count = reader.ReadInt32();
			List<GameConfig> configs=new List<GameConfig>();
			for(int i=0;i<count;i++){
				GameConfig config =new GameConfig();
				config.Name = reader.ReadUnicode(20);
				config.BanList = reader.ReadUnicode(20);
				config.LfList = reader.ReadInt16();
				config.Rule = reader.ReadInt16();
				config.Mode = reader.ReadInt16();
				config.EnablePriority = reader.ReadBoolean();
				config.NoCheckDeck = reader.ReadBoolean();
				config.NoShuffleDeck = reader.ReadBoolean();
				config.StartLp = reader.ReadInt32();
				config.StartHand = reader.ReadInt16();
				config.DrawCount= reader.ReadInt16();
				config.GameTimer = reader.ReadInt32();
				config.IsStart = reader.ReadBoolean();
				configs.Add(config);
			}
			client.OnServerRoomList(configs);
		}
		#endregion
		
	}
}
