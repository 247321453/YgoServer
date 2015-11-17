using System;
using AsyncServer;
using System.Diagnostics;

namespace YGOCore
{
	public class ServerProcess
	{
		private Process process;
		public bool isRunning{get;private set;}
		private string m_fileName;
		private int m_port;
		private string m_config;
		public int Port{get{return m_port;}}
		public ServerProcess(int port,string fileName="GameServer.exe", string config="config.txt")
		{
			this.m_port = port;
			this.m_fileName = fileName;
			this.m_config = config;
		}
		public void Start(){
			if(isRunning)return;
			isRunning = true;
			if(process==null||process.HasExited){
				process=new Process();
			}
			process.StartInfo.FileName = m_fileName;
			//设定程式执行参数
			process.StartInfo.Arguments = " "+m_config +(m_port>0?" "+m_port:"");
			process.EnableRaisingEvents=true;
			process.StartInfo.WindowStyle=ProcessWindowStyle.Hidden;
			process.Exited+=new EventHandler(Exited);
			try{
				process.Start();
			}catch(Exception e){
				Logger.Error(e);
			}
		}
		private void Exited(object sender, EventArgs e){
			if(isRunning){
				Close();
				//异常结束
				Start();
			}else{
				Close();
			}
		}
		public void Close(){
			if(!isRunning)return;
			isRunning = false;
			if(process!=null){
				try{
					process.Kill();
				}catch(Exception){
					
				}finally{
					try{
						process.Close();
					}catch{}
					process = null;
				}
			}
		}
	}
}
