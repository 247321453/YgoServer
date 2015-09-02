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
		private string[] json;
		private int[] lastTime;
		private const int MINTIME = 500;
		private bool IsListening=false;
		
		public MyHttpServer(Server server,int port): base(port) {
			this.server=server;
			json=new string[4];
			lastTime=new int[4];
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
			int index=p.http_url.IndexOf("?");
			string url=p.http_url;
			string arg="";
			if(index>0){
				url=p.http_url.Substring(0, index);
				if(index+1<p.http_url.Length){
					arg=p.http_url.Substring(index+1);
				}
			}
			if(url.EndsWith("/room.json")||url.EndsWith("/room")){
				p.writeSuccess();
				p.outputStream.Write(GetContent(url, arg));
			}else{
				p.writeFailure();
			}
		}
		
		public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData) {
			//Console.WriteLine("POST request: {0}", p.http_url);
			if(p.http_url.EndsWith("/room.json")||p.http_url.EndsWith("/room")){
				string data = inputData.ReadToEnd();
				p.outputStream.Write(GetContent(p.http_url, data));
			}else{
				p.writeFailure();
			}
		}
		
		private string GetContent(string url, string arg){
			bool hasLock=true;
			bool hasStart=true;
			if(arg.Contains("lock=false")){
				hasLock=false;
			}
			if(arg.Contains("start=false")){
				hasStart=false;
			}
			//Console.WriteLine("request: {0} hasLock={1}  hasStart={2}", url, hasLock, hasStart);
			int now=Environment.TickCount;
			int pos=0;
			if(hasLock && hasStart){
				pos=0;
			}else if(hasLock){
				pos=1;
			}else if(hasStart){
				pos=2;
			}else{
				pos=3;
			}
			if((now-lastTime[pos])>=MINTIME){
				lastTime[pos]=now;
				json[pos]=server.getRoomJson(hasLock, hasStart);
			}
			return json[pos];
		}
	}
}
