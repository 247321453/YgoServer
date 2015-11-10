using System;
using AsyncServer;
using System.Collections.Generic;
using YGOCore.Net;
using YGOCore.Game;
using OcgWrapper.Enums;

namespace YGOCore.Net
{
	/// <summary>
	/// Description of GameSession.
	/// </summary>
	public class GameSession
	{
		MyTimer CheckTimer;
		public GameSession(Connection<GameSession> client,int version,int timeout)
		{
			this.m_client = client;
			//异步发送
			this.Type = (int)PlayerType.Undefined;
			this.State = PlayerState.None;
			this.ClientVersion=version;
			CheckTimer = new MyTimer(3000, timeout*1000);
			CheckTimer.AutoReset = true;
			CheckTimer.Elapsed += delegate { 
				if( !string.IsNullOrEmpty(Name) ){
					CheckTimer.Stop();
					CheckTimer.Close();
				}
				if(CheckTimer.CheckStop()){
					//超时自动断开
					Close();
					CheckTimer.Close();
				}
			};
		}
		
		#region Member
		private bool m_close;
		private Connection<GameSession> m_client;
		private string namepassword;
		/// <summary>
		/// 房间
		/// </summary>
		public GameRoom Game;
		/// <summary>
		/// 名字
		/// </summary>
		public string Name {
			get{
				return Password.OnlyName(namepassword);
			}
			set{
				if(value!=null){
					namepassword=value.Trim();
				}
			}
		}
		/// <summary>
		/// 是否认证
		/// </summary>
		public bool IsAuthentified { get; set; }
		/// <summary>
		/// 类型
		/// </summary>
		public int Type { get; set; }
		
		/// <summary>
		/// 跳过的回合数
		/// </summary>
		public int TurnSkip { get; set; }
		/// <summary>
		/// 卡组
		/// </summary>
		public Deck Deck { get; set; }
		/// <summary>
		/// 状态
		/// </summary>
		public PlayerState State { get; set; }
		
		/// <summary>
		/// 是否连接
		/// </summary>
		public bool IsConnected{
			get{return m_client!=null && m_client.Connected;}
		}
		/// <summary>
		/// socket
		/// </summary>
		public Connection<GameSession> Client{
			get{return m_client;}
		}
		public int ClientVersion{get;private set;}
		#endregion

		#region packet
		public void OnReceive(object statu){
			if(m_close) return;
			//线程处理
			List<GameClientPacket> packets=new List<GameClientPacket>();
			lock(m_client.SyncRoot){
				while(m_client.ReceiveQueue.Count > 2){
					byte[] blen = new byte[2];
					m_client.ReceiveQueue.Dequeue(blen);
					int len = BitConverter.ToUInt16(blen, 0);
					byte[] data = new byte[len];
					if(m_client.ReceiveQueue.Count >= len){
						m_client.ReceiveQueue.Dequeue(data);
						GameClientPacket packet = new GameClientPacket(data);
						packets.Add(packet);
						//Logger.Debug("add packet");
					}else{
						break;
					}
				}
			}
			//处理游戏事件
			GameEvent.Handler(this, packets);
		}

		public void Send(GameServerPacket packet,bool isNow = true){
			if(m_close) return;
			packet.Use();
			//	Logger.Debug("send "+System.Text.Encoding.Default.GetString(packet.Content));
			//发送大量数据可能会卡
//			Logger.Debug("packet:"+packet.PacketMsg);
//			if(packet.GameMsg != GameMessage.Waiting){
//				Logger.Debug("game:"+packet.GameMsg);
//			}
			m_client.SendPackage(packet.Content, isNow);
		}
		
		public void PeekSend(){
			try{
				m_client.PeekSend();
			}catch{}
		}
		public void Close(){
			if(m_close) return;
			m_close = true;
			if(Game!=null){
				Game.RemovePlayer(this);
			}
			m_client.Close();
		}
		#endregion
	}
}
