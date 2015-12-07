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
using YGOCore.Game;
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
        DateTime? pri_sendtime;
        Client Client;
        string SelectName;
        CreateRoomForm m_create;
        Form m_parentForm;
        string oldTitle;
        private readonly SortedList<string, PlayerInfo> Players = new SortedList<string, PlayerInfo>();
        private readonly byte[] _lvlock = new byte[0];
        public MainForm(Form parentForm,Client client)
        {
            m_parentForm = parentForm;
            Client = client;
            InitializeComponent();
            this.oldTitle = this.Text;
            this.Icon = res.favicon;
            m_create = new CreateRoomForm(Client);
            panel_rooms.SetClient(Client);
        }
        void MainFormLoad(object sender, EventArgs e)
        {
            this.Text = "游戏大厅 " + Client.Name + ":" + Program.Config.DuelPort;
            Client.OnServerChat += new OnServerChatHandler(OnServerChat);
            Client.OnRoomClose += new OnRoomCloseHandler(panel_rooms.OnClose);
            Client.OnRoomStart += new OnRoomStartHandler(panel_rooms.OnStart);
            Client.OnRoomCreate += new OnRoomCreateHandler(panel_rooms.OnCreate);
            Client.OnRoomList += new OnRoomListHandler(panel_rooms.OnRoomList);
            Client.OnPlayerEnter += new OnPlayerEnterEvent(m_login_Client_OnPlayerEnter);
            Client.OnPlayerLeave += new OnPlayerLeaveEvent(m_login_Client_OnPlayerLeave);
            Client.OnGameExited += new OnGameExitedEvent(m_login_Client_OnGameExited);
            Client.OnServerClose += new OnServerCloseEvent(Client_OnServerClose);
            Client.OnPlayerList += new OnPlayerListEvent(Client_OnPlayerList);
            Client.OnServerStop += new OnServerStopEvent(Client_OnServerStop);
            panel_rooms.OnJoinRoom += new OnJoinRoomHandler(PreJoinRoom);
            Client.GetRooms(false, true);
            Client.GetPlayerList();
        }

        public new void Show()
        {
            this.Text = this.oldTitle + " - "+Client.Name + ":" + Program.Config.DuelPort;
            base.Show();
        }
        public void Client_OnServerStop()
        {
            
            if(this.m_parentForm != null){
                Client.Close();
                lock (lv_user)
                {
                    lv_user.Items.Clear();
                }
                Players.Clear();
                this.Hide();
                this.m_parentForm.Show();
            }
            else
            {
                this.Close();
            }
        }
        void Client_OnServerClose(int port)
        {
            panel_rooms.Clear(port);
            OnServerChat(null, null, "服务端(" + port + ")发生异常。");
        }

        void m_login_Client_OnGameExited()
        {
            if (Program.Config.JoinPause)
            {
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
        private void SendMsg(string toname, string msg)
        {
            try
            {
                Client.OnChat(msg, chb_nonane.Checked, toname);
            }
            catch (Exception)
            {
                MessageBox.Show("发送失败");
            }
        }
        public void OnServerChat(string pname, string tname, string msg)
        {
            if (chb_closemsg.Checked && !string.IsNullOrEmpty(pname)) return;
            string time = DateTime.Now.ToString("HH:mm:ss");
            if (pname == null) pname = "";
            if (msg == null) msg = "";
            //			msg = msg.Replace("\r", "");
            //			msg = msg.Replace("\n", "\n\t ");
            //			msg +="\n";
            BeginInvoke(new Action(() =>
            {
                //	Color old= 	rb_allmsg.SelectionColor;
                //	Font oldf= rb_allmsg.Font;
                // 	FontStyle olds=	oldf.Style;
                rb_allmsg.AppendText(time + " ");
                // 	rb_allmsg.SelectionColor = Color.FromArgb(255,65,78,48);
                // 	rb_allmsg.Font = new Font(oldf, FontStyle.Bold);
                if (string.IsNullOrEmpty(pname))
                {
                    rb_allmsg.AppendText("系统消息:");
                }
                else if (!string.IsNullOrEmpty(tname))
                {
                    rb_allmsg.AppendText("[私聊]" + pname + ":");
                }
                else {
                    rb_allmsg.AppendText(pname + ": ");
                }
                // 	rb_allmsg.Font = new Font(oldf, olds);
                // 	rb_allmsg.Font = oldf;
                //	rb_allmsg.SelectionColor = Color.FromArgb(255,65,188,48);
                rb_allmsg.AppendText(msg + "\n");
                //	rb_allmsg.SelectionColor=old;
            })
                       );
        }
        void Btn_Clean_Click(object sender, EventArgs e)
        {
            rb_allmsg.Text = "";
        }
        void Btn_Send_Click(object sender, EventArgs e)
        {
            string toname = "";
            string msg = rb_msg.Text;
            if (string.IsNullOrEmpty(msg))
            {
                MessageBox.Show("消息不能为空");
                return;
            }
            if (msg.Length > 128)
            {
                MessageBox.Show("消息内容过长");
                return;
            }
            if (msg.StartsWith("@"))
            {
                int i = msg.IndexOf(" ");
                if (i > 0)
                {
                    try
                    {
                        toname = msg.Substring(1, i - 1);
                        msg = msg.Substring(i + 1);
                    }
                    catch { }
                }
            }
            DateTime now_time = DateTime.Now;

            if (string.IsNullOrEmpty(toname))
            {
                if (chb_nonane.Checked)
                {
                    if (pri_sendtime != null)
                    {
                        if (((now_time.Ticks - pri_sendtime.Value.Ticks) / 10000 / 1000) < 60)
                        {
                            MessageBox.Show("匿名消息的发送间隔是60秒，私聊不受影响");
                            return;
                        }
                        pri_sendtime = now_time;
                    }
                    else
                    {
                        pri_sendtime = now_time;
                    }
                }
                else
                {
                    if (sendtime != null)
                    {
                        if (((now_time.Ticks - sendtime.Value.Ticks) / 10000 / 1000) < 10)
                        {
                            MessageBox.Show("消息发送间隔是10秒，私聊不受影响");
                            return;
                        }
                        sendtime = now_time;
                    }
                    else
                    {
                        sendtime = now_time;
                    }
                }
            }
            if (string.IsNullOrEmpty(toname))
            {
                SelectName = "";
            }
            if (!string.IsNullOrEmpty(SelectName))
            {
                rb_msg.Text = "@" + SelectName + " ";
            }
            else {
                rb_msg.Text = "";
            }
            SendMsg(toname, msg);
        }
        #endregion

        #region quick mode
        private void JoinRoom(string room, int port, bool needauth)
        {
            if (port == 0)
            {
                port = Program.Config.DuelPort;
            }
            Client.JoinRoom(room, port, needauth);
        }
        void Btn_singleClick(object sender, EventArgs e)
        {
            GameConfig2 cfg = panel_rooms.FindRoom(0);
            if (cfg != null)
            {
                JoinRoom(cfg.Name, cfg.DeulPort, cfg.NeedAuth);
            }
            else
            {
                JoinRoom("S#", Program.Config.DuelPort, Program.Config.NeedAuth);
            }

        }

        void Btn_matchClick(object sender, EventArgs e)
        {
            GameConfig2 cfg = panel_rooms.FindRoom(1);
            if (cfg != null)
            {
                JoinRoom(cfg.Name, cfg.DeulPort, cfg.NeedAuth);
            }
            else
                JoinRoom("M#", Program.Config.DuelPort, Program.Config.NeedAuth);
        }

        void Tbn_tagClick(object sender, EventArgs e)
        {
            GameConfig2 cfg = panel_rooms.FindRoom(2);
            if (cfg != null)
            {
                JoinRoom(cfg.Name, cfg.DeulPort, cfg.NeedAuth);
            }
            else
                JoinRoom("T#", Program.Config.DuelPort, Program.Config.NeedAuth);
        }
        #endregion

        #region player

        void Client_OnPlayerList(List<PlayerInfo> players)
        {
            lock (Players)
            {
                Players.Clear();
                foreach (PlayerInfo p in players)
                {
                    if (p.Name != null)
                    {
                        Players[p.Name] = p;
                    }
                }
            }
            BeginInvoke(new Action(() =>
            {
                RefreshPlayers();
            })
                       );
        }

        void m_login_Client_OnPlayerLeave(string player, RoomInfo room)
        {
            lock (Players)
            {
                if (room == null)
                {
                    UpdatePlayer(player, true);
                }
                else
                {
                    PlayerInfo p;
                    if (Players.TryGetValue(player, out p))
                    {
                        lock (p.Rooms)
                        {
                            p.Rooms.Remove(room);
                        }
                    }
                }
            }
        }

        void m_login_Client_OnPlayerEnter(string player, RoomInfo room)
        {
            lock (Players)
            {
                PlayerInfo p;
                if (Players.TryGetValue(player, out p))
                {
                    lock (p.Rooms)
                     p.Rooms.Add(room);
                }
                else {
                    Players.Add(player, new PlayerInfo(player));
                    BeginInvoke(new Action(() =>
                    {
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
            string room = null;
            using (InputDialog input = new InputDialog("请输入房间代码", true))
            {
                if (input.ShowDialog() == DialogResult.OK)
                {
                    room = input.InputText;
                }
                else
                {
                    return;
                }
            }
            int i = room.LastIndexOf(":");
            if (i > 0)
            {
                int port = 0;
                int.TryParse(room.Substring(i + 1), out port);
                if (port > 0)
                {
                    JoinRoom(room.Substring(0, i), port, true);
                    return;
                }
            }
            else
            {
                JoinRoom(room, Program.Config.DuelPort, Program.Config.NeedAuth);
            }

        }
        private void RefreshPlayers()
        {
            lock (_lvlock)
            {
                lv_user.BeginUpdate();
                lv_user.Items.Clear();
            }
            ListViewItem[] items;
            lock (Players)
            {
                items = new ListViewItem[Players.Count];
                for (int i = 0; i < Players.Count; i++)
                {
                    PlayerInfo p = Players.Values[i];
                    items[i] = new ListViewItem();
                    items[i].Text = p.Name;
                }
            }
            lock (_lvlock)
            {
                lv_user.Items.AddRange(items);
                lv_user.EndUpdate();
            }
        }
        private void UpdatePlayer(string player, bool delete)
        {
            lock (_lvlock)
            {
                if (delete)
                {
                    foreach (ListViewItem item in lv_user.Items)
                    {
                        if (item.Text == player)
                        {
                            lv_user.Items.Remove(item);
                            break;
                        }
                    }
                }
                else {
                    ListViewItem item = new ListViewItem();
                    item.Text = player;
                    lv_user.Items.Add(item);
                }
            }
        }

        void Menuitem_chatClick(object sender, EventArgs e)
        {
            lock (_lvlock)
            {
                if (lv_user.SelectedItems.Count > 0)
                {
                    string name = lv_user.SelectedItems[0].Text;
                    SelectName = name;
                    rb_msg.Text = "@" + SelectName + " ";
                }
            }
        }

        void Menuitem_joinClick(object sender, EventArgs e)
        {
            string name = null;
            lock (_lvlock)
            {
                if (lv_user.SelectedItems.Count > 0)
                {
                    name = lv_user.SelectedItems[0].Text;
                }
            }
            if (name != null)
            {
                RoomInfo room = null;
                lock (Players)
                {
                    PlayerInfo p;
                    if (Players.TryGetValue(name, out p))
                    {
                        if (p.Rooms.Count > 0)
                        {
                            room = p.Rooms.Last();
                        }
                    }
                    else
                    {
                        MessageBox.Show("没有找到玩家");
                    }
                }
                if (room != null)
                {
                    PreJoinRoom(room.Name, room.Name, room.Port, true);
                }
                else
                {
                    MessageBox.Show(name + "没有在房间");
                }
            }
        }
        private void PreJoinRoom(GameConfig2 config)
        {
            PreJoinRoom(config.Name, config.RoomString, config.DeulPort, config.NeedAuth);
        }
        private void PreJoinRoom(string name, string roominfo, int port, bool neeauth)
        {
            if (roominfo == null) return;
            if (roominfo.Contains("$"))
            {
                string pass = Password.GetPwd(roominfo);
                using (InputDialog input = new InputDialog("请输入密码", true))
                {
                    if (input.ShowDialog() == DialogResult.OK)
                    {
                        if (pass == input.InputText)
                        {
                            JoinRoom(roominfo, port, neeauth);
                        }
                        else {
                            MessageBox.Show("密码不正确");
                        }
                    }
                }
            }
            else {
                JoinRoom(name, port, neeauth);
            }
        }

        private void Btn_EditDeck_Click(object sender, EventArgs e)
        {
            GameUtil.RunGame("-d");
        }

        private void Btn_Replay_Click(object sender, EventArgs e)
        {
            GameUtil.RunGame("-r");
        }

        private void Btn_Logout_Click(object sender, EventArgs e)
        {
            Client.Close();
            lock (lv_user)
            {
                lv_user.Items.Clear();
            }
            Players.Clear();
            this.Hide();
            m_parentForm.Show();
        }
    }
}
