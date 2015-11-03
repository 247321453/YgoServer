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
		public List<string> players = new List<string>();
		/// <summary>
		/// 观战
		/// </summary>
		[DataMember(Order = 7, Name="watchs")]
		public List<string> observers = new List<string>();
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
		public GameRoom Room{get; set;}
		
		public RoomInfo(){
		}

		public string toJson(){
			return "{\"name\":\""+Tool.StringToUnicode(RoomName)+"\",\"Rule\":"+Rule+",\"Mode\":"+Mode+
				",\"NeedPass\":"+(NeedPass?1:0)+",\"IsStart\":"+(IsStart?1:0)
				+",\"Lflist\":\""+(Lflist==0?"o":"t")+"\","+"\"player\":"+Array2Json(players)+"}";
		}
		public string toJsonWithWatch(){
			return "{\"name\":\""+Tool.StringToUnicode(RoomName)+"\",\"Rule\":"+Rule+",\"Mode\":"+Mode+
				",\"NeedPass\":"+(NeedPass?1:0)+",\"IsStart\":"+(IsStart?1:0)
				+",\"Lflist\":\""+(Lflist==0?"o":"t")+"\""+
				",\"player\":"+Array2Json(players)+",\"watch\":"+Array2Json(observers)+"}";
		}
		private string Array2Json(List<string> arr){
			if(arr.Count==0){
				return "[]";
			}
			int count=arr.Count;
			string str="[";
			for(int i=0;i<count-1;i++){
				str+="\""+Tool.StringToUnicode(arr[i])+"\",";
			}
			str+="\""+Tool.StringToUnicode(arr[count-1])+"\"]";
			return str;
		}
	}
}
