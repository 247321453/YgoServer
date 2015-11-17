/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/11/17
 * 时间: 16:02
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using AsyncServer;
using System.Collections.Generic;
using System.Diagnostics;
using YGOCore.Game;

namespace YGOCore
{

	/// <summary>
	/// Description of DuelServer.
	/// </summary>
	public class DuelServer
	{
		public RoomServer Server{get;private set;}
		public Connection<DuelServer> Client{get;private set;}
		private bool m_close;
		public readonly byte[] AsyncLock=new byte[0];
		public int Port{get; private set;}
		public bool NeedAuth{get;private set;}
		public int Count{
			get{lock(AsyncLock)return m_count;}
			set{lock(AsyncLock)m_count=value;}
		}
		private int m_count;
		public readonly Dictionary<string, GameConfig> Rooms = new Dictionary<string, GameConfig>();
		public DuelServer(RoomServer server,Connection<DuelServer> client)
		{
			this.Server =server;
			this.Client  = client;
		}
		public void Close(){
			if(m_close)return;
			m_close= true;
			if(Client!=null){
				Client.Close();
			}
		}
		public void OnRecevice(){
			//api
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
			ServerEvent.Handler(this, packets);
		}
	}
}
