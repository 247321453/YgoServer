/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/11/5
 * 时间: 13:43
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using YGOCore.Net;
using System.Text;
using AsyncServer;
using YGOCore.Game;
using System.Net;
using System.Net.Sockets;
using OcgWrapper;
using OcgWrapper.Enums;
using System.IO;
using System.Threading;

namespace YGOCore.Game
{
	/// <summary>
	/// Description of RoomManager.
	/// </summary>
	public class RoomManager
	{
		#region member
		private static readonly SortedList<string, GameRoom> Games = new SortedList<string, GameRoom>();
		private static readonly Random Random= new Random();
		private static readonly Queue<WinInfo> WinInfos = new Queue<WinInfo>();
		private static System.Timers.Timer WinSaveTimer;
		private static List<string> banNames=new List<string>();
		public  static int Count{
			get{lock(Games){return Games.Count;}}
		}
		#endregion
		
		#region public
		public static int OnWorldMessage(string msg, PlayerType color= PlayerType.Yellow){
			List<GameRoom> rooms = new List<GameRoom>();
			int i=0;
			lock(Games){
				foreach(GameRoom room in Games.Values){
					if(room!=null&& room.IsOpen){
						room.ServerMessage(msg, color);
						i++;
					}
				}
			}
			return i;
		}
		public static void OnWin(string roomName, int roomMode,int win,int reason,string yrpFileName,
		                         string[] names,bool force){
			WinInfo info =new WinInfo(roomName, win, reason, yrpFileName, names, force);
			lock(WinInfos){
				WinInfos.Enqueue(info);
			}
		}
		public static bool OnLogin(string name, string pwd){
			//长密码，短密码
			if(string.IsNullOrEmpty(name)){
				return false;
			}
			if(name!=null&&name.StartsWith("[AI]")){
				Logger.Debug("[AI]login:"+pwd+"=="+Program.Config.AIPass+"?");
				return Program.Config.AIPass == pwd;
			}
			//服务器接口
			return true;
		}
		public static void init(){
			if(Program.Config.RecordWin){
				SatrtWinTimer();
			}
			ReadBanNames();
		}
		#endregion
		
		#region 比赛记录
		private static void SatrtWinTimer(){
			//10s保存一次结果
			if(WinSaveTimer==null){
				WinSaveTimer = new System.Timers.Timer(10*1000);
				WinSaveTimer.AutoReset=true;
				WinSaveTimer.Enabled=true;
				WinSaveTimer.Elapsed+=new System.Timers.ElapsedEventHandler(WinSaveTimer_Elapsed);
			}
			WinSaveTimer.Start();
		}
		private  static void WinSaveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if(!Program.Config.RecordWin){
				return;
			}
			string[] sqls=null;
			lock(WinInfos){
				if(WinInfos.Count==0) return;
				sqls=new string[WinInfos.Count];
				int i=0;
				while(WinInfos.Count >0){
					WinInfo info = WinInfos.Dequeue();
					sqls[i++]=info.GetSQL();
				}
			}
			ThreadPool.QueueUserWorkItem(new WaitCallback(
				delegate(object obj)
				{
					//string[] sqls = (string[])obj;
					SQLiteTool.Command(Program.Config.WinDbName, sqls);
					Logger.Debug("save wins record:"+sqls.Length);
				}
			));
		}
		#endregion
		
		#region 禁止登录
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns>false=被禁止</returns>
		public static bool CheckPlayerBan(string name){
			if(string.IsNullOrEmpty(name)){
				return false;
			}
			name = name.Split('$')[0];
			if(Program.Config.BanMode==0){
				return true;
			}
			else if(Program.Config.BanMode==1){
				return !banNames.Contains(name);
			}
			else{
				return banNames.Contains(name);
			}
		}
		private static void ReadBanNames()
		{
			if(File.Exists(Program.Config.File_BanAccont)){
				string[] lines = File.ReadAllLines(Program.Config.File_BanAccont);
				foreach(string line in lines){
					if(string.IsNullOrEmpty(line)||line.StartsWith("#")){
						continue;
					}
					string name=line.Trim();
					if(!banNames.Contains(name)){
						banNames.Add(name);
					}
				}
			}
		}
		#endregion
		
		#region 房间创建
		public  static bool CheckRoomPassword(string namepwd){
			string name = Password.OnlyName(namepwd);
			lock(Games){
				if(Games.ContainsKey(name)){
					//存在这个房间
					GameRoom room = Games[name];
					if(room!=null && room.Config!=null){
						return namepwd == room.Config.Name;
					}
				}
			}
			return true;
		}
		/// <summary>
		/// 创建房间
		/// </summary>
		/// <param name="server"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public  static GameRoom CreateOrGetGame(GameConfig config){
			string roomname = Password.OnlyName(config.Name);
			lock(Games){
				if (Games.ContainsKey(roomname)){
					return Games[roomname];
				}
				if(Games.Count >= Program.Config.MaxRoomCount){
					return null;
				}
			}
			Logger.Info("create room");
			GameRoom room = new GameRoom(config);
			Add(room);
			OnRoomCreate(room);
			return room;
		}
		/// <summary>
		/// 得到一个不存在的随机房间名
		/// </summary>
		/// <param name="server"></param>
		/// <returns></returns>
		public  static string NewRandomRoomName(){
			List<string> rooms=new List<string>();
			lock(Games){
				rooms.AddRange(Games.Keys);
			}
			if(rooms.Count==0){
				return GetGuidString();
			}
			while (true) //keep searching till one is found!!
			{
				string roomname = GetGuidString();
				//MutexRooms.WaitOne();
				if (!rooms.Contains(roomname)){
					//MutexRooms.ReleaseMutex();
					return roomname;
				}else{
					//MutexRooms.ReleaseMutex();
				}
			}
		}

		/// <summary>
		/// 得到一个存在的随机房间名，不能带密码
		/// </summary>
		/// <param name="server"></param>
		/// <param name="tag"></param>
		/// <returns></returns>
		public  static string RandomRoomName(string tag=null){
			if(!string.IsNullOrEmpty(tag) && !tag.EndsWith("#")){
				tag+="#";
			}
			List<GameRoom> rooms = null;
			lock(Games){
				rooms = GetNoPwdRoom(Games, tag);
			}
			if(rooms.Count == 0){
				return GetGuidString();
			}
			int index=Random.Next(rooms.Count);
			GameRoom room = rooms[index];
			return room.Config==null?null:room.Config.Name;
		}
		
		private  static List<GameRoom> GetNoPwdRoom(SortedList<string, GameRoom> rooms, string tag=null)
		{
			List<GameRoom> roomList=new List<GameRoom>();
			foreach(GameRoom room in rooms.Values){
				if(room!=null && room.Config.Name != null && !room.Config.Name.Contains("$")){
					//不为空，没有密码
					if(!string.IsNullOrEmpty(tag) && !room.Name.StartsWith(tag)){
						continue;
					}
					if(room.IsRandom && room.IsOpen && room.GetAvailablePlayerPos() >=0){
						roomList.Add(room);
					}
				}else{
					Logger.Debug("room is null?"+(room!=null));
				}
			}
			return roomList;
		}
		
		private  static string GetGuidString(){
			string GuidString = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
			StringBuilder sb = new StringBuilder(GuidString);
			sb.Replace("=", "");
			sb.Replace("+", "");
			sb.Replace("#", "");
			sb.Replace("$", "");
			sb.Replace("/", "");
			sb.Replace("!", "");
			sb.Replace("@", "");
			sb.Replace("%", "");
			sb.Replace("^", "");
			sb.Replace("*", "");
			sb.Replace(" ", "");
			return GuidString.Substring(0, 6);
		}
		#endregion
		
		#region 房间
		public  static void Add(GameRoom room){
			if(room==null)return;
			lock(Games){
				if(!Games.ContainsKey(room.Name)){
					Games.Add(room.Name, room);
				}
			}
		}
		public  static void Remove(GameRoom room){
			if(room==null)return;
			lock(Games){
				if(Games.ContainsKey(room.Name)){
					Games.Remove(room.Name);
				}
			}
			OnRoomClose(room);
		}
		#endregion
		
		#region 事件
		public static void OnClientLogout(GameSession client){ }

		public static void OnClientLogin(GameSession client,string name,string pwd){ }
		
		public static void OnRoomCreate(GameRoom room){
			Println("create$"+room.Name+"$"+room.Config.GetString());
		}
		
		public static void OnRoomStart(GameRoom room){
			Println("start$"+room.Name);
		}
		
		public static void OnRoomClose(GameRoom room){
			Println("close$"+room.Name);
		}

		public static void OnPlayerLeave(GameSession player, GameRoom room){
			Println("leave$"+player.Name+"$"+room.Name);
		}
		
		public static void OnPlayerJoin(GameSession player, GameRoom room){
			Println("join$"+player.Name+"$"+room.Name);
		}
		public static void Println(string str){
			if(Program.Config.ConsoleApi){
				Console.WriteLine("::"+str);
			}
		}
		#endregion
	}
}
