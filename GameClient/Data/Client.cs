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
using GameClient.Data;
using OcgWrapper.Enums;
using YGOCore;
using YGOCore.Game;

namespace GameClient
{
	public delegate void OnLoginHandler(ServerInfo info);
	public delegate void OnServerChatHandler(string pname, string tname, string msg);
	public delegate void OnRoomCreateHandler(GameConfig config);
	public delegate void OnRoomStartHandler(string name);
	public delegate void OnRoomCloseHandler(string name);
	public delegate void OnRoomListHandler(List<GameConfig> configs);
	/// <summary>
	/// Description of Client.
	/// </summary>
	public class Client
	{
		public event OnLoginHandler OnLogin;
		public event OnServerChatHandler OnServerChat;
		public event OnRoomCreateHandler OnRoomCreate;
		public event OnRoomStartHandler OnRoomStart;
		public event OnRoomCloseHandler OnRoomClose;
		public event OnRoomListHandler OnRoomList;
		private TcpClient client;
		public string Name="???";
		public string Pwd = "";
		public ServerInfo GameServerInfo;
		public bool IsLogin=false;
		readonly ArrayQueue<byte> ReceviceQueue=new ArrayQueue<byte>();
		//名字，房间名
		public Client()
		{
		}
		
		#region socket
		public bool Connect(ServerInfo server){
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
		
		public void GetRooms(){
			using(PacketWriter writer=new PacketWriter(2)){
				writer.Write((byte)CtosMessage.RoomList);
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
				writer.Write((byte)CtosMessage.PlayerInfo);
				writer.WriteUnicode("client",20);
				writer.WriteUnicode(Name, 20);
				writer.WriteUnicode(pwd, 32);
				writer.Use();
				Send(writer.Content);
			}
		}
		/// <summary>
		/// 发送聊天消息
		/// </summary>
		public bool OnChat(string msg, string toname=""){
			using(PacketWriter writer=new PacketWriter(2)){
				writer.Write((byte)CtosMessage.Chat);
				writer.WriteUnicode(Name, 20);
				writer.WriteUnicode(toname, 20);
				writer.WriteUnicode(msg, msg.Length+1);
				writer.Use();
				AsyncSend(writer.Content);
			}
			return false;
		}
		public void OnServerInfo(ServerInfo info){
			IsLogin = true;
			GameServerInfo = info;
			if(OnLogin!=null){
				OnLogin(info);
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
		#endregion
		
		#region room
		public void OnServerRoomCreate(GameConfig config){
			if(OnRoomCreate!=null){
				OnRoomCreate(config);
			}
		}
		public void OnServerRoomClose(string name){
			if(OnRoomClose!=null){
				OnRoomClose(name);
			}
		}
		public void OnServerRoomStart(string name){
			if(OnRoomStart!=null){
				OnRoomStart(name);
			}
		}
		public void OnServerRoomList(List<GameConfig> configs){
			if(OnRoomList!=null){
				OnRoomList(configs);
			}
		}
		#endregion
		

	}
}
