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
		public static List<RoomInfo> getRoomInfos(){
			List<RoomInfo> rooms=new List<RoomInfo>();
			GameRoom[] grooms;
			//MutexRooms.WaitOne();
			grooms = new GameRoom[m_rooms.Count];
			m_rooms.Values.CopyTo(grooms, 0);
			//MutexRooms.ReleaseMutex();
			foreach(GameRoom room in grooms){
				if(room!=null&&room.Game!=null){
					RoomInfo info=new RoomInfo(room.Game);
					rooms.Add(info);
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
				if (room.Game.State == GameState.Lobby
				    && (filter == -1 ? true : room.Game.Config.Rule == filter))
					filteredRooms.Add(room);
			}

			if (filteredRooms.Count == 0)
				return null;

			return filteredRooms[Program.Random.Next(0, filteredRooms.Count)];
		}

		public static GameRoom SpectateRandomGame()
		{
			List<GameRoom> filteredRooms = new List<GameRoom>();
			GameRoom[] rooms;
			//MutexRooms.WaitOne();
			rooms = new GameRoom[m_rooms.Count];
			m_rooms.Values.CopyTo(rooms, 0);
			//MutexRooms.ReleaseMutex();

			foreach (GameRoom room in rooms)
			{
				if (room.Game.State != GameState.Lobby)
					filteredRooms.Add(room);
			}

			if (filteredRooms.Count == 0)
				return null;

			return filteredRooms[Program.Random.Next(0, filteredRooms.Count)];
		}

		private static GameRoom CreateRoom(GameConfig config)
		{
			GameRoom room = new GameRoom(config);
			//MutexRooms.WaitOne();
			m_rooms.Add(config.Name, room);
			//Logger.WriteLine(String.Format("++Game:{0}",config.Name));
			//MutexRooms.ReleaseMutex();
			return room;
		}
		public static bool SendWarringMessage(string finalmsg, string name=null){
			return SendMessage(finalmsg, PlayerType.Yellow, name);
		}
		public static bool SendErrorMessage(string finalmsg, string name=null){
			return SendMessage(finalmsg, PlayerType.Red, name);
		}
		private static bool SendMessage(string finalmsg,PlayerType type, string name=null){
			GameRoom[] rooms;
			//MutexRooms.WaitOne();
			rooms = new GameRoom[m_rooms.Count];
			m_rooms.Values.CopyTo(rooms, 0);
			//MutexRooms.ReleaseMutex();
			foreach (GameRoom room in rooms)
			{
				if (room.IsOpen){
					try{
						GameServerPacket msg=getMessage(finalmsg,type);
						if(string.IsNullOrEmpty(name)){
							room.Game.SendToAll(msg);
							return true;
						}else{
							foreach(Player pl in room.Game.Players){
								if(pl!=null && pl.Name==name){
									pl.Send(msg);
									return true;
								}
							}
						}
					}catch(Exception){
						
					}
				}
			}
			return false;
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
				if (room.Value.IsOpen)
					room.Value.HandleGame();
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
			int index=name.LastIndexOf("#");
			string namepass=name;
			if(index>0 || (index+1) < name.Length){
				namepass=name.Substring(index+1);
			}
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
					if(key.StartsWith(_names[0]+"$")){
						//Logger.WriteLine("check pass");
						return key==namepass;
					}
				}
			}
			return true;
		}

		public static string RandomRoomName()
		{
			while (true) //keep searching till one is found!!
			{
				string GuidString = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
				GuidString = GuidString.Replace("=", "");
				GuidString = GuidString.Replace("+", "");
				GuidString = GuidString.Replace("#", "");
				GuidString = GuidString.Replace("$", "");
				string roomname = GuidString.Substring(0, 5);
				//MutexRooms.WaitOne();
				if (!m_rooms.ContainsKey(roomname)){
					//MutexRooms.ReleaseMutex();
					return roomname;
				}else{
					//MutexRooms.ReleaseMutex();
				}
			}
		}
		public static string RandomRoomName(String tag)
		{
			string[] rooms;
			//MutexRooms.WaitOne();
			rooms = new string[m_rooms.Count];
			m_rooms.Keys.CopyTo(rooms, 0);
			//MutexRooms.ReleaseMutex();
			foreach(string name in rooms){
				if(name.StartsWith(tag)){
					return name;
				}
			}
			return null;
		}

	}
}
