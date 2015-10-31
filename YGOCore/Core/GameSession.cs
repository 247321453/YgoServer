/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/10/31
 * 时间: 15:47
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using AsyncServer;
using System.Collections.Generic;
using YGOCore.Game;

namespace YGOCore.Core
{
	/// <summary>
	/// Description of GameSession.
	/// </summary>
	public class GameSession
	{
		public GameSession()
		{
			m_handler = new GameHandler(this);
		}
		
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
					}else{
						break;
					}
				}
			}
			//处理游戏事件
			m_handler.Handler(packets);
		}
		
		public void OnSend(GameServerPacket packet,bool isAsync){
			if(m_close) return;
			packet.Use();
			//发送大量数据可能会卡
			m_client.SendPackage(packet.Content, isAsync);
		}
		public void Close(){
			if(m_close) return;
			m_close = true;
			m_client.Close();
			m_handler.Disable();
		}
		private bool m_close;
		private GameHandler m_handler;
		private Connection<GameSession> m_client;
		public Connection<GameSession> Client{
			get{return m_client;}set{m_client =value;}
		}
	}
}
