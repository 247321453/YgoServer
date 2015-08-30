/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/8/30
 * 时间: 17:42
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;

namespace YGOCore.Game
{
	/// <summary>
	/// Description of RoomInfo.
	/// </summary>
	public class RoomInfo
	{
		public string roomname{get;private set;}
		//public string pass{get;private set;}
		public int rule{get;private set;}
		public int mode{get;private set;}
		public bool needPass;
		public string banname{get;private set;}
		public string hostPlayer{get;private set;}
		public string[] players{get;private set;}
		public string[] observers{get;private set;}
		public int count{get;private set;}
		
		public RoomInfo(Game game){
			if(game!=null){
				foreach(Player p in game.Players){
					if(p!=null){
						count++;
					}
				}
				count+=game.Observers.Count;
			}
		}
		public RoomInfo(string roomname,string roominfo,int rule,int mode,string banlist,string[] players,string[] observers)
		{
			this.roomname=roomname;
			this.rule=rule;
			this.mode=mode;
			this.players=players;
			this.observers=observers;
		}
		
		public string toJson(){
			return "{\"name\":\""+roomname+"\",}";
		}
	}
}
