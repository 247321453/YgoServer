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
using System.IO;
using System.Diagnostics;

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
			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
			Console.CancelKeyPress+= new ConsoleCancelEventHandler(Console_CancelKeyPress);
			Logger.SetLogLevel(LogLevel.Info);
			#if DEBUG
			Logger.SetLogLevel(LogLevel.Debug);
			#endif
			RoomServer Server;
			if(args.Length >= 2){
				Server = new RoomServer(int.Parse(args[0]), int.Parse(args[1]));
			}else{
				Console.WriteLine("使用默认端口");
				Server = new RoomServer();
			}
			Server.Start();
			Command(Server);
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
		private static void Command(RoomServer server){
			string cmd="";
			while(server.isStarted){
				cmd = Console.ReadLine();
				//server.OnCommand(cmd);
			}
		}
		private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			e.Cancel=true;
			ConsoleColor color=Console.ForegroundColor;
			Console.ForegroundColor=ConsoleColor.Cyan;
			Console.WriteLine("Please input to close server.");
			Console.ForegroundColor=color;
		}

		private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			File.WriteAllText("crash_room_" + DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt", e.ExceptionObject.ToString());
			Process.GetCurrentProcess().Kill();
		}
	}
}