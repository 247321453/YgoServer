
using System;
using System.Collections.Generic;
using AsyncServer;
using OcgWrapper.Enums;
using YGOCore.Game;
using System.Text;
using System.IO;

namespace YGOCore.Net
{
	/// <summary>
	/// 游戏事件处理
	/// </summary>
	public static class GameEvent
	{
		#region 消息匹配
		static readonly EventHandler<ushort, GameSession, GameClientPacket> EventHandler = new EventHandler<ushort, GameSession, GameClientPacket>();
		
		static GameEvent(){
			RegisterEvents();
		}
		static void RegisterEvents(){
			EventHandler.Register((ushort)CtosMessage.PlayerInfo, 	OnPlayerInfo);
			EventHandler.Register((ushort)CtosMessage.JoinGame,		OnJoinGame);
			EventHandler.Register((ushort)CtosMessage.CreateGame,	OnCreateGame);
			EventHandler.Register((ushort)CtosMessage.Chat, 		OnChat);
			EventHandler.Register((ushort)CtosMessage.HsToDuelist, 	OnMoveToDeulList);
			EventHandler.Register((ushort)CtosMessage.HsToObserver,	OnMoveToObserver);
			EventHandler.Register((ushort)CtosMessage.LeaveGame,	OnLeaveGame);
			EventHandler.Register((ushort)CtosMessage.HsReady,		OnSetReady);
			EventHandler.Register((ushort)CtosMessage.HsNotReady,	OnNotReady);
			EventHandler.Register((ushort)CtosMessage.HsKick,		OnKick);
			EventHandler.Register((ushort)CtosMessage.HsStart,		OnStartDuel);
			EventHandler.Register((ushort)CtosMessage.HandResult,	OnHandResult);
			EventHandler.Register((ushort)CtosMessage.TpResult,		OnTpResult);
			EventHandler.Register((ushort)CtosMessage.UpdateDeck,	OnUpdateDeck);
			EventHandler.Register((ushort)CtosMessage.Response,		OnResponse);
			EventHandler.Register((ushort)CtosMessage.Surrender,	OnSurrender);
			EventHandler.Register((ushort)CtosMessage.TimeConfirm,  OnTimeConfirm);
		}
		public static CtosMessage Handler(GameSession player, params GameClientPacket[] packets){
			CtosMessage firstmsg = CtosMessage.Unknown;
			if(packets == null || packets.Length == 0) 
				return firstmsg;
			foreach(GameClientPacket packet in packets){
				//			Parse(player, packet);
				if(packet.Length==0){
					continue;
				}
				CtosMessage msg = packet.ReadCtos();
				if(firstmsg == CtosMessage.Unknown){
					firstmsg = msg;
				}
				if(msg == CtosMessage.CreateGame || msg == CtosMessage.JoinGame || msg == CtosMessage.PlayerInfo){
					
				}else{
					if(!player.IsAuthentified){
						Logger.Warn("auth error:"+player.Name);
						player.CloseAsync();
						break;
					}
					if(player.Type == (int)PlayerType.Undefined){
						Logger.Warn("player type error:"+player.Name);
						player.CloseAsync();
						break;
					}
				}
				if(player.Game!=null){
					lock(player.Game.AsyncRoot){
						EventHandler.Do((ushort)msg, player, packet);
					}
				}else{
					EventHandler.Do((ushort)msg, player, packet);
				}
				packet.Close();
			}
			return firstmsg;
		}

		#endregion
		
		#region 消息
		public static void LobbyError(this GameSession client, string message,bool isNow=true)
		{
			using(GameServerPacket join = new GameServerPacket(StocMessage.JoinGame)){
				join.Write(0U);
				join.Write((byte)0);
				join.Write((byte)0);
				join.Write(0);
				join.Write(0);
				join.Write(0);
				// C++ padding: 5 bytes + 3 bytes = 8 bytes
				for (int i = 0; i < 3; i++)
					join.Write((byte)0);
				join.Write(0);
				join.Write((byte)0);
				join.Write((byte)0);
				join.Write((short)0);
				client.Send(join, false);
			}
			
			using(GameServerPacket enter = new GameServerPacket(StocMessage.HsPlayerEnter)){
				enter.WriteUnicode("[err]" + message, 20);
				enter.Write((byte)0);
				client.Send(enter, isNow);
			}
		}

		public static void ServerMessage(this GameSession client, string msg, PlayerType type=PlayerType.Yellow,bool isNow=true)
		{
			string finalmsg = "[Server] " + msg;
			using(GameServerPacket packet = new GameServerPacket(StocMessage.Chat)){
				packet.Write((short)type);
				packet.WriteUnicode(finalmsg, finalmsg.Length + 1);
				client.Send(packet, isNow);
			}
		}
		#endregion
		
		#region 玩家信息/登录
		public static void OnPlayerInfo(GameSession client, GameClientPacket packet){
			if (client.Name != null)
				return;
			string name = packet.ReadUnicode(20);
			Logger.Debug("player name:"+name);
			if(name == "client"){
				client.LobbyError("[err]404");
				return;
			}
			if (string.IsNullOrEmpty(name)){
				client.LobbyError(Messages.ERR_NO_NAME);
			}
			client.Name = name;
			client.IsAuthentified = client.CheckAuth(name);
		}
		#endregion
		
		#region 加入游戏
		public static bool CheckAuth(this GameSession client, string namepassword){
			if(namepassword==null){
				return true;
			}
			if(!RoomManager.CheckPlayerBan(namepassword)){
				client.ServerMessage(Messages.MSG_PLAYER_BAN);
				return false;
			}
			if(Program.Config.isNeedAuth || namepassword.StartsWith("[AI]")){
				string[] _names=namepassword.Split(new char[]{'$'}, 2);
				if(_names.Length==1){
					client.ServerMessage(Messages.ERR_NO_PASS);
					return false;
				}else{
					if(!RoomManager.OnLogin(_names[0].Trim(),_names[1])){
						//LobbyError("Auth Fail");
						if(Encoding.Default.GetBytes(namepassword).Length>=20){
							client.ServerMessage(Messages.ERR_NAME_PASSWORD_LONG);
						}else{
							client.ServerMessage(Messages.ERR_NAME_PASSWORD);
						}
						return false;
					}
				}
			}
			return true;
		}
		public static void OnJoinGame(GameSession client, GameClientPacket packet){
			if (string.IsNullOrEmpty(client.Name) || client.Type != (int)PlayerType.Undefined){
				Logger.Debug("join room fail:"+client.Name);
				return;
			}
			int version = packet.ReadInt16();
			if (version < Program.Config.ClientVersion)
			{
				client.LobbyError(Messages.ERR_LOW_VERSION);
				return;
			}
			else if (version > Program.Config.ClientVersion){
				client.ServerMessage(Messages.MSG_HIGH_VERSION);
			}
			int gameid = packet.ReadInt32();//gameid
			packet.ReadInt16();

			string joinCommand = packet.ReadUnicode(60);
			
			GameRoom room = null;
			//IsAuthentified = CheckAuth();
			if(!client.IsAuthentified){
				client.LobbyError(Messages.ERR_AUTH_FAIL);
				return;
			}
			if(!RoomManager.CheckRoomPassword(joinCommand)){
				client.LobbyError(Messages.ERR_PASSWORD);
				return;
			}
			GameConfig config = GameConfigBuilder.Build(joinCommand);
			room =  RoomManager.CreateOrGetGame(config);
			if (room == null){
				client.LobbyError(Messages.MSG_FULL);
				return;
			}
			if (!room.IsOpen)
			{
				client.LobbyError(Messages.MSG_GAMEOVER);
				return;
			}
			if(room!=null && room.Config!=null){
				if(room.Config.NoCheckDeck){
					client.ServerMessage(Messages.MSG_NOCHECKDECK);
				}
				if(room.Config.NoShuffleDeck){
					client.ServerMessage(Messages.MSG_NOSHUFFLEDECK);
				}
				if(room.Config.EnablePriority){
					client.ServerMessage(Messages.MSG_ENABLE_PROIORITY);
				}
			}
			client.Game = room;
            lock (room.AsyncRoot)
            {
                room.AddPlayer(client);
            }
		}
		#endregion
		
		#region 创建游戏
		public static void OnCreateGame(GameSession client, GameClientPacket packet){
			if (string.IsNullOrEmpty(client.Name) || client.Type != (int)PlayerType.Undefined)
				return;
			GameRoom room = null;
			GameConfig config = GameConfigBuilder.Build(packet);
			room = RoomManager.CreateOrGetGame(config);

			if (room == null)
			{
				client.LobbyError(Messages.MSG_FULL);
				return;
			}
			client.Game = room;
            lock (room.AsyncRoot)
            {
                room.AddPlayer(client);
            }
			//IsAuthentified = CheckAuth();
			if(!client.IsAuthentified){
				client.LobbyError(Messages.ERR_AUTH_FAIL);
			}
		}
		#endregion
		
		#region 决斗事件
		public static void OnTimeConfirm(GameSession client, GameClientPacket packet){
			if(client!=null){
				//Logger.Debug("OnTimeConfirm "+client.Name);
			}
		}
		public static void SendTypeChange(this GameSession client)
		{
			if(client == null||client.Game==null)return;
			using(GameServerPacket packet = new GameServerPacket(StocMessage.TypeChange)){
				packet.Write((byte)(client.Type + (client.Equals(client.Game.HostPlayer) ? (int)PlayerType.Host : 0)));
				client.Send(packet);
			}
		}
		public static void OnChat(GameSession client, GameClientPacket packet){
			if (!client.IsAuthentified){
				return;
			}
			string msg = packet.ReadUnicode(256);
			if(client.Game==null){
				return;
			}
			if(!client.OnChatCommand(msg)){
				using(GameServerPacket chat = new GameServerPacket(StocMessage.Chat)){
					chat.Write((short)client.Type);
					chat.WriteUnicode(msg, msg.Length + 1);
					client.Game.SendToAllBut(chat, client);
				}
			}
		}
		public static void OnTpResult(GameSession client, GameClientPacket packet){
			bool tp = packet.ReadByte() != 0;
			if(client.Game!=null)
				client.Game.TpResult(client, tp);
		}
		public static void OnUpdateDeck(GameSession client, GameClientPacket packet){
			if (client.Game==null||client.Type == (int)PlayerType.Observer||client.Type == (int)PlayerType.Undefined)
				return;
			Deck deck = new Deck();
			int main = packet.ReadInt32();
			int side = packet.ReadInt32();
			for (int i = 0; i < main; i++)
				deck.AddMain(packet.ReadInt32());
			for (int i = 0; i < side; i++)
				deck.AddSide(packet.ReadInt32());
			if (client.Game.State == GameState.Lobby)
			{
				client.Deck = deck;
				client.Game.IsReady[client.Type] = false;
			}
			else if (client.Game.State == GameState.Side)
			{
				if (client.Game.IsReady[client.Type])
					return;
				if (!client.Deck.Check(deck))
				{
					using(GameServerPacket error = new GameServerPacket(StocMessage.ErrorMsg)){
						error.Write((byte)3);
						error.Write(0);
						client.Send(error);
					}
					return;
				}
				client.Deck = deck;
				client.Game.IsReady[client.Type] = true;
				client.Game.ServerMessage(string.Format(Messages.MSG_READY, client.Name));
				client.Send(GameServerPacket.EmtryMessage(StocMessage.DuelStart));
				client.Game.MatchSide();
			}
		}
		public static void OnResponse(GameSession client, GameClientPacket packet){
			if (client.Game==null||client.Game.State != GameState.Duel)
				return;
			if (client.State != PlayerState.Response)
				return;
			byte[] resp = packet.ReadToEnd();
			if (resp.Length > 64)
				return;
			client.State = PlayerState.None;
			client.Game.SetResponse(resp);
		}
		public static void OnSurrender(GameSession client, GameClientPacket packet){
			if(client.Game!=null)
				client.Game.Surrender(client, 0);
		}
		
		public static void OnHandResult(GameSession client, GameClientPacket packet){
			int res = packet.ReadByte();
			if(client.Game!=null)
				client.Game.HandResult(client, res);
		}
		public static void OnStartDuel(GameSession client, GameClientPacket packet){
			if(client.Game!=null){
				client.Game.StartDuel(client);
			}
		}
		#endregion
		
		#region 决斗前
		public static void OnKick(GameSession client, GameClientPacket packet){
			int pos = packet.ReadByte();
			if(client.Game!=null)
				client.Game.KickPlayer(client, pos);
		}
		public static void OnSetReady(GameSession client, GameClientPacket packet){
			if (!client.IsAuthentified){
				return;
			}
			if(client.Game!=null)
				client.Game.SetReady(client, true);
		}
		public static void OnNotReady(GameSession client, GameClientPacket packet){
			if (!client.IsAuthentified){
				return;
			}
			if(client.Game!=null)
				client.Game.SetReady(client, false);
		}
		public static void OnMoveToDeulList(GameSession client, GameClientPacket packet){
			if (!client.IsAuthentified){
				return;
			}
			if(client.Game!=null)
				client.Game.MoveToDuelist(client);
		}
		public static void OnMoveToObserver(GameSession client, GameClientPacket packet){
			if (!client.IsAuthentified){
				return;
			}
			if(client.Game!=null)
				client.Game.MoveToObserver(client);
		}
		public static void OnLeaveGame(GameSession client, GameClientPacket packet){
			if(client.Game!=null)
				client.Game.RemovePlayer(client);
		}
        #endregion

    }
}
