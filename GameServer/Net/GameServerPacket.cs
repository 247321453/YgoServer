using System;
using AsyncServer;
using System.IO;
using OcgWrapper.Enums;

namespace YGOCore.Net
{
	public class GameServerPacket : PacketWriter
	{
		const int GamePacketByteLength = 2;
		public StocMessage PacketMsg = StocMessage.GameMsg;
		public GameMessage GameMsg = GameMessage.Waiting;
		public GameServerPacket(StocMessage message):base(GamePacketByteLength)
		{
			m_writer.Write((byte)message);
			this.PacketMsg=message;
		}

		public GameServerPacket(GameMessage message):base(GamePacketByteLength)
		{
			m_writer.Write((byte)(StocMessage.GameMsg));
			this.PacketMsg=StocMessage.GameMsg;
			m_writer.Write((byte)message);
			this.GameMsg = message;
		}
	}
}