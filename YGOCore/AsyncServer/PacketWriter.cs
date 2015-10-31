
using System;
using System.IO;

namespace AsyncServer
{
	/// <summary>
	/// Description of PacketWriter.
	/// </summary>
	public class PacketWriter : IDisposable
	{
		protected BinaryWriter m_writer;
		protected MemoryStream m_stream;
		protected int m_PacketByteLength = 4;
		public byte[] Bytes{
			get { return m_stream.ToArray(); }
		}
		public PacketWriter(int packetByteLength)
		{
			m_PacketByteLength = packetByteLength;
			m_stream = new MemoryStream();
			m_writer = new BinaryWriter(m_stream);
		}
		
		public int PacketByteLength{
			get{return m_PacketByteLength;}
		}
		
		public void Write(byte[] array)
		{
			m_writer.Write(array);
		}

		public void Write(bool value)
		{
			m_writer.Write((byte)(value ? 1 : 0));
		}

		public void Write(sbyte value)
		{
			m_writer.Write(value);
		}

		public void Write(byte value)
		{
			m_writer.Write(value);
		}

		public void Write(short value)
		{
			m_writer.Write(value);
		}

		public void Write(int value)
		{
			m_writer.Write(value);
		}

		public void Write(uint value)
		{
			m_writer.Write(value);
		}

		public void WriteUnicode(string text, int len)
		{
			m_writer.WriteUnicode(text, len);
		}

		public long GetPosition()
		{
			return m_stream.Position;
		}

		public void SetPosition(long pos)
		{
			m_stream.Position = pos;
		}
		public void Close(){
			if(m_stream!=null){
				m_stream.Close();
			}
			if(m_writer!=null){
				m_writer.Close();
			}
		}
		public void Dispose(){
			Close();
			m_stream = null;
			m_writer = null;
		}
	}
}
