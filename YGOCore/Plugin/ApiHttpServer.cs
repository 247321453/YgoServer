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
//				if(ip!="127.0.0.1"){
//					httpListener.Prefixes.Add("http://127.0.0.1:"+port+"/");
//				}
//				if(ip!="localhost"){
//					httpListener.Prefixes.Add("http://localhost:"+port+"/");
//				}
				httpListener.Prefixes.Add("http://+:"+port+"/");
			}catch(Exception){
				
			}
			Logger.WriteLine("Room List = http://{your ip}:"+port+"/room.json");
		}
		
		public void Stop(){
			try{
				httpListener.Stop();
			}catch(Exception){
				
			}
		}
		public void Start(){
			if(!HttpListener.IsSupported){
				Logger.WriteError("Don't support HttpListener.");
				return;
			}
			if(isStart){
				return;
			}
			
			isStart=true;
			Timer.Start();
			Thread thread=new Thread(new ThreadStart(Listen));
			thread.IsBackground=true;
			thread.Start();
		}

		void Listen(){
			try{
				httpListener.Start();
			}catch(Exception){
				Logger.WriteError("Please run as Administrator.");
			}
			while (httpListener.IsListening)
			{
				HttpListenerContext httpListenerContext = httpListener.GetContext();
				ThreadPool.QueueUserWorkItem(new WaitCallback(TaskProc), httpListenerContext);
			}
		}
		void TaskProc(object obj){
			HttpListenerContext context=obj as HttpListenerContext;
			if(context!=null){
				try{
					string url=context.Request.Url.AbsoluteUri;
					context.Response.StatusCode = 200;
					if(url.EndsWith("room")||url.EndsWith("room.json")){
						//httpListenerContext.Request.Url
						using (StreamWriter writer = new StreamWriter(context.Response.OutputStream))
						{
							lock(_lock){
								writer.Write(json);
							}
						}
					}else{
						context.Response.StatusCode = 404;
						context.Response.Close();
					}
				}catch(Exception){}
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
