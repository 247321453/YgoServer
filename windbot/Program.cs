using System;
using System.Threading;
using WindBot.Game;
using WindBot.Game.AI;
using WindBot.Game.Data;
using System.IO;
using System.Diagnostics;
using OcgWrapper;

namespace WindBot
{
	public class Program
	{
		public const short ProVersion = 0x1336;

		public static Random Rand;

		public static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
			if(args.Length < 4)
			{
				Console.Out.WriteLine("String username, String deck, String serverIP, int serverPort,String password");
				//  args=new String[] {"AI", ""};
				return;
			}
			try
			{
				if(args.Length==4){
					Run(args[0], args[1], args[2], int.Parse(args[3]), "");
				}else{
					Run(args[0], args[1], args[2], int.Parse(args[3]), args[4]);
				}
				
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Error: " + ex);
			}
		}

		private static void Run(String username, String deck, String serverIP, int serverPort,String room)
		{
			Rand = new Random();
			Api.Init();
			DecksManager.Init();

			// Start two clients and connect them to the same room. Which deck is gonna win?
			AIGameClient clientA = new AIGameClient(username, deck, serverIP, serverPort, room);
			clientA.Start();
			while (clientA.Connection.IsConnected)
			{
				clientA.Tick();
				Thread.Sleep(1);
			}
			//Thread.Sleep(3000);
		}
		private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception exception = e.ExceptionObject as Exception ?? new Exception();

			File.WriteAllText("crash_" + DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt", exception.ToString());

			Process.GetCurrentProcess().Kill();
		}
	}
}
