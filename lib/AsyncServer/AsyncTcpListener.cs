using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Timers;
using System.Net;

namespace AsyncServer{
	
	/// <summary>
	/// Implaments an asynchronous TCP server.
	/// </summary>
	public class AsyncTcpListener<T> {
		
		#region delegate
		/// <summary>
		/// Receive event handler.
		/// </summary>
		public delegate void ReceiveEventHandler(Connection<T> Client);
		/// <summary>
		/// Connect event handler.
		/// </summary>
		public delegate void ConnectEventHandler(Connection<T> Client);
		/// <summary>
		/// Disconnect event handler.
		/// </summary>
		public delegate void DisconnectEventHandler(Connection<T> Client);
		/// <summary>
		/// Timeout event handler.
		/// </summary>
		public delegate void TimeoutEventHandler(Connection<T> timeoutConnection, double time);
		#endregion
		
		#region private member
		/// <summary>
		/// The listening socket.
		/// </summary>
		private TcpListener listener;
		/// <summary>
		/// The host the server will listen on.
		/// </summary>
		private IPAddress host;
		/// <summary>
		/// The port the server will listen on.
		/// </summary>
		private int port;
		/// <summary>
		/// The client count lock.
		/// </summary>
		private object clients_l = new object();
		/// <summary>
		/// The clients number of clients on the server.
		/// </summary>
		private volatile int clients    = 0;
		/// <summary>
		/// The maximum clients the server will hold.
		/// </summary>
		private int capacity   = 0;
		/// <summary>
		/// Client timeout. The maximum amount of time a client is permitted not to send data for.
		/// </summary>
		private double timeout = 0;
		/// <summary>
		/// Whether the server has been started.
		/// </summary>
		public bool Started = false;
		
		private List<Connection<T>> m_clients;
		/// <summary>
		/// Occurs when a packet has been received.
		/// </summary>
		public event ReceiveEventHandler OnReceive;
		/// <summary>
		/// Occurs when a client has connected.
		/// </summary>
		public event ConnectEventHandler OnConnect;
		/// <summary>
		/// Occurs when a client disconnects.
		/// </summary>
		public event DisconnectEventHandler OnDisconnect;
		/// <summary>
		/// Occurs when a client times out.
		/// </summary>
		public event TimeoutEventHandler OnTimeout;
		#endregion
		
		#region public member
		public List<Connection<T>> Clients{
			get{return m_clients;}
		}
		/// <summary>
		/// Gets the host the server is listening in.
		/// </summary>
		/// <value>The host.</value>
		public IPAddress Host {
			get { return host; }
		}
		/// <summary>
		/// Gets the port the server is listening on.
		/// </summary>
		/// <value>The port.</value>
		public int Port {
			get { return port; }
		}
		/// <summary>
		/// Gets the count of clients currently connected to the server.
		/// </summary>
		/// <value>The count.</value>
		public int Count {
			get { return clients; }
		}
		/// <summary>
		/// Gets the client capacity of the server.
		/// </summary>
		/// <value>The client capacity of the server.</value>
		public int Capacity {
			get { return capacity; }
		}
		/// <summary>
		/// Gets the timeout time.
		/// </summary>
		/// <value>The timeout time.</value>
		public double Timeout {
			get { return timeout; }
		}
		#endregion
		
		#region Initializes
		/// <summary>
		/// Initializes a new instance of the <see cref="Asterion.Server"/> class.
		/// </summary>
		/// <param name="host">Host to listen on.</param>
		/// <param name="port">Port to listen on.</param>
		/// <param name="capacity">Server capacity.</param>
		/// <param name="timeout">Client timeout time.</param>
		public AsyncTcpListener(IPAddress host, int port, int capacity = 0, int timeout = 0) {
			Init(host, port, capacity, timeout);
		}
		
		internal void Init(IPAddress host, int port, int capacity, int timeout){
			this.host = host;
			this.port = port;
			this.timeout = timeout;
			if(capacity != 0) this.capacity = capacity;
			listener = new TcpListener(host, port);
			m_clients = new List<Connection<T>>();
		}
		#endregion

		#region private method
		/// <summary>
		/// Listens for incoming connections.
		/// </summary>
		private void Listen() {
			listener.Start();
			listener.Server.UseOnlyOverlappedIO = true;
			listener.BeginAcceptTcpClient(AcceptCallback, null);
		}


		/// <summary>
		/// Callback for an incoming connection
		/// </summary>
		/// <param name="result">Asynchronous result.</param>
		private void AcceptCallback(System.IAsyncResult result) {
			TcpClient client  = null;
			try {
				client = listener.EndAcceptTcpClient(result);
				client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, 1);
				Heard(client);
			}catch(SocketException ex) {
				// If this happens, socket error code information is at: http://msdn.microsoft.com/en-us/library/windows/desktop/ms740668(v=vs.85).aspx
				Logger.Error("Could not accept socket (" + ex.ErrorCode.ToString() + "): " + ex.ToString());
			}catch(Exception ex) {
				// Either the server is full or the client has reached the maximum connections per IP.
				Logger.Error("Could not add client: " + ex.ToString());
				if(client!=null){
					DisconnectClient(client);
				}
			}finally{
			}
			listener.BeginAcceptTcpClient(AcceptCallback, null);
		}


		/// <summary>
		/// Handles a new client connecting to the server and starts reading from the client.
		/// </summary>
		/// <param name="client">The new client.</param>
		private void Heard(TcpClient client) {
			Connection<T> connection = new Connection<T>() {
				Client = client,
			};
			lock(clients_l) {
				if(capacity != 0 && clients >= capacity){
					DisconnectClient(connection);
					return;
				}else{
					clients++;
					m_clients.Add(connection);
				}
			}
			if(timeout != 0) {
				connection.Timer.Interval = timeout;
				connection.Timer.Elapsed += OnTimeoutEventHandler;
				connection.Timer.Start();
			}
			Logger.Debug("accpet client");
			Connected(connection);
			BeginRead(connection);
		}
		/// <summary>
		/// Timer elapse event handler, raised only when the client has timed out
		/// </summary>
		/// <param name="source">The timer that elapsed.</param>
		/// <param name="e">Event arguments.</param>
		private void OnTimeoutEventHandler(object source, ElapsedEventArgs e) {
			TimeoutTimer timer = (TimeoutTimer) source;
			timer.Stop();
			Connection<T> connection = (Connection<T>) timer.Tag;
			if(OnTimeout != null) OnTimeout(connection, timer.Interval);
		}
		/// <summary>
		/// Begins reading from a connected client.
		/// </summary>
		/// <param name="connection">The connection to read from.</param>
		private void BeginRead(Connection<T> connection) {
			try {
				byte[] cache = connection.ResetCache();
				lock(connection.SyncRoot)
					if(connection.Connected)
						connection.Client.GetStream().BeginRead(cache, 0, cache.Length, ReceiveCallback, connection);
					else
						DisconnectHandler(connection);
			}catch(System.IO.IOException) {
				DisconnectHandler(connection);
			}
		}

		/// <summary>
		/// Callback for received data.
		/// </summary>
		/// <param name="result">Asynchronous result.</param>
		private void ReceiveCallback(System.IAsyncResult result) {
			Connection<T> connection = (Connection<T>) result.AsyncState;
			int read = 0;
			bool connected = false;
			int available = 0;
			lock(connection.SyncRoot)
				if(connection.Connected) {
				read = EndRead(connection, result);
				connected = connection.Client.Connected;
				available = connection.Client.Available;
			}
		//	Logger.Debug("receive:"+read);
			if(read != 0 && connected) {
				connection.ReceiveQueue.Enqueue(connection.Bytes, 0, read);
				if(read == connection.Bytes.Length){
					//还有内容
				}else{
					if(available == 0) {
						Received(connection);
					}
				}
				if(timeout >0){
					if(connection.Timer.Interval != timeout)
						connection.Timer.Interval = timeout;
					connection.Timer.Restart();
				}
				BeginRead(connection);
			}else{
				DisconnectHandler(connection);
			}
		}

		/// <summary>
		/// Ends reading from a client.
		/// </summary>
		/// <returns>The read.</returns>
		/// <param name="connection">The connection to end reading from.</param>
		/// <param name="result">Asynchronous result.</param>
		private int EndRead(Connection<T> connection, System.IAsyncResult result) {
			try {
				lock(connection.SyncRoot)
					if(connection.Connected)
						return connection.Client.GetStream().EndRead(result);
					else
						return 0;
			}catch(System.IO.IOException) {
				return 0;
			}
		}

		/// <summary>
		/// Handles a client disconnect
		/// </summary>
		/// <param name="connection">Disconnected connection.</param>
		private void DisconnectHandler(Connection<T> connection) {
			Logger.Debug("Disconnect client");
			lock(clients_l) {
				clients--;
				m_clients.Remove(connection);
			}
			Disconnected(connection);
			try{
				if(timeout>0){
					connection.Timer.Stop();
					connection.Timer.Close();
				}
				connection.Client.Close();
			}catch(Exception){
				//Logger.Error("Close error:"+e.ToString());
			}
		}
		#endregion

		#region start/close
		/// <summary>
		/// Starts the server.
		/// </summary>
		public void Start() {
			if(!Started) {
				Started = true;
				Listen();
			}
		}
		
		/// <summary>
		/// Disconnects the connection from the server.
		/// </summary>
		/// <param name="connection">The connection to disconnect.</param>
		public void DisconnectClient(Connection<T> connection) {
			try {
				lock(connection.SyncRoot)
					if(connection.Connected) {
					connection.Client.Client.Shutdown(SocketShutdown.Both);
					connection.Client.Close();
				}
			}catch(System.Exception e) {
				Logger.Error("Could not disconnect socket: " + e.ToString());
			}
		}
		private void DisconnectClient(TcpClient connection) {
			try {
				if(connection.Connected) {
					connection.Client.Shutdown(SocketShutdown.Both);
					connection.Client.Close();
				}
			}catch(System.Exception e) {
				Logger.Error("Could not disconnect socket: " + e.ToString());
			}
		}
		public void Stop(){
			lock(clients_l){
				foreach(Connection<T> client in m_clients){
					client.Close();
				}
			}
			listener.Stop();
		}
		#endregion
		
		#region Events
		/// <summary>
		/// Raises the OnConnect event.
		/// </summary>
		/// <param name="client">Client.</param>
		private void Connected(Connection<T> client) {
			if(OnConnect != null) OnConnect(client);
		}
		
		/// <summary>
		/// Raises the on receive event.
		/// </summary>
		/// <param name="client">Client.</param>
		/// <param name="packet">Packet.</param>
		private void Received(Connection<T> client) {
			if(OnReceive != null) OnReceive(client);
		}
		
		/// <summary>
		/// Raises the OnDisconnect event.
		/// </summary>
		/// <param name="client">Client.</param>
		private void Disconnected(Connection<T> client) {
			if(OnDisconnect != null) OnDisconnect(client);
		}
		#endregion
	}

}