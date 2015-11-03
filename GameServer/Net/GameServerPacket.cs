using System;
using AsyncServer;
using System.IO;
using OcgWrapper.Enums;

namespace YGOCore.Net
{
	public class GameServerPacket : PacketWriter
	{
		const int GamePacketByteLength = 2;
		public byte[] Content{
			get{return content;}
		}
		private bool appendLength = false;
		private byte[] content;
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
		/// <summary>
		/// 添加包长度
		/// </summary>
		public void Use(){
			if(appendLength) return;
			appendLength = true;
			byte[] raw = Bytes;
			using(MemoryStream stream = new MemoryStream(raw.Length + m_PacketByteLength)){
				using(BinaryWriter writer = new BinaryWriter(stream)){
					writer.Write((ushort)raw.Length);
					writer.Write(raw);
				}
				content = stream.ToArray();
			}
			Close();
		}
	}
}