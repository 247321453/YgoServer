/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/7
 * 时间: 20:31
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
namespace GameClient
{
	partial class MainForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.btn_single = new System.Windows.Forms.Button();
			this.btn_match = new System.Windows.Forms.Button();
			this.tbn_tag = new System.Windows.Forms.Button();
			this.panel_rooms = new System.Windows.Forms.RoomGrid();
			this.rb_allmsg = new System.Windows.Forms.RichTextBox();
			this.rb_msg = new System.Windows.Forms.RichTextBox();
			this.btn_send = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btn_other = new System.Windows.Forms.Button();
			this.btn_clean = new System.Windows.Forms.Button();
			this.chb_closemsg = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.chb_nonane = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btn_single
			// 
			this.btn_single.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btn_single.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btn_single.Location = new System.Drawing.Point(6, 20);
			this.btn_single.Name = "btn_single";
			this.btn_single.Size = new System.Drawing.Size(90, 40);
			this.btn_single.TabIndex = 0;
			this.btn_single.Text = "加入单局";
			this.btn_single.UseVisualStyleBackColor = true;
			this.btn_single.Click += new System.EventHandler(this.Btn_singleClick);
			// 
			// btn_match
			// 
			this.btn_match.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btn_match.Location = new System.Drawing.Point(104, 20);
			this.btn_match.Name = "btn_match";
			this.btn_match.Size = new System.Drawing.Size(90, 40);
			this.btn_match.TabIndex = 0;
			this.btn_match.Text = "加入比赛";
			this.btn_match.UseVisualStyleBackColor = true;
			this.btn_match.Click += new System.EventHandler(this.Btn_matchClick);
			// 
			// tbn_tag
			// 
			this.tbn_tag.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.tbn_tag.Location = new System.Drawing.Point(200, 20);
			this.tbn_tag.Name = "tbn_tag";
			this.tbn_tag.Size = new System.Drawing.Size(90, 40);
			this.tbn_tag.TabIndex = 0;
			this.tbn_tag.Text = "加入双打";
			this.tbn_tag.UseVisualStyleBackColor = true;
			this.tbn_tag.Click += new System.EventHandler(this.Tbn_tagClick);
			// 
			// panel_rooms
			// 
			this.panel_rooms.Location = new System.Drawing.Point(7, 34);
			this.panel_rooms.Name = "panel_rooms";
			this.panel_rooms.Size = new System.Drawing.Size(598, 576);
			this.panel_rooms.TabIndex = 1;
			// 
			// rb_allmsg
			// 
			this.rb_allmsg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.rb_allmsg.BackColor = System.Drawing.SystemColors.Window;
			this.rb_allmsg.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.rb_allmsg.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.rb_allmsg.Location = new System.Drawing.Point(611, 88);
			this.rb_allmsg.Name = "rb_allmsg";
			this.rb_allmsg.ReadOnly = true;
			this.rb_allmsg.Size = new System.Drawing.Size(397, 430);
			this.rb_allmsg.TabIndex = 2;
			this.rb_allmsg.Text = "";
			// 
			// rb_msg
			// 
			this.rb_msg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.rb_msg.BackColor = System.Drawing.SystemColors.Window;
			this.rb_msg.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.rb_msg.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.rb_msg.Location = new System.Drawing.Point(611, 524);
			this.rb_msg.Name = "rb_msg";
			this.rb_msg.Size = new System.Drawing.Size(397, 52);
			this.rb_msg.TabIndex = 3;
			this.rb_msg.Text = "";
			// 
			// btn_send
			// 
			this.btn_send.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_send.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btn_send.Location = new System.Drawing.Point(926, 582);
			this.btn_send.Name = "btn_send";
			this.btn_send.Size = new System.Drawing.Size(75, 33);
			this.btn_send.TabIndex = 4;
			this.btn_send.Text = "发送";
			this.btn_send.UseVisualStyleBackColor = true;
			this.btn_send.Click += new System.EventHandler(this.Btn_Send_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.btn_single);
			this.groupBox1.Controls.Add(this.btn_match);
			this.groupBox1.Controls.Add(this.tbn_tag);
			this.groupBox1.Location = new System.Drawing.Point(611, 14);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(297, 68);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "随机模式";
			// 
			// btn_other
			// 
			this.btn_other.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_other.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btn_other.Location = new System.Drawing.Point(918, 10);
			this.btn_other.Name = "btn_other";
			this.btn_other.Size = new System.Drawing.Size(90, 72);
			this.btn_other.TabIndex = 0;
			this.btn_other.Text = "自定义建房";
			this.btn_other.UseVisualStyleBackColor = true;
			this.btn_other.Click += new System.EventHandler(this.Btn_otherClick);
			// 
			// btn_clean
			// 
			this.btn_clean.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_clean.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btn_clean.Location = new System.Drawing.Point(730, 582);
			this.btn_clean.Name = "btn_clean";
			this.btn_clean.Size = new System.Drawing.Size(75, 33);
			this.btn_clean.TabIndex = 4;
			this.btn_clean.Text = "清空消息";
			this.btn_clean.UseVisualStyleBackColor = true;
			this.btn_clean.Click += new System.EventHandler(this.Btn_Clean_Click);
			// 
			// chb_closemsg
			// 
			this.chb_closemsg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.chb_closemsg.AutoSize = true;
			this.chb_closemsg.Location = new System.Drawing.Point(612, 581);
			this.chb_closemsg.Name = "chb_closemsg";
			this.chb_closemsg.Size = new System.Drawing.Size(96, 16);
			this.chb_closemsg.TabIndex = 5;
			this.chb_closemsg.Text = "屏蔽所有消息";
			this.chb_closemsg.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(107, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(317, 12);
			this.label1.TabIndex = 6;
			this.label1.Text = "（仅显示自定义房间，随机房间请在点击右边的快速加入）";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label2.Location = new System.Drawing.Point(12, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(89, 19);
			this.label2.TabIndex = 7;
			this.label2.Text = "房间列表";
			// 
			// chb_nonane
			// 
			this.chb_nonane.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.chb_nonane.AutoSize = true;
			this.chb_nonane.Location = new System.Drawing.Point(612, 601);
			this.chb_nonane.Name = "chb_nonane";
			this.chb_nonane.Size = new System.Drawing.Size(72, 16);
			this.chb_nonane.TabIndex = 5;
			this.chb_nonane.Text = "匿名消息";
			this.chb_nonane.UseVisualStyleBackColor = true;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1014, 622);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.chb_nonane);
			this.Controls.Add(this.chb_closemsg);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.btn_other);
			this.Controls.Add(this.btn_clean);
			this.Controls.Add(this.btn_send);
			this.Controls.Add(this.rb_msg);
			this.Controls.Add(this.rb_allmsg);
			this.Controls.Add(this.panel_rooms);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "游戏大厅";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainFormFormClosed);
			this.Load += new System.EventHandler(this.MainFormLoad);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.CheckBox chb_nonane;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btn_other;
		private System.Windows.Forms.CheckBox chb_closemsg;
		private System.Windows.Forms.Button btn_clean;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button btn_send;
		private System.Windows.Forms.RichTextBox rb_msg;
		private System.Windows.Forms.RichTextBox rb_allmsg;
		private System.Windows.Forms.RoomGrid panel_rooms;
		private System.Windows.Forms.Button tbn_tag;
		private System.Windows.Forms.Button btn_match;
		private System.Windows.Forms.Button btn_single;
	}
}
