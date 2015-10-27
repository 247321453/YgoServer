 using System;
namespace Asterion {
	using TcpClient = System.Net.Sockets.TcpClient;
	using TcpListener = System.Net.Sockets.TcpListener;
	using LingerOption = System.Net.Sockets.LingerOption;
	using ManualResetEvent = System.Threading.ManualResetEvent;
	using ElapsedEventArgs = System.Timers.ElapsedEventArgs;
	using Interlocked = System.Threading.Interlocked;
	using IPAddress = System.Net.IPAddress;
	
	/// <summary>
	/// Receive event handler.
	/// </summary>
	public delegate void ReceiveEventHandler(Connection Client, byte[] datas);
	/// <summary>
	/// Connect event handler.
	/// </summary>
	public delegate void ConnectEventHandler(Connection Client);
	/// <summary>
	/// Disconnect event handler.
	/// </summary>
	public delegate void DisconnectEventHandler(Connection Client);
	/// <summary>
	/// Timeout event handler.
	/// </summary>
	public delegate void TimeoutEventHandler(Connection timeoutConnection, double time);

	/// <summary>
	/// Implaments an asynchronous TCP server.
	/// </summary>
	public class Server {
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
		private bool started = false;
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
		/// Occurs when information is logged by the server.
		/// </summary>
		public event Logging.LogEventHandler OnLog;
		/// <summary>
		/// Occurs when a client times out.
		/// </summary>
		public event TimeoutEventHandler OnTimeout;

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
		/// <summary>
		/// Initializes a new instance of the <see cref="Asterion.Server"/> class.
		/// </summary>
		/// <param name="host">Host to listen on.</param>
		/// <param name="port">Port to listen on.</param>
		/// <param name="capacity">Server capacity.</param>
		/// <param name="timeout">Client timeout time.</param>
		public Server(string host, int port, int capacity = 0, int timeout = 0) {
			Init(IPAddress.Parse(host), port, capacity, timeout);
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Asterion.Server"/> class.
		/// </summary>
		/// <param name="host">Host to listen on.</param>
		/// <param name="port">Port to listen on.</param>
		/// <param name="capacity">Server capacity.</param>
		/// <param name="timeout">Client timeout time.</param>
		public Server(IPAddress host, int port, int capacity = 0, int timeout = 0) {
			Init(host, port, capacity, timeout);
		}
		
		private void Init(IPAddress host, int port, int capacity = 0, int timeout = 0){
			this.host = host;
			this.port = port;
			this.timeout = timeout;
			if(capacity != 0) this.capacity = capacity;
			listener = new TcpListener(host, port);
		}

		/// <summary>
		/// Starts the server.
		/// </summary>
		public void Start() {
			if(!started) {
				started = true;
				Listen();
			}
		}

		/// <summary>
		/// Listens for incoming connections.
		/// </summary>
		private void Listen() {
			listener.Start();
			Log("Awaiting connections...");
			listener.BeginAcceptTcpClient(AcceptCallback, null);
		}


		/// <summary>
		/// Callback for an incoming connection
		/// </summary>
		/// <param name="result">Asynchronous result.</param>
		private void AcceptCallback(System.IAsyncResult result) {
			bool accepting = false;
			try {
				TcpClient client = listener.EndAcceptTcpClient(result);
				listener.BeginAcceptTcpClient(AcceptCallback, null);
				accepting = true;
				client.Client.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, System.Net.Sockets.SocketOptionName.KeepAlive, 1);
				Heard(client);
			}catch(System.Net.Sockets.SocketException ex) {
				// If this happens, socket error code information is at: http://msdn.microsoft.com/en-us/library/windows/desktop/ms740668(v=vs.85).aspx
				Log("Could not accept socket (" + ex.ErrorCode.ToString() + "): " + ex.Message, Logging.LogLevel.Error);
			}catch(Exceptions.AsterionException ex) {
				// Either the server is full or the client has reached the maximum connections per IP.
				Log("Could not add client: " + ex.Message, Logging.LogLevel.Error);
				DisconnectClient(ex.Connection);
			}finally{
				if(!accepting) listener.BeginAcceptTcpClient(AcceptCallback, null);
			}
		}


		/// <summary>
		/// Handles a new client connecting to the server and starts reading from the client.
		/// </summary>
		/// <param name="client">The new client.</param>
		private void Heard(TcpClient client) {
			Connection connection = new Connection {
				Client = client,
			};
			lock(clients_l) {
				if(capacity != 0 && clients >= capacity)
					throw new Exceptions.ServerFullException("Server full, rejecting client with IP '" + connection.Address + "'.", connection);
				clients++;
			}
			if(timeout != 0) {
				connection.Timer.Interval = timeout;
				connection.Timer.Elapsed += TimeoutEventHandler;
				connection.Timer.Start();
			}
			Connected(connection);
			BeginRead(connection);
		}

		/// <summary>
		/// Begins reading from a connected client.
		/// </summary>
		/// <param name="connection">The connection to read from.</param>
		private void BeginRead(Connection connection) {
			try {
				connection.Bytes = new byte[1024];
				lock(connection.SyncRoot)
					if(connection.Connected)
						connection.Client.GetStream().BeginRead(connection.Bytes, 0, 1024, ReceiveCallback, connection);
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
			Connection connection = (Connection) result.AsyncState;
			int read = 0;
			bool connected = false;
			int available = 0;
			lock(connection.SyncRoot)
				if(connection.Connected) {
				read = EndRead(connection, result);
				connected = connection.Client.Connected;
				available = connection.Client.Available;
			}
			if(read != 0 && connected) {
				byte[] datas =new byte[read];
				Array.Copy(connection.Bytes, datas, read);
				if(read != 1024 && available == 0) {
					Received(connection, datas);
				}
				if(connection.Timer.Interval != timeout) connection.Timer.Interval = timeout;
				connection.Timer.Restart();
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
		private int EndRead(Connection connection, System.IAsyncResult result) {
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
		private void DisconnectHandler(Connection connection) {
			lock(clients_l) clients--;
			connection.Timer.Stop();
			Disconnected(connection);
			connection.Timer.Close();
			connection.Client.Close();
		}

		/// <summary>
		/// Writes data to a client.
		/// </summary>
		/// <returns><c>true</c>, if data could be written, <c>false</c> otherwise.</returns>
		/// <param name="connection">The connection to write to.</param>
		/// <param name="data">The data to write.</param>
		/// <param name="isAsync">If set to <c>true</c>, the write will be performed asynchronously.</param>
		public bool WriteData(Connection connection, string data, bool isAsync) {
			try {
				byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);
				lock(connection.SyncRoot) {
					if(connection.Connected) {
						if(isAsync) {
							connection.Client.GetStream().BeginWrite(bytes, 0, data.Length, WriteCallback, connection);
						}else{
							connection.Client.GetStream().Write(bytes, 0, data.Length);
						}
						return connection.Client.GetStream().CanWrite;
					}else{
						return false;
					}
				}
			}catch(System.IO.IOException ex) {
				Log("Could not write to client: " + ex.Message + ".", Logging.LogLevel.Error);
				return false;
			}
		}
		
		/// <summary>
		/// Writes data to a client.
		/// </summary>
		/// <returns><c>true</c>, if data could be written, <c>false</c> otherwise.</returns>
		/// <param name="connection">The connection to write to.</param>
		/// <param name="data">The data to write.</param>
		public bool WriteData(Connection connection, string data) {
			return WriteData(connection, data, true);
		}

		/// <summary>
		/// Callback handling data being written to a client.
		/// </summary>
		/// <param name="result">Result.</param>
		private void WriteCallback(System.IAsyncResult result) {
			Connection connection = (Connection) result.AsyncState;
			try {
				lock(connection.SyncRoot) {
					if(connection.Connected) {
						connection.Client.GetStream().EndWrite(result);
						connection.Client.LingerState = new LingerOption(false, 0);
					}
				}
			}catch(System.IO.IOException ex) {
				Log("Could not end write to client: " + ex.Message + ".", Logging.LogLevel.Error);
			}
		}

		/// <summary>
		/// Disconnects the connection from the server.
		/// </summary>
		/// <param name="connection">The connection to disconnect.</param>
		public void DisconnectClient(Connection connection) {
			try {
				lock(connection.SyncRoot)
					if(connection.Connected) {
					connection.Client.Client.Shutdown(System.Net.Sockets.SocketShutdown.Both);
					connection.Client.Close();
				}
			}catch(System.Exception e) {
				Log("Could not disconnect socket: " + e.Message, Asterion.Logging.LogLevel.Error);
			}
		}


		/// <summary>
		/// Timer elapse event handler, raised only when the client has timed out
		/// </summary>
		/// <param name="source">The timer that elapsed.</param>
		/// <param name="e">Event arguments.</param>
		private void TimeoutEventHandler(object source, ElapsedEventArgs e) {
			Limits.TimeoutTimer timer = (Limits.TimeoutTimer) source;
			timer.Stop();
			Connection connection = (Connection) timer.Tag;
			if(OnTimeout != null) OnTimeout(connection, timer.Interval);
		}
		
		/// <summary>
		/// Raises the OnConnect event.
		/// </summary>
		/// <param name="client">Client.</param>
		private void Connected(Connection client) {
			if(OnConnect != null) OnConnect(client);
		}
		
		/// <summary>
		/// Raises the on receive event.
		/// </summary>
		/// <param name="client">Client.</param>
		/// <param name="packet">Packet.</param>
		private void Received(Connection client, byte[] packet) {
			if(OnReceive != null) OnReceive(client, packet);
		}
		
		/// <summary>
		/// Raises the OnDisconnect event.
		/// </summary>
		/// <param name="client">Client.</param>
		private void Disconnected(Connection client) {
			if(OnDisconnect != null) OnDisconnect(client);
		}

		/// <summary>
		/// Raises the OnLog event.
		/// </summary>
		/// <param name="text">Log text.</param>
		/// <param name="level">Log level.</param>
		public void Log(string text, Logging.LogLevel level) {
			if(OnLog != null) OnLog(text, level);
		}

		/// <summary>
		/// Raises the OnLog event.
		/// </summary>
		/// <param name="text">Log text.</param>
		public void Log(string text) {
			Log(text, Logging.LogLevel.Info);
		}
	}

}