/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/9/3
 * 时间: 16:56
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Windows.Forms;
using System.Xml;

namespace YGOClient
{
	public sealed class Client
	{
		public User m_user;
		private RoomForm m_room;
		private NotifyIcon m_notifyIcon;
		public bool isClose{get;set;}
		public bool ShortPasswrod = false;
		public Client(NotifyIcon notifyIcon)
		{
			m_user=null;
			m_notifyIcon=notifyIcon;
			ShortPasswrod = ConfigManager.readBoolean("shortpwd");
		}
		
		#region setting
		public bool UserIsNull(){
			return m_user==null;
		}

		public void SetGamePath(string path){
			if(m_user!=null){
				m_user.setGamePath(path);
			}
		}
		#endregion
		
		#region form
		public void ChangedUser(){
			if(m_user==null){
				return;
			}
			using(UserForm form=new UserForm(false, ShortPasswrod)){
				if(form.ShowDialog() == DialogResult.OK){
					User user = form.GetUser();
					if(m_notifyIcon!=null && user!=null){
						m_user=user;
						m_notifyIcon.Text="用户："+m_user.Name;
						m_user.Save();
					}
				}
			}
			Show();
		}
		
		public bool IsShowing(){
			return m_room!=null&&m_room.Visible;
		}
		
		
		public void Hide(){
			if(m_room!=null){
				m_room.Hide();
			}
		}
		public void Show(){
			if(m_user==null){
				//需要登录
				using(UserForm form=new UserForm(true,ShortPasswrod)){
					if(form.ShowDialog() == DialogResult.OK){
						m_user = form.GetUser();
						if(m_user!=null && m_notifyIcon!=null){
							m_notifyIcon.Text="用户："+m_user.Name;
							m_user.Save();
						}
					}
				}
			}
			if(m_user==null){
				return;
			}
			if(m_room==null || m_room.IsDisposed){
				m_room=new RoomForm(this);
			}
			m_room.Text = ""+m_user.Name;
			m_room.Show();
			m_room.Activate();
		}
		
		public void Close(){
			if(m_room==null || m_room.IsDisposed){
				return;
			}
			m_room.Close();
		}
		#endregion
	}
}
