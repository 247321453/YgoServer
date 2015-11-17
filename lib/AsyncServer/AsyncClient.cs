/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/11/10
 * 时间: 9:38
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Net;
using System.Net.Sockets;

namespace AsyncServer
{
	public delegate void OnReceviceHanlder(AsyncClient sender);
	/// <summary>
	/// Description of AsyncClient.
	/// </summary>
	public class AsyncClient
	{
		private TcpClient client;
		public TcpClient Client{get{return client;}}
		public event OnReceviceHanlder OnRecevice;
		public readonly ArrayQueue<byte> ReceviceQueue=new ArrayQueue<byte>();
		public bool Connected{get{return client!=null&&client.Client.Connected;}}
		public AsyncClient()
		{
		}
		public bool Connect(string host,int port){
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
				client.Connect(host, port);
				return client.Connected;
			}catch(Exception e){
				Logger.Warn(e);
			}
			return false;
		}
		
		public void AsyncConnect(string host,int port){
			if(client == null){
				client = new TcpClient();
			}
			if(!client.Client.Connected){
				try{
					client.Close();
				}catch{}
				client = new TcpClient();
			}else{
				return;
			}
			try{
				client.BeginConnect(host, port, new AsyncCallback(delegate(IAsyncResult ar){
				                                                  	try{
				                                                  		client.EndConnect(ar);
				                                                  	}catch{}
				                                                  }), client);
			}catch(Exception e){
				Logger.Warn(e);
			}
		}
		
		public void BeginRecevice(){
			if(!Connected)return;
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
					if(OnRecevice!=null){
						OnRecevice(this);
					}
				}
			}catch(Exception e){
				Logger.Warn(e);
			}finally{
				BeginRecevice();
			}
		}
		public void Send(byte[] data){
			if(!Connected)return;
			try{
				client.Client.Send(data, data.Length, SocketFlags.None);
			}catch(Exception){
				
			}
		}
		public void AsyncSend(byte[] data){
			if(!Connected)return;
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
	}
}
