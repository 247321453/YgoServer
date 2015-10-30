using System;
using System.Net.Sockets;
using System.Collections.Generic;

namespace System.Net {
	#region TimeoutTimer
	/// <summary>
	/// Timeout Timer
	/// </summary>
	public class TimeoutTimer : Timers.Timer {
		private object tag;
		
		public object Tag {
			get { return tag; }
		}
		
		public TimeoutTimer(object tag) : base() {
			this.tag = tag;
		}
		
		public void Restart() {
			Stop();
			Start();
		}
	}
	#endregion
	
	/// <summary>
	/// Represents an Asterion client connection.
	/// </summary>
	public class Connection : IDisposable{
				
		public static int sCacheSize = 1024;
		public static int sPacketLength = 2;
		public static int sMaxCacheSize = ByteArray.BUFFER_SIZE;
		private Server m_Server;
		/// <summary>
		/// Initializes a new instance of the Connection class.
		/// </summary>
		public Connection(Server server) {
			m_Server = server;
			timer = new TimeoutTimer(this);
			m_ReceiveQueue = new ByteArray(sPacketLength, sMaxCacheSize);
		}
		

		/// <summary>
		/// 等待需要发出的数据
		/// </summary>
		private readonly Queue<IPacket> m_PendingBuffer = new Queue<IPacket>(32);

		private bool isSending;

		/// <summary>
		/// 向客户端发送数据
		/// </summary>
		/// <param name="buff"></param>
		/// <param name="isSendNow">是否立即发送</param>
		public void SendPackage(IPacket buff, bool isSendNow = true)
		{
			if (client == null || !client.Connected)
				return;
			buff.Use();
			lock (m_PendingBuffer)
			{
				m_PendingBuffer.Enqueue(buff);
			}
			if (isSendNow)
				PeekSend();
		}

		/// <summary>
		/// 检查队列里是否有要发送的数据，如果有则进行发送处理
		/// </summary>
		public void PeekSend()
		{
			lock (m_PendingBuffer)
			{
				if (isSending || m_PendingBuffer.Count == 0)
					return;

				//  TODO 这里要不要考虑进行并报发送处理
				isSending = true;

				if (m_PendingBuffer.Count > 1)
				{
					//   2 个包以上，进行拼包后再发送
					var buffs = m_PendingBuffer.ToArray();
					m_PendingBuffer.Clear();

					int offSet = 0;
					foreach (var b in buffs)
						offSet += b.Length;

					var sendBuff = new ByteArray(sPacketLength, offSet);
					
					foreach (var buff in buffs)
					{
						var bys = buff.GetBytes();
						sendBuff.Enqueue(bys, 0, bys.Length);
					}
					m_Server.WriteData(this, sendBuff.Bytes, true);
				}
				else
				{
					var bys = m_PendingBuffer.Dequeue().GetBytes();
					var sendBuff = new ByteArray(sPacketLength, bys.Length);
					sendBuff.Enqueue(bys, 0, bys.Length);
					m_Server.WriteData(this, sendBuff.Bytes, true);
				}
			}
		}

		#region private member
		private ByteArray m_ReceiveQueue;
		
		private TimeoutTimer timer;
		internal readonly object SyncRoot = new object();
		private TcpClient client;
		private bool _Dispose;
		
		internal byte[] Bytes;
		
		internal byte[] ResetCache(){
			Bytes = new byte[sCacheSize];
			return Bytes;
		}
		/// <summary>
		/// Gets the timeout timer.
		/// </summary>
		/// <value>The timeout timer.</value>
		internal TimeoutTimer Timer {
			get { return timer; }
		}

		/// <summary>
		/// Gets or sets the client.
		/// </summary>
		/// <value>The client.</value>
		internal TcpClient Client {
			get { return client; }
			set {
				client = value;
				try {
					Address = (client.Client.RemoteEndPoint as IPEndPoint).Address;
				}catch{
					throw new UnavailableEndPointException("Could not get the client's IP address.", this);
				}
			}
		}
		#endregion

		#region public member
		/// <summary>
		/// Gets the client's address.
		/// </summary>
		/// <value>The client's address.</value>
		public System.Net.IPAddress Address {
			get;
			private set;
		}

		public ByteArray ReceiveQueue{
			get{return m_ReceiveQueue;}
		}
		/// <summary>
		/// Gets a value indicating whether this <see cref="Asterion.Connection"/> is connected.
		/// </summary>
		/// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
		public bool Connected {
			get {
				lock(SyncRoot)
					return (Client != null && Client.Connected);
			}
		}
		#endregion
		
		#region public method
		/// <summary>
		/// close
		/// </summary>
		public void Close(){
			Dispose();
		}
		/// <summary>
		/// Dispose
		/// </summary>
		public  void Dispose(){
			if(_Dispose){
				return;
			}
			_Dispose = true;
			timer.Close();
			timer=null;
		}
		#endregion
	}
}