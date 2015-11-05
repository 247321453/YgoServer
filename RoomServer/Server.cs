/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/5
 * 时间: 20:41
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using AsyncServer;
using System.Collections.Generic;

namespace YGOCore
{
	public class RoomInfo{
		public string BanList;
		public ushort Rule;
		public ushort Mode;
		public bool EnablePriority;
		public bool NoCheckDeck;
		public bool NoShuffleDeck;
		public int StartLp;
		public int StartHand;
		public int DrawCount;
		public int GameTimer;
		public string Name;
		
		public bool IsStart;
		public readonly List<string> Players =new List<string>();
	}
	/// <summary>
	/// Description of Server.
	/// </summary>
	public class Server
	{
		Connection<Server> Client;
		public RoomServer RoomServer {get;private set;}
		public readonly SortedList<string, RoomInfo> Rooms =new SortedList<string, RoomInfo>();
		public Server(RoomServer server,Connection<Server> client)
		{
			RoomServer = server;
			Client = client;
		}
		public string Name;
		public string Desc;
		public int Version;
		public int Port;
		
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
