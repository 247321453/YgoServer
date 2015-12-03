
using OcgWrapper.Enums;
using System.IO;

namespace YGOCore.Net
{
	public class GameClientPacket : PacketReader
    {
		public byte[] Content { get{return Bytes;} }

		public GameClientPacket(byte[] content):base(content)
        {
        }
		private byte ctos = byte.MaxValue;
		public byte CtosValue{get{return ctos;}}
        public CtosMessage ReadCtos()
        {
        	if(ctos == byte.MaxValue){
        		ctos = ReadByte();
        	}
        	if(CtosMessage.IsDefined(typeof(CtosMessage), ctos)){
        		return (CtosMessage)ctos;
        	}
        	return CtosMessage.Unknown;
        }
    }
}