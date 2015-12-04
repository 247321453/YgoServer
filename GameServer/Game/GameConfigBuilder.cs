/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/8
 * 时间: 10:04
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using YGOCore.Net;
using AsyncServer;
using System.IO;

namespace YGOCore.Game
{
    public static class GameConfigBuilder
    {
        public static GameConfig Build(string gameinfo)
        {
            //默认值
            GameConfig config = new GameConfig();
            config.IsRandom = true;
            config.LfList = 0;
            config.BanList = BanlistManager.GetName(0);
            config.Rule = 2;
            config.Mode = 0;
            config.EnablePriority = false;
            config.NoCheckDeck = false;
            config.NoShuffleDeck = false;
            config.StartLp = 8000;
            config.StartHand = 5;
            config.DrawCount = 1;
            config.GameTimer = 120;
            if (!string.IsNullOrEmpty(gameinfo))
            {
                gameinfo = gameinfo.Trim();
            }
            gameinfo = gameinfo.Replace("*", "");
            gameinfo = gameinfo.Replace(":", "");
            config.RoomString = gameinfo;
            if (string.IsNullOrEmpty(gameinfo) || gameinfo == "random" || gameinfo == "#")
            {
                //random
                config.Name = RoomManager.RandomRoomName();
                Logger.Debug("random:" + config.Name);
                return config;
            }
            config.Parse(gameinfo);
            config.BanList = BanlistManager.GetName(config.LfList);
            if (string.IsNullOrEmpty(config.Name))
            {
                if (gameinfo.EndsWith("#"))
                {
                    string _name = RoomManager.RandomRoomName(gameinfo);
                    if (_name == null)
                    {
                        //条件#的随机房间名没找到，则创建一个
                        config.Name = gameinfo + RoomManager.NewRandomRoomName();
                    }
                    else
                    {
                        //条件#的随机房间名存在，则进去，可能重复观战
                        config.Name = _name;
                        Logger.Debug("1," + config.Name);
                    }
                }
                else
                {
                    config.IsRandom = false;
                    config.Name = gameinfo;
                }
            }
            Logger.Debug("2,"+config.Name);
            return config;
        }

        public static GameConfig Build(GameClientPacket packet)
        {
            GameConfig config = new GameConfig();
            config.LfList = BanlistManager.GetIndex(packet.ReadUInt32());
            config.BanList = BanlistManager.GetName(config.LfList);
            config.Rule = packet.ReadByte();
            config.Mode = packet.ReadByte();
            config.EnablePriority = Convert.ToBoolean(packet.ReadByte());
            config.NoCheckDeck = Convert.ToBoolean(packet.ReadByte());
            config.NoShuffleDeck = Convert.ToBoolean(packet.ReadByte());
            //C++ padding: 5 bytes + 3 bytes = 8 bytes
            for (int i = 0; i < 3; i++)
                packet.ReadByte();
            config.StartLp = packet.ReadInt32();
            config.StartHand = packet.ReadByte();
            config.DrawCount = packet.ReadByte();
            config.GameTimer = packet.ReadInt16();
            packet.ReadUnicode(20);
            config.Name = packet.ReadUnicode(30);
            
            if (string.IsNullOrEmpty(config.Name))
                config.Name = RoomManager.NewRandomRoomName();
            config.RoomString = config.Name;
            return config;
        }


    }
}
