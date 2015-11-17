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
		public int Port{get; private set;}
		public int ApiPort{get; private set;}
		public int[] Ports{get; private set;}
		public string ServerExe{get; private set;}
		public string Config{get; private set;}
		public int Timeout{get; private set;}
		
		public RoomConfig(){
			Port = 18910;
			Timeout = 15;
			ServerExe = "GameServer.exe";
			Ports = new int[]{8910};
			Config = "config.txt";
			ConfigManager.XmlFile = "roomserver.xml";
			ApiPort = 10001;
		}
		public void Load(){
			Port = ConfigManager.readInteger("port", Port);
			ApiPort = ConfigManager.readInteger("apiport", ApiPort);
			string tmp = ConfigManager.readString("gameserver");
			if(!string.IsNullOrEmpty(tmp)){
				ServerExe = tmp;
			}
			Timeout = ConfigManager.readInteger("timeout", Timeout);
			Ports = ConfigManager.readIntegers("ports",0);
			tmp =  ConfigManager.readString("config");
			if(!string.IsNullOrEmpty(tmp)){
				Config = tmp;
			}
		}
	}
}
