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

        [DllImport("Kernel32")]
        public extern static int FormatMessage(int flag, ref IntPtr source, int msgid, int langid, ref string buf, int size, ref IntPtr args);
        /// <summary>
        /// 获取系统错误信息描述
        /// </summary>
        /// <param name="errCode">系统错误码</param>
        /// <returns></returns>
        public static string GetSysErrMsg(int errCode)
        {
            IntPtr tempptr = IntPtr.Zero;
            string msg = null;
            FormatMessage(0x1300, ref tempptr, errCode, 0, ref msg, 255, ref tempptr);
            return msg;
        }
        public static int GetLastError()
        {
            return Marshal.GetLastWin32Error();
        }
        public static string GetLastErrorMessage()
        {
            return GetSysErrMsg(GetLastError());
        }
    }

    public class User32
    {
        [DllImport("user32", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32", EntryPoint = "FindWindowEx", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32", EntryPoint = "ShowWindow", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

        public static IntPtr FindConsoleWindow(string title)
        {
            return FindWindow("ConsoleWindowClass", title);
        }


        public static bool ShowWindow(IntPtr hWnd)
        {
            return ShowWindow(hWnd, 1);
        }
        public static bool HideWindow(IntPtr hWnd)
        {
            return ShowWindow(hWnd, 0);
        }
    }
}
