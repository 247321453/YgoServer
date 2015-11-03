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
	public class EventHandler<T1,T2,T3>
	{
		readonly SortedList<T1, Action<T2, T3>> m_actions = new SortedList<T1, Action<T2, T3>>();
		
		public void Register(T1 id, Action<T2, T3> action){
			if(m_actions.ContainsKey(id)){
				m_actions[id] = action;
				Logger.Warn("a same id action."+id);
			}else{
				m_actions.Add(id, action);
			}
		}
		
		public Action<T2, T3> GetAction(T1 id){
			if(m_actions.ContainsKey(id)){
				return m_actions[id];
			}
			return null;
		}
		
		public void Do(T1 id, T2 a, T3 b){
			Action<T2, T3> action= GetAction(id);
			if(action != null){
				action(a, b);
			}else{
				Logger.Warn("don't do id "+id);
			}
		}
		
	}
}
