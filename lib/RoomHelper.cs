/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/11/2
 * 时间: 17:47
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Text;
using YGOCore;
using YGOCore.Game;
using System.IO;
using System.Net;
using OcgWrapper.Enums;
using System.Net.Sockets;

namespace YGOCore.Net
{
	public enum StoSMessage{
        /// <summary>
        /// 添加一个房间
        /// </summary>
        RoomCreate = 0x101,
        /// <summary>
        /// 关闭一个房间
        /// </summary>
        RoomClose  = 0x102,
        /// <summary>
        /// 添加一个玩家
        /// </summary>
        RoomAdd    = 0x103,
        /// <summary>
        /// 移除一个玩家
        /// </summary>
        RoomRemove = 0x104,
        /// <summary>
        /// 所有房间信息
        /// </summary>
        RoomList   = 0x105,
	}
	public static class RoomHelper
	{
		public static void OnRoomCreate(this TcpClient client, RoomInfo info){
			string str = Tool.ToJson(info);
			byte[] bs = Encoding.Unicode.GetBytes(str);
			using(MemoryStream stream = new MemoryStream(bs.Length+2)){
				using(BinaryWriter writer=new BinaryWriter(stream)){
					writer.Write((ushort)bs.Length);
					writer.Write((ushort)StoSMessage.RoomCreate);
					writer.Write(bs);
				}
				bs = stream.ToArray();
			}
			client.Send(bs);
		}
		private static void Send(this TcpClient client, byte[] bs){
			try{
				if(client.Client.Connected){
					client.Client.BeginSend(bs, 0, bs.Length, SocketFlags.None, SendDataEnd, client);
				}
			}catch(SocketException){
				
			}catch(Exception){
				
			}
		}
		private static void SendDataEnd(IAsyncResult ar)
		{
			try{
				TcpClient client = (TcpClient)ar.AsyncState;
				client.Client.EndSend(ar);
			}catch(SocketException){
			}catch(Exception){
				
			}
		}
	}
}
