/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/10/29
 * 时间: 21:58
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;

namespace AsyncServer
{
	/// <summary>
	/// Description of ReceiveQueue.
	/// </summary>
	public class ByteArray
	{
		public const int BUFFER_SIZE = 1024*32;
		private int m_Head;
		private int m_Tail;
		private int m_PacketLen;
		private byte[] m_Buffer;
		private object m_LockBuffer = new object();
		private int m_Size;
		
		public byte[] Bytes{
			get{return m_Buffer;}
		}
		/// <summary>
		/// 长度
		/// </summary>
		public int Length
		{
			get
			{
				return this.m_Size;
			}
		}
		
		public ByteArray(int packetLen,int maxLength=BUFFER_SIZE){
			this.m_PacketLen=packetLen;
			m_Buffer = new byte[maxLength];
		}
		/// <summary>
		/// 取
		/// </summary>
		/// <param name="byteBuffer"></param>
		/// <param name="iOffset"></param>
		/// <param name="iSize"></param>
		/// <returns></returns>
		public int Dequeue(byte[] byteBuffer, int iOffset, int iSize)
		{
			if (byteBuffer == null)
			{
				throw new ArgumentNullException("byteBuffer", "ReceiveQueue.Dequeue(...) - byteBuffer == null error!");
			}
			if (iOffset < 0 || iOffset >= byteBuffer.Length)
			{
				throw new Exception("ReceiveQueue.Dequeue(...) - iOffset < 0 || iOffset >= byteBuffer.Length error!");
			}
			if (iSize < 0 || iSize > byteBuffer.Length)
			{
				throw new Exception("ReceiveQueue.Dequeue(...) - iSize < 0 || iSize > byteBuffer.Length error!");
			}
			if (byteBuffer.Length - iOffset < iSize)
			{
				throw new Exception("ReceiveQueue.Dequeue(...) - ( byteBuffer.Length - iOffset ) < iSize error!");
			}
			if (iSize == 0)
			{
				return 0;
			}
			lock (this.m_LockBuffer)
			{
				if (iSize > this.m_Size)
				{
					iSize = this.m_Size;
				}
				if (this.m_Head < this.m_Tail)
				{
					Buffer.BlockCopy(this.m_Buffer, this.m_Head, byteBuffer, iOffset, iSize);
				}
				else
				{
					int num = this.m_Buffer.Length - this.m_Head;
					if (num >= iSize)
					{
						Buffer.BlockCopy(this.m_Buffer, this.m_Head, byteBuffer, iOffset, iSize);
					}
					else
					{
						Buffer.BlockCopy(this.m_Buffer, this.m_Head, byteBuffer, iOffset, num);
						Buffer.BlockCopy(this.m_Buffer, 0, byteBuffer, (iOffset + num), (iSize - num));
					}
				}
				this.m_Head = (this.m_Head + iSize) % this.m_Buffer.Length;
				this.m_Size -= iSize;
				if (this.m_Size == 0)
				{
					this.m_Head = 0;
					this.m_Tail = 0;
				}
			}
			return iSize;
		}
		/// <summary>
		/// 存
		/// </summary>
		/// <param name="byteBuffer"></param>
		/// <param name="iOffset"></param>
		/// <param name="iSize"></param>
		public void Enqueue(byte[] byteBuffer, int iOffset, int iSize)
		{
			if (byteBuffer == null)
			{
				throw new ArgumentNullException("byteBuffer", "ReceiveQueue.Enqueue(...) - byteBuffer == null error!");
			}
			if (iOffset < 0L || iOffset >= byteBuffer.Length)
			{
				throw new Exception("ReceiveQueue.Enqueue(...) - iOffset < 0 || iOffset >= byteBuffer.Length error!");
			}
			if (iSize < 0L || iSize > byteBuffer.Length)
			{
				throw new Exception("ReceiveQueue.Enqueue(...) - iSize < 0 || iSize > byteBuffer.Length error!");
			}
			if (byteBuffer.Length - iOffset < iSize)
			{
				throw new Exception("ReceiveQueue.Enqueue(...) - ( byteBuffer.Length - iOffset ) < iSize error!");
			}
			lock (this.m_LockBuffer)
			{
				if (this.m_Size + iSize >= this.m_Buffer.Length)
				{
					this.SetCapacityInLock(this.m_Size + iSize + 2047L & -2048L);
				}
				if (this.m_Head < this.m_Tail)
				{
					int num = this.m_Buffer.Length - this.m_Tail;
					if (num >= iSize)
					{
						Buffer.BlockCopy(byteBuffer, iOffset, this.m_Buffer, this.m_Tail, iSize);
					}
					else
					{
						Buffer.BlockCopy(byteBuffer, iOffset, this.m_Buffer, this.m_Tail, num);
						Buffer.BlockCopy(byteBuffer, (iOffset + num), this.m_Buffer, 0, (iSize - num));
					}
				}
				else
				{
					Buffer.BlockCopy(byteBuffer, iOffset, this.m_Buffer, this.m_Tail, iSize);
				}
				this.m_Tail = (this.m_Tail + iSize) % this.m_Buffer.Length;
				this.m_Size += iSize;
			}
		}
		public void Clear()
		{
			lock (this.m_LockBuffer)
			{
				this.m_Head = 0;
				this.m_Tail = 0;
				this.m_Size = 0;
			}
		}

		public int GetPacketLength()
		{
			int result = 0;
			lock (this.m_LockBuffer)
			{
				if (m_PacketLen > this.m_Size)
				{
					return 0;
				}
				if (this.m_Head + m_PacketLen < (long)this.m_Buffer.Length)
				{
					int head = (int)this.m_Head;
					switch(m_PacketLen){
						case 1:
							result = (int)m_Buffer[head];
							break;
						case 2:
							result = (int)BitConverter.ToUInt16(m_Buffer, head);
							break;
						case 4:
							result = (int)BitConverter.ToUInt32(m_Buffer, head);
							break;
					}
					return result;
				}
			}
			return 0;
		}
		private void SetCapacityInLock(long iCapacity)
		{
			byte[] array = new byte[iCapacity];
			if (this.m_Size > 0L)
			{
				if (this.m_Head < this.m_Tail)
				{
					Buffer.BlockCopy(this.m_Buffer, (int)this.m_Head, array, 0, (int)this.m_Size);
				}
				else
				{
					long num = (long)this.m_Buffer.Length - this.m_Head;
					Buffer.BlockCopy(this.m_Buffer, (int)this.m_Head, array, 0, (int)num);
					Buffer.BlockCopy(this.m_Buffer, 0, array, (int)num, (int)this.m_Tail);
				}
			}
			this.m_Head = 0;
			this.m_Tail = this.m_Size;
			this.m_Buffer = array;
		}
	}
}
