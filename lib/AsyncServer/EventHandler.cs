/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/11/2
 * 时间: 13:04
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;

namespace AsyncServer
{
	/// <summary>
	/// Description of Handler.
	/// </summary>
	public class EventHandler<T1,T2>
	{
		readonly SortedList<ushort, Action<T1, T2>> m_actions = new SortedList<ushort, Action<T1, T2>>();
		
		public void Register(ushort id, Action<T1, T2> action){
			if(m_actions.ContainsKey(id)){
				m_actions[id] = action;
				Logger.Warn("a same id action."+id);
			}else{
				m_actions.Add(id, action);
			}
		}
		
		public Action<T1, T2> GetAction(ushort id){
			if(m_actions.ContainsKey(id)){
				return m_actions[id];
			}
			return null;
		}
		
		public void Do(ushort id, T1 a, T2 b){
			Action<T1, T2> action= GetAction(id);
			if(action != null){
				action(a, b);
			}else{
				Logger.Warn("don't do id "+id);
			}
		}
		
	}
}
