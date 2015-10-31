/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/10/31
 * 时间: 15:45
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Net;
using System.Collections.Generic;
using AsyncServer;
using OcgWrapper;
using System.Net.Sockets;
using YGOCore.Game;
using System.IO;
using System.Threading;

namespace YGOCore.Core
{
	public class GameServer
	{
		#region member
		private AsyncTcpListener<GameSession> m_listener;
		private static List<string> banNames=new List<string>();
		private bool IsListening = false;
		#endregion
		
		public GameServer()
		{
		}
		
		#region socket
		public bool Start(int port = 0)
		{
			try
			{
				Api.Init(Program.Config.Path, Program.Config.ScriptFolder, Program.Config.CardCDB);
				BanlistManager.Init(Program.Config.BanlistFile);
				MsgSystem.Init(Program.Config.File_ServerMsgs);
				WinInfo.Init(Program.Config.WinDbName);
				ReadBanNames();
				m_listener = new AsyncTcpListener<GameSession>(IPAddress.Any, port == 0 ? Program.Config.ServerPort : port);
				m_listener.OnConnect    += new AsyncTcpListener<GameSession>.ConnectEventHandler(Listener_OnConnect);
				m_listener.OnReceive    += new AsyncTcpListener<GameSession>.ReceiveEventHandler(Listener_OnReceive);
				m_listener.OnTimeout    += new AsyncTcpListener<GameSession>.TimeoutEventHandler(Listener_OnTimeout);
				m_listener.Start();
				IsListening = true;
			}
			catch (SocketException)
			{
				Logger.Error("The " + (port == 0 ? Program.Config.ServerPort : port) + " port is currently in use.");
				return false;
			}
			catch (Exception e)
			{
				Logger.Error(e);
				return false;
			}

			Logger.Info("Listening on port " + (port == 0 ? Program.Config.ServerPort : port));
			if(BanlistManager.Banlists!=null && BanlistManager.Banlists.Count>0){
				Logger.Info("Banlist = "+BanlistManager.Banlists[0].Name);
			}
			return true;
		}
		
		public void Stop(){
			if(IsListening){
				m_listener.Stop();
				IsListening = false;
			}
		}
		#endregion
		
		#region listener
		void Listener_OnTimeout(Connection<GameSession> Client, double time)
		{
			if(Client!=null){
				if(Client.Tag!=null){
					Client.Tag.Close();
				}
				m_listener.DisconnectClient(Client);
			}
		}

		void Listener_OnReceive(Connection<GameSession> Client)
		{
			if(Client!=null && Client.Tag!=null){
				//处理接收数据
				ThreadPool.QueueUserWorkItem(Client.Tag.OnReceive);
			}
		}
		
		void Listener_OnConnect(Connection<GameSession> Client)
		{
			if(Client!=null){
				//绑定关系
				GameSession session = new GameSession();
				session.Client = Client;
				Client.Tag = session;
			}
		}
		#endregion
		
		#region public method
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns>false=被禁止</returns>
		public static bool CheckPlayerBan(string name){
			if(string.IsNullOrEmpty(name)){
				return false;
			}
			name = name.Split('$')[0];
			if(Program.Config.BanMode==0){
				return true;
			}
			else if(Program.Config.BanMode==1){
				return !banNames.Contains(name);
			}
			else{
				return banNames.Contains(name);
			}
		}
		#endregion
		
		#region private method
		private static void ReadBanNames()
		{
			if(File.Exists(Program.Config.File_BanAccont)){
				string[] lines = File.ReadAllLines(Program.Config.File_BanAccont);
				foreach(string line in lines){
					if(string.IsNullOrEmpty(line)||line.StartsWith("#")){
						continue;
					}
					string name=line.Trim();
					if(!banNames.Contains(name)){
						banNames.Add(name);
					}
				}
			}
		}
		#endregion
		
	}
}
