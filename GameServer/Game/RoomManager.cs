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
		private static readonly SortedList<string, GameSession> Users = new SortedList<string, GameSession>();
		private static readonly SortedList<string, string> SPwds =new SortedList<string, string>();
		private static System.Timers.Timer RefreshTimer;
		public  static int Count{
			get{lock(Games){return Games.Count;}}
		}
		#endregion
		
		#region public
		public static void OnWorldMessage(string msg, PlayerType color= PlayerType.Yellow){
			List<GameRoom> rooms = new List<GameRoom>();
			lock(Games){
				foreach(GameRoom room in Games.Values){
					if(room!=null&& room.IsOpen){
						room.ServerMessage(msg, color);
					}
				}
			}
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
			lock(SPwds){
				if(SPwds.ContainsKey(name)){
					if(SPwds[name] == pwd){
						return true;
					}
				}
			}
			//服务器接口
			return true;
		}
		public static void init(){
			if(Program.Config.RecordWin){
				SatrtWinTimer();
			}
			StartRefreshTimer();
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
		
		private static void StartRefreshTimer(){
			if(RefreshTimer==null){
				//3秒发送一次包
				RefreshTimer=new System.Timers.Timer(3000);
				RefreshTimer.AutoReset = true;
				RefreshTimer.Elapsed+= delegate {
					lock(Users){
						foreach(GameSession user in Users.Values){
							user.Client.PeekSend();
						}
					}
				};
			}
			RefreshTimer.Start();
		}
		private static void SendAll(byte[] data,bool isNow = false){
			//发送
			lock(Users){
				foreach(GameSession user in Users.Values){
					user.Client.SendPackage(data, isNow);
				}
			}
		}
		public static void OnRoomCreate(GameRoom room){
			Logger.Debug("room create");
			GameConfig config = room.Config;
			if(room.IsRandom){
				//随机房间不触发
				return;
			}
			if(config==null) return;
			using(PacketWriter writer=new PacketWriter(2)){
				writer.Write((byte)StocMessage.RoomCreate);
				writer.WriteUnicode(config.Name, 20);
				writer.WriteUnicode(config.BanList, 20);
				writer.Write((short)config.LfList);
				writer.Write((short)config.Rule);
				writer.Write((short)config.Mode);
				writer.Write(config.EnablePriority);
				writer.Write(config.NoCheckDeck);
				writer.Write(config.NoShuffleDeck);
				writer.Write(config.StartLp);
				writer.Write((short)config.StartHand);
				writer.Write((short)config.DrawCount);
				writer.Write(config.GameTimer);
				writer.Write(config.IsStart);
				writer.Use();
				//发送
				SendAll(writer.Content);
			}
			
		}
		public static void OnRoomStart(GameRoom room){
			Logger.Debug("room start");
			if(room.IsRandom){
				//随机房间不触发
				return;
			}
			GameConfig config = room.Config;
			if(config==null) return;
			using(PacketWriter writer=new PacketWriter(2)){
				writer.Write((byte)StocMessage.RoomStart);
				writer.WriteUnicode(Password.OnlyName(config.Name), 20);
				writer.Use();
				//发送
				SendAll(writer.Content);
			}
		}
		
		public static void OnRoomClose(GameRoom room){
			Logger.Debug("room close");
			if(room.IsRandom){
				//随机房间不触发
				return;
			}
			GameConfig config = room.Config;
			if(config==null) return;
			using(PacketWriter writer=new PacketWriter(2)){
				writer.Write((byte)StocMessage.RoomClose);
				writer.WriteUnicode(Password.OnlyName(config.Name), 20);
				writer.Use();
				//发送
				SendAll(writer.Content);
			}
		}
		
		#region room
		public static void OnRoomList(GameSession client, GameClientPacket packet){
			List<GameConfig> rooms=new List<GameConfig>();
			lock(Games){
				foreach(GameRoom room in Games.Values){
					if(room !=null && room.IsOpen && !room.IsRandom && !room.Config.IsStart){
						//没有结束，非随机房间,未开始决斗
						rooms.Add(room.Config);
					}
				}
			}
			using(PacketWriter writer=new PacketWriter(2)){
				writer.Write((byte)StocMessage.RoomList);
				writer.Write((int)rooms.Count);
				foreach(GameConfig config in rooms){
					writer.WriteUnicode(config.Name, 20);
					writer.WriteUnicode(config.BanList, 20);
					writer.Write((short)config.LfList);
					writer.Write((short)config.Rule);
					writer.Write((short)config.Mode);
					writer.Write(config.EnablePriority);
					writer.Write(config.NoCheckDeck);
					writer.Write(config.NoShuffleDeck);
					writer.Write(config.StartLp);
					writer.Write((short)config.StartHand);
					writer.Write((short)config.DrawCount);
					writer.Write(config.GameTimer);
					writer.Write(config.IsStart);
				}
				writer.Use();
				client.Client.SendPackage(writer.Content, true);
			}
		}
		#endregion
		public static void OnClientLogout(GameSession client){
			if(client==null || client.Name ==null) return;
			if(client.Type != (int)PlayerType.Client) return;
			if(!client.IsAuthentified) return;
			lock(Users){
				if(Users.ContainsKey(client.Name)){
					Users.Remove(client.Name);
				}
			}
			lock(SPwds){
				if(SPwds.ContainsKey(client.Name)){
					SPwds.Remove(client.Name);
				}
			}
		}

		public static void OnClientLogin(GameSession client,string name,string pwd){
			string namepwd = string.IsNullOrEmpty(pwd)?name:name+'$'+pwd;
			client.Name = name;
			client.Type = (int)PlayerType.Client;
			client.IsAuthentified = client.CheckAuth(namepwd);
			//必须开启异步
			client.Client.isAsync = true;
			Logger.Debug("client");
			string token = Tool.SubString(Tool.GetMd5(pwd+DateTime.Now.ToString()), 4, 4);
			if(!client.IsAuthentified){
				//登陆失败
				Logger.Debug("client "+name+" login fail");
				client.LobbyError(Messages.ERR_AUTH_FAIL);
				return;
			}else{
				lock(Users){
					if(Users.ContainsKey(client.Name)){
//						已经登陆
						Logger.Debug("client "+name+" is logined");
						client.IsAuthentified = false;
						client.LobbyError(Messages.ERR_IS_LOGIN);
						return;
					}else{
						Logger.Debug("client login ok");
						//短密码
						SPwds.Add(client.Name, token);
						Users.Add(client.Name, client);
					}
				}
			}
			using(GameServerPacket writer=new GameServerPacket(StocMessage.ServerInfo)){
				writer.WriteUnicode(Program.Config.ServerName, 20);
				writer.WriteUnicode(Program.Config.ServerIp, 20);
				writer.Write((int)Program.Config.ServerPort);
				writer.Write(Program.Config.isNeedAuth);
				writer.WriteUnicode(token, 20);
				writer.WriteUnicode(Program.Config.ServerDesc, Program.Config.ServerDesc.Length+1);
				writer.Use();
				client.Send(writer, true);
			}
		}
		
		public static bool OnChat(GameSession client, GameClientPacket packet){
			if(client.Type != (int)PlayerType.Client){
				return false;
			}
			string name = packet.ReadUnicode(20);
			string toname = packet.ReadUnicode(20);
			string msg = packet.ReadUnicode(255);
			lock(Users){
				using(GameServerPacket tomsg=new GameServerPacket(StocMessage.ClientChat)){
					tomsg.WriteUnicode(client.Name, 20);
					tomsg.WriteUnicode(toname, 20);
					tomsg.WriteUnicode(msg, msg.Length + 1);
					tomsg.Use();
					if(!string.IsNullOrEmpty(toname)){
						if(Users.ContainsKey(toname)){
							Logger.Debug("private chat:"+toname);
							Users[toname].Client.SendPackage(tomsg.Content, true);
							client.Client.SendPackage(tomsg.Content, true);
						}else{
							//没有该玩家
							client.LobbyError(string.Format(Messages.ERR_NO_CLIENT, toname));
						}
					}else{
						Logger.Debug("send :" +Users.Count);
						SendAll(tomsg.Content, true);
					}
				}
			}
			return true;
		}
		#endregion
	}
}
