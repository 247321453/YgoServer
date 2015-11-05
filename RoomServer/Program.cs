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
	/// <summary>
	/// 1期
	/// 多个服务端的房间列表
	/// 玩家的状态（决斗，等待，还side，围观）
	/// 大厅聊天
	/// 房间密码验证
	/// 2期
	/// 指定玩家私聊，观战，邀请决斗
	/// 3期
	/// 游戏更新
	/// </summary>
	class Program
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args">本地端口，对外端口</param>
		public static void Main(string[] args)
		{
			RoomServer Server;
			if(args.Length >= 2){
				Server = new RoomServer(int.Parse(args[0]), int.Parse(args[1]));
				Server.Start();
			}else{
				Console.Write("exe 本地空间名 对外端口");
			}
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
	}
}