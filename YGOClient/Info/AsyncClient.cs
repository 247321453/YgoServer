/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/6
 * 时间: 21:19
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Net;
using System.Net.Sockets;

namespace YGOClient
{
	/// <summary>
	/// Description of AsyncClient.
	/// </summary>
	public class AsyncClient : TcpClient
	{
		public AsyncClient():base()
		{
		}
		public bool IsConnected{get;private set;}
		public void ConnectTo(string host, int port){
			BeginConnect(host, port, new AsyncCallback(EndConnectTo), this);
		}
		private void EndConnectTo(IAsyncResult ar){
			try{
				Client.EndConnect(ar);
			}catch(Exception){
				IsConnected =false;
			}
		}
		public void BeginSend(byte[] data){
			if(Client.Connected)
				Client.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(EndSend), this);
		}
		
		private void EndSend(IAsyncResult ar){
			Client.EndSend(ar);
		}
	}
}
