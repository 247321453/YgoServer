/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/11/12
 * 时间: 17:38
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using AsyncServer;
using System.Collections.Generic;

namespace YGOCore
{
	/// <summary>
	/// 客户端
	/// </summary>
	public class Session
	{
		public Connection<Session> Client{get; private set;}
		private bool m_close = false;
		/// <summary>
		/// 暂停推送
		/// </summary>
		public bool IsPause=false;
		/// <summary>
		/// 名字
		/// </summary>
		public string Name;
		/// <summary>
		/// 所在房间名
		/// </summary>
		public string RoomName;
		/// <summary>
		/// 当前所在的服务器
		/// </summary>
		public Server ServerInfo;
		public Session(Connection<Session> client)
		{
			Client = client;
			Client.Tag = this;
		}
		public void Close(){
			if(m_close)return;
			m_close = true;
			try{
				Client.PeekSend();
			}catch(Exception){
				
			}
			try{
				Client.Close();
			}catch(Exception){
				
			}
		}
		public void OnRecevice(){
			if(m_close) return;
			//线程处理
			List<PacketReader> packets=new List<PacketReader>();
			lock(Client.SyncRoot){
				while(Client.ReceiveQueue.Count > 2){
					byte[] blen = new byte[2];
					Client.ReceiveQueue.Dequeue(blen);
					int len = BitConverter.ToUInt16(blen, 0);
					byte[] data = new byte[len];
					if(Client.ReceiveQueue.Count >= len){
						Client.ReceiveQueue.Dequeue(data);
						PacketReader packet = new PacketReader(data);
						packets.Add(packet);
						//Logger.Debug("add packet");
					}else{
						break;
					}
				}
			}
			//处理游戏事件
			ClinetEvent.Handler(this, packets);
		}
	}
}
