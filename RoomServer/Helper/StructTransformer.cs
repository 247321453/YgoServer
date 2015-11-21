using System;
using System.IO;
using System.Text;
//pot
using System.Runtime.InteropServices;

namespace YGOCore
{
    public class StructTransformer
    {
        //struct转换为byte[]
        public static byte[] StructToBytes(object structObj)
        {
            int size = Marshal.SizeOf(structObj);
            IntPtr buffer = Marshal.AllocHGlobal(size * 10);
            try
            {
                Marshal.StructureToPtr(structObj, buffer, false);
                byte[] bytes = new byte[size];
                Marshal.Copy(buffer, bytes, 0, size);
                return bytes;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        //byte[]转换为struct
        public static object BytesToStruct(byte[] bytes, Type strcutType)
        {
            int size = Marshal.SizeOf(strcutType);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(bytes, 0, buffer, size);
                return Marshal.PtrToStructure(buffer, strcutType);

            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        public static int SizeOf(Type strcutType)
        {
            return Marshal.SizeOf(strcutType);
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