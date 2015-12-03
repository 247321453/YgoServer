using System;
using System.Runtime.InteropServices;

namespace YGOCore.Game
{
    public static class YgopotEx
    {
        public static HostInfo ToHostInfo(this GameConfig config)
        {
            HostInfo info = new HostInfo();
            if (config != null)
            {
                info.LfList = (uint)config.LfList;
                info.Rule = (byte)config.Rule;
                info.Mode = (byte)config.Mode;
                info.EnablePriority = (byte)(config.EnablePriority ? 1 : 0);
                info.NoCheckDeck = (byte)(config.NoCheckDeck ? 1 : 0);
                info.NoShuffleDeck = (byte)(config.NoShuffleDeck ? 1 : 0);
                info.StartLp = (uint)config.StartLp;
                info.StartHand = (byte)config.StartHand;
                info.DrawCount = (byte)config.DrawCount;
                info.GameTimer = (ushort)config.GameTimer;
            }
            return info;
        }
        public static void Parse(this GameConfig config, HostInfo info)
        {
            config.LfList = BanlistManager.GetIndex(info.LfList);
            config.BanList = BanlistManager.GetName(config.LfList);
            config.Rule = info.Rule;
            config.Mode = info.Mode;
            config.EnablePriority = info.EnablePriority==1;
            config.NoCheckDeck = info.NoCheckDeck==1;
            config.NoShuffleDeck = info.NoShuffleDeck == 1;
            config.StartLp = (int)info.StartLp;
            config.StartHand = info.StartHand;
            config.DrawCount = info.DrawCount;
            config.GameTimer = info.GameTimer;
        }
        public static GameConfig Build(this CtosCreateGame roomconfig)
        {
            GameConfig config = new GameConfig();
            config.Parse(roomconfig.info);
            config.Name = new string(roomconfig.name).Split('\0')[0];
            config.RoomString = config.Name;
            try
            {
                char[] chs = new char[roomconfig.pass.Length];
                for(int i = 0; i < roomconfig.pass.Length; i++)
                {
                    chs[i] = (char)roomconfig.pass[i];
                }
                string pwd = new string(chs).Split('\0')[0];
                if (!string.IsNullOrEmpty(pwd))
                {
                    config.RoomString = config.Name + '$' + pwd;
                }
            }
            catch
            {

            }
            return config;
        }
    }
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    // [Serializable()]
    [StructLayout(LayoutKind.Explicit)]
    public struct HostInfo
    {
        [FieldOffset(0)]
        public uint LfList;
        [FieldOffset(4)]
        public byte Rule;
        [FieldOffset(5)]
        public byte Mode;
        [FieldOffset(6)]
        public byte EnablePriority;
        [FieldOffset(7)]
        public byte NoCheckDeck;
        [FieldOffset(8)]
        public byte NoShuffleDeck;
        [FieldOffset(12)]
        public uint StartLp;
        [FieldOffset(16)]
        public byte StartHand;
        [FieldOffset(17)]
        public byte DrawCount;
        [FieldOffset(18)]
        public ushort GameTimer;
    }

    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    public struct CtosCreateGame
    {
        [FieldOffset(0)]
        public HostInfo info;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        [FieldOffset(20)]
        public char[] name;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        [FieldOffset(60)]
        public ushort[] pass;
    }

    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    public struct StocHostPacket
    {
        [FieldOffset(0)]
        public ushort identifier;
        [FieldOffset(2)]
        public ushort version;
        [FieldOffset(4)]
        public ushort port;
        [FieldOffset(8)]
        public uint ipaddr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        [FieldOffset(12)]
        public char[] name;
        [FieldOffset(52)]
        public HostInfo host;
    };
}
