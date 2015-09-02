/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/9/1
 * 时间: 16:01
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Diagnostics;
using System.Security.Principal;

namespace YGOClient
{
	/// <summary>
	/// Description of Protocol.
	/// </summary>
	public class Protocol
	{
		public static bool IsAdministrator()
		{
			WindowsIdentity identity = WindowsIdentity.GetCurrent();
			WindowsPrincipal principal = new WindowsPrincipal(identity);
			return principal.IsInRole(WindowsBuiltInRole.Administrator);
		}
		/// <summary>
		/// 注册启动项到注册表
		/// </summary>
		public static void Reg(string pro)
		{
			//注册的协议头，即在地址栏中的路径 如QQ的：tencent://xxxxx/xxx 我注册的是jun 在地址栏中输入：jun:// 就能打开本程序
			var surekamKey = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(pro);
			//以下这些参数都是固定的，不需要更改，直接复制过去
			var shellKey = surekamKey.CreateSubKey("shell");
			var openKey = shellKey.CreateSubKey("open");
			var commandKey = openKey.CreateSubKey("command");
			surekamKey.SetValue("URL Protocol", "");
			//这里可执行文件取当前程序全路径，可根据需要修改
			var exePath = System.Windows.Forms.Application.ExecutablePath;
			commandKey.SetValue("", "\"" + exePath + "\"" + " \"%1\"");
		}
		/// <summary>
		/// 取消注册
		/// </summary>
		public static void UnReg(string pro)
		{
			//直接删除节点
			Microsoft.Win32.Registry.ClassesRoot.DeleteSubKeyTree(pro);
		}
	}
}
