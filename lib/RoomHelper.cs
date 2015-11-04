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
	#region 服务2服务
	public enum StoSMessage{
		/// <summary>
		/// 添加一个房间
		/// </summary>
		RoomCreate = 0x1,
		/// <summary>
		/// 关闭一个房间
		/// </summary>
		RoomClose  = 0x2,
		/// <summary>
		/// 更新房间信息
		/// </summary>
		RoomUpdate = 0x3,
		PlayerReady = 0x4,
		PlayerDeul = 0x5,
		PlayerSide = 0x6,
		PlayerLeave = 0x7,
		PlayerWatch = 0x8,
	}
	#endregion
	
	public static class RoomHelper
	{
		#region 房间信息
		public static RoomInfo GetRoomInfo(this GameRoom game){
			if(game!=null&&game.Config!=null){
				RoomInfo info=new RoomInfo();
				info.RoomName = game.Name;
				info.Pwd = Password.GetPwd(game.Config.Name);
				info.StartLP=game.Config.StartLp;
				info.Warring=game.Config.EnablePriority|game.Config.NoCheckDeck|game.Config.NoShuffleDeck;
				info.Rule=game.Config.Rule;
				info.Mode=game.Config.Mode;
				info.Lflist=game.Banlist.Name;
				info.IsStart= (game.State!=GameState.Lobby);
				int count = game.Players.Length;
				info.players = new string[count];
				for(int i=0;i<count;i++){
					GameSession player =  game.Players[i];
					if(player!=null){
						info.players[i] = player.Name;
					}
					if(info.players[i]==null){
						info.players[i] = "";
					}
				}
				
				lock(game.Observers){
					foreach(GameSession player in game.Observers){
						if(player!=null){
							info.observers.Add(player.Name);
						}
					}
				}
				return info;
			}
			return null;
		}
		#endregion
		
		public static void OnRoomEvent(this GameServer server, StoSMessage msg, RoomInfo info){
			using(PacketWriter writer = new PacketWriter(2)){
				writer.Write((ushort)msg);
				string str = Tool.ToJson(info);
				byte[] bs = Encoding.Unicode.GetBytes(str);
				writer.Write(bs);
				writer.Use();
				Send(server.LocalClient, writer.Content);
			}
		}
		public static void OnPlayEvent(this GameServer server, StoSMessage msg, string name){
			using(PacketWriter writer = new PacketWriter(2)){
				writer.Write((ushort)msg);
				writer.WriteUnicode(name, 20);
				writer.Use();
				Send(server.LocalClient, writer.Content);
			}
		}
		public static void OnPlayEvent(this GameServer server, StoSMessage msg, GameSession player){
			OnPlayEvent(server, msg, player == null?"":player.Name);
		}
		
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
	}
}
