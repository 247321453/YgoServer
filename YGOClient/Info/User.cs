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
	[DataContract(Name="User")]
	public class User
	{
		public static string TAG ="user";
		[DataMember(Order = 0, Name="Name")]
		public string Name{get;private set;}
		[DataMember(Order = 1, Name="Password")]
		public string Password{get;private set;}
		[DataMember(Order = 2, Name="GamePath")]
		public string GamePath{get;private set;}
		[DataMember(Order = 3, Name="GameArgs")]
		public string GameArgs{get;private set;}
		[DataMember(Order = 4, Name="RePassword")]
		public bool RePassword{get;private set;}
		[IgnoreDataMember()]
		public string GameDir{get{
				return Path.GetDirectoryName(GamePath);
			}
		}
		[IgnoreDataMember()]
		public bool ShortPwd{get; set;}
		public string getPassword(){
			if(ShortPwd){
				string pwd = Tool.GetMd5(Password);
				if(pwd.Length >= 32){
					return pwd.Substring(0,2)+pwd.Substring(30, 2);
				}else if(pwd.Length>=4){
					return pwd.Substring(0,4);
				}else{
					return pwd;
				}
			}else{
				return Password;
			}
		}
		
		//	private static string KEY="IBM855";
		
		public static string DIR=Tool.Combine(Application.StartupPath, "data");
		public static string EX=".user";
		private static string key="caicai00";
		private static string key2="00caicai";
		
		public User(User _user){
			SetUser(_user);
			ShortPwd = false;
		}
		public User(string name, string password)
		{
			this.Name=name;
			this.Password=password;
			ShortPwd = false;
		}
		
		public void SetUser(User _user){
			this.Name=_user.Name;
			this.Password=_user.Password;
			this.GameArgs=_user.GameArgs;
			this.GamePath=_user.GamePath;
			this.RePassword=_user.RePassword;
		}
		public void setGamePath(string path){
			this.GamePath=path;
		}
		
		public void setGameArgs(string args){
			this.GameArgs=args;
		}
		public void setRecord(bool record){
			this.RePassword=record;
		}
		public void Save(){
			string oldpass=Password;
			if(!RePassword){
				Password="";
			}else{
				//加密
				Password = Tool.Encrypt(Password, key, key2);// Convert.ToBase64String(Encoding.GetEncoding(KEY).GetBytes(Password));
			}
			string json=Tool.ToJson(this);

			if(!Directory.Exists(DIR)){
				Directory.CreateDirectory(DIR);
			}
			string file=Tool.Combine(DIR, Name+EX);
			File.WriteAllText(file, json);
			Password=oldpass;
		}
		public static User Load(string Name){
			if(string.IsNullOrEmpty(Name)){
				return null;
			}
			User user=null;
			if(!Name.EndsWith(EX)){
				Name+=EX;
			}
			string file=Tool.Combine(DIR, Name);
			if(File.Exists(file)){
				try{
					string json=File.ReadAllText(file);
					user=Tool.Parse<User>(json);
					if(!string.IsNullOrEmpty(user.Password)){
						user.Password = Tool.Decrypt(user.Password, key, key2);//Encoding.GetEncoding(KEY).GetString(Convert.FromBase64String(user.Password));
					}
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
