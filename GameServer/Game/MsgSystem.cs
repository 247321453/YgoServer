/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/9/1
 * 时间: 16:18
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.IO;
using System.Collections.Generic;

namespace YGOCore.Net
{
	/// <summary>
	/// Description of ServerMessage.
	/// </summary>
	public class MsgSystem
	{
		static int Index=0;
		public static List<string> Msgs=new List<string>();
		public static void Init(string file){
			try{
				if(File.Exists(file)){
					Msgs.AddRange(File.ReadAllLines(file));
				}
			}catch(Exception){
				
			}
		}
		
		public static string getNextMessage(){
			if(Msgs.Count==0){
				return null;
			}
			string msg=getMessage("[Server]",Index);
			Index++;
			if(Index>=Msgs.Count){
				Index=0;
			}
			return msg;
		}
		
		public static string getMessage(string name,int index){
			if(Msgs.Count==0 || index>=Msgs.Count){
				return "Welcome use this server.--by caicai.";
			}
			return Msgs[index];
		}
	}
}
