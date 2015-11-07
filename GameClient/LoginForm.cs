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
		MainForm m_main;
		public Client m_client{get;private set;}
		private ServerInfo m_server;
		readonly SortedList<string, ServerInfo> ServerInfos = new SortedList<string, ServerInfo>();
		public LoginForm()
		{
			m_main = new MainForm(this);
			m_client = new Client();
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
		
		void Button1Click(object sender, EventArgs e)
		{
			string username = tb_username.Text;
			string pwd = Tool.GetMd5(tb_password.Text);
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
			if(!m_client.Connect(m_server)){
				MessageBox.Show("无法连接服务器");
				return;
			}
			if(m_client.OnLogin(username, pwd)){
				//登陆成功
				m_main.Show();
				this.Hide();
			}else{
				MessageBox.Show("登陆失败");
			}
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
	}
}
