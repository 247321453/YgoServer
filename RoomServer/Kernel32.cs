/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/15
 * 时间: 22:05
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System.Runtime.InteropServices;

namespace System
{
	public delegate bool ControlCtrlDelegate(int CtrlType);
	/// <summary>
	/// Description of K.
	/// </summary>
	public class Kernel32
	{
		[DllImport("kernel32")]
		public static extern bool SetConsoleCtrlHandler(ControlCtrlDelegate HandlerRoutine, bool Add);
	}
}
