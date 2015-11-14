/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/7
 * 时间: 21:09
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.Net.Sockets;

using AsyncServer;
using YGOCore;
using YGOCore.Game;

namespace GameClient
{
	public delegate void OnLoginHandler();
	public delegate void OnServerChatHandler(string pname, string tname, string msg);
	public delegate void OnRoomCreateHandler(GameConfig2 config);
	public delegate void OnRoomStartHandler(int port, string name);
	public delegate void OnRoomCloseHandler(int port, string name);
	public delegate void OnRoomListHandler(List<GameConfig2> configs);
	public delegate void OnPlayerEnterEvent(int port, string name, string room);
	public delegate void OnPlayerLeaveEvent(int port, string name, string room);
	public delegate void OnGameExitedEvent();
	/// <summary>
	/// Description of Client.
	/// </summary>
	public class Client
	{
		public event OnGameExitedEvent OnGameExited;
		public event OnLoginHandler OnLoginSuccess;
		public event OnServerChatHandler OnServerChat;
		public event OnRoomCreateHandler OnRoomCreate;
		public event OnRoomStartHandler OnRoomStart;
		public event OnRoomCloseHandler OnRoomClose;
		public event OnRoomListHandler OnRoomList;
		public event OnPlayerEnterEvent OnPlayerEnter;
		public event OnPlayerLeaveEvent OnPlayerLeave;
		private TcpClient client;
		public string Name="???";
		public string Pwd = "";
		public bool IsLogin=false;
		readonly ArrayQueue<byte> ReceviceQueue=new ArrayQueue<byte>();
		//名字，房间名
		public Client()
		{
		}
		
		#region socket
		public bool Connect(ClientConfig server){
			if(server==null) return false;
			if(client == null){
				client = new TcpClient();
			}
			if(!client.Client.Connected){
				try{
					client.Close();
				}catch{}
				client = new TcpClient();
			}else{
				return true;
			}
			try{
				client.Connect(server.Host, server.Port);
				BeginRecevice();
				return client.Connected;
			}catch{
			}
			return false;
		}
		private void BeginRecevice(){
			byte[] m_buff = new byte[1024];
			try{
				client.Client.BeginReceive(m_buff, 0, m_buff.Length, SocketFlags.None, new AsyncCallback(EndRecevice), m_buff);
			}catch{}
		}
		private void EndRecevice(IAsyncResult ar){
			try{
				byte[] buff = (byte[])ar.AsyncState;
				int len = client.Client.EndReceive(ar);
				lock(ReceviceQueue){
					ReceviceQueue.Enqueue((byte[])ar.AsyncState, 0, len);
				}
				if(len != buff.Length){
					OnRecevice();
				}
			}catch(Exception e){
				Logger.Warn(e);
				System.Windows.Forms.MessageBox.Show("服务器已经断开"
				                                     #if DEBUG
				                                     +"\n"+e
				                                     #endif
				                                    );
			}finally{
				BeginRecevice();
			}
		}
		public void OnRecevice(){
			List<PacketReader> packets=new List<PacketReader>();
			lock(ReceviceQueue){
				while(ReceviceQueue.Count > 2){
					byte[] blen = new byte[2];
					ReceviceQueue.Dequeue(blen);
					int len = BitConverter.ToUInt16(blen, 0);
					byte[] data = new byte[len];
					if(ReceviceQueue.Count >= len){
						ReceviceQueue.Dequeue(data);
						PacketReader packet = new PacketReader(data);
						packets.Add(packet);
						//Logger.Debug("add packet");
					}else{
						break;
					}
				}
			}
			ClientEvent.Handler(this, packets);
		}
		
		public void GetRooms(bool nolock, bool nostart){
			using(PacketWriter writer=new PacketWriter(2)){
				writer.Write((byte)RoomMessage.RoomList);
				writer.Write(nolock);
				writer.Write(nostart);
				writer.Use();
				Send(writer.Content);
			}
		}
		public void Close(){
			try{
				client.Close();
			}catch(Exception){
				
			}
			client = null;
		}
		private void Send(byte[] data){
			try{
				client.Client.Send(data, data.Length, SocketFlags.None);
			}catch(Exception){
				
			}
		}
		private void AsyncSend(byte[] data){
			try{
				client.Client.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(EndSend), data);
			}catch(Exception){
				
			}
		}
		private void EndSend(IAsyncResult ar){
			try{
				client.Client.EndSend(ar);
			}catch(Exception){
				
			}
		}
		#endregion
		
		#region login/chat
		/// <summary>
		/// 联网登录
		/// </summary>
		public void Login(string name, string pwd){
			Name = name;
			Pwd = pwd;
			pwd= Tool.GetMd5(pwd);
			using(PacketWriter writer=new PacketWriter(2)){
				writer.Write((byte)RoomMessage.Info);
				writer.WriteUnicode(Name, 20);
				writer.WriteUnicode(pwd, 32);
				writer.Use();
				Send(writer.Content);
			}
		}
		/// <summary>
		/// 发送聊天消息
		/// </summary>
		public bool OnChat(string msg, bool hidename,string toname=""){
			using(PacketWriter writer=new PacketWriter(2)){
				writer.Write((byte)RoomMessage.Chat);
				if(string.IsNullOrEmpty(toname)){
					if(hidename){
						writer.WriteUnicode("匿名", 20);
					}else{
						writer.WriteUnicode(Name, 20);
					}
				}
				writer.WriteUnicode(toname, 20);
				writer.WriteUnicode(msg, msg.Length+1);
				writer.Use();
				AsyncSend(writer.Content);
			}
			return false;
		}
		public void OnLoginOk(){
			IsLogin = true;
			if(OnLoginSuccess!=null){
				OnLoginSuccess();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pname">消息者</param>
		/// <param name="msg"></param>
		public void ServerChat(string pname, string tname, string msg){
			if(OnServerChat!=null){
				OnServerChat(pname, tname, msg);
			}
		}
		public void JoinRoom(string room){
			string namepwd =Name + "$"+Pwd;
			if(GameUtil.JoinRoom(Program.Config.Host, ""+Program.Config.DuelPort, namepwd, room, GameExited)){
				//暂停游戏
				if(Program.Config.JoinPause){
					using(PacketWriter writer=new PacketWriter(2)){
						writer.Write((byte)RoomMessage.Pause);
						writer.Use();
						AsyncSend(writer.Content);
					}
				}
			}
		}
		private void GameExited(){
			GetRooms(false, true);
			if(OnGameExited!=null){
				OnGameExited();
			}
		}
		#endregion
		
		#region room
		public void OnServerRoomCreate(GameConfig2 config){
			if(OnRoomCreate!=null){
				OnRoomCreate(config);
			}
		}
		public void OnServerRoomClose(int port, string name){
			if(OnRoomClose!=null){
				OnRoomClose(port, name);
			}
		}
		public void OnServerRoomStart(int port, string name){
			if(OnRoomStart!=null){
				OnRoomStart(port, name);
			}
		}
		public void OnServerRoomList(List<GameConfig2> configs){
			if(OnRoomList!=null){
				OnRoomList(configs);
			}
		}
		public void OnServerPlayerEnter(int port, string name, string room){
			if(OnPlayerEnter!=null){
				OnPlayerEnter(port, name, room);
			}
		}
		public void OnServerPlayerLeave(int port, string name, string room){
			if(OnPlayerLeave!=null){
				OnPlayerLeave(port, name, room);
			}
		}
		#endregion
		

	}
}
