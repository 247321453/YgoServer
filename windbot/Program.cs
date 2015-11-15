using System;
using System.Threading;
using WindBot.Game;
using WindBot.Game.AI;
using WindBot.Game.Data;
using System.IO;
using System.Diagnostics;
using OcgWrapper.Managers;

namespace WindBot
{
	public class Program
	{
		public static int ProVersion = 0x1337;

		public static Random Rand;
		public static bool Replay{get;private set;}

		public static void Main(string[] args)
		{
			//暂时不用开启
			Replay=false;
			if(args.Length < 3)
			{
				Console.WriteLine("String pass, String serverIP, int serverPort,int version,String room,String deck");
				//  args=new String[] {"AI", ""};
				Console.ReadKey();
				return;
			}
			try
			{
				if(args.Length==3){
					Run(args[0], args[1], int.Parse(args[2]), ProVersion);
				}else if(args.Length==4){
					Run(args[0], args[1], int.Parse(args[2]), int.Parse(args[3]));
				}else if(args.Length==5){
					Run(args[0], args[1], int.Parse(args[2]), int.Parse(args[3]), args[4]);
				}else{
					if(args[6].Trim() == "hide"){
						
					}
					Run(args[0], args[1], int.Parse(args[2]), int.Parse(args[3]), args[4], args[5]);
				}
			}
			catch (Exception ex)
			{
				Logger.WriteLine("Error: " + ex);
				Console.ReadKey();
			}
		}

		private static void Run(String pass, String serverIP, int serverPort,int version, String room="", String deck="")
		{
			ProVersion = version;
			if(pass=="--"){
				pass="";
			}
			Console.WriteLine(pass+" "+serverIP+":"+serverPort+" 0x"+version.ToString("x")+" room:"+room+" deck "+deck);
			Rand = new Random();
			PathManager.Init(".", "script", "cards.cdb");
			CardsManager.Init();
			DecksManager.Init();

			// Start two clients and connect them to the same room. Which deck is gonna win?
			AIGameClient clientA = new AIGameClient(pass, deck, serverIP, serverPort, room);
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
