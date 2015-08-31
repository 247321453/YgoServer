/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/8/30
 * 时间: 17:42
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using OcgWrapper.Enums;
using System.Collections.Generic;

namespace YGOCore.Game
{
	/// <summary>
	/// Description of RoomInfo.
	/// </summary>
	public class RoomInfo
	{
		/// <summary>
		/// 房间名(需要去除$后面)
		/// </summary>
		public string RoomName{get;private set;}
		//public string pass{get;private set;}
		/// <summary>
		/// 规则
		/// </summary>
		public int Rule{get;private set;}
		/// <summary>
		/// 模式
		/// </summary>
		public int Mode{get;private set;}
		/// <summary>
		/// 是否需要密码
		/// </summary>
		public bool NeedPass{get;private set;}
		/// <summary>
		/// 是否开始
		/// </summary>
		public bool IsStart{get;private set;}
		/// <summary>
		/// 禁卡表
		/// </summary>
		public int Lflist{get;private set;}
		/// <summary>
		/// 玩家
		/// </summary>
		public string[] players{get;private set;}
		/// <summary>
		/// 观战
		/// </summary>
		public string[] observers{get;private set;}
		/// <summary>
		/// 玩家数
		/// </summary>
		public int Count{get;private set;}
		
		public RoomInfo(Game game){
			if(game!=null&&game.Config!=null){
				RoomName=game.Config.Name;
				int i=RoomName.LastIndexOf("$");
				if(i>=0){
					RoomName=RoomName.Substring(0, i);
					NeedPass =true;
				}else{
					NeedPass =false;
				}
				Rule=game.Config.Rule;
				Mode=game.Config.Mode;
				Lflist=game.Config.LfList;
				IsStart= (game.State!=GameState.Lobby);
				if(game.Players!=null){
					int len=game.Players.Length;
					Player[] pls=new Player[len];
					players=new string[len];
					game.Players.CopyTo(pls, 0);
					for(i=0;i<len;i++){
						if(pls[i]!=null){
							players[i]=pls[i].Name;
							Count++;
						}
					}
				}
				if(game.Observers!=null){
					Player[] pls=game.Observers.ToArray();
					int len=pls.Length;
					observers=new string[len];
					for(i=0;i<len;i++){
						if(pls[i]!=null){
							observers[i]=pls[i].Name;
						}
					}
					Count+=len;
				}
			}
		}
		public RoomInfo(string roomname,string roominfo,int rule,int mode,string banlist,string[] players,string[] observers)
		{
			this.RoomName=roomname;
			this.Rule=rule;
			this.Mode=mode;
			this.players=players;
			this.observers=observers;
		}
		
		public string toJson(){
			return "{\"name\":\""+Tool.StringToUnicode(RoomName)+"\",\"Rule\":"+Rule+",\"Mode\":"+Mode+
				",\"NeedPass\":"+(NeedPass?1:0)+",\"IsStart\":"+(IsStart?1:0)
				+",\"Lflist\":\""+(Lflist==0?"o":"t")+"\",\"Count\":"+Count+",\"player\":"+Array2Json(players)+"}";
		}
		public string toJsonWithWatch(){
			return "{\"name\":\""+Tool.StringToUnicode(RoomName)+"\",\"Rule\":"+Rule+",\"Mode\":"+Mode+
				",\"NeedPass\":"+(NeedPass?1:0)+",\"IsStart\":"+(IsStart?1:0)
				+",\"Lflist\":\""+(Lflist==0?"o":"t")+"\",\"Count\":"+Count+
				",\"player\":"+Array2Json(players)+",\"watch\":"+Array2Json(observers)+"}";
		}
		private string Array2Json(string[] arr){
			if(arr.Length==0){
				return "[]";
			}
			int count=arr.Length;
			string str="[";
			for(int i=0;i<count-1;i++){
				str+="\""+Tool.StringToUnicode(arr[i])+"\",";
			}
			str+="\""+Tool.StringToUnicode(arr[count-1])+"\"]";
			return str;
		}
	}
}
