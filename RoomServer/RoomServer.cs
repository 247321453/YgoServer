/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/11/12
 * 时间: 11:24
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using AsyncServer;

namespace YGOCore
{
	/// <summary>
	/// Description of GameServer.
	/// </summary>
	public class RoomServer
	{
		public bool IsListening{get;private set;}
		public readonly List<Server> Servers=new List<Server>();
		private string[] Configs;
		private string ServerExe;
		private int Port;
		public RoomServer(int port=18910,string serverExe="GameServer.exe",string[] configs=null)
		{
			Port = port;
			ServerExe = serverExe;
			Configs = configs;
		}
		public bool Start(){
			if(Configs!=null){
				foreach(string config in Configs){
					Server server=new Server(ServerExe, config);
					Servers.Add(server);
					server.Start();
				}
			}else{
				Logger.Error("no configs");
			}
			return false;
		}
		public void Stop(){
			//Server.Close();
			if(!IsListening) return;
			IsListening = false;
			lock(Servers){
				foreach(Server server in Servers){
					server.Close();
				}
			}
		}
	}
}
