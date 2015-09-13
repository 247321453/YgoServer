using System;
using YGOCore.Game;
using OcgWrapper.Enums;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace YGOCore
{
	public static class GameManager
	{

		//private static Mutex MutexRooms=new Mutex();
		static Dictionary<string, GameRoom> m_rooms = new Dictionary<string,GameRoom>();

		public static GameRoom CreateOrGetGame(GameConfig config)
		{
			//MutexRooms.WaitOne();
			int count=m_rooms.Count;
			if (m_rooms.ContainsKey(config.Name)){
				GameRoom room=m_rooms[config.Name];
				//MutexRooms.ReleaseMutex();
				return room;
			}
			if(count>=Program.Config.MaxRoomCount){
				return null;
			}
			return CreateRoom(config);
		}

		public static GameRoom GetGame(string name)
		{
			//MutexRooms.WaitOne();
			if (m_rooms.ContainsKey(name)){
				GameRoom room=m_rooms[name];
				//MutexRooms.ReleaseMutex();
				return room;
			}
			return null;
		}
		/// <summary>
		/// 房间列表
		/// </summary>
		/// <returns></returns>
		public static List<RoomInfo> getRoomInfos(bool Lock=true,bool Start=true){
			List<RoomInfo> rooms=new List<RoomInfo>();
			GameRoom[] grooms;
			//MutexRooms.WaitOne();
			grooms = new GameRoom[m_rooms.Count];
			m_rooms.Values.CopyTo(grooms, 0);
			//MutexRooms.ReleaseMutex();
			foreach(GameRoom room in grooms){
				if(room!=null&&room.Game!=null){
					if(!Start){
						//仅等待的房间
						if(room.Game.State !=GameState.Lobby){
							continue;
						}
					}
					if(!Lock){
						if(room.Game.Config.HasPassword()){
							continue;
						}
					}
					RoomInfo info=room.Game.GetRoomInfo();
					if(info!=null){
						rooms.Add(info);
					}
				}
			}
			return rooms;
		}

		public static GameRoom GetRandomGame(int filter = -1)
		{
			List<GameRoom> filteredRooms = new List<GameRoom>();
			GameRoom[] rooms;
			//MutexRooms.WaitOne();
			rooms = new GameRoom[m_rooms.Count];
			m_rooms.Values.CopyTo(rooms, 0);
			//MutexRooms.ReleaseMutex();
			foreach (GameRoom room in rooms)
			{
				//随机房间不能带密码
				if (!room.Game.Config.HasPassword()&& room.Game.State == GameState.Lobby
				    && (filter == -1 ? true : room.Game.Config.Rule == filter)){
					filteredRooms.Add(room);
				}
			}

			if (filteredRooms.Count == 0)
				return null;

			return filteredRooms[Program.Random.Next(0, filteredRooms.Count)];
		}
//
//		public static GameRoom SpectateRandomGame()
//		{
//			List<GameRoom> filteredRooms = new List<GameRoom>();
//			GameRoom[] rooms;
//			//MutexRooms.WaitOne();
//			rooms = new GameRoom[m_rooms.Count];
//			m_rooms.Values.CopyTo(rooms, 0);
//			//MutexRooms.ReleaseMutex();
//
//			foreach (GameRoom room in rooms)
//			{
//				if (room.Game.State != GameState.Lobby)
//					filteredRooms.Add(room);
//			}
//
//			if (filteredRooms.Count == 0)
//				return null;
//
//			return filteredRooms[Program.Random.Next(0, filteredRooms.Count)];
//		}

		private static GameRoom CreateRoom(GameConfig config)
		{
			GameRoom room = new GameRoom(config);
			//MutexRooms.WaitOne();
			m_rooms.Add(config.Name, room);
			//Logger.WriteLine(String.Format("++Game:{0}",config.Name));
			//MutexRooms.ReleaseMutex();
			return room;
		}
		public static List<string> SendWarringMessage(string finalmsg, string name=null){
			return SendMessage(getMessage(finalmsg, PlayerType.Yellow), name);
		}
		public static List<string> SendErrorMessage(string finalmsg, string name=null){
			return SendMessage(getMessage(finalmsg, PlayerType.Red), name);
		}
		public static List<string> SendMessage(GameServerPacket msg, string name=null){
			List<string> playernames=new List<string>();
			GameRoom[] rooms;
			//MutexRooms.WaitOne();
			rooms = new GameRoom[m_rooms.Count];
			m_rooms.Values.CopyTo(rooms, 0);
			//MutexRooms.ReleaseMutex();
			foreach (GameRoom room in rooms)
			{
				if (room.IsOpen){
					try{
						if(string.IsNullOrEmpty(name)){
							playernames.AddRange(room.Game.SendToAll(msg).ToArray());
						}else{
							foreach(Player pl in room.Game.Players){
								if(pl!=null && pl.Name==name){
									playernames.Add(pl.Name);
									pl.Send(msg);
								}
							}
							foreach(Player pl in room.Game.Observers){
								if(pl!=null && pl.Name==name){
									playernames.Add(pl.Name);
									pl.Send(msg);
								}
							}
						}
					}catch(Exception){
						
					}
				}
			}
			return playernames;
		}
		
		public static GameServerPacket getMessage(string finalmsg,PlayerType type=PlayerType.Yellow){
			GameServerPacket packet = new GameServerPacket(StocMessage.Chat);
			packet.Write((short)type);
			packet.Write(finalmsg, finalmsg.Length + 1);
			return packet;
		}
		
		public static void HandleRooms()
		{
			List<string> toRemove = new List<string>();
			//MutexRooms.WaitOne();
			foreach (var room in m_rooms)
			{
				if (room.Value.IsOpen){
					if(Program.Config.AsyncMode){
						ThreadPool.QueueUserWorkItem(new WaitCallback(room.Value.HandleGameAsync));
					}else{
						room.Value.HandleGame();
					}
				}
				else{
					toRemove.Add(room.Key);
				}
			}
			//MutexRooms.ReleaseMutex();
			//MutexRooms.WaitOne();
			foreach (string room in toRemove)
			{
				m_rooms.Remove(room);
				//Logger.WriteLine(String.Format("--Game:{0}",room));
			}
			//MutexRooms.ReleaseMutex();
		}

		public static bool GameExists(string name)
		{
			//MutexRooms.WaitOne();
			bool exist=m_rooms.ContainsKey(name);
			//MutexRooms.ReleaseMutex();
			return exist;
		}
		
		public static bool CheckPassword(string name){
			if(string.IsNullOrEmpty(name)){
				return true;
			}
			if(name.IndexOf("$")<0){
				return true;
			}
			//		int index=name.LastIndexOf("#");
			string namepass=name;
//			if(index>0 && (index+1) < name.Length){
//				namepass=name.Substring(index+1);
//			}
			string[] _names=namepass.Split('$');;
			if(_names.Length==1){
				return GameExists(_names[0]);
			}else{
				string[] rooms;
				//MutexRooms.WaitOne();
				rooms = new string[m_rooms.Count];
				m_rooms.Keys.CopyTo(rooms, 0);
				//MutexRooms.ReleaseMutex();
				foreach(string key in rooms){
					//Logger.WriteLine(key+"=="+namepass);
					if(key.StartsWith(_names[0]+"$")){
						return key==namepass;
					}
				}
			}
			return true;
		}

		public static string GetGuidString(){
			string GuidString = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
			GuidString = GuidString.Replace("=", "");
			GuidString = GuidString.Replace("+", "");
			GuidString = GuidString.Replace("#", "");
			GuidString = GuidString.Replace("$", "");
			string roomname = GuidString.Substring(0, 5);
			return roomname;
		}
		
		/// <summary>
		/// 返回没条件的随机名
		/// </summary>
		/// <returns></returns>
		public static string RandomRoomName()
		{
			List<string> rooms=new List<string>();
			rooms.AddRange(m_rooms.Keys);
			while (true) //keep searching till one is found!!
			{
				string GuidString = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
				GuidString = GuidString.Replace("=", "");
				GuidString = GuidString.Replace("+", "");
				GuidString = GuidString.Replace("#", "");
				GuidString = GuidString.Replace("$", "");
				string roomname = GuidString.Substring(0, 5);
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
		/// 返回有条件的随机名，并且从多个随机一个，如果为null，则请
		/// </summary>
		/// <param name="tag"></param>
		/// <returns></returns>
		public static string RandomRoomName(string tag)
		{
			if(tag==null){
				return null;
			}
			if(!tag.EndsWith("#")){
				tag+="#";
			}
			List<string> rooms=new List<string>();
			foreach(string key in m_rooms.Keys){
				if(key!=null&&key.IndexOf('$')<0&&key.StartsWith(tag)){
					rooms.Add(key);
				}
			}
			if(rooms.Count == 0){
				return null;
			}
			int index=Program.Random.Next(rooms.Count);
			string name=null;
			try{
				name=rooms[index];
				//最好做个判断是否满了
				GameRoom room=m_rooms[name];
				if(room!=null){
					if(room.Game.GetAvailablePlayerPos()<0){
						//满了
						name=null;
					}
				}
			}catch(Exception){
				
			}
			return name;
		}

	}
}
