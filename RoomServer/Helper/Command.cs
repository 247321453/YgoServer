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
		public static void OnCommand(this RoomServer server,string cmd)
		{
			if(cmd==null)return;
			cmd = cmd.Trim();
			string[] args = cmd.Split(new char[]{' '}, 2);
			switch(args[0]){
				case "server":
					if(args.Length>1){
						string name = args[1];
						//服务信息，玩家数，房间数
						lock(server.Servers){
							foreach(Server srv in server.Servers){
								if(srv!=null&&srv.Name == name){
									Console.WriteLine(">>"+srv.ToString());
									return;
								}
							}
							Console.WriteLine(">>no find "+name);
						}
					}else{
						//数量
						lock(server.Servers){
							Console.WriteLine(">>count="+server.Servers.Count);
						}
					}
					break;
			}
		}
	}
}
