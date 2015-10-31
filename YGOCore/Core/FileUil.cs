/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/10/31
 * 时间: 16:01
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.IO;

namespace YGOCore.Core
{
	/// <summary>
	/// Description of FileUil.
	/// </summary>
	public static class FileUil
	{

		public static bool CreateDirectory(string dir){
			bool ok = false;
			if(!Directory.Exists(dir)){
				try{
					Directory.CreateDirectory(dir);
				}catch{ ok = false;}
			}
			return ok;
		}
	}
}
