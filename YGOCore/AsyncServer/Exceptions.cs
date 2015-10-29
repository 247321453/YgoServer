using System;

namespace System.Net {
	/**
	 * Asterion base exception.
	 */
	public class AsterionException : Exception {
		private Connection connection; //< The connection which was involved in the exception.

		//! Gets the connection involved in the exception.
		public Connection Connection {
			get { return connection; }
		}

		public AsterionException(Connection connection) : base() {
			this.connection = connection;
		}
		
		public AsterionException(string exceptionMessage, Connection connection) : base(exceptionMessage) {
			this.connection = connection;
		}
		
		public AsterionException(string exceptionMessage, AsterionException innerException, Connection connection) : base(exceptionMessage, innerException) {
			this.connection = connection;
		}
		
	}
	/**
	 * Thrown when we cannot get a connection's address.
	 */
	public class UnavailableEndPointException : AsterionException {
		public UnavailableEndPointException(Connection connection) : base(connection) { }
		public UnavailableEndPointException(string exceptionMessage, Connection connection) : base(exceptionMessage, connection) { }
		public UnavailableEndPointException(string exceptionMessage, UnavailableEndPointException innerException, Connection connection) : base(exceptionMessage, innerException, connection) { }
	}
	/**
	 * Thrown when the server is full.
	 */
	public class ServerFullException : AsterionException {
		public ServerFullException(Connection connection) : base(connection) { }
		public ServerFullException(string exceptionMessage, Connection connection) : base(exceptionMessage, connection) { }
		public ServerFullException(string exceptionMessage, ServerFullException innerException, Connection connection) : base(exceptionMessage, innerException, connection) { }
	}
}
