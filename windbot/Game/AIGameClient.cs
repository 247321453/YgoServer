using System.Net;
using System.Text;
using WindBot.Game;
using WindBot.Game.AI;
using WindBot.Game.Data;
using WindBot.Game.Network;
using OcgWrapper.Enums;

namespace WindBot.Game
{
	public class AIGameClient
	{
		public AIGameConnection Connection { get; private set; }
		public string Username;
		public string Deck_;

		private string _serverHost;
		private int _serverPort;
		private string _roomInfos;

		private GameBehavior _behavior;

		public AIGameClient(string username, string deck, int serverPort, string roomInfos)
			:this(username, deck, "localhost", serverPort,roomInfos)
		{
		}
		public AIGameClient(string username, string deck, string serverHost, int serverPort, string roomInfos)
		{
			Username = username;
			Deck_ = deck;
			_serverHost = serverHost;
			_serverPort = serverPort;
			_roomInfos = roomInfos;
		}

		public void Start()
		{
			Connection = new AIGameConnection(IPAddress.Parse(_serverHost), _serverPort);
			_behavior = new GameBehavior(this);

			GameClientPacket packet = new GameClientPacket(CtosMessage.PlayerInfo);
			packet.Write(Username, 20);
			Connection.Send(packet);

			byte[] junk = { 0xCC, 0xCC, 0x00, 0x00, 0x00, 0x00 };
			packet = new GameClientPacket(CtosMessage.JoinGame);
			packet.Write(Program.ProVersion);
			packet.Write(junk);

			packet.Write(_roomInfos, 30);

			Deck deck = Deck.Load(Deck_);
			if(deck == null) {
				return;
			}

			packet.Write((byte)CtosMessage.UpdateDeck);

			packet.Write(deck.Cards.Count);
			packet.Write(deck.SideCards.Count);

			foreach (var k in deck.Cards) {
				packet.Write(k.Id);
			}

			foreach (var k in deck.SideCards) {
				packet.Write(k.Id);
			}

			packet.Write((byte)CtosMessage.HsReady);
			Connection.Send(packet);
		}

		public void Tick()
		{
			if (!Connection.IsConnected)
			{
				return;
			}
			while (Connection.HasPacket())
			{
				GameServerPacket packet = Connection.Receive();
				_behavior.OnPacket(packet);
			}
		}

		public void Chat(string message)
		{
			byte[] content = Encoding.Unicode.GetBytes(message + "\0");
			GameClientPacket chat = new GameClientPacket(CtosMessage.Chat);
			chat.Write(content);
			Connection.Send(chat);
		}
	}
}