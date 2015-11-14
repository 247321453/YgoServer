/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/14
 * 时间: 11:44
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Xml;
using System.IO;

namespace GameClient
{
	/// <summary>
	/// Description of ClientConfig.
	/// </summary>
	public class ClientConfig
	{
		public string Host{get; set;}
		public int Port{get; set;}
		public int ChatPort{get; set;}
		public int DuelPort{get; set;}
		public string GamePath{get;set;}
		public string GameExe{get;set;}
		public ClientConfig(){
			ConfigManager.XmlFile = "ygoclient.xml";
		}
		public void Load(){
			Host = ConfigManager.readString("server");
			Port = ConfigManager.readInteger("port", 18910);
			GameExe = ConfigManager.readString("game");
			GamePath = Path.GetDirectoryName(GameExe);
		}
	}
}
