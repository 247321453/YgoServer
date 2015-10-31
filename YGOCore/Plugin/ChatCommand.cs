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
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using OcgWrapper.Enums;
using AsyncServer;

namespace YGOCore
{
	/// <summary>
	/// Description of ChatCommand.
	/// </summary>
	public class ChatCommand
	{
		delegate void ThreadDelagate();
		static List<Process> AIs=new List<Process>();
		
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
			Console.WriteLine("│NeedAtuh="+config.isNeedAuth+", AutoReplay="+config.AutoReplay
			                 +", RecordWin="+config.RecordWin
			                 +", BanMode="+config.BanMode);
			Console.WriteLine("│"+config.ServerDesc);
			Console.WriteLine("└───────────────────────────────────");
		}
		public static bool onChat(GameConfig config,Player player,string msg)
		{
			if(!string.IsNullOrEmpty(msg)){
				msg = msg.Trim();
				if(msg=="/ai"){
					if(Program.Config.MaxAICount==0){
						player.ServerMessage(Messages.MSG_NO_AI);
					}
					else if(config!=null && AddAI(config.Name)){
						player.ServerMessage(Messages.MSG_ADD_AI);
					}else{
						player.ServerMessage(Messages.MSG_NO_FREE_AI);
					}
					return false;
				}
//				if(msg.StartsWith("@") && !Program.Config.PrivateChat){
//					player.ServerMessage(Messages.MSG_BAN_PCHAT);
//					return false;
//				}
//				if(msg.StartsWith("@server ")){
//					Logger.WriteLineWithColor(player.Name+":"+msg.Replace("@server ",""), ConsoleColor.Yellow);
//					return false;
//				}else if(msg.StartsWith("@system ")){
//					Logger.WriteLineWithColor(player.Name+":"+msg.Replace("@system ",""), ConsoleColor.Yellow);
//					return false;
//				}else if(msg.StartsWith("@@ ")){
//					Logger.WriteLineWithColor(player.Name+":"+msg.Replace("@@ ",""), ConsoleColor.Yellow);
//					return false;
//				}else if(msg.StartsWith("@")){
//					//私聊
//					int i=msg.IndexOf(' ');
//					if(i>0){
//						try{
//							string name=msg.Substring(1, i-1);
//							string cxt=msg.Substring(i+1);
//							if(GameManager.SendErrorMessage(player.Name+": "+cxt, name).Count==0){
//								player.ServerMessage(Messages.MSG_SEND_FAIL);
//							}else{
//								return false;
//							}
//						}catch(Exception){
//
//						}
//					}
//				}
			}
			return true;
		}
		
		/// <summary>
		/// 拥有一定数量
		/// </summary>
		/// <param name="name"></param>
		/// <param name="room"></param>
		/// <param name="deck"></param>
		/// <returns></returns>
		private static bool AddAI(string room){
			if(AIs.Count>=Program.Config.MaxAICount){
				return false;
			}
			Process ai=new Process();
			ai.StartInfo.FileName = "ai";
			//设定程式执行参数
			ai.StartInfo.Arguments =
				" [AI]Robot$"+Program.Config.AIPass
				+" 127.0.0.1 "
				+Program.Config.ServerPort
				+ " "+room;
			ai.EnableRaisingEvents=true;
			if(Program.Config.AIisHide){
				ai.StartInfo.WindowStyle=ProcessWindowStyle.Hidden;
			}
			ai.Exited+=new EventHandler(ai_Exited);
			ai.Start();
			AIs.Add(ai);
			return true;
		}

		static void ai_Exited(object sender, EventArgs e)
		{
			if(sender is Process){
				Process p = sender as Process;
				p.Close();
				AIs.Remove(p);
				Logger.Debug("close ai");
			}else{
				Logger.Debug("close ai:"+sender.GetType().ToString());
			}
			Logger.Debug("AI exit game. count="+AIs.Count);
		}
		
		public static void onCommand(Server server,string cmd){
			if(cmd==null){
				return;
			}
			cmd=cmd.Trim();
			if(cmd=="roomcount"){
				Console.WriteLine(">>room count:"+server.getRoomCount());
			}else if(cmd=="playercount"){
				Console.WriteLine(">>player count:"+server.getPlayerCount());
			}else if(cmd=="cls"){
				Console.Clear();
				WriteHead(Program.Config);
			}else if(cmd=="roomlist"){
				Console.WriteLine(">>count:"+server.getRoomCount());
				string json=server.getRoomJson(true, true);
				Console.WriteLine(json);
				File.WriteAllText("room.json", json);
			}else if(cmd.StartsWith("say ")){
				try{
					int count=server.Say(GameManager.getMessage("[Server] "+cmd.Substring("say ".Length),
					                                            PlayerType.Yellow)).Count;
					Console.WriteLine(">>count:"+count);
				}catch{
					
				}
			}else if(cmd.StartsWith("warring ")){
				try{
					int count =server.Say(GameManager.getMessage("[Server] "+cmd.Substring("warring ".Length),
					                                             PlayerType.Red)).Count;
					Console.WriteLine(">>count:"+count);
				}catch{
					
				}
			}else if(cmd.StartsWith("to ")){
				string[] names=cmd.Split(' ');
				if(names.Length>=2){
					try{
						if(server.Say(GameManager.getMessage("[Server] "+cmd.Substring(("to "+names[1]).Length+1)
						                                     , PlayerType.Yellow)
						              ,names[1]).Count>0){
							Console.WriteLine(">>send to "+names[1]);
						}else{
							Console.WriteLine(">>send fail. no find "+names[1]);
						}
					}catch{
						
					}
				}
			}else if(cmd.StartsWith("config ")){
				string[] args=cmd.Split(' ');
				if(args.Length>=3){
					if(Program.Config.setValue(args[1], args[2])){
						Console.WriteLine(">>"+args[1]+"="+ args[2]);
					}
				}
			}else if(cmd=="reload"){
				server.Reload();
				Console.WriteLine(">>reload ok");
			}else if(cmd=="banlist"){
				if(BanlistManager.Banlists!=null && BanlistManager.Banlists.Count>0){
					Console.WriteLine(">>Banlist = "+BanlistManager.Banlists[0].Name);
				}
			}else if(cmd=="close"){
				Console.WriteLine(">>Server will close after 5 minute.");
				server.CloseDealyed();
			}else if(cmd == "cancel close"){
				Console.WriteLine(">>Server cancel close.");
				server.CacelCloseDealyed();
			}else if(cmd=="addai"){
				try{
					GameRoom room=GameManager.GetRandomGame();
					string _rname=GameManager.RandomRoomName();
					if(room!=null){
						_rname=room.Game.Config.Name;
					}
					if(AddAI(_rname)){
						Console.WriteLine("Add AI success:"+_rname+" "
						                  +(Program.Config.MaxAICount-AIs.Count)+"/"+Program.Config.MaxAICount);
					}else{
						Console.WriteLine("Add AI fail, max="+Program.Config.MaxAICount);
					}
				}catch(Exception){
					
				}
			}else if(cmd=="ai count"){
				Console.WriteLine(">>count " +AIs.Count+"/"+Program.Config.MaxAICount);
			}else if(cmd=="help"){
				Console.WriteLine(">>");
				Console.WriteLine("roomcount		room count");
				Console.WriteLine("playercount		plyaer count");
				Console.WriteLine("cls			clear");
				Console.WriteLine("roomlist		room list json");
				Console.WriteLine("say xxx			text is yellow");
				Console.WriteLine("warring xxx		text is red");
				Console.WriteLine("config {key} {value}   	set key=value");
				Console.WriteLine("load config		reload config,but some config need restart.");
				Console.WriteLine("banlist			look banlist");
				Console.WriteLine("to xxx			send msg to player");
				Console.WriteLine("close			close server");
				Console.WriteLine("cancel close		cancel close server");
				Console.WriteLine("maxai			max Ai count");
				Console.WriteLine("addai			add a AI to random room");
			}
			else{
				Console.WriteLine(">>no this cmd", ConsoleColor.Yellow);
			}
		}
	}

}
