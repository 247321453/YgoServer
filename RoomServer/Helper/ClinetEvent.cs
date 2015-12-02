/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/11/13
 * 时间: 11:20
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using AsyncServer;
using System.Collections.Generic;
using System.IO;

namespace YGOCore
{
	/// <summary>
	/// Description of ClinetEvent.
	/// </summary>
	public static class ClinetEvent
	{
		#region 消息匹配
		static readonly EventHandler<ushort, Session, PacketReader> EventHandler = new EventHandler<ushort, Session, PacketReader>();
		
		static ClinetEvent(){
			RegisterEvents();
		}
		static void RegisterEvents(){
			//EventHandler.Register((ushort)RoomMessage.Error, 	OnError);
			EventHandler.Register((ushort)RoomMessage.RoomList,	OnRoomList);
			EventHandler.Register((ushort)RoomMessage.Info,	OnInfo);
			EventHandler.Register((ushort)RoomMessage.Chat,	OnChat);
			EventHandler.Register((ushort)RoomMessage.Pause,OnPause);
			EventHandler.Register((ushort)RoomMessage.PlayerList, OnPlayerList);
            EventHandler.Register((ushort)RoomMessage.NETWORK_CLIENT_ID, OnRoomList2);
            EventHandler.Register((ushort)RoomMessage.STOP_CLIENT, OnClose);
            //EventHandler.Register((ushort)RoomMessage.SystemChat,	OnSystemChat);
            //EventHandler.Register((ushort)RoomMessage.RoomCreate,	OnRoomCreate);
            //EventHandler.Register((ushort)RoomMessage.RoomStart,	OnRoomStart);
            //EventHandler.Register((ushort)RoomMessage.RoomClose,	OnRoomClose);
        }
		public static void Handler(Session session, List<PacketReader> packets){
			foreach(PacketReader packet in packets){
				//			Parse(player, packet);
				ushort id = packet.ReadByte();
				//if(RoomMessage.IsDefined(typeof(RoomMessage), id)){
				EventHandler.Do(id, session, packet);
				packet.Close();
				//}
			}
		}
		#endregion
		
		#region 处理客户端消息
		private static bool Login(string name, string pwd){
			return true;
		}
		private static void OnPause(Session session, PacketReader packet){
			session.IsPause = true;
		}

        private static void OnClose(Session session, PacketReader packet)
        {
            session.IsPause = true;
            session.Close();
        }
        private static void OnRoomList2(Session session, PacketReader packet)
        {
            session.IsPause = false;
            if (session.Server != null)
            {
                session.Server.SendRoomList(session);
            }
        }
        private static void OnRoomList(Session session, PacketReader packet){
			bool nolock = packet.ReadBoolean();
			bool nostart = packet.ReadBoolean();
			session.IsPause  = false;
			if(session.Server!=null){
				session.Server.OnRoomList(session, nolock, nostart);
			}
		}
		private static void OnPlayerList(Session session, PacketReader packet){
			session.OnPlayerList();
		}
		//登录
		private static void OnInfo(Session session, PacketReader packet){
			string name = packet.ReadUnicode(20);
			string pwd = packet.ReadUnicode(32);//md5
			//登录
			if(Login(name, pwd)){
				session.Name = name;
				session.IsPause = false;
				//返回聊天端口，对战端口
				if(session.Server!=null){
					lock(session.Server.Clients){
                        if (session.Server.Clients.ContainsKey(session.Name)){
                            session.Name = null;
                            session.IsPause = true;
                            session.SendError("[err]已经登录");
                            return;
                        }
                        else{
							session.Server.Clients.Add(session.Name, session);
                            session.Server.OnSendServerInfo(session);
                            session.Server.server_OnPlayerJoin(session.ServerInfo, session.Name, null);
                        }
					}
                }
			}else{
				session.SendError("[err]认证失败");
            }
		}
		private static void OnChat(Session session, PacketReader packet){
			string name = packet.ReadUnicode(20);
			string toname = packet.ReadUnicode(20);
			string msg= packet.ReadUnicode(256);
			if(session.Server!=null){
				session.Server.OnChatMessage(name, toname, msg);
			}
		}
		#endregion
	}
}
