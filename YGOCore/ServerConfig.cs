using System.IO;
using System;
using System.Globalization;

namespace YGOCore
{
	public class ServerConfig
	{
		/// <summary>
		/// 服务端口
		/// </summary>
		public int ServerPort { get; private set; }
		
		//	public string ApiIp{get;private set;}
		public bool ApiIsLocal{get; private set;}
		/// <summary>
		/// api端口
		/// </summary>
		public int ApiPort{get;private set;}
		/// <summary>
		/// 工作目录
		/// </summary>
		public string Path { get; private set; }
		/// <summary>
		/// 脚本目录
		/// </summary>
		public string ScriptFolder { get; private set; }
		/// <summary>
		/// 卡牌数据库
		/// </summary>
		public string CardCDB { get; private set; }
		/// <summary>
		/// 禁卡表文件
		/// </summary>
		public string BanlistFile { get; private set; }
		/// <summary>
		/// log
		/// </summary>
		public bool ErrorLog { get; private set; }
		/// <summary>
		/// 控制台日志
		/// </summary>
		public bool ConsoleLog { get; private set; }
		/// <summary>
		/// 
		/// </summary>
		public bool HandShuffle { get; private set; }
		/// <summary>
		/// 
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
		
//		/// <summary>
//		/// 允许重复登陆
//		/// </summary>
//		public bool RepeatLogin{get;private set;}
		/// <summary>
		/// 需要密码
		/// </summary>
		public bool isNeedAuth{get;private set;}
		/// <summary>
		/// 最大客户端数量
		/// </summary>
		public int MaxRoomCount{get;private set;}
		/// <summary>
		/// 自动录像
		/// </summary>
		public bool AutoReplay{get;private set;}
		/// <summary>
		/// 录像保存文件夹
		/// </summary>
		public string replayFolder { get; private set; }
		
		public bool RecordWin{get;private set;}
		
		/// <summary>
		/// 记录
		/// </summary>
		public string WinDbName{get;private set;}
		/// <summary>
		/// 登录接口
		/// </summary>
		public string LoginUrl{get;private set;}
		
		/// <summary>
		/// 私聊
		/// </summary>
		public bool PrivateChat{get;private set;}
		
		/// <summary>
		/// 服务端名字
		/// </summary>
		public string ServerName{get;private set;}
		/// <summary>
		/// 服务端描述
		/// </summary>
		public string ServerDesc{get;private set;}
		/// <summary>
		/// 公告文本
		/// </summary>
		public string ServerMsgs{get;private set;}
		
		public int MaxAICount{get;private set;}
		
		public bool AIisHide{get;private set;}
		public string AIPass{get;private set;}
		
		//public int Timeout{get;private set;}
		
		public ServerConfig()
		{
			ClientVersion = 0x1335;
			ServerPort = 8911;
			ServerName="YGOserver";
			//	ApiIp="127.0.0.1";
			ApiPort=18911;
			Path = ".";
			ScriptFolder = "script";
			replayFolder="replay";
			CardCDB = "cards.cdb";
			BanlistFile = "lflist.conf";
			ErrorLog = true;
			//RepeatLogin=true;
			ConsoleLog = true;
			HandShuffle = false;
			AutoEndTurn = true;
			isNeedAuth=false;
			MaxRoomCount=200;
			WinDbName="wins.db";
			RecordWin=false;
			PrivateChat=false;
			//SaveRecordTime=1;//
			ServerDesc="Server is Testing.";
			ServerMsgs="server_msg.txt";
			MaxAICount=10;
			AIPass="3ab51053212386455461483e66c65425";//kenan123
			LoginUrl="http://127.0.0.1/login.php";
			AIisHide=false;
			ApiIsLocal=true;
			AsyncMode=false;
		//	Timeout = 20;
		}

		public bool Load(string file = "config.txt")
		{
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
				}
				catch (Exception ex)
				{
					Logger.WriteError(ex);
					reader.Close();
					return false;
				}
				reader.Close();
				return true;
			}

			return false;
		}
		
		public bool setValue(string variable,string value){
			if(string.IsNullOrEmpty(value)||string.IsNullOrEmpty(variable)){
				return false;
			}
			variable=variable.ToLower();
			switch (variable)
			{
				case "aipassword":
					AIPass=value;
					break;
				case "maxai":
					MaxAICount=Convert.ToInt32(value, 10);
					break;
				case "servername":
					ServerName=value;
					break;
				case "serverport":
					ServerPort = Convert.ToInt32(value);
					break;
				case "servermsgs":
					ServerMsgs=value;
					break;
				case "apiport":
					ApiPort = Convert.ToInt32(value);
					break;
				case "path":
					Path = value;
					break;
				case "scriptfolder":
					ScriptFolder = value;
					break;
				case "cardcdb":
					CardCDB = value;
					break;
				case "banlist":
					BanlistFile = value;
					break;
				case "privatechat":
					PrivateChat=Convert.ToBoolean(value);
					break;
				case "errorlog":
					ErrorLog = Convert.ToBoolean(value);
					break;
//				case "repeatlogin":
//					RepeatLogin = Convert.ToBoolean(value);
//					break;
				case "consolelog":
					ConsoleLog = Convert.ToBoolean(value);
					break;
				case "handshuffle":
					HandShuffle = Convert.ToBoolean(value);
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
					MaxRoomCount=Convert.ToInt32(value, 10);
					break;
				case "autoreplay":
					AutoReplay= (value.ToLower()=="true"||value=="1");
					break;
				case "replayfolder":
					replayFolder=value;
					break;
				case "windbname":
					WinDbName=value;
					break;
				case "recordwin":
					RecordWin=(value.ToLower()=="true"||value=="1");
					break;
				case "aiishide":
					AIisHide=(value.ToLower()=="true"||value=="1");
					break;
				case "loginurl":
					LoginUrl=value;
					break;
				case "serverdesc":
					ServerDesc=value;
					break;
				case "apiislocal":
					ApiIsLocal=(value.ToLower()=="true"||value=="1");
					break;
				case "asyncmode":
					AsyncMode= (value.ToLower()=="true"||value=="1");
					break;
//				case "timeout":
//					Timeout = Convert.ToInt32(value, 20);
//					break;
				default:
					return false;
			}
			return true;
		}

	}
}
