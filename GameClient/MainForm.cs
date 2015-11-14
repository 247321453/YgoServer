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
		Client Client;
		CreateRoomForm m_create;
		public MainForm(Client client)
		{
			Client = client;
			InitializeComponent();
			m_create = new CreateRoomForm(Client);
			panel_rooms.SetClient(Client);
		}
		void MainFormLoad(object sender, EventArgs e)
		{
			Client.OnServerChat += new OnServerChatHandler(OnServerChat);
			Client.OnRoomClose+=new OnRoomCloseHandler(panel_rooms.OnClose);
			Client.OnRoomStart+=new OnRoomStartHandler(panel_rooms.OnStart);
			Client.OnRoomCreate+=new OnRoomCreateHandler(panel_rooms.OnCreate);
			Client.OnRoomList +=new OnRoomListHandler(panel_rooms.OnRoomList);
			Client.OnPlayerEnter+=new OnPlayerEnterEvent(m_login_Client_OnPlayerEnter);
			Client.OnPlayerLeave+=new OnPlayerLeaveEvent(m_login_Client_OnPlayerLeave);
			Client.OnGameExited+=new OnGameExitedEvent(m_login_Client_OnGameExited);
			Client.OnServerClose+=new OnServerCloseEvent(Client_OnServerClose);
			Client.GetRooms(false, true);
		}

		void Client_OnServerClose(int port)
		{
			panel_rooms.Clear(port);
		}

		void m_login_Client_OnGameExited()
		{
			if(Program.Config.JoinPause){
				panel_rooms.ClearRooms();
			}
		}

		void m_login_Client_OnPlayerLeave(int port, string name, string room)
		{
			
		}

		void m_login_Client_OnPlayerEnter(int port, string name, string room)
		{
			
		}
		
		void MainFormFormClosed(object sender, FormClosedEventArgs e)
		{
			Client.Close();
			System.Diagnostics.Process.GetCurrentProcess().Kill();
			//	m_login.Close();
		}
		#endregion

		#region chat
		private void SendMsg(string toname, string msg){
			try{
				Client.OnChat(msg, chb_nonane.Checked, toname);
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
		private void JoinRoom(string room){
			Client.JoinRoom(room, Program.Config.DuelPort, Program.Config.NeedAuth);
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
