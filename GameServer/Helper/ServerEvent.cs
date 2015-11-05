
using System;
using YGOCore.Game;
using System.Collections.Generic;
using OcgWrapper;
using System.Threading;
using System.Text;
using OcgWrapper.Enums;
using AsyncServer;

namespace YGOCore.Net
{
	public static class ServerEvent
	{
		#region 登录/登出
		public static bool OnLogin(this GameServer server, string name, string pwd){
			if(name!=null&&name.StartsWith("[AI]")){
				Logger.Debug("[AI]login:"+pwd+"=="+server.Config.AIPass+"?");
				return server.Config.AIPass == pwd;
			}
			return true;
		}
		public static void OnLogout(this GameServer server, GameSession client){
			
		}
		#endregion
		
		#region 房间事件
		public static List<int> GameCards(this GameServer server, string name){
			List<int> cards =new List<int>();
			GameRoom room = null;
			lock(server.Games){
				if(server.Games.ContainsKey(name)){
					room = server.Games[name];
				}
			}
			if(room !=null && room!=null && room.Players!=null){
				cards.AddRange(room.CardIds);
			}
			return cards;
		}
		public static int GetRoomCount(this GameServer server){
			lock(server.Rooms){
				return server.Rooms.Count;
			}
		}
		public static string GetRoomJson(this GameServer server,bool noPwd,bool isReady){
			List<RoomInfo> infos =new List<RoomInfo>();
			lock(server.Rooms){
				infos.AddRange(server.Rooms.Values);
			}
			int count = infos.Count;
			for(count--;count>=0;count--){
				RoomInfo info = infos[count];
				if(noPwd){
					if(info.NeedPass()){
						infos.RemoveAt(count);
						continue;
					}
				}
				if(isReady){
					if(info.IsStart){
						infos.RemoveAt(count);
						continue;
					}
				}
			}
			return Tool.ToJson(infos);
		}
		public static void OnJoinRoom(this GameServer server,RoomInfo info, GameSession client){
			string roomName = info.RoomName;
			string playerName = client.Name==null?"":client.Name;
			bool isnew = false;
			lock(server.Rooms){
				if(server.Rooms.ContainsKey(roomName)){
					//房间存在
					server.Rooms[roomName] = info;
				}else{
					//房间不存在
					server.Rooms.Add(roomName, info);
					isnew = true;
				}
			}
			server.OnRoomEvent(isnew?StoSMessage.RoomCreate:StoSMessage.RoomUpdate, info);
		}
		public static void OnLeaveRoom(this GameServer server,RoomInfo info, GameSession client){
			string roomName = info.RoomName;
			bool isclose = false;
			lock(server.Rooms){
				if(server.Rooms.ContainsKey(roomName)){
					//房间存在
					if(client == null|| info.NeedClose()){
						server.Rooms.Remove(roomName);
						Logger.Debug("remove room");
						isclose = true;
					}else{
						server.Rooms[roomName] = info;
					}
				}
			}
			if(client == null){
				//所有人离开
				List<string> names = new List<string>();
				foreach(string p in info.players){
					names.Add(p);
				}
				names.AddRange(info.observers);
				server.OnPlayNameEvent(PlayerStatu.PlayerLeave, names.ToArray());
			}
			server.OnRoomEvent(isclose ? StoSMessage.RoomClose:StoSMessage.RoomUpdate, info);
		}
		#endregion
		
		#region 结果
		public static void OnWin(this GameServer server,string roomName, int roomMode,int win,int reason,string yrpFileName,
		                         string[] names,bool force){
			WinInfo info =new WinInfo(roomName, win, reason, yrpFileName, names, force);
			lock(server.WinInfos){
				server.WinInfos.Enqueue(info);
			}
		}
		#endregion
		
		#region 房间
		public static bool CheckRoomPassword(this GameServer server, string namepwd){
			string name = Password.OnlyName(namepwd);
			lock(server.Games){
				if(server.Games.ContainsKey(name)){
					//存在这个房间
					GameRoom room = server.Games[name];
					if(room!=null && room.Config!=null){
						string roomname = room.Config.Name;
						return namepwd == roomname;
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
		public static GameRoom CreateOrGetGame(this GameServer server,GameConfig config){
			string roomname = Password.OnlyName(config.Name);
			lock(server.Games){
				if (server.Games.ContainsKey(roomname)){
					return server.Games[roomname];
				}
			}
			lock(server.Games){
				if(server.Games.Count >= server.Config.MaxRoomCount){
					return null;
				}
			}
			//创建房间
			GameRoom room = new GameRoom(config, server);
			lock(server.Games){
				if (server.Games.ContainsKey(roomname)){
					return server.Games[roomname];
				}else{
					server.Games.Add(roomname, room);
					return room;
				}
			}
		}
		/// <summary>
		/// 得到一个随机房间，不能带密码
		/// </summary>
		/// <param name="server"></param>
		/// <returns></returns>
		public static GameRoom GetRandomGame(this GameServer server){
			List<GameRoom> rooms = null;
			lock(server.Games){
				rooms = GetNoPwdRoom(server.Games);
				if(rooms.Count>0){
					int i = Program.Random.Next(rooms.Count);
					return rooms[i];
				}
			}
			return null;
		}
		/// <summary>
		/// 得到一个不存在的随机房间名
		/// </summary>
		/// <param name="server"></param>
		/// <returns></returns>
		public static string NewRandomRoomName(this GameServer server){
			List<string> rooms=new List<string>();
			lock(server.Rooms){
				rooms.AddRange(server.Rooms.Keys);
			}
			if(rooms.Count==0){
				return server.GetGuidString();
			}
			while (true) //keep searching till one is found!!
			{
				string roomname = server.GetGuidString();
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
		public static string RandomRoomName(this GameServer server,string tag){
			if(string.IsNullOrEmpty(tag) && !tag.EndsWith("#")){
				tag+="#";
			}
			List<GameRoom> rooms = null;
			lock(server.Games){
				rooms = GetNoPwdRoom(server.Games, tag);
			}
			if(rooms.Count == 0){
				return server.GetGuidString();
			}
			if(tag == null){
				return null;
			}
			int index=Program.Random.Next(rooms.Count);
			GameRoom room = rooms[index];
			return room.Config==null?null:room.Config.Name;
		}
		
		
		private static List<GameRoom> GetNoPwdRoom(SortedList<string, GameRoom> rooms, string tag=null)
		{
			List<GameRoom> roomList=new List<GameRoom>();
			foreach(GameRoom room in rooms.Values){
				if(room!=null && room.Config.Name != null && !room.Config.Name.Contains("$")){
					//不为空，没有密码
					if(!string.IsNullOrEmpty(tag) && !room.Name.StartsWith(tag)){
						continue;
					}
					if(room.IsOpen && room.GetAvailablePlayerPos() >=0){
						roomList.Add(room);
					}
				}else{
					Logger.Debug("room is null?"+(room!=null));
				}
			}
			return roomList;
		}
		public static string GetGuidString(this GameServer server){
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
		
		#region 公告
		public static void OnWorldMessage(this GameServer server, string msg, PlayerType color= PlayerType.Yellow){
			List<GameRoom> rooms = new List<GameRoom>();
			lock(server.Games){
				foreach(GameRoom room in server.Games.Values){
					if(room!=null&& room.IsOpen){
						room.ServerMessage(msg, color);
					}
				}
			}
		}
		#endregion
	}
}
