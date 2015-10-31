using System;
using System.Threading;
using System.IO;
using System.Diagnostics;
using AsyncServer;

namespace YGOCore
{
	class Program
	{
		public static ServerConfig Config { get; private set; }
		public static Random Random;
		
		static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
			Console.CancelKeyPress+= new ConsoleCancelEventHandler(Console_CancelKeyPress);
			Logger.SetLogLevel(LogLevel.Info);
			#if DEBUG
			Logger.SetLogLevel(LogLevel.Debug);
			#endif
			Config = new ServerConfig();
			bool loaded = args.Length > 0 ? Config.Load(args[0]): Config.Load();

			ChatCommand.WriteHead(Config);
			if(loaded)
				Logger.Info("Config loaded.");
			else
				Logger.Warn("Unable to load config.txt, using default settings.");
			if(Config.HandShuffle){
				Logger.Warn("Warning: Hand shuffle requires a custom ocgcore to work.");
			}
			int coreport = Config.ServerPort;
//
//			if (args.Length > 0){
//				int.TryParse(args[0], out coreport);
//				if(coreport==0){
//					coreport=Config.ServerPort;
//				}else{
//					Config.setValue("serverport", ""+coreport);
//					if(coreport<=9999){
//						Config.setValue("apiport", ""+(10000+coreport));
//					}
//				}
//			}
			Random = new Random();
			
			Server server = new Server();
			if (!server.Start(coreport)){
				Console.WriteLine("Server start fail.");
				Console.ReadKey();
			}else{
				ThreadPool.SetMaxThreads(128, 256);
				Command(server);
			}
		}

		static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			e.Cancel=true;
			ConsoleColor color=Console.ForegroundColor;
			Console.ForegroundColor=ConsoleColor.Cyan;
			Console.WriteLine("Please input to close server.");
			Console.ForegroundColor=color;
		}
		
		private static void Command(Server server){
			string cmd="";
			while(server.IsListening){
				cmd=Console.ReadLine();
				ChatCommand.onCommand(server, cmd);
			}
		}

		private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception exception = e.ExceptionObject as Exception ?? new Exception();
			File.WriteAllText("crash_" + DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt", exception.ToString());
			Process.GetCurrentProcess().Kill();
		}
	}
}
