/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/8/29
 * 时间: 21:42
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Net;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;

namespace YGOCore
{
	/// <summary>
	/// Description of Tool.
	/// </summary>
	public class Tool
	{
		#region 获取网址内容
		public static string PostHtmlContentByUrl(string url,string param, int outtime=30*1000)
		{
			string htmlContent = string.Empty;
			try {
				HttpWebRequest httpWebRequest =
					(HttpWebRequest)WebRequest.Create(url);
				httpWebRequest.Timeout = outtime;
				httpWebRequest.UserAgent="Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.94 Safari/537.36";
				
				byte[] bs = Encoding.UTF8.GetBytes(param);
				httpWebRequest.Method = "POST";
				httpWebRequest.ContentType = "application/x-www-form-urlencoded";
				httpWebRequest.ContentLength = bs.Length;
				using (Stream reqStream = httpWebRequest.GetRequestStream())
				{
					reqStream.Write(bs, 0, bs.Length);
					reqStream.Close();
				}
				
				using(HttpWebResponse httpWebResponse =
				      (HttpWebResponse)httpWebRequest.GetResponse())
				{
					using(Stream stream = httpWebResponse.GetResponseStream())
					{
						using(StreamReader streamReader =
						      new StreamReader(stream, Encoding.UTF8))
						{
							htmlContent = streamReader.ReadToEnd();
							streamReader.Close();
						}
						stream.Close();
					}
					httpWebResponse.Close();
				}
				return htmlContent;
			}
			catch{
				
			}
			return "";
		}
		public static string GetHtmlContentByUrl(string url, int outtime=30*1000)
		{
			string htmlContent = string.Empty;
			try {
				HttpWebRequest httpWebRequest =
					(HttpWebRequest)WebRequest.Create(url);
				httpWebRequest.Timeout = outtime;
				httpWebRequest.UserAgent="Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.94 Safari/537.36";
				using(HttpWebResponse httpWebResponse =
				      (HttpWebResponse)httpWebRequest.GetResponse())
				{
					using(Stream stream = httpWebResponse.GetResponseStream())
					{
						using(StreamReader streamReader =
						      new StreamReader(stream, Encoding.UTF8))
						{
							htmlContent = streamReader.ReadToEnd();
							streamReader.Close();
						}
						stream.Close();
					}
					httpWebResponse.Close();
				}
				return htmlContent;
			}
			catch{
				
			}
			return "";
		}
		#endregion
		#region MD5校验
		/// <summary>
		/// MD5　32位加密
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string GetMd5(string str)
		{
			string cl = str;
			string pwd = "";
			MD5 md5 = MD5.Create();//实例化一个md5对像
			// 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择
			byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
			// 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
			for (int i = 0; i < s.Length; i++)
			{
				// 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符

				pwd = pwd + s[i].ToString("x2");
				
			}
			return pwd;
		}
		/// <summary>
		/// 计算文件的MD5校验
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static string GetFileMD5(string fileName)
		{
			if(!File.Exists(fileName))
				return "";
			long filesize=0;
			try
			{
				FileStream file = new FileStream(fileName, FileMode.Open);
				filesize=file.Length;
				MD5 md5 = new MD5CryptoServiceProvider();
				byte[] retVal = md5.ComputeHash(file);
				file.Close();

				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < retVal.Length; i++)
				{
					sb.Append(retVal[i].ToString("x2"));
				}
				return sb.ToString();
			}
			catch //(Exception ex)
			{
				//throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
			}
			return filesize.ToString();
		}
		#endregion
		
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
		public static String GetIP()
		{
			string tempip = "0.0.0.0";
			try
			{
				WebRequest wr = WebRequest.Create("http://ip.cn/");
				Stream s = wr.GetResponse().GetResponseStream();
				StreamReader sr = new StreamReader(s, Encoding.UTF8);
				string all = sr.ReadToEnd(); //读取网站的数据

				int start = all.IndexOf("<code>");
				if(start<0){
					return tempip;
				}
				int end = all.IndexOf("</code>", start);
				tempip = all.Substring(start+"<code>".Length, end - start-"<code>".Length);
				sr.Close();
				s.Close();
			}
			catch
			{
			}
			return tempip;
		}
		/// <summary>
		/// 字符串转为UniCode码字符串
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string StringToUnicode(string s)
		{
			if(string.IsNullOrEmpty(s))return "";
			char[] charbuffers = s.ToCharArray();
			byte[] buffer;
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < charbuffers.Length; i++)
			{
				buffer = System.Text.Encoding.Unicode.GetBytes(charbuffers[i].ToString());
				sb.Append(String.Format("\\u{0:X2}{1:X2}", buffer[1], buffer[0]));
			}
			return sb.ToString();
		}
		/// <summary>
		/// Unicode字符串转为正常字符串
		/// </summary>
		/// <param name="srcText"></param>
		/// <returns></returns>
		public static string UnicodeToString(string srcText)
		{
			string dst = "";
			string src = srcText;
			int len = srcText.Length / 6;
			for (int i = 0; i <= len - 1; i++)
			{
				string str = "";
				str = src.Substring(0, 6).Substring(2);
				src = src.Substring(6);
				byte[] bytes = new byte[2];
				bytes[1] = byte.Parse(int.Parse(str.Substring(0, 2), NumberStyles.HexNumber).ToString());
				bytes[0] = byte.Parse(int.Parse(str.Substring(2, 2), NumberStyles.HexNumber).ToString());
				dst += Encoding.Unicode.GetString(bytes);
			}
			return dst;
		}
	}
}
