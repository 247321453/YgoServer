/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/8/31
 * 时间: 16:43
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.IO;
using System.Net;
using System.Threading;

namespace YGOCore
{
	/// <summary>
	/// Description of ApiHttpServer.
	/// </summary>
	public class ApiHttpServer
	{
		Server server;
		HttpListener httpListener;
		System.Timers.Timer Timer=null;
		static object _lock=new object();
		string json="[]";
		bool isStart=false;
		
		public ApiHttpServer(Server server,int port=18911){
			this.server=server;
			Timer=new System.Timers.Timer(1000);
			Timer.Elapsed+= new System.Timers.ElapsedEventHandler(Timer_Elapsed);
			Timer.Enabled=true;
			Timer.AutoReset=true;
			httpListener = new HttpListener();
			httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
			if(port<=0){
				port=18911;
			}
			try{
				httpListener.Prefixes.Add("http://localhost:"+port+"/");
			}catch(Exception){
				
			}
		}
		
		public void Start(){
			if(isStart){
				return;
			}
			isStart=true;
			Timer.Start();
			httpListener.Start();
			Thread thread=new Thread(new ThreadStart(Listen));
			thread.IsBackground=true;
			thread.Start();
		}

		void Listen(){
			while (true)
			{
				
				HttpListenerContext httpListenerContext = httpListener.GetContext();
				httpListenerContext.Response.StatusCode = 200;
				//httpListenerContext.Request.Url
				using (StreamWriter writer = new StreamWriter(httpListenerContext.Response.OutputStream))
				{
					writer.Write(json);
				}
			}
		}
		
		void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			try{
				lock(_lock){
					json=server.getRoomJson();
				}
			}catch(Exception){}
		}
	}
}
