/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/11/5
 * 时间: 15:06
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;

namespace YGOCore.Game
{
	public enum StoSMessage:byte{
		/// <summary>
		/// 添加一个房间
		/// </summary>
		RoomCreate = 0x1,
		/// <summary>
		/// 关闭一个房间
		/// </summary>
		RoomClose  = 0x2,
		/// <summary>
		/// 更新房间信息
		/// </summary>
		RoomStart = 0x3,
		PlayerJoin= 0x4,
		PlayerLeave = 0x5,
	}
}
