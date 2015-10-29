using System;
using System.Net.Sockets;

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
		/// <summary>
		/// Initializes a new instance of the Connection class.
		/// </summary>
		public Connection() {
			timer = new TimeoutTimer(this);
		}
		#region private member
		private TimeoutTimer timer;
		internal readonly object SyncRoot = new object();
		private TcpClient client;
		private bool _Dispose;
		/// <summary>
		/// Bytes received from the connection temporarily stored here.
		/// </summary>
		/// <value>The received bytes.</value>
		internal byte[] Bytes {
			get;
			set;
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