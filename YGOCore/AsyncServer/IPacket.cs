/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/10/30
 * 时间: 10:48
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;

namespace System.Net
{
	/// <summary>
	/// Description of IPacket.
	/// </summary>
	public interface IPacket
	{
		int Length{get;set;}
		byte[] GetBytes();
		bool Use();
	}
}
