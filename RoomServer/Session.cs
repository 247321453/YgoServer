/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/5
 * 时间: 21:25
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using AsyncServer;
using System.Collections.Generic;

namespace YGOCore
{
	/// <summary>
	/// Description of Session.
	/// </summary>
	public class Session
	{
		Connection<Session> Client;
		bool close;
		public RoomServer RoomServer {get;private set;}
		public Session(RoomServer server,Connection<Session> client)
		{
			RoomServer = server;
			Client = client;
		}
		public void Close(){
			if(close) return;
			close = true;
			Client.Close();
		}
		public void OnReceive(){
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
			this.Handler(packets);
		}
	}
}
