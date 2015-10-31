/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/10/31
 * 时间: 16:19
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using YGOCore.Game;
using System.Collections.Generic;

namespace YGOCore.Core
{
	/// <summary>
	/// 处理游戏包事件
	/// </summary>
	public class GameHandler
	{
		GameSession client;
		public GameHandler(GameSession client)
		{
			this.client=client;
		}
		
		public void Handler(List<GameClientPacket> packets){
			//线程处理？
		//	CtosMessage msg = packets[0].ReadCtos();
		}
		
		/// <summary>
		/// 禁用
		/// </summary>
		public void Disable(){
			
		}
	}
}
