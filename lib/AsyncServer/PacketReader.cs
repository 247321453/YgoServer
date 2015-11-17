
using System;

namespace System.IO
{
	/// <summary>
	/// Description of PacketReader.
	/// </summary>
	public class PacketReader : BinaryReader
	{
		public byte[] Bytes{
			get { return data; }
		}
		public int Length {get{return data==null?0: data.Length;}}
		protected byte[] data;
		protected MemoryStream m_stream;
		public PacketReader(byte[] data):base(new MemoryStream(data))
		{
			this.data=data;
			m_stream = (MemoryStream)BaseStream;
		}

		public long GetPosition()
		{
			return m_stream.Position;
		}

		public void SetPosition(long pos)
		{
			m_stream.Position = pos;
		}
	}
}
