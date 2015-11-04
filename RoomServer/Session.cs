/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/4
 * 时间: 20:12
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using AsyncServer;

namespace YGOCore
{
	/// <summary>
	/// Description of Session.
	/// </summary>
	public class Session
	{
		Connection<Session> Client;
		public Session(Connection<Session> client)
		{
			Client = client;
		}
	}
}
