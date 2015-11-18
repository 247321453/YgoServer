using System;
using System.Text;

namespace System.IO
{
	public static class BinaryExtensions
	{
		public static string ToHexString(this byte[] bytes)
		{
			string hexString = string.Empty;
			if ( bytes != null )
			{
				System.Text.StringBuilder strB = new System.Text.StringBuilder ();
				for ( int i = 0; i < bytes.Length; i++ )
				{
					strB.Append ( "0x"+bytes[i].ToString ( "X2" ) +" ");
				}
				hexString = strB.ToString ();
			}
			return hexString;
		}
		public static void WriteUnicode(this BinaryWriter writer, string text, int len=0)
		{
			if(text==null)text="";
			byte[] unicode = Encoding.Unicode.GetBytes(text);
			if(len <= 0 ){
				len = text.Length + 1;
			}
			byte[] result = new byte[len * 2];
			int max = len * 2 - 2;
			Array.Copy(unicode, result, unicode.Length > max ? max : unicode.Length);
			writer.Write(result);
		}
		public static void Write(this BinaryWriter writer,bool value)
		{
			writer.Write((byte)(value ? 1 : 0));
		}
		public static bool ReadBoolean(this BinaryReader reader)
		{
			return reader.ReadByte() == 1;
		}
		public static byte[] ReadToEnd(this BinaryReader reader)
		{
			int size = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
			return reader.ReadBytes(size);
		}
		public static string ReadUnicode(this BinaryReader reader, int len=0)
		{
			byte[] unicode = (len <= 0) ? reader.ReadToEnd():reader.ReadBytes(len * 2);
			string text = Encoding.Unicode.GetString(unicode);
			int i = text.IndexOf('\0');
			if(i > 0){
				text = text.Substring(0, i);
			}
			return text;
		}
	}
}