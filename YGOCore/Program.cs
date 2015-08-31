using System;
using System.Threading;
using System.IO;
using System.Diagnostics;

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
			Config = new ServerConfig();
			bool loaded = args.Length > 1 ? Config.Load(args[1]): Config.Load();

			ChatCommand.WriteHead(Config);
			if(loaded)
				Logger.WriteLine("Config loaded.");
			else
				Logger.WriteLine("Unable to load config.txt, using default settings.");
			if(Config.HandShuffle){
				Logger.WriteLine("Warning: Hand shuffle requires a custom ocgcore to work.");
			}
			int coreport = 0;

			if (args.Length > 0)
				int.TryParse(args[0], out coreport);

			Random = new Random();
			
			Server server = new Server();
			if (!server.Start(coreport))
				Thread.Sleep(5000);
			ThreadPool.SetMaxThreads(256, 128);
			Thread inputThread=new Thread(new ParameterizedThreadStart(Command));
			inputThread.IsBackground=true;
			inputThread.Start(server);
			while (server.IsListening)
			{
				server.Process();
				Thread.Sleep(1);
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
		
		private static void Command(object obj){
			Server server=obj as Server;
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
