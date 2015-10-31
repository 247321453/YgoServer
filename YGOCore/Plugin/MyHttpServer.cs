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
using System.Collections.Generic;
using System.Threading;
using AsyncServer;

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
			isLocal=Program.Config.ApiIsLocal;
			Logger.Info("Room List = http://{your ip}:"+port+"/room.json");
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
			if(p.http_url==null){
				p.writeFailure();
				return;
			}
			int index=p.http_url.IndexOf("?");
			string url=p.http_url;
			string arg="";
			if(index>0){
				url=p.http_url.Substring(0, index);
				if(index+1<p.http_url.Length){
					arg=p.http_url.Substring(index+1);
				}
			}
			Deal(p, url, arg, true);
		}
		
		public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData) {
			//Console.WriteLine("POST request: {0}", p.http_url);
			string data = inputData.ReadToEnd();
			Deal(p, p.http_url, data, false);
		}
		private void Deal(HttpProcessor p,string url, string data,bool isGet){
			if(url==null){
				return;
			}
			if(url=="/"||url.StartsWith("/room.php")||url.StartsWith("/room.json")){
				//房间列表
				p.writeSuccess();
				p.outputStream.Write(GetContent(url, data));
			}else if(url.StartsWith("/deck.php")){
				//卡片列表
				if(data.IndexOf("pwd=caicai")<0){
					p.writeFailure();
					return;
				}
				p.writeSuccess();
				string[] args = data.Split('&');
				foreach(string a in args){
					if(a != null && a.StartsWith("name=")){
						int i = a.IndexOf("=");
						if(i>=0 && i< a.Length-1){
							string name = a.Substring(i+1);
							List<int> cards = GameManager.GameCards(name);
							foreach(int id in cards){
								p.outputStream.WriteLine(""+id);
							}
						}
					}
				}
			}
			else{
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
			}else{
				hasStart=true;
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
