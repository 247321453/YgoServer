/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/11/12
 * 时间: 16:27
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;

namespace YGOCore
{
	/// <summary>
	/// Description of Command.
	/// </summary>
	public static class Command
	{
		public static void OnCommand(this RoomServer server,string cmd,bool tip=true)
		{
			if(cmd==null)return;
			cmd = cmd.Trim();
			string[] args = cmd.Split(new char[]{' '}, 2);
			switch(args[0]){
				case "server":
					if(args.Length>1){
						int i = 0;
						int.TryParse(args[1], out i);
						//服务信息，玩家数，房间数

					}else{
						//数量
					}
					break;
				case "close":
					if(args.Length>1){
						int i = 0;
						int.TryParse(args[1], out i);
						//服务信息，玩家数，房间数
					}else{
						//数量
					}
					break;
				default:
					if(tip)
					Console.WriteLine(">>no invalid:"+cmd);
					break;
			}
		}
	}
}
