namespace Asterion.Exceptions {
    /**
     * Thrown when we cannot get a connection's address.
     */
     public class UnavailableEndPointException : AsterionException {        
        public UnavailableEndPointException(Connection connection) : base(connection) { }
        public UnavailableEndPointException(string exceptionMessage, Connection connection) : base(exceptionMessage, connection) { }
        public UnavailableEndPointException(string exceptionMessage, UnavailableEndPointException innerException, Connection connection) : base(exceptionMessage, innerException, connection) { }
    }
}
