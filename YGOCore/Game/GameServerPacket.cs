using AsyncServer;
using OcgWrapper.Enums;

namespace YGOCore.Game
{
	public class GameServerPacket : PacketWriter
	{
		public byte[] Content{
			get{return Bytes;}
		}
		
		public GameServerPacket(StocMessage message):base(2)
		{
			m_writer.Write((byte)message);
		}

		public GameServerPacket(GameMessage message):base(2)
		{
			m_writer.Write((byte)(StocMessage.GameMsg));
			m_writer.Write((byte)message);
		}

	}
}