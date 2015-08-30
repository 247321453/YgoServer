/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/8/30
 * 时间: 22:03
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using YGOCore.Game;

using System.Reflection;

namespace YGOCore
{
	/// <summary>
	/// Description of ChatCommand.
	/// </summary>
	public class ChatCommand
	{
		
		public static void WriteHead(ServerConfig config){
			if(config==null){
				return;
			}
			string Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			Logger.WriteLine("┌───────────────────────────────────",false);
			Logger.WriteLine("│ __     _______  ____   _____",false);
			Logger.WriteLine("│ \\ \\   / / ____|/ __ \\ / ____|", false);
			Logger.WriteLine("│  \\ \\_/ / |  __| |  | | |     ___  _ __ ___", false);
			Logger.WriteLine("│   \\   /| | |_ | |  | | |    / _ \\| '__/ _ \\", false);
			Logger.WriteLine("│    | | | |__| | |__| | |___| (_) | | |  __/", false);
			Logger.WriteLine("│    |_|  \\_____|\\____/ \\_____\\___/|_|  \\___|    Build: " + Version, false);
			Logger.WriteLine("│", false);
			Logger.WriteLine("│Client version 0x" + config.ClientVersion.ToString("x") + " or new, MaxRooms = "+config.MaxRoomCount, false);
			Logger.WriteLine("│NeedAtuh = "+config.isNeedAuth+", AutoReplay = "+config.AutoReplay+", RecordWin = "+config.RecordWin, false);
			Logger.WriteLine("└───────────────────────────────────",false);
		}
		public static void onCommand(Player player,string msg)
		{
			if(!string.IsNullOrEmpty(msg)){
				if(msg.StartsWith("/help")){
					Logger.WriteLine(player.Name+":"+msg.Replace("/help",""),false);
				}
			}
		}
		
		public static void onCommand(Server server,string cmd){
			if(cmd=="roomcount"){
				Console.WriteLine(">>room count:"+server.getRoomCount());
			}else if(cmd=="playercount"){
				Console.WriteLine(">>player count:"+server.getPlayerCount());
			}else if(cmd=="cls"){
				Console.Clear();
				WriteHead(Program.Config);
			}else if(cmd=="roomlist"){
				Console.WriteLine(">>count:"+server.getRoomCount());
				Console.WriteLine(server.getRoomJson());
			}else if(cmd.StartsWith("say ")){
				try{
					GameManager.SendWarringMessage("[Server] "+cmd.Substring("say ".Length));
				}catch{
					
				}
			}else if(cmd.StartsWith("warring ")){
				try{
					GameManager.SendErrorMessage("[Server] "+cmd.Substring("warring ".Length));
				}catch{
					
				}
			}else if(cmd.StartsWith("config ")){
				string[] args=cmd.Split(' ');
				if(args.Length>=3){
					if(Program.Config.setValue(args[1], args[2])){
						Console.WriteLine(">>"+args[1]+"="+ args[2]);
					}
				}
			}else if(cmd=="load config"){
				Program.Config.Load();
			}else if(cmd=="banlist"){
				if(BanlistManager.Banlists!=null && BanlistManager.Banlists.Count>0){
					Console.WriteLine(">>Banlist = "+BanlistManager.Banlists[0].Name);
				}
			}else if(cmd.StartsWith("to ")){
				string[] names=cmd.Split(' ');
				if(names.Length>=2){
					try{
						
						if(GameManager.SendErrorMessage("[Server] "+cmd.Substring(("to "+names[1]).Length+1), names[1])){
							Console.WriteLine(">>send to "+names[1]);
						}else{
							Console.WriteLine(">>send fail. no find "+names[1]);
						}
					}catch{
						
					}
				}
			}
			else{
				Console.WriteLine(">>no this cmd");
			}
		}
	}
}
