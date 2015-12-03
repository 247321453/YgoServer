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
using System.IO;

namespace YGOCore
{
	/// <summary>
	/// 客户端
	/// </summary>
	public class Session
	{
		#region member
		public RoomServer Server{get; private set;}
		public Connection<Session> Client{get; private set;}

        public bool IsLogin;

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
		public DuelServer ServerInfo;
		#endregion
		
		#region public
		public Session(Connection<Session> client,RoomServer server,int timeout=15)
		{
			this.Client = client;
			this.Client.Tag = this;
			this.Server = server;
			MyTimer CheckTimer = new MyTimer(1000, timeout);
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

        public bool OnCheck()
        {
            if (Client.ReceiveQueue.Count > 2)
            {
                byte[] blen = new byte[2];
                Client.ReceiveQueue.Dequeue(blen);
                int len = BitConverter.ToUInt16(blen, 0);
                byte[] data;
                int lastcount = Client.ReceiveQueue.Count;
                if (Client.ReceiveQueue.Count >= len)
                {
                    data = new byte[len];
                    Client.ReceiveQueue.Dequeue(data);
                    PacketReader packet = new PacketReader(data);
                    if(ClinetEvent.Handler(this, packet) == RoomMessage.Info)
                    {
                        return true;
                    }
                    //Logger.Debug("add packet");
                }
                else {
                    data = new byte[lastcount];
                    Client.ReceiveQueue.Dequeue(data);
                    Client.ReceiveQueue.Enqueue(blen);
                    Client.ReceiveQueue.Enqueue(data);
                }
            }
            Close();
            return false;
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
					byte[] data;
                    int lastcount = Client.ReceiveQueue.Count;
                    if (Client.ReceiveQueue.Count >= len){
                        data = new byte[len];
                        Client.ReceiveQueue.Dequeue(data);
						PacketReader packet = new PacketReader(data);
						packets.Add(packet);
						//Logger.Debug("add packet");
					}else{
                        data = new byte[lastcount];
                        Client.ReceiveQueue.Dequeue(data);
                        Client.ReceiveQueue.Enqueue(blen);
                        Client.ReceiveQueue.Enqueue(data);
                        break;
					}
				}
			}
			//处理游戏事件
			ClinetEvent.Handler(this, packets.ToArray());
		}
		#endregion

	}
}
