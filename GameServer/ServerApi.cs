/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/11/12
 * 时间: 13:30
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using YGOCore.Game;
using YGOCore.Net;

namespace YGOCore
{
	/// <summary>
	/// Description of ServerApi.
	/// </summary>
	public static class ServerApi
	{
		public const char SEP = '\t';
		public const string HEAD="::";
		public static void OnClientLogout(GameSession client){ }

		public static void OnClientLogin(GameSession client,string name,string pwd){ }
		
		public static void OnServerInfo(GameServer server){
			ServerConfig Config = server.Config;
			Println("server"+SEP+Config.ServerPort+SEP+Config.isNeedAuth);
		}
		public static void OnRoomCreate(GameRoom room){
			if(room==null||room.Config==null){
				return;
			}
			Println("create"+SEP+room.Name+SEP+room.Config.BanList+SEP+room.Config.Name);
		}
		
		public static void OnRoomStart(GameRoom room){
			Println("start"+SEP+room.Name);
		}
		
		public static void OnRoomClose(GameRoom room){
			Println("close"+SEP+room.Name);
		}

		public static void OnPlayerLeave(GameSession player, GameRoom room){
			Println("leave"+SEP+player.Name+SEP+room.Name);
		}
		
		public static void OnPlayerJoin(GameSession player, GameRoom room){
			Println("join"+SEP+player.Name+SEP+room.Name);
		}
		public static void Println(string str){
			if(Program.Config.ConsoleApi){
				Console.WriteLine(HEAD+str);
			}
		}
	}
}
