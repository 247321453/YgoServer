namespace Asterion.Exceptions {
    /**
     * Thrown when the server is full.
     */
     public class ServerFullException : AsterionException {        
        public ServerFullException(Connection connection) : base(connection) { }
        public ServerFullException(string exceptionMessage, Connection connection) : base(exceptionMessage, connection) { }
        public ServerFullException(string exceptionMessage, ServerFullException innerException, Connection connection) : base(exceptionMessage, innerException, connection) { }
    }
}
