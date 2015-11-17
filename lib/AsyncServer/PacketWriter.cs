
using System;

namespace System.IO
{
	/// <summary>
	/// Description of PacketWriter.
	/// </summary>
	public class PacketWriter : BinaryWriter
	{
		protected MemoryStream m_stream;
		protected int m_PacketByteLength = 4;
		public byte[] Bytes{
			get { return m_stream.ToArray(); }
		}
		public byte[] Content{
			get{
				if(!appendLength)
					Use();
				return content;
			}
		}
		public int PacketByteLength{
			get{return m_PacketByteLength;}
		}
		private bool appendLength = false;
		private byte[] content;
		
		public PacketWriter(int packetByteLength):base(new MemoryStream())
		{
			m_PacketByteLength = (packetByteLength == 2 )?2:4;
			m_stream = (MemoryStream)OutStream;
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
