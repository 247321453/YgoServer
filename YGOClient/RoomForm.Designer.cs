/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/9/3
 * 时间: 17:10
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
namespace YGOClient
{
	partial class RoomForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoomForm));
			this.cb_server = new System.Windows.Forms.ComboBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.chk_wait = new System.Windows.Forms.CheckBox();
			this.chk_nopass = new System.Windows.Forms.CheckBox();
			this.btn_replay = new System.Windows.Forms.Button();
			this.btn_deck = new System.Windows.Forms.Button();
			this.cb_auth = new System.Windows.Forms.CheckBox();
			this.btn_reg = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.btn_add = new System.Windows.Forms.Button();
			this.btn_refresh = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.fp_rooms = new System.Windows.Forms.RoomGrid();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// cb_server
			// 
			this.cb_server.FormattingEnabled = true;
			this.cb_server.Location = new System.Drawing.Point(7, 19);
			this.cb_server.Name = "cb_server";
			this.cb_server.Size = new System.Drawing.Size(145, 20);
			this.cb_server.TabIndex = 2;
			this.cb_server.SelectedIndexChanged += new System.EventHandler(this.Cb_serverSelectedIndexChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.chk_wait);
			this.groupBox1.Controls.Add(this.chk_nopass);
			this.groupBox1.Controls.Add(this.btn_replay);
			this.groupBox1.Controls.Add(this.btn_deck);
			this.groupBox1.Controls.Add(this.cb_auth);
			this.groupBox1.Controls.Add(this.btn_reg);
			this.groupBox1.Controls.Add(this.button1);
			this.groupBox1.Controls.Add(this.btn_add);
			this.groupBox1.Controls.Add(this.btn_refresh);
			this.groupBox1.Controls.Add(this.cb_server);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(868, 66);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "服务器信息";
			// 
			// chk_wait
			// 
			this.chk_wait.AutoSize = true;
			this.chk_wait.Checked = true;
			this.chk_wait.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chk_wait.Location = new System.Drawing.Point(778, 43);
			this.chk_wait.Name = "chk_wait";
			this.chk_wait.Size = new System.Drawing.Size(60, 16);
			this.chk_wait.TabIndex = 10;
			this.chk_wait.Text = "等待中";
			this.chk_wait.UseVisualStyleBackColor = true;
			// 
			// chk_nopass
			// 
			this.chk_nopass.AutoSize = true;
			this.chk_nopass.Location = new System.Drawing.Point(778, 21);
			this.chk_nopass.Name = "chk_nopass";
			this.chk_nopass.Size = new System.Drawing.Size(60, 16);
			this.chk_nopass.TabIndex = 10;
			this.chk_nopass.Text = "无密码";
			this.chk_nopass.UseVisualStyleBackColor = true;
			// 
			// btn_replay
			// 
			this.btn_replay.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btn_replay.Location = new System.Drawing.Point(670, 19);
			this.btn_replay.Name = "btn_replay";
			this.btn_replay.Size = new System.Drawing.Size(92, 40);
			this.btn_replay.TabIndex = 9;
			this.btn_replay.Text = "观看录像";
			this.btn_replay.UseVisualStyleBackColor = true;
			this.btn_replay.Click += new System.EventHandler(this.Btn_replayClick);
			// 
			// btn_deck
			// 
			this.btn_deck.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btn_deck.Location = new System.Drawing.Point(566, 19);
			this.btn_deck.Name = "btn_deck";
			this.btn_deck.Size = new System.Drawing.Size(90, 40);
			this.btn_deck.TabIndex = 9;
			this.btn_deck.Text = "卡组编辑";
			this.btn_deck.UseVisualStyleBackColor = true;
			this.btn_deck.Click += new System.EventHandler(this.Btn_deckClick);
			// 
			// cb_auth
			// 
			this.cb_auth.AutoSize = true;
			this.cb_auth.Enabled = false;
			this.cb_auth.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.cb_auth.Location = new System.Drawing.Point(7, 44);
			this.cb_auth.Name = "cb_auth";
			this.cb_auth.Size = new System.Drawing.Size(94, 16);
			this.cb_auth.TabIndex = 7;
			this.cb_auth.Text = "需要帐号密码";
			this.cb_auth.UseVisualStyleBackColor = true;
			// 
			// btn_reg
			// 
			this.btn_reg.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btn_reg.Location = new System.Drawing.Point(159, 19);
			this.btn_reg.Name = "btn_reg";
			this.btn_reg.Size = new System.Drawing.Size(90, 40);
			this.btn_reg.TabIndex = 8;
			this.btn_reg.Text = "注册";
			this.btn_reg.UseVisualStyleBackColor = true;
			this.btn_reg.Click += new System.EventHandler(this.Btn_regClick);
			// 
			// button1
			// 
			this.button1.BackColor = System.Drawing.SystemColors.Control;
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button1.ForeColor = System.Drawing.Color.RoyalBlue;
			this.button1.Location = new System.Drawing.Point(463, 19);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(90, 40);
			this.button1.TabIndex = 2;
			this.button1.Text = "创建房间";
			this.button1.UseVisualStyleBackColor = false;
			this.button1.Click += new System.EventHandler(this.Btn_createClick);
			// 
			// btn_add
			// 
			this.btn_add.BackColor = System.Drawing.SystemColors.Control;
			this.btn_add.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btn_add.ForeColor = System.Drawing.Color.DarkViolet;
			this.btn_add.Location = new System.Drawing.Point(362, 19);
			this.btn_add.Name = "btn_add";
			this.btn_add.Size = new System.Drawing.Size(90, 40);
			this.btn_add.TabIndex = 1;
			this.btn_add.Text = "快速加入(F6)";
			this.btn_add.UseVisualStyleBackColor = false;
			this.btn_add.Click += new System.EventHandler(this.Btn_joinClick);
			// 
			// btn_refresh
			// 
			this.btn_refresh.BackColor = System.Drawing.SystemColors.Control;
			this.btn_refresh.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btn_refresh.ForeColor = System.Drawing.Color.ForestGreen;
			this.btn_refresh.Location = new System.Drawing.Point(261, 19);
			this.btn_refresh.Name = "btn_refresh";
			this.btn_refresh.Size = new System.Drawing.Size(90, 40);
			this.btn_refresh.TabIndex = 0;
			this.btn_refresh.Text = "刷新(F5)";
			this.btn_refresh.UseVisualStyleBackColor = false;
			this.btn_refresh.Click += new System.EventHandler(this.Btn_refreshClick);
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.fp_rooms);
			this.groupBox2.Location = new System.Drawing.Point(8, 81);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(868, 478);
			this.groupBox2.TabIndex = 4;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "房间列表";
			// 
			// fp_rooms
			// 
			this.fp_rooms.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.fp_rooms.AutoScroll = true;
			this.fp_rooms.BackColor = System.Drawing.SystemColors.Control;
			this.fp_rooms.Location = new System.Drawing.Point(6, 20);
			this.fp_rooms.Name = "fp_rooms";
			this.fp_rooms.Size = new System.Drawing.Size(854, 448);
			this.fp_rooms.TabIndex = 0;
			// 
			// RoomForm
			// 
			this.AcceptButton = this.btn_refresh;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(888, 564);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.DoubleBuffered = true;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "RoomForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "房间列表";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RoomFormFormClosing);
			this.Load += new System.EventHandler(this.RoomFormLoad);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.CheckBox chk_nopass;
		private System.Windows.Forms.CheckBox chk_wait;
		private System.Windows.Forms.Button btn_replay;
		private System.Windows.Forms.Button btn_deck;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button btn_add;
		private System.Windows.Forms.Button btn_reg;
		private System.Windows.Forms.RoomGrid fp_rooms;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox cb_auth;
		private System.Windows.Forms.Button btn_refresh;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox cb_server;
	}
}
