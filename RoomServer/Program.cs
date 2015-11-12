using System;

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
			Console.WriteLine("Hello World!");
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
			GameServer server=new GameServer(port, serverExe, configs);
			if(server.Start()){
				
			}else{
				Console.WriteLine("start fail.");
			}
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
	}
}