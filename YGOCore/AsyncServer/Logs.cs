namespace System.Net {
	/**
	 * Enum of log levels for the writeOutput method.
	 */
	public delegate void LogEventHandler(string text, LogLevel level);
	
	public enum LogLevel {
		Debug,
		Info,
		Warn,
		Error,
		Fatal
	}
}
