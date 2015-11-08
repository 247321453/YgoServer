/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/8
 * 时间: 12:31
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GameClient
{
	/// <summary>
	/// Description of GameUtil.
	/// </summary>
	public class GameUtil
	{
		public static void JoinRoom(string ip, string port, string name, string room){
			#if DEBUG
			MessageBox.Show(ip+":"+port);
			#endif
			Dictionary<string, string> args=new Dictionary<string, string>();
			if(!string.IsNullOrEmpty(name)){
				args.Add("nickname", name);
			}
			args.Add("lastip", ip);
			args.Add("lastport", port);
			args.Add("roompass", room);
			string file = Combine(Application.StartupPath, "system.conf");
			if(Write(file, args)){
				RunGame(" -j");
			}
		}
		public static void RunGame(string arg){
			string file= Combine(Application.StartupPath, "ygopro.exe");
			if(File.Exists(file)){
				Run(file, arg);
				return;
			}
			file= Combine(Application.StartupPath, "ygopro_vs.exe");
			if(File.Exists(file)){
				Run(file, arg);
				return;
			}
			MessageBox.Show("no find ygopro.exe");
		}
		private static void Run(string file, string arg=""){
			Process p =new Process();
			p.StartInfo.FileName = file;
			p.StartInfo.WorkingDirectory = Path.GetDirectoryName(file);
			p.StartInfo.Arguments = arg;
			p.EnableRaisingEvents=true;
			p.Exited += delegate { p.Close(); p=null; };
			p.Start();
		}
		public static bool Write(string file, Dictionary<string, string> args){
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
				return true;
			}
			return false;
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
