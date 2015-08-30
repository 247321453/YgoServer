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
			//ThreadPool.SetMaxThreads(32,32);
			Thread inputThread=new Thread(new ParameterizedThreadStart(Command));
			inputThread.IsBackground=true;
			inputThread.Start(server);
			while (server.IsListening)
			{
				server.Process();
				Thread.Sleep(1);
			}
		}

		
		private static void Command(object obj){
			Server server=obj as Server;
			string cmd="";
			while((cmd=Console.ReadLine())!=null){
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
