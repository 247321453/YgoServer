/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/8/30
 * 时间: 17:42
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Data;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace YGOCore.Game
{
	[DataContract]
	public class RoomInfo
	{
		/// <summary>
		/// 房间名(需要去除$后面)
		/// </summary>
		[DataMember(Order = 0, Name="room")]
		public string RoomName{get; set;}
		//public string pass{get;private set;}
		/// <summary>
		/// 规则
		/// </summary>
		[DataMember(Order = 1, Name="rule")]
		public int Rule{get; set;}
		/// <summary>
		/// 模式
		/// </summary>
		[DataMember(Order = 2, Name="mode")]
		public int Mode{get; set;}
		/// <summary>
		/// 是否需要密码
		/// </summary>
		[DataMember(Order = 3, Name="lock")]
		public bool NeedPass{get; set;}
		/// <summary>
		/// 是否开始
		/// </summary>
		[DataMember(Order = 4, Name="start")]
		public bool IsStart{get; set;}
		/// <summary>
		/// 禁卡表
		/// </summary>
		[DataMember(Order = 5, Name="banlist")]
		public int Lflist{get; set;}
		/// <summary>
		/// 玩家
		/// </summary>
		[DataMember(Order = 6, Name="players")]
		public string[] players{get; set;}
		/// <summary>
		/// 观战
		/// </summary>
		[DataMember(Order = 7, Name="watchs")]
		public string[] observers{get; set;}
		[DataMember(Order = 8, Name="lp")]
		public int StartLP{get; set;}
		/// <summary>
		/// 特殊规则
		/// </summary>
		[DataMember(Order = 9, Name="warring")]
		public bool Warring{get; set;}
		/// <summary>
		/// 玩家数
		/// </summary>
		[IgnoreDataMember()]
		public int Count{get; set;}

		public RoomInfo(){
			players=new string[2];
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
