/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/7
 * 时间: 21:09
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Net;
using System.Net.Sockets;
using GameClient.Data;
using System.Threading;

namespace GameClient
{
	/// <summary>
	/// Description of Client.
	/// </summary>
	public class Client
	{
		TcpClient client;
		public Client()
		{
		}
		public bool OnLogin(string name, string pwd){
			return false;
		}
		
		public bool OnChat(string msg, string toname){
			return false;
		}
		
		public bool OnJoinGame(string name){
			return false;
		}
		
		public bool OnLeaveGame(string name){
			return false;
		}
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
				return client.Connected;
			}catch{}
			return false;
		}
		
		public void Recevice(object obj){
			while(client!=null && client.Connected){
				//接收数据
			}
		}
	}
}
