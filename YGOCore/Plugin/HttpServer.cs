/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/9/1
 * 时间: 21:39
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

// offered to the public domain for any use with no restriction
// and also with no warranty of any kind, please enjoy. - David Jeske.

// simple HTTP explanation
// http://www.jmarshall.com/easy/http/

namespace Bend.Util {

	public class HttpProcessor {
		public TcpClient socket;
		public HttpServer srv;

		private StreamReader inputStream;
		public StreamWriter outputStream;

		public String http_method;
		public String http_url;
		public String http_protocol_versionstring;
		public Hashtable httpHeaders = new Hashtable();


		private static int MAX_POST_SIZE = 10 * 1024 * 1024; // 10MB

		public HttpProcessor(TcpClient s, HttpServer srv) {
			this.socket = s;
			this.srv = srv;
		}

		public void process(object obj=null) {
			// bs = new BufferedStream(s.GetStream());
			try {
				inputStream = new StreamReader(socket.GetStream());
				outputStream = new StreamWriter(new BufferedStream(socket.GetStream()));
				parseRequest();
				readHeaders();
				if(http_method==null){
					return;
				}
				if (http_method.ToLower()=="get") {
					handleGETRequest();
				} else if (http_method.ToLower() == "post") {
					handlePOSTRequest();
				} else{
					handleGETRequest();
				}
			} catch (Exception e) {
				Console.WriteLine("Exception: " + e.ToString());
				writeFailure();
			}finally{
				outputStream.Flush();
				// bs.Flush(); // flush any remaining output
				inputStream = null; outputStream = null; // bs = null;
				socket.Close();
			}
		}

		public void parseRequest() {
			String request = inputStream.ReadLine();
			if(request==null){
				return;
			}
			string[] tokens = request.Split(' ');
			if (tokens.Length != 3) {
				throw new Exception("invalid http request line");
			}
			http_method = tokens[0].ToUpper();
			http_url = tokens[1];
			http_protocol_versionstring = tokens[2];

			//.WriteLine("starting: " + request);
		}

		public void readHeaders() {
			//	Console.WriteLine("readHeaders()");
			String line;
			while ((line = inputStream.ReadLine()) != null) {
				if (line.Equals("")) {
					//Console.WriteLine("got headers");
					return;
				}
				
				int separator = line.IndexOf(':');
				if (separator == -1) {
					throw new Exception("invalid http header line: " + line);
				}
				String name = line.Substring(0, separator);
				int pos = separator + 1;
				while ((pos < line.Length) && (line[pos] == ' ')) {
					pos++; // strip any spaces
				}
				
				string value = line.Substring(pos, line.Length - pos);
				//Console.WriteLine("header: {0}:{1}",name,value);
				httpHeaders[name] = value;
			}
		}

		public void handleGETRequest() {
			srv.handleGETRequest(this);
		}

		public void handlePOSTRequest() {
			// this post data processing just reads everything into a memory stream.
			// this is fine for smallish things, but for large stuff we should really
			// hand an input stream to the request processor. However, the input stream
			// we hand him needs to let him see the "end of the stream" at this content
			// length, because otherwise he won't know when he's seen it all!

			//Console.WriteLine("get post data start");
			int content_len = 0;
			MemoryStream ms = new MemoryStream();
			if (this.httpHeaders.ContainsKey("Content-Length")) {
				content_len = Convert.ToInt32(this.httpHeaders["Content-Length"]);
				if (content_len > MAX_POST_SIZE) {
					throw new Exception(
						String.Format("POST Content-Length({0}) too big for this simple server",
						              content_len));
				}
				byte[] buf = new byte[4096];
				int to_read = content_len;
				while (to_read > 0) {
					int numread = this.inputStream.BaseStream.Read(buf, 0, Math.Min(4096, to_read));
					to_read -= numread;
					ms.Write(buf, 0, numread);
				}
				ms.Seek(0, SeekOrigin.Begin);
			}
			//Console.WriteLine("get post data end");
			srv.handlePOSTRequest(this, new StreamReader(ms));

		}

		public void writeSuccess() {
			outputStream.Write("HTTP/1.0 200 OK\n");
			outputStream.Write("Content-Type: text/html;charset=utf-8\n");
			if(srv.isLocal){
				outputStream.Write("Access-Control-Allow-Origin: * \n");
				outputStream.Write("Access-Control-Allow-Methods: * \n");
				outputStream.Write("Access-Control-Allow-Headers: x-requested-with,content-type \n");
			}
			outputStream.Write("Connection: close\n");
			outputStream.Write("\n");
		}

		public void writeFailure() {
			outputStream.Write("HTTP/1.0 404 File not found\n");
			outputStream.Write("Connection: close\n");
			outputStream.Write("\n");
		}
	}

	public abstract class HttpServer {
		public bool isLocal=false;
		protected int port;
		TcpListener listener;
		protected bool is_active = true;
		
		public HttpServer(int port) {
			this.port = port;
		}

		public void listen() {
			try{
				if(isLocal){
					listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
				}else{
					listener = new TcpListener(IPAddress.Any, port);
				}
				listener.Start();
			}catch(Exception ex){
				Console.WriteLine(ex);
				return;
			}
			while (is_active) {
				TcpClient s = listener.AcceptTcpClient();
				HttpProcessor processor = new HttpProcessor(s, this);
				ThreadPool.QueueUserWorkItem(new WaitCallback(processor.process));
//				Thread thread = new Thread(new ThreadStart(processor.process));
//				thread.IsBackground=true;
//				thread.Start();
				Thread.Sleep(1);
			}
		}

		public abstract void handleGETRequest(HttpProcessor p);
		public abstract void handlePOSTRequest(HttpProcessor p, StreamReader inputData);
	}
}
