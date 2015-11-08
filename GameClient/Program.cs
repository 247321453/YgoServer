/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/7
 * 时间: 20:31
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using AsyncServer;

namespace GameClient
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal sealed class Program
	{
		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Logger.SetLogLevel(LogLevel.Info);
			#if DEBUG
			Logger.SetLogLevel(LogLevel.Debug);
			#endif
			Application.Run(new LoginForm());
		}
		
		private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			File.WriteAllText("crash_" + DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt", e.ExceptionObject.ToString());
			Process.GetCurrentProcess().Kill();
		}
		
	}
}
