using System.Collections.Generic;

namespace YGOCore.Game
{
	public class GameRoom
	{
		public Game Game { get; private set; }
		public List<GameClient> m_clients { get; private set; }
		public bool IsOpen { get; private set; }
		private bool m_closePending { get; set; }

		public GameRoom(GameConfig config)
		{
			m_clients = new List<GameClient>();
			Game = new Game(this, config);
			IsOpen = true;
		}

		public void AddClient(GameClient client)
		{
			m_clients.Add(client);
		}
		public void RemoveClient(GameClient client)
		{
			m_clients.Remove(client);
		}
		public void Close(bool forceclose=false)
		{
			if(forceclose){
				foreach(Player plager in Game.Players){
					if(plager==null){
						continue;
					}
					try{
						plager.Disconnect();
					}catch(KeyNotFoundException){
						
					}
				}
				foreach(Player plager in Game.Observers){
					if(plager==null){
						continue;
					}
					try{
						plager.Disconnect();
					}catch(KeyNotFoundException){
						
					}
				}
			}
			IsOpen = false;
			foreach (GameClient client in m_clients){
				client.Tick();
			}
		}
		public void CloseDelayed()
		{
			foreach (GameClient client in m_clients)
				client.CloseDelayed();
			m_closePending = true;
		}

		public void HandleGameAsync(object obj){
			HandleGame();
		}
		public void HandleGame()
		{
			foreach (GameClient user in m_clients)
				user.Tick();

			try{
				Game.TimeTick();
			}catch{}
			if (m_closePending && m_clients.Count == 0)
				Close();
		}
		
	}
}
