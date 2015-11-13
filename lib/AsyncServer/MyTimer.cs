/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/11/13
 * 时间: 17:37
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;

namespace AsyncServer
{
	public class MyTimer : System.Timers.Timer{
		private DateTime? timer;
		private long maxtime;
		public long MaxTime{get{return maxtime;}}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="interval">每隔多少毫秒</param>
		/// <param name="maxtime">最大秒数</param>
		public MyTimer(long interval, long maxtime) : base(interval) {
			timer = DateTime.Now;
			this.maxtime=maxtime;
		}
		public new void Start(){
			timer = DateTime.Now;
			base.Start();
		}
		public long Second{
			get{return (long)Math.Round((float)(DateTime.Now.Ticks - timer.Value.Ticks)/(1000f*10000f)); }
		}
		public void Restart() {
			Stop();
			Start();
		}
		public bool CheckStop(){
			if(Second >= maxtime){
				Stop();
				return true;
			}
			return false;
		}
	}
}
