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