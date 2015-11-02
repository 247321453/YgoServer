/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/1
 * 时间: 9:00
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using YGOCore.Game;
using YGOCore.Net;
using System.Reflection;

namespace YGOCore
{
	/// <summary>
	/// Description of ChatCommand.
	/// </summary>
	public class ChatCommand
	{
		public ChatCommand()
		{
		}
		public static void WriteHead(ServerConfig config){
			if(config==null){
				return;
			}
			string Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			Console.Title=(string.IsNullOrEmpty(config.ServerName)?"YgoServer":config.ServerName);
			Console.WriteLine("┌───────────────────────────────────");
			Console.WriteLine("│ __     _______  ____   _____");
			Console.WriteLine("│ \\ \\   / / ____|/ __ \\ / ____|");
			Console.WriteLine("│  \\ \\_/ / |  __| |  | | |     ___  _ __ ___");
			Console.WriteLine("│   \\   /| | |_ | |  | | |    / _ \\| '__/ _ \\");
			Console.WriteLine("│    | | | |__| | |__| | |___| (_) | | |  __/");
			Console.WriteLine("│    |_|  \\_____|\\____/ \\_____\\___/|_|  \\___|    Build: " + Version);
			Console.WriteLine("│");
			Console.WriteLine("│Client version 0x" + config.ClientVersion.ToString("x") + " or new, MaxRooms = "+config.MaxRoomCount, false);
			Console.WriteLine("│NeedAtuh="+config.isNeedAuth+", AsyncMode="+config.AsyncMode
			                  +", RecordWin="+config.RecordWin
			                  +", BanMode="+config.BanMode);
			Console.WriteLine("│"+config.ServerDesc);
			Console.WriteLine("└───────────────────────────────────");
		}
		public static void OnCommand(GameServer Server, string cmd){
			if(cmd==null){
				return;
			}
			cmd = cmd.ToLower();
			string[] args = cmd.Split(new char[]{' '}, 2);
			cmd = args[0];
			switch(cmd){
				case "room":
					bool isdo = true;
					if(args.Length>1){
						switch(args[1]){
							case "-json":
							case "-j":
								
								if(Server!=null){
									Console.WriteLine(""+Server.GetRoomJson(false, false));
								}
								break;
								default :
									isdo = false;
								break;
						}
					}
					if(!isdo){
						if(Server!=null){
							Console.WriteLine(">>count="+Server.GetRoomCount());
						}
					}
					break;
				case "cls":
					Console.Clear();
					if(Server!=null)
						WriteHead(Server.Config);
					break;
				case "close":
					if(Server!=null)
						Server.Stop();
					break;
				default:
					Console.WriteLine(">>invalid command:"+cmd);
					break;
			}
		}
	}
}
