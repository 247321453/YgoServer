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
using System.Diagnostics;
using System.Windows.Forms;

namespace YGOClient
{
	/// <summary>
	/// Description of RoomTool.
	/// </summary>
	public class RoomTool
	{
		/// <summary>
		/// ccygo://127.0.0.1:8911/hello$123
		/// </summary>
		/// <param name="cmd"></param>
		public static void Command(string cmd){
			if(cmd==null){
				return;
			}
			int index=(Program.PRO+"://").Length+1;
			if(index<cmd.Length){
				string room =cmd.Substring((Program.PRO+"://").Length+1);
				Start(cmd);
			}else{
				MessageBox.Show("error");
			}
		}
		
		public static void Start(string room)
		{
			string[] names=room.Split('/');
			string pass=names[0];
			string user=names[1];
			if(!string.IsNullOrEmpty(user)){
				Write("nickname", user);
			}
			Write("password", pass);
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
		
		private static void Write(string key,string value){
			string file= Combine(Application.StartupPath, "system.conf");
			if(File.Exists(file)){
				string[] lines=File.ReadAllLines(file);
				for(int i=0;i<lines.Length;i++){
					if(lines[i]==null){
						continue;
					}
					if(lines[i].StartsWith(key)){
						lines[i]=key+" = "+value;
					}
				}
				File.WriteAllLines(file, lines);
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
