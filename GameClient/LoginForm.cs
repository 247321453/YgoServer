/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/7
 * 时间: 20:34
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace GameClient
{
	/// <summary>
	/// Description of LoginForm.
	/// </summary>
	public partial class LoginForm : Form
	{
        const string USER_NAME = "username";

        MainForm m_main;
		public Client Client{get;private set;}
		public LoginForm()
		{
			Client = new Client();
			m_main = new MainForm(this, Client);

            Client.OnLoginSuccess += delegate {
				BeginInvoke(new Action(()=>{
                   
				                       	this.Hide();
				                       	m_main.Show();
				                       })
				           );
				
			};
			InitializeComponent();
			this.Icon = res.favicon;
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
            ConfigManager.Save(USER_NAME, username);
            if (!Client.Connect(Program.Config)){
				MessageBox.Show("无法连接服务器");
				return;
			}
            Client.Login(username, pwd);
		}
		#endregion
		
		void Btn_addaiClick(object sender, EventArgs e)
		{
			Thread thread = new Thread(new ThreadStart(GameUtil.AddAi));
			thread.IsBackground = true;
			thread.Start();
		}

        private void LoginForm_Load(object sender, EventArgs e)
        {
            string name = ConfigManager.readString(USER_NAME);
            tb_username.Text = name;
        }

        private void Btn_Game_Click(object sender, EventArgs e)
        {
            GameUtil.RunGame("");
        }

        private void Btn_Single_Click(object sender, EventArgs e)
        {
            GameUtil.RunGame("-s");
        }

        private void Btn_SetGamePath_Click(object sender, EventArgs e)
        {
            using(OpenFileDialog dlg=new OpenFileDialog())
            {
                dlg.Title = "选择游戏主程序";
                dlg.InitialDirectory = ".";
                dlg.Filter = "游戏王|ygo*.exe|执行程序(*.exe)|*.exe";
                if(dlg.ShowDialog() == DialogResult.OK)
                {
                    ConfigManager.Save("game", dlg.FileName);
                }
            }
        }

        public void Logout()
        {

        }
    }
}
