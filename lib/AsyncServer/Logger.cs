using System;
using System.IO;

namespace AsyncServer {
	public enum LogLevel:byte {
		Debug=0,
		Info=1,
		Warn=3,
		Error=4
	}
	
	public class Logger{
		static string ErrFile = "errors.log";
		static LogLevel sLevel = LogLevel.Debug;
		public static void SetLogLevel(int level){
			if(LogLevel.IsDefined(typeof(LogLevel), (byte)level)){
				sLevel = (LogLevel)level;
			}else{
				sLevel = LogLevel.Info;
			}
		}
		public static void SetLogLevel(LogLevel level){
			sLevel = level;
		}
		public static void SetErrorFile(string file){
			ErrFile = file;
		}
		public static void Debug(object obj){
			if(sLevel <= LogLevel.Debug){
				WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") +" "+obj, ConsoleColor.White);
			}
		}
		public static void Info(object obj){
			if(sLevel <= LogLevel.Info){
				WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") +" "+obj, ConsoleColor.Cyan);
			}
		}
		public static void Warn(object obj){
			if(sLevel <= LogLevel.Warn){
				WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") +" "+obj, ConsoleColor.Yellow);
			}
		}
		public static void Error(object obj){
			if(sLevel <= LogLevel.Error){
				string str =DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") +" "+obj;
				WriteLine(str, ConsoleColor.Red);
				File.AppendAllText(ErrFile, str);
			}
		}
		private static void WriteLine(object obj, ConsoleColor color=ConsoleColor.White){
			ConsoleColor old=Console.ForegroundColor;
			Console.ForegroundColor=color;
			Console.WriteLine("" + obj);
			Console.ForegroundColor=old;
		}
	}
}
