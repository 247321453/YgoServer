
using System;
using YGOCore.Game;
using System.Collections.Generic;
using OcgWrapper;
using System.Threading;
using System.Text;
using AsyncServer;

namespace YGOCore.Net
{
	public static class ServerEvent
	{
		#region 登录/登出
		public static bool OnLogin(this GameServer server, string name, string pwd){
			return false;
		}
		public static void OnLogout(this GameServer server, GameSession client){
			
		}
		#endregion
		
		#region 房间事件
		public static void OnJoinRoom(this GameServer server,GameRoom room, GameSession client){
			if(room == null||room.Config == null){
				return;
			}
			string roomName = room.Name;
			string playerName = client.Name==null?"":client.Name;
			lock(server.Rooms){
				if(server.Rooms.ContainsKey(roomName)){
					//房间存在
					RoomInfo info = null;
					info =server.Rooms[roomName];
					if(info != null && !string.IsNullOrEmpty(playerName)){
						if(info.Room == null){
							info.Room = room;
						}
						info.players.Add(playerName);
					}
				}else{
					//房间不存在
					RoomInfo info= GameRoom.GetRoomInfo(room);
					if(info!=null){
						info.Room = room;
						if(!string.IsNullOrEmpty(playerName)){
							info.players.Add(playerName);
						}
						server.Rooms.Add(roomName, info);
					}
				}
			}
		}
		public static void OnLeaveRoom(this GameServer server,GameRoom room, GameSession client){
			if(room == null||room.Config == null){
				return;
			}
			string roomName = room.Name;
			lock(server.Rooms){
				if(server.Rooms.ContainsKey(roomName)){
					//房间存在
					if(client==null){
						Logger.Debug("remove room");
						server.Rooms.Remove(roomName);
						return;
					}
					RoomInfo info=server.Rooms[roomName];
					string playerName = client.Name;
					if(info.players.Contains(playerName))
						info.players.Remove(playerName);
					if(info.observers.Contains(playerName))
						info.observers.Remove(playerName);
					if(info.players.Count==0 && info.observers.Count==0){
						server.Rooms.Remove(roomName);
						Logger.Debug("remove room");
					}
				}
			}
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
			lock(server.Rooms){
				if(server.Rooms.ContainsKey(name)){
					//存在这个房间
					RoomInfo info = server.Rooms[name];
					if(info!=null && info.Room!=null && info.Room.Config!=null){
						string roomname = info.Room.Config.Name;
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
			lock(server.Rooms){
				if (server.Rooms.ContainsKey(roomname)){
					RoomInfo info = server.Rooms[roomname];
					if(info!=null && info.Room!=null){
						return info.Room;
					}
				}
			}
			lock(server.Rooms){
				if(server.Rooms.Count >= server.Config.MaxRoomCount){
					return null;
				}
			}
			//创建房间
			GameRoom room = new GameRoom(config, server);
			lock(server.Rooms){
				RoomInfo info = GameRoom.GetRoomInfo(room);
				info.Room = room;
				server.Rooms.Add(room.Name, info);
			}
			return room;
		}
		/// <summary>
		/// 得到一个随机房间，不能带密码
		/// </summary>
		/// <param name="server"></param>
		/// <returns></returns>
		public static GameRoom GetRandomGame(this GameServer server){
			List<GameRoom> rooms = null;
			lock(server.Rooms){
				rooms = GetNoPwdRoom(server.Rooms);
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
			lock(server.Rooms){
				rooms = GetNoPwdRoom(server.Rooms, tag);
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
		
		
		private static List<GameRoom> GetNoPwdRoom(SortedList<string, RoomInfo> rooms, string tag=null)
		{
			List<GameRoom> roomList=new List<GameRoom>();
			foreach(RoomInfo info in rooms.Values){
				if(!info.NeedPass && info.Room!=null){
					if(info.Room.Name == null){
						continue;
					}
					if(!string.IsNullOrEmpty(tag) && !info.Room.Name.StartsWith(tag)){
						continue;
					}
					if(info.Room.IsOpen && info.Room.GetAvailablePlayerPos() >=0){
						roomList.Add(info.Room);
					}
				}else{
					Logger.Debug("null:"+(info.Room!=null));
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
		
	}
}
