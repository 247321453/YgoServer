/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/9/1
 * 时间: 21:43
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.IO;
using System.Net;
using Bend.Util;
using System.Threading;

namespace YGOCore.Plugin
{
	public class MyHttpServer : HttpServer {
		private Server server;
		private string json;
		private int lastTime;
		private const int MINTIME = 500;
		private bool IsListening=false;
		
		public MyHttpServer(Server server,int port): base(port) {
			this.server=server;
			Logger.WriteLine("Room List = http://{your ip}:"+port+"/room.json");
		}
		
		public void Start(){
			if(IsListening){
				return;
			}
			IsListening=true;
			Thread thread = new Thread(new ThreadStart(listen));
			thread.IsBackground=true;
			thread.Start();
		}
		
		public void Stop(){
			is_active = false;
			IsListening=false;
		}
		
		public override void handleGETRequest(HttpProcessor p) {
			//Console.WriteLine("request: {0}", p.http_url);
			if(p.http_url.EndsWith("/room.json")||p.http_url.EndsWith("/room")){
				p.writeSuccess();
				int now=Environment.TickCount;
				if((now-lastTime)>=MINTIME){
					lastTime=now;
					json=server.getRoomJson();
				}
				p.outputStream.Write(json);
			}else{
				p.writeFailure();
			}
		}

		public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData) {
			//Console.WriteLine("POST request: {0}", p.http_url);
			if(p.http_url.EndsWith("/room.json")||p.http_url.EndsWith("/room")){
				string data = inputData.ReadToEnd();
				int now=Environment.TickCount;
				if((now-lastTime)>=MINTIME){
					lastTime=now;
					json=server.getRoomJson();
				}
				p.outputStream.Write(json);
			}else{
				p.writeFailure();
			}
		}
	}
}
