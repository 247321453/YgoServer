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
                case "say":
                    if(args.Length > 1)
                    {
                        server.Tip = args[1];
                        server.Message(args[1]);
                        Console.WriteLine(">>say ok");
                    }
                    else
                    {
                        Console.WriteLine(">>say fail");
                    }
                    break;
				case "server":
					if(args.Length>1){
						int i = 0;
						int.TryParse(args[1], out i);
                        //服务信息，玩家数，房间数
                        server.PrintServer(i);
                    }
                    else{
                        //数量
                        server.PrintServer();
                    }
					break;
				case "close":
					if(args.Length>1){
						int i = 0;
						int.TryParse(args[1], out i);
						//服务信息，玩家数，房间数
						lock(server.Porcess){
							if(i<server.Porcess.Count){
								ServerProcess p = server.Porcess[i];
								Console.WriteLine(">>close "+(i+1)+":"+p.Port);
								p.Close();
								server.Porcess.Remove(p);
							}
						}
					}else{
						//数量
						server.Stop();
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
