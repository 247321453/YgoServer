
using System;
using System.IO;

namespace AsyncServer
{
	/// <summary>
	/// Description of PacketReader.
	/// </summary>
	public class PacketReader : IDisposable
	{
		protected byte[] data;
		protected BinaryReader m_reader;
		protected MemoryStream m_stream;
		public byte[] Bytes{
			get { return data; }
		}
		public int Length {get{return data==null?0: data.Length;}}
		public PacketReader(byte[] data)
		{
			m_stream = new MemoryStream(data);
			m_reader = new BinaryReader(m_stream);
		}

		public BinaryReader Reader(){
			return m_reader;
		}
		public bool ReadBoolean()
		{
			return m_reader.ReadByte() ==1;
		}
		public byte ReadByte()
		{
			return m_reader.ReadByte();
		}

		public byte[] ReadToEnd()
		{
			return m_reader.ReadBytes((int)m_reader.BaseStream.Length - (int)m_reader.BaseStream.Position);
		}

		public sbyte ReadSByte()
		{
			return m_reader.ReadSByte();
		}
		public ushort ReadUInt16()
		{
			return m_reader.ReadUInt16();
		}
		public short ReadInt16()
		{
			return m_reader.ReadInt16();
		}

		public int ReadInt32()
		{
			return m_reader.ReadInt32();
		}

		public uint ReadUInt32()
		{
			return m_reader.ReadUInt32();
		}

		public string ReadUnicode(int len)
		{
			//0 2 4
			if(m_stream.Position == m_stream.Length){
				return "";
			}
			if(m_stream.Position + len > m_stream.Length ){
				len = (int)(m_stream.Length - m_stream.Position);
			}
			return m_reader.ReadUnicode(len);
		}

		public long GetPosition()
		{
			return m_reader.BaseStream.Position;
		}

		public void SetPosition(long pos)
		{
			m_reader.BaseStream.Position = pos;
		}
		
		public void Close(){
			if(m_stream!=null){
				m_stream.Close();
			}
			if(m_reader!=null){
				m_reader.Close();
			}
		}
		public void Dispose(){
			Close();
			m_stream = null;
			m_reader = null;
		}
	}
}
