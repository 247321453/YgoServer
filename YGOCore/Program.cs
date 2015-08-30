using System;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Reflection;

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

			WriteHead();
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
		
		private static void WriteHead(){
			string Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			Logger.WriteLine("©°©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤",false);
			Logger.WriteLine("©¦ __     _______  ____   _____",false);
			Logger.WriteLine("©¦ \\ \\   / / ____|/ __ \\ / ____|", false);
			Logger.WriteLine("©¦  \\ \\_/ / |  __| |  | | |     ___  _ __ ___", false);
			Logger.WriteLine("©¦   \\   /| | |_ | |  | | |    / _ \\| '__/ _ \\", false);
			Logger.WriteLine("©¦    | | | |__| | |__| | |___| (_) | | |  __/", false);
			Logger.WriteLine("©¦    |_|  \\_____|\\____/ \\_____\\___/|_|  \\___|    Build: " + Version, false);
			Logger.WriteLine("©¦", false);
			Logger.WriteLine("©¦Client version 0x" + Config.ClientVersion.ToString("x") + " or new, Max Room Count:"+Config.MaxRoomCount, false);
			Logger.WriteLine("©¦Login NeedAtuh = "+Config.isNeedAuth+", AutoReplay = "+Config.AutoReplay, false);
			Logger.WriteLine("©¸©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤",false);
		}
		
		private static void Command(object obj){
			Server server=obj as Server;
			string cmd="";
			while((cmd=Console.ReadLine())!=null){
				if(cmd=="roomcount"){
					Console.WriteLine(">>room count:"+server.getRoomCount());
				}else if(cmd=="playercount"){
					Console.WriteLine(">>player count:"+server.getPlayerCount());
				}else if(cmd=="cls"){
					Console.Clear();
					WriteHead();
				}else if(cmd=="roomlist"){
					Console.WriteLine(">>count:"+server.getRoomCount());
					Console.WriteLine(server.getRoomJson());
				}else{
					Console.WriteLine(">>no this cmd");
				}
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
