using System;
using System.Threading;
using WindBot.Game;
using WindBot.Game.AI;
using WindBot.Game.Data;
using System.IO;
using System.Diagnostics;
using OcgWrapper;
using OcgWrapper.Managers;

namespace WindBot
{
	public class Program
	{
		public const short ProVersion = 0x1336;

		public static Random Rand;

		public static void Main(string[] args)
		{
			if(args.Length < 3)
			{
				Console.Out.WriteLine("String username, String serverIP, int serverPort,String room,String deck");
				//  args=new String[] {"AI", ""};
				Console.ReadKey();
				return;
			}
			try
			{
				if(args.Length==3){
					Run(args[0], args[1], int.Parse(args[2]), "", "");
				}else if(args.Length==4){
					Run(args[0], args[1], int.Parse(args[2]), args[3], "");
				}else{
					Run(args[0], args[1], int.Parse(args[2]), args[3], args[4]);
				}
			}
			catch (Exception ex)
			{
				Logger.WriteLine("Error: " + ex);
				Console.ReadKey();
			}
		}

		private static void Run(String username, String serverIP, int serverPort,String room, String deck)
		{
			Logger.WriteLine(username+" join "+serverIP+":"+serverPort+" "+room+" use "+deck);
			Rand = new Random();
			PathManager.Init(".", "script", "cards.cdb");
			CardsManager.Init();
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
	}
}
