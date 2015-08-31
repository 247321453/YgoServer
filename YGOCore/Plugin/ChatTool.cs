/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/8/31
 * 时间: 22:36
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;

namespace YGOCore
{
	/// <summary>
	/// 聊天管理
	/// </summary>
	public class ChatTool
	{
		/// <summary>
		/// 是否合法
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool Check(string name,string str)
		{
			if(str==null){
				return false;
			}
			return str.Contains("妈") || str.Contains("fuck")||str.Contains("全家");
		}
	}
}
