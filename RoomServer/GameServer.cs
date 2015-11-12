/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/11/12
 * 时间: 11:24
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;

namespace YGOCore
{
	/// <summary>
	/// Description of GameServer.
	/// </summary>
	public class GameServer
	{
		public bool IsListening{get;private set;}
		public GameServer(int port=18910,string serverExe="GameServer.exe",string[] configs=null)
		{
		}
		public bool Start(){
			return false;
		}
		public void Stop(){
			
		}
	}
}
