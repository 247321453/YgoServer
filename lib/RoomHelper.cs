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
using AsyncServer;
using System.Net.Sockets;

namespace YGOCore.Net
{
	public static class RoomHelper
	{
		
		#region room event
		public static void OnRoomEvent(this GameServer server, StoSMessage msg, RoomInfo info){
			using(PacketWriter writer = new PacketWriter(2)){
				writer.Write((ushort)msg);
				//length
				writer.Write((ushort)1);
				writer.WriteUnicode(info.Name, 20);
				writer.WriteUnicode(info.Pwd, 20);
				writer.Write((byte)info.Rule);
				writer.Write((byte)info.Mode);
				writer.Write(info.IsStart);
				writer.WriteUnicode(info.Lflist, 60);
				writer.Write(info.Warring);
				writer.Write(info.StartLP);
				writer.Write((ushort)info.players.Length);
				for(int i =0; i<info.players.Length;i++){
					writer.WriteUnicode(info.players[i], 20);
				}
				writer.Write((ushort)info.observers.Count);
				for(int i =0; i<info.observers.Count;i++){
					writer.WriteUnicode(info.observers[i], 20);
				}
				//string str = Tool.ToJson(info);
				//byte[] bs = Encoding.Unicode.GetBytes(str);
				//writer.Write(bs);
				writer.Use();
				//Send(server.LocalClient, writer.Content);
			}
		}
		#endregion
		
		/*
		#region player
		public static void OnPlayNameEvent(this GameServer server, PlayerStatu msg,params string[] names){
			if(names==null||names.Length==0){
				return;
			}
			using(PacketWriter writer = new PacketWriter(2)){
				writer.Write((ushort)msg);
				writer.Write((ushort)names.Length);
				for(int i=0;i<names.Length;i++){
					writer.WriteUnicode(names[i], 20);
				}
				writer.Use();
				Send(server.LocalClient, writer.Content);
			}
		}
		public static void OnPlayEvent(this GameServer server, PlayerStatu msg,params GameSession[] players){
			if(players==null||players.Length==0){
				return;
			}
			string[] names = new string[players.Length];
			for(int i=0;i<names.Length;i++){
				names[i] = players[i] == null? "":players[i].Name;
			}
			OnPlayNameEvent(server, msg, names);
		}
		#endregion
		
		#region send
		private static void Send(TcpClient client, byte[] bs){
			if(client==null){
				return;
			}
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
		#endregion
		*/
	}
}
