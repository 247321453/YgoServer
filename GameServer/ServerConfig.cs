using System;
using System.IO;
using AsyncServer;

namespace YGOCore
{
	public class ServerConfig
	{
		/// <summary>
		/// 服务端口
		/// </summary>
		public int ServerPort { get; private set; }
		/// <summary>
		/// api端口
		/// </summary>
		public int ApiPort{get;private set;}
		/// <summary>
		/// 工作目录
		/// </summary>
		public string Path { get; private set; }
		/// <summary>
		/// 超时自动结束回合
		/// </summary>
		public bool AutoEndTurn { get; private set; }
		/// <summary>
		/// 客户端版本
		/// </summary>
		public int ClientVersion { get; private set; }
		/// <summary>
		/// 异步模式
		/// </summary>
		public bool AsyncMode{get;private set;}
		/// <summary>
		/// 日志等级
		/// </summary>
		public int LogLevel{get;private set;}
		/// <summary>
		/// 需要密码
		/// </summary>
		public bool isNeedAuth{get;private set;}
		/// <summary>
		/// 最大客户端数量
		/// </summary>
		public int MaxRoomCount{get;private set;}
		/// <summary>
		/// 最大AI数
		/// </summary>
		public int MaxAICount{get;private set;}
		/// <summary>
		/// AI密码
		/// </summary>
		public string AIPass{get;private set;}
		/// <summary>
		/// 帐号禁止模式
		/// 0 不禁止
		/// 1 禁止列表
		/// 2 只允许列表
		/// </summary>
		public int BanMode{get;private set;}
		/// <summary>
		/// 超时
		/// </summary>
		public int Timeout{get;private set;}
		/// <summary>
		/// 控制台api 
		/// </summary>
		public bool ConsoleApi{get;private set;}

		public ServerConfig()
		{
			ClientVersion = 0x1336;
			ServerPort = 8911;
			//	ApiIp="127.0.0.1";
			Path = ".";
			AutoEndTurn = true;
			isNeedAuth=false;
			MaxRoomCount=200;
			//PrivateChat=false;
			//SaveRecordTime=1;//
			MaxAICount=10;
			AIPass="kenan123";
			AsyncMode=false;
			BanMode = 0;
			Timeout = 15;
			ConsoleApi = true;
			//	Timeout = 20;
		}

		public bool Load(string file = "config.txt")
		{
			bool loaded = false;
			if (File.Exists(file))
			{
				StreamReader reader = null;
				try
				{
					reader = new StreamReader(File.OpenRead(file));
					while (!reader.EndOfStream)
					{
						string line = reader.ReadLine();
						if (line == null) continue;
						line = line.Trim();
						if (line.Equals(string.Empty)) continue;
						if (!line.Contains("=")) continue;
						if (line.StartsWith("#")) continue;

						string[] data = line.Split(new[] { '=' }, 2);
						string variable = data[0].Trim().ToLower();
						string value = data[1].Trim();
						setValue(variable, value);
					}
					loaded = true;
				}
				catch (Exception ex)
				{
					Logger.Error(ex);
				}finally{
					reader.Close();
				}
			}
			return loaded;
		}
		public void setServerPort(int port){
			ServerPort = port;
		}
		public bool setValue(string variable,string value){
			if(string.IsNullOrEmpty(value)||string.IsNullOrEmpty(variable)){
				return false;
			}
			variable=variable.ToLower();
			switch (variable)
			{
				case "apiport":
					ApiPort = Convert.ToInt32(value);
					break;
				case "aipassword":
					AIPass=value;
					break;
				case "maxai":
					MaxAICount=Convert.ToInt32(value);
					break;
				case "serverport":
					ServerPort = Convert.ToInt32(value);
					break;
				case "path":
					Path = value;
					break;
				case "bannamemode":
					BanMode = Convert.ToInt32(value);
					break;
				case "loglevel":
					LogLevel = Convert.ToInt32(value);
					break;
				case "autoendturn":
					AutoEndTurn = Convert.ToBoolean(value);
					break;
				case "clientversion":
					ClientVersion = Convert.ToInt32(value, 16);
					break;
				case "needauth":
					isNeedAuth = (value.ToLower()=="true"||value=="1");
					break;
				case "maxroom":
					MaxRoomCount=Convert.ToInt32(value);
					if(MaxRoomCount<=10){
						MaxRoomCount = 10;
					}
					break;
				case "asyncmode":
					AsyncMode= (value.ToLower()=="true"||value=="1");
					break;
				case "timeout":
					Timeout = Convert.ToInt32(value);
					break;
				case "consoleapi":
					ConsoleApi = Convert.ToBoolean(value);
					break;
				default:
					return false;
			}
			return true;
		}

	}
}
