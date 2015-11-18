using System;
using AsyncServer;
using System.IO;
using OcgWrapper.Enums;

namespace YGOCore.Net
{
	public class GameServerPacket : PacketWriter
	{
		public const int GamePacketByteLength = 2;
		public StocMessage PacketMsg = StocMessage.GameMsg;
		public GameMessage GameMsg = GameMessage.Waiting;
		public GameServerPacket(StocMessage message):base(GamePacketByteLength)
		{
			Write((byte)message);
			this.PacketMsg=message;
		}

		public GameServerPacket(GameMessage message):base(GamePacketByteLength)
		{
			Write((byte)(StocMessage.GameMsg));
			this.PacketMsg=StocMessage.GameMsg;
			Write((byte)message);
			this.GameMsg = message;
		}
		
		public static byte[] EmtryMessage(GameMessage message){
			byte[] data =null;
			using(GameServerPacket packet=new GameServerPacket(message)){
				data = packet.Content;
			}
			return data;
		}
		public static byte[] EmtryMessage(StocMessage message){
			byte[] data =null;
			using(GameServerPacket packet=new GameServerPacket(message)){
				data = packet.Content;
			}
			return data;
		}
	}
}