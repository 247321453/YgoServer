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
using System.Linq;
using System.Windows.Forms;

using YGOCore;

namespace GameClient
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		#region ...
		DateTime? sendtime;
		Client Client;
		string SelectName;
		CreateRoomForm m_create;
		private readonly SortedList<string, PlayerInfo> Players=new SortedList<string, PlayerInfo>();
		private readonly byte[] _lvlock=new byte[0];
		public MainForm(Client client)
		{
			Client = client;
			InitializeComponent();
			this.Icon = res.favicon;
			m_create = new CreateRoomForm(Client);
			panel_rooms.SetClient(Client);
		}
		void MainFormLoad(object sender, EventArgs e)
		{
			this.Text = "游戏大厅 "+Client.Name+":"+Program.Config.DuelPort;
			Client.OnServerChat += new OnServerChatHandler(OnServerChat);
			Client.OnRoomClose+=new OnRoomCloseHandler(panel_rooms.OnClose);
			Client.OnRoomStart+=new OnRoomStartHandler(panel_rooms.OnStart);
			Client.OnRoomCreate+=new OnRoomCreateHandler(panel_rooms.OnCreate);
			Client.OnRoomList +=new OnRoomListHandler(panel_rooms.OnRoomList);
			Client.OnPlayerEnter+=new OnPlayerEnterEvent(m_login_Client_OnPlayerEnter);
			Client.OnPlayerLeave+=new OnPlayerLeaveEvent(m_login_Client_OnPlayerLeave);
			Client.OnGameExited+=new OnGameExitedEvent(m_login_Client_OnGameExited);
			Client.OnServerClose+=new OnServerCloseEvent(Client_OnServerClose);
			Client.OnPlayerList+=new OnPlayerListEvent(Client_OnPlayerList);
			Client.GetRooms(false, true);
			Client.GetPlayerList();
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
			if(msg==null) msg="";
//			msg = msg.Replace("\r", "");
//			msg = msg.Replace("\n", "\n\t ");
//			msg +="\n";
			BeginInvoke(new Action(()=>{
			                       	//	Color old= 	rb_allmsg.SelectionColor;
			                       	//	Font oldf= rb_allmsg.Font;
			                       	// 	FontStyle olds=	oldf.Style;
			                       	rb_allmsg.AppendText(time+" ");
			                       	// 	rb_allmsg.SelectionColor = Color.FromArgb(255,65,78,48);
			                       	// 	rb_allmsg.Font = new Font(oldf, FontStyle.Bold);
			                       	if(!string.IsNullOrEmpty(tname)){
			                       		rb_allmsg.AppendText(pname+" 私聊:");
			                       	}else{
			                       		rb_allmsg.AppendText(pname+" 说: ");
			                       	}
			                       	// 	rb_allmsg.Font = new Font(oldf, olds);
			                       	// 	rb_allmsg.Font = oldf;
			                       	//	rb_allmsg.SelectionColor = Color.FromArgb(255,65,188,48);
			                       	rb_allmsg.AppendText(msg+"\n");
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
			if(sendtime!=null && string.IsNullOrEmpty(toname)){
				DateTime now = DateTime.Now;
				if(((now.Ticks-sendtime.Value.Ticks)/10000/1000) < 10){
					MessageBox.Show("发送间隔10秒，私聊不受影响");
					return;
				}
				sendtime = now;
			}else{
				sendtime = DateTime.Now;
			}
			if(string.IsNullOrEmpty(toname)){
				SelectName = "";
			}
			if(!string.IsNullOrEmpty(SelectName)){
				rb_msg.Text = "@"+SelectName+" ";
			}else{
				rb_msg.Text = "";
			}
			SendMsg(toname, msg);
		}
		#endregion
		
		#region quick mode
		private void JoinRoom(string room,int port=0){
			if(port==0){
				port = Program.Config.DuelPort;
			}
			Client.JoinRoom(room, port, Program.Config.NeedAuth);
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
		#region player

		void Client_OnPlayerList(List<PlayerInfo> players)
		{
			lock(Players){
				Players.Clear();
				foreach(PlayerInfo p in players){
					if(!Players.ContainsKey(p.Name)){
						Players.Add(p.Name, p);
					}
				}
			}
			BeginInvoke(new Action(()=>{
			                       	RefreshPlayers();
			                       })
			           );
		}

		void m_login_Client_OnPlayerLeave(PlayerInfo player)
		{
			lock(Players){
				if(Players.ContainsKey(player.Name)){
					if(string.IsNullOrEmpty(player.Room.Name)){
						Players.Remove(player.Name);
						UpdatePlayer(player, true);
					}else{
						Players[player.Name].Room = null;
					}
				}
			}
		}

		void m_login_Client_OnPlayerEnter(PlayerInfo player)
		{
			lock(Players){
				if(Players.ContainsKey(player.Name)){
					Players[player.Name].Room = player.Room;
				}else{
					Players.Add(player.Name, player);
					BeginInvoke(new Action(()=>{
					                       	UpdatePlayer(player, false);
					                       })
					           );
				}
			}
		}
		#endregion
		void Btn_otherClick(object sender, EventArgs e)
		{
			m_create.ShowDialog();
		}
		
		void Btn_joinClick(object sender, EventArgs e)
		{
			string room = tb_join.Text;
			JoinRoom(room);
			tb_join.Text = "";
		}
		private void RefreshPlayers(){
			lock(_lvlock){
				lv_user.BeginUpdate();
				lv_user.Items.Clear();
			}
			ListViewItem[] items;
			lock(Players){
				items = new ListViewItem[Players.Count];
				for (int i = 0; i < Players.Count; i++)
				{
					PlayerInfo p = Players.Values[i];
					items[i] = new ListViewItem();
					items[i].Text = p.Name;
				}
			}
			lock(_lvlock){
				lv_user.Items.AddRange(items);
				lv_user.EndUpdate();
			}
		}
		private void UpdatePlayer(PlayerInfo player,bool delete){
			lock(_lvlock){
				if(delete){
					foreach(ListViewItem item in lv_user.Items){
						if(item.Text == player.Name){
							lv_user.Items.Remove(item);
							break;
						}
					}
				}else{
					ListViewItem item = new ListViewItem();
					item.Text = player.Name;
					lv_user.Items.Add(item);
				}
			}
		}
		
		void Menuitem_chatClick(object sender, EventArgs e)
		{
			lock(_lvlock){
				if (lv_user.SelectedItems.Count > 0)
				{
					string name = lv_user.SelectedItems[0].Text;
					SelectName = name;
					rb_msg.Text = "@"+SelectName+" ";
				}
			}
		}
		
		void Menuitem_joinClick(object sender, EventArgs e)
		{
			string name = null;
			lock(_lvlock){
				if (lv_user.SelectedItems.Count > 0)
				{
					name = lv_user.SelectedItems[0].Text;
				}
			}
			if(name != null){
				RoomInfo room =null;
				lock(Players){
					if(Players.ContainsKey(name)){
						room = Players[name].Room;
					}
				}
				if(room !=null){
					if(room.Name.Contains("$")){
						string pass = Password.GetPwd(room.Name);
						using(InputDialog input=new InputDialog("请输入密码", true)){
							if(input.ShowDialog()==DialogResult.OK){
								if(pass == input.InputText){
									JoinRoom(room.Name, room.Port);
								}else{
									MessageBox.Show("密码不正确");
								}
							}
						}
					}else{
						JoinRoom(room.Name);
					}
				}
			}
		}
	}
}
