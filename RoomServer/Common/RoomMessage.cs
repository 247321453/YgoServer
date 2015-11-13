/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/11/12
 * 时间: 17:36
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;

namespace YGOCore
{
	/// <summary>
	/// Description of RoomMessage.
	/// </summary>
	public enum RoomMessage : byte
	{
		Error,
		/// <summary>
		/// 请求房间列表
		/// </summary>
		RoomList,
		/// <summary>
		/// 登录/服务器信息
		/// </summary>
		Info,
		/// <summary>
		/// 聊天
		/// </summary>
		Chat,
		/// <summary>
		/// 暂停推送
		/// </summary>
		Pause,
		SystemChat,
		RoomCreate,
		RoomStart,
		RoomClose,
	}
}
