/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/1
 * 时间: 9:42
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;

namespace YGOCore
{
	/// <summary>
	/// Description of Password.
	/// </summary>
	public static class Password
	{
		public static string OnlyName(string namepassword){
			if(string.IsNullOrEmpty(namepassword)){
				return namepassword;
			}
			return namepassword.Split('$')[0].Trim();
		}
		public static string GetPwd(string namepassword){
			if(string.IsNullOrEmpty(namepassword)){
				return namepassword;
			}
			int i = namepassword.IndexOf('$');
			if(i>0 && i+1 < namepassword.Length){
				return namepassword.Substring(i+1).Trim();
			}
			return "";
		}
	}
}
