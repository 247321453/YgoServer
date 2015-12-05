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
                case "hide":
                    if (args.Length > 1)
                    {
                        int i = 0;
                        int.TryParse(args[1], out i);
                        //服务信息，玩家数，房间数
                        lock (server.Porcess)
                        {
                            if (i < server.Porcess.Count)
                            {
                                ServerProcess p = server.Porcess[i];
                                Console.WriteLine(">>hide " + i + " => " + p.Hide());
                            }
                        }
                    }
                    else {
                        //数量
                        Console.WriteLine(">>hide %1");
                    }
                    break;
                case "show":
                    if (args.Length > 1)
                    {
                        int i = 0;
                        int.TryParse(args[1], out i);
                        //服务信息，玩家数，房间数
                        lock (server.Porcess)
                        {
                            if (i < server.Porcess.Count)
                            {
                                ServerProcess p = server.Porcess[i];
                                Console.WriteLine(">>show " + i + " => " + p.Show());
                            }
                        }
                    }
                    else {
                        //数量
                        Console.WriteLine(">>show %1");
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
								Console.WriteLine(">>close "+i+":"+p.Port);
								p.Close();
								server.Porcess.Remove(p);
							}
						}
					}else{
						//数量
						server.Stop();
					}
					break;
                case "help":
                    Console.WriteLine(">>server %1  查看所有服务器信息，参数：0-max，显示某个服务器信息，不填则是全部");
                    Console.WriteLine(">>say %1     服务器公告，每3分钟发送一次，参数：消息内容");
                    Console.WriteLine(">>hide %1    查看所有服务器信息，参数：0-max，隐藏某个服务器");
                    Console.WriteLine(">>show %1    查看所有服务器信息，参数：0-max，显示某个服务器");
                    Console.WriteLine(">>close %1   查看所有服务器信息，参数：0-max，关闭某个服务器");
                    break;
				default:
					if(tip)
						Console.WriteLine(">>no invalid:"+cmd);
					break;
			}
		}
	}
}
