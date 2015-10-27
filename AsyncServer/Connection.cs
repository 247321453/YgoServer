namespace Asterion {
    using TcpClient = System.Net.Sockets.TcpClient;
    using NetworkStream = System.Net.Sockets.NetworkStream;
    using IPEndPoint = System.Net.IPEndPoint;
    using Diagnostics = System.Diagnostics;

    /// <summary>
    /// Represents an Asterion client connection.
    /// </summary>
    public class Connection {
        private Limits.TimeoutTimer timer;
        internal readonly object SyncRoot = new object();
        private TcpClient client;

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
        internal Limits.TimeoutTimer Timer {
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
                    throw new Exceptions.UnavailableEndPointException("Could not get the client's IP address.", this);
                }
            }
        }

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

        /// <summary>
        /// Gets or sets the tag object.
        /// </summary>
        /// <value>The tag object.</value>
        public object Tag {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Asterion.Connection"/> class.
        /// </summary>
        public Connection() {
            timer = new Limits.TimeoutTimer(this);
        }
    }
        

}