/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/9/3
 * 时间: 17:02
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using YGOCore;
using System.Windows.Forms;

namespace YGOClient
{
	[DataContract(Name="Server")]
	public class Server
	{
		public static string TAG ="server";
		[DataMember(Order = 0, Name="ip")]
		public string IP{get;private set;}
		[DataMember(Order = 1, Name="port")]
		public int Port{get;private set;}
		[DataMember(Order = 2, Name="roomurl")]
		public string RoomUrl{get;private set;}
		[DataMember(Order = 3, Name="needauth")]
		public bool NeedAuth{get;private set;}
		[DataMember(Order = 4, Name="registerurl")]
		public string RegisterUrl{get;private set;}
		//	private static string KEY="IBM855";
		
		public static string DIR=Tool.Combine(Application.StartupPath, "data");
		public static string EX=".srv";
		
		public Server(Server srv){
			SetServer(srv);
		}
		public Server(string ip, int port, string roomurl,bool needauth)
		{
			this.IP=ip;
			this.Port=port;
			this.RoomUrl= roomurl;
			this.NeedAuth=needauth;
		}
		
		public void SetServer(Server srv){
			this.IP=srv.IP;
			this.Port=srv.Port;
			this.RoomUrl=srv.RoomUrl;
			this.NeedAuth=srv.NeedAuth;
			this.RegisterUrl=srv.RegisterUrl;
		}

		public void setRegisterUrl(string regurl){
			this.RegisterUrl=regurl;
		}
		public void Save(string Name){
			string json=JsonTool.ToJson(this);

			if(!Directory.Exists(DIR)){
				Directory.CreateDirectory(DIR);
			}
			string file=Tool.Combine(DIR, Name+EX);
			File.WriteAllText(file, json);
		}
		public static Server Load(string Name){
			if(string.IsNullOrEmpty(Name)){
				return null;
			}
			Server user=null;
			if(!Name.EndsWith(EX)){
				Name+=EX;
			}
			string file=Tool.Combine(DIR, Name);
			if(File.Exists(file)){
				try{
					string json=File.ReadAllText(file);
					user=JsonTool.Parse<Server>(json);
				}catch(Exception){
				}
			}else{
			}
			return user;
		}
		
		public static void Delete(string Name){
			string file=Tool.Combine(DIR, Name+EX);
			if(File.Exists(file)){
				File.Delete(file);
			}
		}
	}
}
