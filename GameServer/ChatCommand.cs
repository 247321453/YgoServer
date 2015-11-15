/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/1
 * 时间: 9:00
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using AsyncServer;
using YGOCore.Game;
using YGOCore.Net;

namespace YGOCore
{
    /// <summary>
    /// Description of ChatCommand.
    /// </summary>
    public static class ChatCommand
    {
        #region start info
        public static void WriteHead(this GameServer server)
        {
            if (server == null || server.Config == null)
            {
                return;
            }
            ServerConfig config = server.Config;
            string Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Logger.Info("┌───────────────────────────────────", true);
            Logger.Info("│ __     _______  ____   _____", true);
            Logger.Info("│ \\ \\   / / ____|/ __ \\ / ____|", true);
            Logger.Info("│  \\ \\_/ / |  __| |  | | |     ___  _ __ ___", true);
            Logger.Info("│   \\   /| | |_ | |  | | |    / _ \\| '__/ _ \\", true);
            Logger.Info("│    | | | |__| | |__| | |___| (_) | | |  __/", true);
            Logger.Info("│    |_|  \\_____|\\____/ \\_____\\___/|_|  \\___|    Build: " + Version, true);
            Logger.Info("│", true);
            Logger.Info("│Client version 0x" + config.ClientVersion.ToString("x")
                        + " or new, MaxRooms = " + config.MaxRoomCount, true);
            Logger.Info("│NeedAtuh=" + config.isNeedAuth + ", AsyncMode=" + config.AsyncMode
                        + ", BanMode=" + config.BanMode, true);
            Logger.Info("└───────────────────────────────────", true);
        }
        static readonly List<Process> AIs = new List<Process>();

        #endregion

        #region ai
        /// <summary>
        /// 拥有一定数量
        /// </summary>
        /// <param name="name"></param>
        /// <param name="room"></param>
        /// <param name="deck"></param>
        /// <returns></returns>
        /*private static bool AddAI(ServerConfig Config, string room)
        {
            lock (AIs)
            {
                if (AIs.Count >= Config.MaxAICount)
                {
                    return false;
                }
            }
            Process ai = new Process();
            ai.StartInfo.FileName = "ai";
            //设定程式执行参数
            ai.StartInfo.Arguments =
                " [AI]Robot$" + Config.AIPass
                + " 127.0.0.1 "
                + Config.ServerPort
                + " " + Config.ClientVersion
                + " " + room;
            ai.EnableRaisingEvents = true;
#if !DEBUG
            if (Config.AIisHide)
            {
                ai.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }
#endif
            ai.Exited += new EventHandler(ai_Exited);
            try
            {
                ai.Start();
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return false;
            }
            lock (AIs)
            {
                AIs.Add(ai);
            }
            return true;
        }*/

        static void ai_Exited(object sender, EventArgs e)
        {
            if (sender is Process)
            {
                Process p = sender as Process;
                p.Close();
                AIs.Remove(p);
                Logger.Debug("close ai");
            }
            else
            {
                Logger.Debug("close ai:" + sender.GetType().ToString());
            }
            Logger.Debug("AI exit game. count=" + AIs.Count);
        }
        #endregion

        #region client msg
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="msg"></param>
        /// <returns>处理返回true，未处理返回false</returns>
        public static bool OnChatCommand(this GameSession client, string msg)
        {/*
            if (msg == null)
            {
                return true;
            }
            if (client == null)
            {
                return false;
            }
            msg = msg.Trim();
            if (msg == "/ai")
            {
                if (Program.Config.MaxAICount == 0)
                {
                    client.ServerMessage(Messages.MSG_NO_AI);
                }
                else if (client.Game != null && AddAI(Program.Config, client.Game.Config.Name))
                {
                    client.ServerMessage(Messages.MSG_ADD_AI);
                }
                else
                {
                    client.ServerMessage(Messages.MSG_NO_FREE_AI);
                }
                return false;
            }*/
            return false;
        }
        #endregion

        #region server
        public static void OnCommand(this GameServer Server, string cmd)
        {
            if (cmd == null)
            {
                return;
            }
            cmd = cmd.ToLower();
            string[] args = cmd.Split(' ');
            cmd = args[0];
            if (args.Length < 2)
            {
                args = new string[] { args[0], "" };
            }
            switch (cmd)
            {
                case "sendall":
                    //发送给所有玩家
                    int count = RoomManager.OnWorldMessage(args[1]);
                    Console.WriteLine(">>room=" + count);
                    break;
                case "room":
                    Console.WriteLine(">>count=" + RoomManager.Count);
                    break;
                case "cls":
                    Console.Clear();
                    Server.WriteHead();
                    break;
                case "close":
                    if (Server != null)
                    {
                        Server.Stop();
                        Console.WriteLine(">>close ok");
                        Console.WriteLine("Press any key...");
                        Console.ReadKey(true);
                    }
                    break;
                case "ai":
                    AICmd(Server, args);
                    break;
                default:
                    Console.WriteLine(">>invalid command:" + cmd);
                    break;
            }
        }

        private static void AICmd(GameServer Server, string[] args)
        {
            switch (args[1])
            {/*
                case "add":
                    AddAI(Server);
                    break;*/
                case "kill":
                    KillAIs();
                    break;
                default:
                    lock (AIs)
                        Console.WriteLine(">>AI count:" + AIs.Count);
                    break;
            }
        }
        private static void KillAIs()
        {
            lock (AIs)
            {
                foreach (Process p in AIs)
                {
                    if (p != null)
                    {
                        if (!p.HasExited)
                        {
                            p.Kill();
                            p.Close();
                        }
                    }
                }
                AIs.Clear();
            }
        }/*
        private static void AddAI(GameServer Server)
        {
            string name = null;
            GameRoom room = RoomManager.CreateOrGetGame(GameConfigBuilder.Build(""));
            if (room == null && room.Config != null)
            {
                name = RoomManager.RandomRoomName();
            }
            else
            {
                name = room.Config.Name;
            }
            if (AddAI(Server.Config, "" + name))
            {
                Console.WriteLine(">>add ai to " + name);
            }
            else
            {
                Console.WriteLine(">>add ai fail");
            }
        }*/
        #endregion
    }
}