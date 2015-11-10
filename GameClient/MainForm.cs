/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/7
 * 时间: 20:31
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GameClient
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		#region ...
		LoginForm m_login;
		CreateRoomForm m_create;
		public MainForm(LoginForm parent)
		{
			InitializeComponent();
			m_login = parent;
			m_create = new CreateRoomForm(this);
			panel_rooms.SetParent(this);
		}
		void MainFormLoad(object sender, EventArgs e)
		{
			m_login.Client.OnServerChat += new OnServerChatHandler(OnServerChat);
			m_login.Client.OnRoomClose+=new OnRoomCloseHandler(panel_rooms.OnClose);
			m_login.Client.OnRoomStart+=new OnRoomStartHandler(panel_rooms.OnStart);
			m_login.Client.OnRoomCreate+=new OnRoomCreateHandler(panel_rooms.OnCreate);
			m_login.Client.OnRoomList +=new OnRoomListHandler(panel_rooms.OnRoomList);
			m_login.Client.GetRooms();
		}
		
		void MainFormFormClosed(object sender, FormClosedEventArgs e)
		{
			m_login.Client.Close();
			m_login.Close();
		}
		#endregion

		#region chat
		private void SendMsg(string toname, string msg){
			try{
				m_login.Client.OnChat(msg, chb_nonane.Checked, toname);
			}catch(Exception){
				MessageBox.Show("发送失败");
			}
		}
		public void OnServerChat(string pname, string tname, string msg){
			if(chb_closemsg.Checked) return;
			string time = DateTime.Now.ToString("HH:mm:ss");
			if(pname==null)pname = "";
			if(msg==null) return;
			msg = msg.Replace("\r", "");
			msg = msg.Replace("\n", "\n\t ");
			msg +="\n";
			BeginInvoke(new Action(()=>{
			                       	//	Color old= 	rb_allmsg.SelectionColor;
			                       	//	Font oldf= rb_allmsg.Font;
			                       	// 	FontStyle olds=	oldf.Style;
			                       	rb_allmsg.AppendText(time+" ");
			                       	// 	rb_allmsg.SelectionColor = Color.FromArgb(255,65,78,48);
			                       	// 	rb_allmsg.Font = new Font(oldf, FontStyle.Bold);
			                       	rb_allmsg.AppendText(pname);
			                       	if(!string.IsNullOrEmpty(tname)){
			                       		rb_allmsg.AppendText(":@"+tname+" ");
			                       	}else{
			                       		rb_allmsg.AppendText(":");
			                       	}
			                       	// 	rb_allmsg.Font = new Font(oldf, olds);
			                       	// 	rb_allmsg.Font = oldf;
			                       	//	rb_allmsg.SelectionColor = Color.FromArgb(255,65,188,48);
			                       	rb_allmsg.AppendText(msg);
			                       	//	rb_allmsg.SelectionColor=old;
			                       })
			           );
		}
		void Btn_Clean_Click(object sender, EventArgs e)
		{
			rb_allmsg.Text="";
		}
		void Btn_Send_Click(object sender, EventArgs e)
		{
			string toname="";
			string msg = rb_msg.Text;
			if(string.IsNullOrEmpty(msg)){
				MessageBox.Show("消息不能为空");
				return;
			}
			if(msg.Length > 128){
				MessageBox.Show("消息内容过长");
				return;
			}
			if(msg.StartsWith("@")){
				int i = msg.IndexOf(" ");
				if(i > 0){
					try{
						toname = msg.Substring(1, i-1);
						msg = msg.Substring(i+1);
					}catch{}
				}
			}
			rb_msg.Text = "";
			SendMsg(toname, msg);
		}
		#endregion
		
		#region quick mode
		public void JoinRoom(string room){
			if(m_login.Client.GameServerInfo==null){
				MessageBox.Show("服务器信息错误");
				return;
			}
			string namepwd =m_login.Client.Name;
			if(m_login.Client.GameServerInfo.NeedAuth){
				if(string.IsNullOrEmpty(m_login.Client.GameServerInfo.Token)){
					namepwd += "$"+m_login.Client.Pwd;
				}else{
					namepwd += "$"+m_login.Client.GameServerInfo.Token;
				}
			}
			GameUtil.JoinRoom(m_login.Client.GameServerInfo.Host, ""+m_login.Client.GameServerInfo.Port, namepwd, room);
		}
		void Btn_singleClick(object sender, EventArgs e)
		{
			JoinRoom("S#");
		}
		
		void Btn_matchClick(object sender, EventArgs e)
		{
			JoinRoom("M#");
		}
		
		void Tbn_tagClick(object sender, EventArgs e)
		{
			JoinRoom("T#");
		}
		#endregion
		
		void Btn_otherClick(object sender, EventArgs e)
		{
			m_create.ShowDialog();
		}
	}
}
