/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/9/3
 * 时间: 19:26
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using YGOCore;
using System.Collections.Generic;
using System.Xml;
using System.Net;
using YGOCore.Net;
using OcgWrapper.Enums;
using System.Net.Sockets;
using YGOCore.Game;

namespace YGOClient
{
	public class ServerInfo{
		public ServerInfo(string val){
			string[] vs= val.Split(':');
			if(val.Length >= 3){
				Name = vs[0];
				Host = vs[1];
				try{
					Port = int.Parse(vs[2]);
				}catch{Port = 0;}
			}else{
				Port = 0;
			}
		}
		public bool isOk{get{return Port > 0;}}
		public string Name{get;private set;}
		public string Host{get;private set;}
		public int Port{get;private set;}
	}
	/// <summary>
	/// Description of UserForm.
	/// </summary>
	public partial class UserForm : Form
	{
		/// <summary>
		/// 必须输入返回一个用户，否则关闭就是退出程序
		/// </summary>
		private bool NeedUser=false;
		User m_User;
		AsyncClient client;
		private string m_dir;
		bool ClickOK=false;
		bool ShortPwd= false;
		public readonly Dictionary<string, ServerInfo> Servers=new Dictionary<string, ServerInfo>();

		public UserForm(bool needuser,bool shortpwd)
		{
			InitializeComponent();
			this.NeedUser=needuser;
			this.ShortPwd = shortpwd;
			m_dir=User.DIR;
			loadUsers();
			int i = 0;
			for(i=1;i<99;i++){
				string val = ConfigManager.readString("server"+i.ToString("00"));
				if(string.IsNullOrEmpty(val))
					break;
				ServerInfo s = new ServerInfo(val);
				if(s.isOk){
					Servers.Add(s.Name, s);
				}
			}
			string last = ConfigManager.readString("server");
			i = 0;
			int index = 0;
			foreach(string str in Servers.Keys){
				if(last == str){
					index = 0;
				}
				i++;
				cb_server.Items.Add(str);
			}
			if(cb_server.Items.Count>0){
				cb_server.SelectedIndex = index;
			}
		}
		
		public void Connect(ServerInfo info, User user){
			if(client!=null){
				client.Close();
			}
			client = new AsyncClient();
			try{
				client.Connect(info.Host, info.Port);
				using(GameServerPacket login = new GameServerPacket()){
					login.WriteUnicode(user.Name, 20);
					login.WriteUnicode(user.Password, 32);
					login.Use();
					client.BeginSend(login.Content);
				}
			}catch(Exception){
				
			}
		}
		#region user
		public User GetUser(){
			if(string.IsNullOrEmpty(cb_username.Text)){
				return null;
			}
			m_User=new User(cb_username.Text, tb_password.Text);
			m_User.setGamePath(tb_gamepath.Text);
			m_User.setGameArgs(tb_args.Text);
			m_User.setRecord(chkb_record.Checked);
			return m_User;
		}
		
		void loadUsers(){
			cb_username.Items.Clear();
			if(Directory.Exists(m_dir)){
				DirectoryInfo dir=new DirectoryInfo(m_dir);
				FileInfo[] files=dir.GetFiles("*"+User.EX);
				foreach(FileInfo info in files){
					string name=info.Name;
					name=name.Substring(0, name.Length-5);
					cb_username.Items.Add(name);
				}
			}
			if(cb_username.Items.Count>0){
				string name=ConfigManager.readString(User.TAG);
				cb_username.SelectedIndex=0;
				int index=0;
				foreach(object o in cb_username.Items){
					if(o.ToString() == name){
						cb_username.SelectedIndex = index;
						break;
					}
					index++;
				}
			}else{
				cb_username.Text="";
				tb_password.Text="";
				tb_args.Text="";
				chkb_record.Checked= false;
				string file= Tool.Combine(Application.StartupPath, "ygopro.exe");
				if(File.Exists(file)){
					tb_gamepath.Text=file;
				}else{
					file= Tool.Combine(Application.StartupPath, "ygopro_vs.exe");
					if(File.Exists(file)){
						tb_gamepath.Text=file;
					}
				}
			}
		}
		#endregion
		
		#region func
		void UserFormLoad(object sender, EventArgs e)
		{

		}
		void UserFormFormClosing(object sender, FormClosingEventArgs e)
		{
			if(NeedUser){
				if(GetUser()==null){
					Application.Exit();
				}else{
					if(!ClickOK){
						Application.Exit();
					}
				}
			}
		}
		
		void Btn_gameClick(object sender, EventArgs e)
		{
			using (OpenFileDialog dlg = new OpenFileDialog())
			{
				dlg.Title = "请选择ygopro.exe或者ygopro_vs.exe";
				dlg.Filter = "游戏王|ygopro*.exe|可执行文件|*.exe|所有文件(*.*)|*.*";
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					tb_gamepath.Text = dlg.FileName;
				}
			}
		}
		
		void Btn_deleteClick(object sender, EventArgs e)
		{
			if(!string.IsNullOrEmpty(cb_username.Text)){
				if(MessageBox.Show("是否删除 "+cb_username.Text, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning)!=DialogResult.OK){
					return;
				}
				User.Delete(cb_username.Text);
				loadUsers();
			}
		}
		
		#endregion
		
		#region ok
		void Btn_okClick(object sender, EventArgs e)
		{
			ClickOK=true;
			ConfigManager.Save(User.TAG, cb_username.Text);
		}
		
		void Cb_usernameSelectedIndexChanged(object sender, EventArgs e)
		{
			try{
				int index=cb_username.SelectedIndex;
				string name=(index>=0)?cb_username.Items[index].ToString():"";
				User user=User.Load(name);
				if(user!=null){
					cb_username.Text=""+user.Name;
					tb_password.Text=""+user.Password;
					tb_gamepath.Text=""+user.GamePath;
					tb_args.Text=""+user.GameArgs;
					chkb_record.Checked=user.RePassword;
					user.ShortPwd = ShortPwd;
				}else{
					//MessageBox.Show("加载失败");
				}
			}catch(Exception){}
			ChangedOk();
		}
		
		void Cb_usernameTextChanged(object sender, EventArgs e)
		{
			ChangedOk();
		}
		
		void Tb_gamepathTextChanged(object sender, EventArgs e)
		{
			ChangedOk();
		}
		void ChangedOk(){
			if(string.IsNullOrEmpty(cb_username.Text)){
				btn_ok.Enabled=false;
			}else{
				if(File.Exists(tb_gamepath.Text)){
					btn_ok.Enabled=true;
				}else{
					btn_ok.Enabled=false;
				}
			}
		}
		#endregion

	}
}
