
using System;
using System.Collections.Generic;
using AsyncServer;
using OcgWrapper.Enums;
using YGOCore.Game;

namespace YGOCore.Net
{
	/// <summary>
	/// 游戏事件处理
	/// </summary>
	public static class GameEvent
	{
		#region 消息匹配
		public static void Handler(GameSession player, List<GameClientPacket> packets){
			foreach(GameClientPacket packet in packets){
				Parse(player, packet);
			}
		}
		
		
		private static void Parse(GameSession m_player, GameClientPacket packet)
		{
			CtosMessage msg = packet.ReadCtos();
			Logger.Debug("CtosMessage:"+msg);
			switch (msg)
			{
				case CtosMessage.PlayerInfo:
					m_player.OnPlayerInfo(packet);
					break;
				case CtosMessage.JoinGame:
					m_player.OnJoinGame(packet);
					break;
				case CtosMessage.CreateGame:
					m_player.OnCreateGame(packet);
					break;
				case CtosMessage.Unknown:
					Logger.Error("unknown packet id:"+packet.CtosValue);
					return;
			}
			if (!m_player.IsAuthentified){
				return;
			}
			switch (msg)
			{
				case CtosMessage.Chat:
					m_player.OnChat(packet);
					break;
				case CtosMessage.HsToDuelist:
					m_player.OnMoveTo(true);
					break;
				case CtosMessage.HsToObserver:
					m_player.OnMoveTo(false);
					break;
				case CtosMessage.LeaveGame:
					m_player.OnLeaveGame(packet);
					break;
				case CtosMessage.HsReady:
					m_player.OnSetReady(true);
					break;
				case CtosMessage.HsNotReady:
					m_player.OnSetReady(false);
					break;
				case CtosMessage.HsKick:
					m_player.OnKick(packet);
					break;
				case CtosMessage.HsStart:
					m_player.OnStartDuel(packet);
					break;
				case CtosMessage.HandResult:
					m_player.OnHandResult(packet);
					break;
				case CtosMessage.TpResult:
					m_player.OnTpResult(packet);
					break;
				case CtosMessage.UpdateDeck:
					m_player.OnUpdateDeck(packet);
					break;
				case CtosMessage.Response:
					m_player.OnResponse(packet);
					break;
				case CtosMessage.Surrender:
					m_player.OnSurrender(packet);
					break;
			}
		}
		#endregion
		
		#region 消息
		public static void LobbyError(this GameSession client, string message)
		{
			GameServerPacket join = new GameServerPacket(StocMessage.JoinGame);
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
			client.Send(join);

			GameServerPacket enter = new GameServerPacket(StocMessage.HsPlayerEnter);
			enter.WriteUnicode("[err]" + message, 20);
			enter.Write((byte)0);
			client.Send(enter);
		}

		public static void ServerMessage(this GameSession client, string msg, PlayerType type=PlayerType.Yellow, string head="[Server] ")
		{
			string finalmsg = head + msg;
			GameServerPacket packet = new GameServerPacket(StocMessage.Chat);
			packet.Write((short)type);
			packet.WriteUnicode(finalmsg, finalmsg.Length + 1);
			client.Send(packet);
		}
		#endregion
		
		#region 玩家信息/登录
		public static void OnPlayerInfo(this GameSession client, GameClientPacket packet){
			if (client.Name != null)
				return;
			string name = packet.ReadUnicode(20);
			Logger.Debug("player name:"+name);
			if (string.IsNullOrEmpty(name)){
				client.LobbyError(Messages.ERR_NO_NAME);
			}
			client.Name = name;
			client.IsAuthentified = client.CheckAuth(name);
			if(client.IsAuthentified){
				client.ServerMessage(MsgSystem.getMessage(client.Name, 0), PlayerType.White);
			}
		}
		#endregion
		
		#region 加入游戏
		public static void OnJoinGame(this GameSession client, GameClientPacket packet){
			if (string.IsNullOrEmpty(client.Name) || client.Type != (int)PlayerType.Undefined){
				Logger.Debug("join room fail:"+client.Name);
				return;
			}
			int version = packet.ReadInt16();
			if (version < client.Server.Config.ClientVersion)
			{
				client.LobbyError(Messages.ERR_LOW_VERSION);
				return;
			}
			else if (version > client.Server.Config.ClientVersion){
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
			if(!client.Server.CheckRoomPassword(joinCommand)){
				client.LobbyError(Messages.ERR_PASSWORD);
				return;
			}
			GameConfig config = GameConfig.Parse(client.Server, joinCommand);
			if(string.IsNullOrEmpty(joinCommand) ||joinCommand.ToLower()=="random"){
				room = client.Server.GetRandomGame();
			}
			if (room == null){
				room =  client.Server.CreateOrGetGame(config);
			}
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
			Logger.Debug("room "+room.Name+" add "+client.Name);
			room.AddPlayer(client);
		}
		#endregion
		
		#region 创建游戏
		public static void OnCreateGame(this GameSession client, GameClientPacket packet){
			if (string.IsNullOrEmpty(client.Name) || client.Type != (int)PlayerType.Undefined)
				return;
			GameRoom room = null;
			GameConfig config =GameConfig.Parse(client.Server, packet);
			room = client.Server.CreateOrGetGame(config);

			if (room == null)
			{
				client.LobbyError(Messages.MSG_FULL);
				return;
			}
			room.AddPlayer(client);
			//IsAuthentified = CheckAuth();
			if(!client.IsAuthentified){
				client.LobbyError(Messages.ERR_AUTH_FAIL);
			}
		}
		#endregion
		
		#region 决斗事件
		public static void SendTypeChange(this GameSession client)
		{
			if(client.Game==null)return;
			GameServerPacket packet = new GameServerPacket(StocMessage.TypeChange);
			packet.Write((byte)(client.Type + (client.Game.HostPlayer.Equals(client) ? (int)PlayerType.Host : 0)));
			client.Send(packet);
		}
		public static void OnChat(this GameSession client, GameClientPacket packet){
			string msg = packet.ReadUnicode(256);
			if(client.Game==null){
				return;
			}
			GameServerPacket chat = new GameServerPacket(StocMessage.Chat);
			chat.Write((short)client.Type);
			chat.WriteUnicode(msg, msg.Length + 1);
			client.Game.SendToAllBut(chat, client);
		}
		public static void OnTpResult(this GameSession client, GameClientPacket packet){
			bool tp = packet.ReadByte() != 0;
			if(client.Game!=null)
				client.Game.TpResult(client, tp);
		}
		public static void OnUpdateDeck(this GameSession client, GameClientPacket packet){
			if (client.Game==null||client.Type == (int)PlayerType.Observer)
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
					GameServerPacket error = new GameServerPacket(StocMessage.ErrorMsg);
					error.Write((byte)3);
					error.Write(0);
					client.Send(error);
					return;
				}
				client.Deck = deck;
				client.Game.IsReady[client.Type] = true;
				client.Game.ServerMessage(string.Format(Messages.MSG_READY, client.Name));
				client.Send(new GameServerPacket(StocMessage.DuelStart));
				client.Game.MatchSide();
			}
		}
		public static void OnResponse(this GameSession client, GameClientPacket packet){
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
		public static void OnSurrender(this GameSession client, GameClientPacket packet){
			if(client.Game!=null)
				client.Game.Surrender(client, 0);
		}
		
		public static void OnHandResult(this GameSession client, GameClientPacket packet){
			int res = packet.ReadByte();
			if(client.Game!=null)
				client.Game.HandResult(client, res);
		}
		public static void OnStartDuel(this GameSession client, GameClientPacket packet){
			if(client.Game!=null){
				client.Game.StartDuel(client);
			}
		}
		#endregion
		
		#region 决斗前
		public static void OnKick(this GameSession client, GameClientPacket packet){
			int pos = packet.ReadByte();
			if(client.Game!=null)
				client.Game.KickPlayer(client, pos);
		}
		public static void OnSetReady(this GameSession client, bool ready){
			if(client.Game!=null)
				client.Game.SetReady(client, ready);
		}

		public static void OnMoveTo(this GameSession client, bool isdeul){
			if(client.Game==null)return;
			if(isdeul){
				client.Game.MoveToDuelist(client);
			}else{
				client.Game.MoveToObserver(client);
			}
		}
		public static void OnLeaveGame(this GameSession client, GameClientPacket packet){
			if(client.Game!=null)
				client.Game.RemovePlayer(client);
		}
		#endregion
		
	}
}
