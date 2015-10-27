namespace Asterion.Exceptions {
    /**
     * Asterion base exception.
     */
     public class AsterionException : System.Exception {
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
}
