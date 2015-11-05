/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/8/29
 * 时间: 21:42
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;

namespace YGOCore
{
	/// <summary>
	/// Description of Tool.
	/// </summary>
	public class JsonTool
	{
		#region json
		public static T Parse<T>(string jsonString)
		{
			T t=default(T);
			using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
			{
				t=(T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
			}
			return t;
		}

		public static string ToJson(object jsonObject)
		{
			string json="";
			using (var ms = new MemoryStream())
			{
				new DataContractJsonSerializer(jsonObject.GetType()).WriteObject(ms, jsonObject);
				json = Encoding.UTF8.GetString(ms.ToArray());
			}
			return json;
		}
		#endregion
	}
}
