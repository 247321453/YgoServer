/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/11/12
 * 时间: 16:26
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using AsyncServer;
using YGOCore.Game;
using System.Threading;

namespace YGOCore
{
	public delegate void OnRoomCreateEvent(Server server,string name,string banlist,string gameinfo);
	public delegate void OnRoomStartEvent(Server server,string name);
	public delegate void OnRoomCloseEvent(Server server,string name);
	public delegate void OnPlayerJoinEvent(Server server,string name,string room);
	public delegate void OnPlayerLeaveEvent(Server server,string name,string room);
	public delegate void OnCommandEvent(Server server, string line);
	public delegate void OnServerCloseEvent(Server server);
	                                
	/// <summary>
	/// 服务信息
	/// </summary>
	public class Server
	{
		
		#region
		public event OnServerCloseEvent OnServerClose;
		public event OnRoomCreateEvent OnRoomCreate;
		public event OnRoomStartEvent OnRoomStart;
		public event OnRoomCloseEvent OnRoomClose;
		public event OnPlayerJoinEvent OnPlayerJoin;
		public event OnPlayerLeaveEvent OnPlayerLeave;
		public event OnCommandEvent OnCommand;
		private const char SEP = '\t';
		private const string HEAD="::";
		public bool IsOpen {get; private set;}
		private Process process;
		public readonly SortedList<string, GameConfig> Rooms=new SortedList<string, GameConfig>();
		public readonly SortedList<string, List<string>> Users=new SortedList<string, List<string>>();
		private string m_config;
		private string m_fileName;
		private Thread m_read;
		public int Port{get;private set;}
		public bool NeedAuth{get;private set;}
		public int RoomCount{get{lock(Rooms)return Rooms.Count;}}
		public Server(string fileName,string config="config.txt")
		{
			m_config = config;
			m_fileName = fileName;
			Logger.Debug(fileName+":"+config);
		}

		public override string ToString()
		{
			int room = 0;
			int user = 0;
			lock(Rooms) room = Rooms.Count;
			lock(Users) user= Users.Count;
			return Port+" "+"Rooms:"+room+",Users:"+user;
		}
		#endregion

		#region api
		private void OnRead(){
			//读取控制台内容
			while(process!=null && IsOpen && !process.HasExited){
				string str = process.StandardOutput.ReadLine();
//				ThreadPool.QueueUserWorkItem(new WaitCallback(
//					(object obj)=>{
//						OnEvent(str);
//					}
//				), str);
				OnEvent(str);
			}
		}
		#region 事件
		private void OnEvent(string line){
			if(line == null || !line.StartsWith(HEAD)){
				if(OnCommand!=null){
					OnCommand(this, line);
				}
				return;
			}
			line = line.Substring(HEAD.Length);
			string[] args= line.Split(SEP);
			Logger.Debug(""+line);
			switch(args[0]){
				case "server":
					if(args.Length>2){
						int p = 0;
						int.TryParse(args[1], out p);
						Port = p;
						NeedAuth = args[2].ToLower() == "true";
					}
					else{
						Logger.Warn("server");
					}
					break;
				case "create":
					if(args.Length>3){
						RoomCreate(args[1], args[2], args[3]);
					}
					else{
						Logger.Warn("create");
					}
					break;
				case "start":
					if(args.Length>1){
						RoomStart(args[1]);
					}
					else{
						Logger.Warn("start");
					}
					break;
				case "close":
					if(args.Length>1){
						RoomClose(args[1]);
					}
					else{
						Logger.Warn("close");
					}
					break;
				case "leave":
					if(args.Length>2){
						OnPlayer(args[1], args[2], false);
					}
					else{
						Logger.Warn("leave");
					}
					break;
				case "join":
					if(args.Length>2){
						OnPlayer(args[1], args[2], true);
					}
					else{
						Logger.Warn("join");
					}
					break;
			}
		}
		#endregion
		
		private void RoomCreate(string name,string banlist,string gameinfo){
			GameConfig config = new GameConfig();
			config.Parse(gameinfo);
			config.Name = gameinfo;
			config.BanList = banlist;
			lock(Rooms){
				if(Rooms.ContainsKey(name)){
					Rooms[name] = config;
				}else{
					Rooms.Add(name, config);
				}
			}
			if(OnRoomCreate!=null){
				OnRoomCreate(this, name, banlist, gameinfo);
			}
		}
		private void RoomClose(string name){
			lock(Rooms){
				if(Rooms.ContainsKey(name)){
					if(Rooms[name]!=null){
						Rooms[name].IsStart = false;
					}
					Rooms.Remove(name);
				}
			}
			if(OnRoomClose!=null){
				OnRoomClose(this, name);
			}
		}
		private void RoomStart(string name){
			lock(Rooms){
				if(Rooms.ContainsKey(name)){
					if(Rooms[name]!=null){
						Rooms[name].IsStart = true;
					}
				}
			}
			if(OnRoomStart!=null){
				OnRoomStart(this, name);
			}
		}
		
		private void OnPlayer(string name,string room,bool join){
			List<string> rooms;
			lock(Users){
				if(join){
					if(Users.ContainsKey(name)){
						rooms= Users[name];
						if(!rooms.Contains(room)){
							rooms.Add(room);
						}
					}else{
						rooms = new List<string>();
						rooms.Add(room);
						Users.Add(name, rooms);
					}
				}else{
					if(Users.ContainsKey(name)){
						rooms= Users[name];
						if(rooms.Contains(room)){
							rooms.Remove(room);
						}
					}
				}
			}
			if(join){
				if(OnPlayerJoin!=null){
					OnPlayerJoin(this, name, room);
				}
			}else{
				if(OnPlayerLeave!=null){
					OnPlayerLeave(this, name, room);
				}
			}
		}
		#endregion
		
		#region process
		public void Start(){
			IsOpen = true;
			if(process==null||process.HasExited){
				process=new Process();
			}
			process.StartInfo.FileName = m_fileName;
			//设定程式执行参数
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.Arguments = " "+m_config;
			process.EnableRaisingEvents=true;
			process.StartInfo.RedirectStandardInput = false;
			process.StartInfo.RedirectStandardOutput = true;
			//		process.StartInfo.RedirectStandardError = false;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.WindowStyle=ProcessWindowStyle.Hidden;
			#if DEBUG
			process.StartInfo.CreateNoWindow = false;
			process.StartInfo.WindowStyle=ProcessWindowStyle.Normal;
			#endif
			process.Exited+=new EventHandler(Exited);
			try{
				process.Start();
			}catch(Exception e){
				Logger.Error(e);
			}
			if(m_read!=null){
				try{
					m_read.Interrupt();
				}catch(Exception){
					
				}
			}
			if(m_read==null){
				m_read = new Thread(new ThreadStart(OnRead));
				m_read.IsBackground = true;
				m_read.Start();
			}
		}
		private void Exited(object sender, EventArgs e){
			lock(Rooms){
				Rooms.Clear();
			}
			lock(Users){
				Users.Clear();
			}
			if(IsOpen){
				Close();
				//异常结束
				Start();
			}else{
				Close();
			}
		}
		public void Close(){
			if(!IsOpen) return;
			IsOpen = false;
			if(OnServerClose!=null){
				OnServerClose(this);
			}
			try{
				m_read.Interrupt();
				m_read= null;
			}catch(Exception){
				
			}
			try{
				process.Kill();
				process.Close();
				process = null;
			}catch(Exception){
				
			}
		}
		#endregion
	}
}
