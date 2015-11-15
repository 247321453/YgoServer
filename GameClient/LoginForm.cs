/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/7
 * 时间: 20:34
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Xml;
using YGOCore;

namespace GameClient
{
	/// <summary>
	/// Description of LoginForm.
	/// </summary>
	public partial class LoginForm : Form
	{
		MainForm m_main;
		public Client Client{get;private set;}
		public LoginForm()
		{
			Client = new Client();
			m_main = new MainForm(Client);
			Client.OnLoginSuccess += delegate {
				BeginInvoke(new Action(()=>{
				                       	this.Hide();
				                       	m_main.Show();
				                       })
				           );
				
			};
			InitializeComponent();
		}
		
		#region login
		void Button_Login_Click(object sender, EventArgs e)
		{
			string username = tb_username.Text;
			string pwd = tb_password.Text;
			if(string.IsNullOrEmpty(username)){
				MessageBox.Show("用户名不能为空");
				return;
			}
			if(username.Contains("$")||username.Contains("[")||username.Contains("]")){
				MessageBox.Show("用户名不合法");
				return;
			}
			if(!Client.Connect(Program.Config)){
				MessageBox.Show("无法连接服务器");
				return;
			}
			Client.Login(username, pwd);
		}
		#endregion
		
		void Btn_addaiClick(object sender, EventArgs e)
		{
			
		}
	}
}
