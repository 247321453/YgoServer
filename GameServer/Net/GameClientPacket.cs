using AsyncServer;
using OcgWrapper.Enums;

namespace YGOCore.Net
{
	public class GameClientPacket : PacketReader
    {
		public byte[] Content { get{return Bytes;} }

        public new int Length { get { return (int)(m_stream.Length- m_stream.Position); } }

        public GameClientPacket(byte[] content):base(content)
        {
        }
		private byte ctos = byte.MaxValue;
		public byte CtosValue{get{return ctos;}}
        public CtosMessage ReadCtos()
        {
        	if(ctos == byte.MaxValue){
        		ctos = m_reader.ReadByte();
        	}
        	if(CtosMessage.IsDefined(typeof(CtosMessage), ctos)){
        		return (CtosMessage)ctos;
        	}
        	return CtosMessage.Unknown;
        }
    }
}