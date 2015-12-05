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
using System.IO;

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
		/// <summary>
		/// 人数
		/// </summary>
		public int Count{
			get{lock(AsyncLock)return m_count;}
			set{lock(AsyncLock)m_count=value;}
		}
		private int m_count;
		public readonly Dictionary<string, GameConfig> Rooms = new Dictionary<string, GameConfig>();
		public int RoomCount{
			get{lock(Rooms)return Rooms.Count;}
		}
		public DuelServer(RoomServer server,Connection<DuelServer> client)
		{
			this.Server =server;
			this.Client  = client;
		}
		public override string ToString()
		{
			return "port="+Port+",needauth="+NeedAuth+",players:"+Count+",rooms:"+RoomCount;
		}
 
		public void Init(int port,bool needauth){
			Port = port;
			NeedAuth = needauth;
		}
		public void Close(){
			if(m_close)return;
			m_close= true;
			if(Client!=null){
				Client.Close();
			}
			Server.Close(Port);
		}

        public void Send(PacketWriter writer)
        {
            Client.SendPackage(writer);
        }
        public void Send(byte[] data)
        {
            Client.SendPackage(data);
        }
        public void OnRecevice(){
			//api
			if(m_close) return;
			//线程处理
			List<PacketReader> packets=new List<PacketReader>();
            //线程处理
            bool next = true;
            while (next)
            {
                byte[] data;
                next = Client.GetPacketData(2, out data);
                if (data != null && data.Length > 0)
                {
                    //处理游戏事件
                    PacketReader packet = new PacketReader(data);
                    packets.Add(packet);
                }
            }
			//处理游戏事件
			ServerEvent.Handler(this, packets);
		}
	}
}
