using System;
using System.IO;
using System.Diagnostics;

namespace YGOCore
{
	class Program
	{
		/// <summary>
		/// port GameServer.exe config.txt config1.txt config2.txt ...
		/// </summary>
		/// <param name="args"></param>
		public static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
			Console.CancelKeyPress+= new ConsoleCancelEventHandler(Console_CancelKeyPress);
			int port = 0;
			string serverExe="";
			string[] configs=null;
			try{
				port = int.Parse(args[0]);
				serverExe = args[1];
				configs = new string[args.Length-2];
				for(int i=1;i<configs.Length;i++){
					configs[i] = args[i+2];
				}
			}catch(Exception e){
				Console.WriteLine("port GameServer.exe config.txt config1.txt config2.txt ...");
				Console.WriteLine(e.ToString());
				Console.ReadKey(true);
				return;
			}
			RoomServer server=new RoomServer(port, serverExe, configs);
			if(server.Start()){
				
			}else{
				Console.WriteLine("start fail.");
			}
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
		private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			e.Cancel=true;
			ConsoleColor color=Console.ForegroundColor;
			Console.ForegroundColor=ConsoleColor.Cyan;
			Console.WriteLine("Please input to close server.");
			Console.ForegroundColor=color;
		}
		
		private static void Command(RoomServer server){
			string cmd="";
			while(server.IsListening){
				cmd=Console.ReadLine();
				server.OnCommand(cmd);
			}
		}

		private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			File.WriteAllText("crash_room_" + DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt", e.ExceptionObject.ToString());
			Process.GetCurrentProcess().Kill();
		}
	}
}