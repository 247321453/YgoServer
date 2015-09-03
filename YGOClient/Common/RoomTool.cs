/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/9/1
 * 时间: 18:48
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace YGOClient
{
	/// <summary>
	/// Description of RoomTool.
	/// </summary>
	public class RoomTool
	{
		public static string PRO="ccygo";
		/// <summary>
		/// ccygo://127.0.0.1:8911/hello$123/username
		/// </summary>
		/// <param name="cmd"></param>
		public static void Command(string cmd){
			if(cmd==null){
				return;
			}
			int index=(PRO+"://").Length+1;
			if(index<cmd.Length){
				string room =cmd.Substring((PRO+"://").Length+1);
				Start(room);
			}else{
				MessageBox.Show("error");
			}
		}
		
		public static void Start(string cmd)
		{
			string[] names=cmd.Split('/');
			string server=names[0];
			string room=names.Length>1?names[1]:"";
			string name=names.Length>2?names[2]:null;
			string ip=server.Split(':')[0];
			string port=server.IndexOf(':')>0?server.Split(':')[1]:"7911";
			Dictionary<string, string> args=new Dictionary<string, string>();
			if(!string.IsNullOrEmpty(name)){
				args.Add("nickname", name);
			}
			args.Add("lastip", ip);
			args.Add("lastport", port);
			args.Add("roompass", room);
			Write(args);
			RunGame("-r");
		}
		
		private static void RunGame(string arg=" "){
			string file= Combine(Application.StartupPath, "ygopro.exe");
			if(File.Exists(file)){
				Process.Start(file, arg);
				return;
			}
			file= Combine(Application.StartupPath, "ygopro_vs.exe");
			if(File.Exists(file)){
				Process.Start(file, arg);
				return;
			}
			MessageBox.Show("no find ygopro.exe");
		}
		
		private static void Write(Dictionary<string, string> args){
			string file= Combine(Application.StartupPath, "system.conf");
			if(File.Exists(file)){
				string[] lines=File.ReadAllLines(file);
				for(int i=0;i<lines.Length;i++){
					if(lines[i]==null){
						continue;
					}
					foreach(string key in args.Keys){
						if(lines[i].StartsWith(key)){
							lines[i]=key+" = "+args[key];
							args.Remove(key);
							break;
						}
					}
					if(args.Count==0){
						break;
					}
				}
				if(args.Count>0){
					List<string> newlines=new List<string>();
					newlines.AddRange(lines);
					foreach(string key in args.Keys){
						newlines.Add(key+" = "+args[key]);
					}
					File.WriteAllLines(file, newlines.ToArray());
				}else{
					File.WriteAllLines(file, lines);
				}
				return;
			}
			MessageBox.Show("no find system.conf");
		}
		
		#region
		/// <summary>
		/// 合并路径
		/// </summary>
		/// <param name="paths"></param>
		/// <returns></returns>
		public static string Combine(params string[] paths)
		{
			if (paths.Length == 0)
			{
				throw new ArgumentException("please input path");
			}
			else
			{
				StringBuilder builder = new StringBuilder();
				string spliter = Path.DirectorySeparatorChar.ToString();
				string firstPath = paths[0];
				if (firstPath.StartsWith("HTTP", StringComparison.OrdinalIgnoreCase))
				{
					spliter = "/";
				}
				if (!firstPath.EndsWith(spliter))
				{
					firstPath = firstPath + spliter;
				}
				builder.Append(firstPath);
				for (int i = 1; i < paths.Length; i++)
				{
					string nextPath = paths[i];
					if (nextPath.StartsWith("/") || nextPath.StartsWith("\\"))
					{
						nextPath = nextPath.Substring(1);
					}
					if (i != paths.Length - 1)//not the last one
					{
						if (nextPath.EndsWith("/") || nextPath.EndsWith("\\"))
						{
							nextPath = nextPath.Substring(0, nextPath.Length - 1) + spliter;
						}
						else
						{
							nextPath = nextPath + spliter;
						}
					}
					builder.Append(nextPath);
				}
				return builder.ToString();
			}
		}
		#endregion
	}
}
