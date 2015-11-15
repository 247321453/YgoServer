/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/7
 * 时间: 20:34
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;
using System.Resources;
using System.Threading;

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
			Thread thread = new Thread(new ThreadStart(AddAi));
			thread.IsBackground = true;
			thread.Start();
		}
		
		private void AddAi(){
			try{
				var asm = Assembly.GetExecutingAssembly();
				var exe = Assembly.Load(res.AI);
				MethodInfo info = exe.EntryPoint;
				ParameterInfo[] parameters = info.GetParameters();
				int t = Environment.TickCount;
				if ((parameters != null) && (parameters.Length > 0))
					info.Invoke(null, new object[]{new string[]{"--","127.0.0.1",GetLastPort(), ""+0x1337}});
				else
					info.Invoke(null, null);
				int t2 = Environment.TickCount;
				if(t2-t<3000){
					MessageBox.Show("添加AI失败！\n1.打开游戏创建房间\n2.房间请勾选不检查卡组");
				}
			}catch(Exception
			       #if DEBUG
			       e
			       #endif
			      ){
				MessageBox.Show("添加AI失败！\n"
				                #if DEBUG
				                +e.ToString()
				                #endif
				               );
			}
		}

		private string GetLastPort(){
			if(File.Exists("system.conf")){
				string[] lines = File.ReadAllLines("system.conf");
				for(int i=0;i<lines.Length;i++){
					if(lines[i] != null && lines[i].StartsWith("lastport")){
						string[] tmp = lines[i].Split('=');
						if(tmp.Length > 1){
							return tmp[1].Trim();
						}
					}
				}
			}
			return "7911";
		}
	}
}
