using AsyncServer;
using OcgWrapper.Enums;

namespace YGOCore.Game
{
	public class GameClientPacket : PacketReader
    {
		public byte[] Content { get{return Bytes;} }

		public GameClientPacket(byte[] content):base(content)
        {
        }

        public CtosMessage ReadCtos()
        {
            return (CtosMessage)m_reader.ReadByte();
        }
    }
}