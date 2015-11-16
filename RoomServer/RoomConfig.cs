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
		public int Port;
		public int[] Ports;
		public string ServerExe;
		public string Config;
		public int Timeout =15;
		
		public RoomConfig(){
			Port = 18910;
			ServerExe = "GameServer.exe";
			Ports = new int[]{8910};
			Config = "config.txt";
			ConfigManager.XmlFile = "roomserver.xml";
		}
		public void Load(){
			Port = ConfigManager.readInteger("port", 18910);
			ServerExe = ConfigManager.readString("gameserver");
			if(string.IsNullOrEmpty(ServerExe)){
				ServerExe = "GameServer.exe";
			}
			Timeout = ConfigManager.readInteger("timeout", 15);
			Ports = ConfigManager.readIntegers("ports",0);
			Config =  ConfigManager.readString("config");
			if(string.IsNullOrEmpty(Config)){
				Config = "config.txt";
			}
		}
	}
}
