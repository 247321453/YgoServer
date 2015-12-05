/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/1
 * 时间: 8:34
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using YGOCore.Net;
using System.IO;
using System.Threading;
using AsyncServer;
using System.Diagnostics;

namespace YGOCore
{
	class Program
	{
		public static ServerConfig Config;
		public static readonly Random Random = new Random(Environment.TickCount);
		public static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
			Console.CancelKeyPress+= new ConsoleCancelEventHandler(Console_CancelKeyPress);
			System.Net.ServicePointManager.DefaultConnectionLimit = 512;
			ThreadPool.SetMaxThreads(128, 256);
			//
			Config = new ServerConfig();
			bool loaded = args.Length > 0 ? Config.Load(args[0]): Config.Load();
			int tmp =0 ;
			if(args.Length > 1){
				int.TryParse(args[1], out tmp);
				if(tmp > 0){
					Config.SetServerPort(tmp);
				}
			}
			if(args.Length > 2){
				int.TryParse(args[2], out tmp);
				if(tmp > 0){
					Config.SetApiPort(tmp);
				}
			}
            if(args.Length > 3)
            {
                Config.SetNeedAuth(args[3].Trim().ToLower() == "true");
            }
			Logger.SetErrorFile(Tool.Combine(Config.Path, "error_"+Environment.TickCount+".log"));
			Logger.SetLogLevel(Config.LogLevel);
#if DEBUG
			Logger.SetLogLevel(LogLevel.Debug);
#endif
            Console.Title = "GameServer:"+Config.ServerPort;
			GameServer Server = new GameServer(Config);
			Server.WriteHead();
			if(loaded)
				Logger.Debug("Config loaded.");
			else
				Logger.Warn("Unable to load config.txt, using default settings.");
			if (!Server.Start()){
				Console.WriteLine(">>server start fail.");
				Console.ReadKey();
			}else{
				Command(Server);
			}
		}
		private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			e.Cancel=true;
			ConsoleColor color=Console.ForegroundColor;
			Console.ForegroundColor=ConsoleColor.Cyan;
			Console.WriteLine("Please input to close server.");
			Console.ForegroundColor=color;
		}
		
		private static void Command(GameServer server){
			string cmd="";
			while(server.IsListening){
				cmd=Console.ReadLine();
				server.OnCommand(cmd);
			}
		}

		private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			File.WriteAllText("crash_" + DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt", e.ExceptionObject.ToString());
			Process.GetCurrentProcess().Kill();
		}
	}
}