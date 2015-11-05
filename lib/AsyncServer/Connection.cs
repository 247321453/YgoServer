using System;
using System.Timers;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace AsyncServer {
	#region TimeoutTimer
	/// <summary>
	/// Timeout Timer
	/// </summary>
	public class TimeoutTimer : Timer {
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
	public class Connection<T> : IDisposable{
		
		/// <summary>
		/// Initializes a new instance of the Connection class.
		/// </summary>
		public Connection() {
			timer = new TimeoutTimer(this);
			m_ReceiveQueue = new ArrayQueue<byte>();
		}
		
		#region send
		/// <summary>
		/// 等待需要发出的数据
		/// </summary>
		private readonly Queue<byte[]> m_PendingBuffer = new Queue<byte[]>(32);

		/// <summary>
		/// 向客户端发送数据
		/// </summary>
		/// <param name="buff"></param>
		/// <param name="isSendNow">是否立即发送</param>
		public void SendPackage(byte[] data,bool isSendNow = true)
		{
			if (client == null || !client.Connected)
				return;
			lock (m_PendingBuffer){
				m_PendingBuffer.Enqueue(data);
			}
			if(isSendNow){
				PeekSend();
			}
		}

		/// <summary>
		/// 检查队列里是否有要发送的数据，如果有则进行发送处理
		/// </summary>
		public void PeekSend()
		{
//			if(isAsync){
//				lock(lock_send){
//					if (isSending){
//						Logger.Debug("wait send");
//						return;
//					}
//					isSending = true;
//				}
//			}
			if(!Connected){
				//已经断开
				return;
			}
			Queue<byte[]> arrays = new Queue<byte[]>();
			lock (m_PendingBuffer){
				while(m_PendingBuffer.Count>0){
					arrays.Enqueue(m_PendingBuffer.Dequeue());
				}
			}
			if (arrays.Count > 1)
			{
				//Logger.Debug("send "+arrays.Count);
				//   2 个包以上，进行拼包后再发送
				byte[] datas = null;
				using(MemoryStream stream  =new MemoryStream()){
					using(BinaryWriter writer=new BinaryWriter(stream)){
						while(arrays.Count>0){
							try{
								var buff = arrays.Dequeue();
								writer.Write(buff);
							}catch(IOException){
							}
						}
					}
					datas = stream.ToArray();
				}
				if(datas!=null && datas.Length>0){
					try{
						if(isAsync){
							client.Client.BeginSend(datas, 0, datas.Length, SocketFlags.None,
							                        new AsyncCallback(SendDataEnd), this);
						}else{
							client.Client.Send(datas);
						}
					}catch(SocketException){
					}catch(Exception){
						
					}
					//WriteData(this, datas, isAsync);
					return;
				}
			}
			else if(arrays.Count==1)	{
				//Logger.Debug("send 1");
				var data = arrays.Dequeue();
				if(data!=null && data.Length>0){
					try{
						if(isAsync){
							client.Client.BeginSend(data, 0, data.Length, SocketFlags.None,
							                        new AsyncCallback(SendDataEnd), client);
						}else{
							client.Client.Send(data);
						}
					}catch(SocketException){
					}catch(Exception){
						
					}
					//WriteData(this, data, isAsync);
					return;
				}
			}else{
				//Logger.Debug("nothing send");
			}
//			if(isAsync){
//				lock(lock_send){
//					isSending = false;
//				}
//			}
		}
		private void SendDataEnd(IAsyncResult ar)
		{
			try{
				TcpClient client = (TcpClient)ar.AsyncState;
				client.Client.EndSend(ar);
			}catch(SocketException){
			}catch(Exception){
				
			}
//			if(isAsync){
//				lock(lock_send){
//					isSending = false;
//				}
//			}
		}
		/*
		 * 		private readonly byte[] lock_send = new byte[0];
		
		private bool isSending = false;
		
		/// <summary>
		/// Writes data to a client.
		/// </summary>
		/// <returns><c>true</c>, if data could be written, <c>false</c> otherwise.</returns>
		/// <param name="connection">The connection to write to.</param>
		/// <param name="data">The data to write.</param>
		/// <param name="isAsync">If set to <c>true</c>, the write will be performed asynchronously.</param>
		public static bool WriteData(Connection<T> connection, byte[] bytes, bool isAsync = true) {
			if(bytes == null || bytes.Length==0){
				return false;
			}
			try {
				lock(connection.SyncRoot) {
					if(connection.Connected) {
						if(isAsync) {
							connection.Client.GetStream().BeginWrite(bytes, 0, bytes.Length, WriteCallback, connection);
						}else{
							connection.Client.GetStream().Write(bytes, 0, bytes.Length);
						}
						return connection.Client.GetStream().CanWrite;
					}else{
						return false;
					}
				}
			}catch(System.IO.IOException ex) {
				Logger.Error("Could not write to client: " + ex.ToString() + ".");
				return false;
			}
		}
		
		/// <summary>
		/// Callback handling data being written to a client.
		/// </summary>
		/// <param name="result">Result.</param>
		private static void WriteCallback(System.IAsyncResult result) {
			Connection<T> connection = (Connection<T>) result.AsyncState;
			try {
				lock(connection.SyncRoot) {
					if(connection.Connected) {
						connection.Client.GetStream().EndWrite(result);
						connection.Client.LingerState = new LingerOption(false, 0);
					}else{
						Logger.Debug("don't send");
					}
				}
				lock(connection.lock_send){
					connection.isSending = false;
					Logger.Debug("peek send:"+connection.isSending);
				}
				connection.PeekSend();
			}catch(System.IO.IOException ex) {
				Logger.Error("Could not end write to client: " + ex.ToString() + ".");
			}
		}
		 */
		#endregion
		
		#region member
		private ArrayQueue<byte> m_ReceiveQueue;
		private TimeoutTimer timer;
		public readonly byte[] SyncRoot = new byte[0];
		private TcpClient client;
		private bool _Dispose;
		
		internal byte[] Bytes;
		private T m_tag;
		/// <summary>
		/// 异步发送
		/// </summary>
		public bool isAsync =false;
		
		internal byte[] ResetCache(){
			Bytes = new byte[1024];
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
					Address = IPAddress.None;
				}
			}
		}
		#endregion

		#region public member
		public T Tag{get{return m_tag;}set{m_tag = value;}}
		/// <summary>
		/// Gets the client's address.
		/// </summary>
		/// <value>The client's address.</value>
		public System.Net.IPAddress Address {
			get;
			private set;
		}

		public int Available{
			get{return client.Available;}
		}
		public ArrayQueue<byte> ReceiveQueue{
			get{return m_ReceiveQueue;}
		}
		/// <summary>
		/// Gets a value indicating whether this <see cref="Asterion.Connection"/> is connected.
		/// </summary>
		/// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
		public bool Connected {
			get {
				return ( Client != null && Client.Connected);
			}
		}

		#endregion
		
		#region public method
		/// <summary>
		/// close
		/// </summary>
		public void Close(){
			if(client!=null){
				try{
					client.Close();
					client = null;
				}catch{}
			}
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
			try{
				timer.Stop();
				timer.Close();
			}catch{}
			timer=null;
		}
		#endregion
	}
}