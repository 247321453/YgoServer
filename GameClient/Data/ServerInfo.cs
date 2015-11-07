/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/7
 * 时间: 20:56
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Xml;
using System.Collections.Generic;

namespace GameClient.Data
{
	public class ServerInfo{
		public ServerInfo(string val){
			string[] vs= val.Split(':');
			if(val.Length >= 3){
				Name = vs[0];
				Host = vs[1];
				try{
					Port = int.Parse(vs[2]);
				}catch{Port = 0;}
			}else{
				Port = 0;
			}
		}
		public bool isOk{get{return Port > 0;}}
		public string Name{get;private set;}
		public string Host{get;private set;}
		public int Port{get;private set;}
		
		
		public static void GetServerInfos(SortedList<string, ServerInfo> list){
			for(int i=1;i<10;i++){
				string val = ConfigManager.readString("server"+i.ToString("00"));
				if(string.IsNullOrEmpty(val))
					break;
				ServerInfo s = new ServerInfo(val);
				if(s.isOk){
					list.Add(s.Name, s);
				}
			}
		}
	}
}
