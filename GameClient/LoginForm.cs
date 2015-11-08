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
using GameClient.Data;
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
		#region init
		MainForm m_main;
		public Client Client{get;private set;}
		private ServerInfo m_server;
		readonly SortedList<string, ServerInfo> ServerInfos = new SortedList<string, ServerInfo>();
		public LoginForm()
		{
			m_main = new MainForm(this);
			Client = new Client();
			Client.OnLogin += delegate {
				BeginInvoke(new Action(()=>{
				                       	this.Hide();
				                       	m_main.Show();
				                       })
				           );
				
			};
			InitializeComponent();
		}

		void LoginFormLoad(object sender, EventArgs e)
		{
			ServerInfo.GetServerInfos(ServerInfos);
			string last = ConfigManager.readString("lastserver");
			int i = 0;
			int index = 0;
			foreach(string str in ServerInfos.Keys){
				if(last == str){
					index = 0;
				}
				i++;
				cb_serverlist.Items.Add(str);
			}
			if(cb_serverlist.Items.Count>0){
				cb_serverlist.SelectedIndex = index;
			}
		}
		#endregion
		
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
			if(m_server==null){
				MessageBox.Show("没有服务器信息");
				return;
			}
			if(!Client.Connect(m_server)){
				MessageBox.Show("无法连接服务器");
				return;
			}
			Client.Login(username, pwd);
		}
		
		void Do_RegisterButton_Click(object sender, EventArgs e)
		{
			MessageBox.Show("不用注册哦，密码随意填");
		}
		
		void Do_ServerList_SelectedIndexChanged(object sender, EventArgs e)
		{
			try{
				int index = cb_serverlist.SelectedIndex;
				string name = cb_serverlist.Items[index].ToString();
				if(ServerInfos.ContainsKey(name)){
					m_server = ServerInfos[name];
				}
			}catch{}
		}

		#endregion
	}
}
