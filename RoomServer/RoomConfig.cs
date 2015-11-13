/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/11/13
 * 时间: 17:45
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Xml;

namespace YGOCore
{
	/// <summary>
	/// Description of RoomConfig.
	/// </summary>
	public class RoomConfig
	{
		public string Name{get;private set;}
		public string Desc{get;private set;}
		public int Port;
		public string[] Configs;
		public string ServerExe;
		public int Timeout =15;
		
		public RoomConfig(){
			Name="ygoServer";
			Desc="";
			Port = 18910;
			ServerExe = "GameServer.exe";
			Configs = new string[]{"config.txt"};
		}
		public void Load(){
			ConfigManager.XmlFile = "roomserver.xml";
			Name = ConfigManager.readString("name");
			Desc= ConfigManager.readString("desc");
			Port = ConfigManager.readInteger("port", 18910);
			ServerExe = ConfigManager.readString("gameserver");
			if(string.IsNullOrEmpty(ServerExe)){
				ServerExe = "GameServer.exe";
			}
			Timeout = ConfigManager.readInteger("timeout", 15);
			string cfgstr = ConfigManager.readString("configs");
			if(cfgstr==null){
				Configs = new string[]{"config.txt"};
			}else{
				Configs =cfgstr.Split(',');					 
			}
		}
	}
}
