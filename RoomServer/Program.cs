/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/4
 * 时间: 20:06
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using AsyncServer;

namespace YGOCore
{
	class Program
	{
		public static void Main(string[] args)
		{
			RoomServer Server;
			if(args.Length==0){
				Server = new RoomServer();
			}else{
				Server = new RoomServer();
			}
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
	}
}