/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/10/31
 * 时间: 14:54
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;

namespace AsyncServer
{
	/// <summary>
	/// Description of ByteQueue.
	/// </summary>
	public class ArrayQueue<T> : Queue<T>
	{
		private readonly byte[] _lock = new byte[0];

		public ArrayQueue()
		{
		}
		public void Enqueue(T[] data)
		{
			Enqueue(data, 0, data.Length);
		}
		public void Enqueue(T[] data, int start, int count)
		{
			if(data == null) return;
			lock(_lock){
				for(;start<data.Length && count>0;count--){
					this.Enqueue(data[start++]);
				}
			}
		}
		public void Dequeue(T[] data)
		{
			Dequeue(data, 0, data.Length);
		}
		/// <summary>
		/// 从0开始移除
		/// </summary>
		/// <param name="data"></param>
		/// <param name="start"></param>
		/// <param name="count"></param>
		public void Dequeue(T[] data, int start, int count)
		{
			if(data == null) return ;
			lock(_lock){
				if(count > this.Count){
					count = this.Count;
				}
				for(;start<data.Length && count>0;count--){
					data[start++] = this.Dequeue();
				}
			}
		}
	}
}
