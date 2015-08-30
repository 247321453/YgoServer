using System;
using System.IO;

namespace WindBot
{
	public static class Logger
	{
		public static void WriteLine(string message)
		{
			string txt="[" + DateTime.Now.ToString("HH:mm:ss") + "] " + message;
			Console.WriteLine(txt);
			WriteError(txt);
		}
		private static void WriteError(string text)
		{
			try
			{
				StreamWriter writer = new StreamWriter("AILog.txt", true);
				writer.WriteLine(text);
				writer.Close();
			}
			catch(Exception)
			{
				
			}
		}
	}
}