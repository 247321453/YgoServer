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
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.rb_allmsg = new System.Windows.Forms.RichTextBox();
			this.rb_msg = new System.Windows.Forms.RichTextBox();
			this.btn_send = new System.Windows.Forms.Button();
			this.listView1 = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btn_single
			// 
			this.btn_single.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btn_single.Location = new System.Drawing.Point(6, 21);
			this.btn_single.Name = "btn_single";
			this.btn_single.Size = new System.Drawing.Size(125, 41);
			this.btn_single.TabIndex = 0;
			this.btn_single.Text = "单局模式";
			this.btn_single.UseVisualStyleBackColor = true;
			this.btn_single.Click += new System.EventHandler(this.Btn_singleClick);
			// 
			// btn_match
			// 
			this.btn_match.Location = new System.Drawing.Point(137, 21);
			this.btn_match.Name = "btn_match";
			this.btn_match.Size = new System.Drawing.Size(125, 41);
			this.btn_match.TabIndex = 0;
			this.btn_match.Text = "比赛模式";
			this.btn_match.UseVisualStyleBackColor = true;
			this.btn_match.Click += new System.EventHandler(this.Btn_matchClick);
			// 
			// tbn_tag
			// 
			this.tbn_tag.Location = new System.Drawing.Point(268, 20);
			this.tbn_tag.Name = "tbn_tag";
			this.tbn_tag.Size = new System.Drawing.Size(125, 41);
			this.tbn_tag.TabIndex = 0;
			this.tbn_tag.Text = "双打模式";
			this.tbn_tag.UseVisualStyleBackColor = true;
			this.tbn_tag.Click += new System.EventHandler(this.Tbn_tagClick);
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 12);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(184, 598);
			this.flowLayoutPanel1.TabIndex = 1;
			// 
			// rb_allmsg
			// 
			this.rb_allmsg.BackColor = System.Drawing.SystemColors.Window;
			this.rb_allmsg.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.rb_allmsg.Location = new System.Drawing.Point(202, 86);
			this.rb_allmsg.Name = "rb_allmsg";
			this.rb_allmsg.ReadOnly = true;
			this.rb_allmsg.Size = new System.Drawing.Size(397, 430);
			this.rb_allmsg.TabIndex = 2;
			this.rb_allmsg.Text = "";
			// 
			// rb_msg
			// 
			this.rb_msg.BackColor = System.Drawing.SystemColors.Window;
			this.rb_msg.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.rb_msg.Location = new System.Drawing.Point(202, 522);
			this.rb_msg.Name = "rb_msg";
			this.rb_msg.Size = new System.Drawing.Size(397, 52);
			this.rb_msg.TabIndex = 3;
			this.rb_msg.Text = "";
			// 
			// btn_send
			// 
			this.btn_send.Location = new System.Drawing.Point(517, 581);
			this.btn_send.Name = "btn_send";
			this.btn_send.Size = new System.Drawing.Size(75, 33);
			this.btn_send.TabIndex = 4;
			this.btn_send.Text = "发送";
			this.btn_send.UseVisualStyleBackColor = true;
			this.btn_send.Click += new System.EventHandler(this.Btn_Send_Click);
			// 
			// listView1
			// 
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
									this.columnHeader1,
									this.columnHeader2,
									this.columnHeader3});
			this.listView1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.listView1.FullRowSelect = true;
			this.listView1.GridLines = true;
			this.listView1.Location = new System.Drawing.Point(605, 12);
			this.listView1.MultiSelect = false;
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(247, 602);
			this.listView1.TabIndex = 5;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "名字";
			this.columnHeader1.Width = 120;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "等级";
			this.columnHeader2.Width = 43;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "状态";
			this.columnHeader3.Width = 53;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Location = new System.Drawing.Point(202, 578);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(306, 42);
			this.pictureBox1.TabIndex = 6;
			this.pictureBox1.TabStop = false;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.btn_single);
			this.groupBox1.Controls.Add(this.btn_match);
			this.groupBox1.Controls.Add(this.tbn_tag);
			this.groupBox1.Location = new System.Drawing.Point(202, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(397, 68);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "快速加入";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(864, 622);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.btn_send);
			this.Controls.Add(this.rb_msg);
			this.Controls.Add(this.rb_allmsg);
			this.Controls.Add(this.flowLayoutPanel1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "游戏大厅";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainFormFormClosed);
			this.Load += new System.EventHandler(this.MainFormLoad);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.Button btn_send;
		private System.Windows.Forms.RichTextBox rb_msg;
		private System.Windows.Forms.RichTextBox rb_allmsg;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Button tbn_tag;
		private System.Windows.Forms.Button btn_match;
		private System.Windows.Forms.Button btn_single;
	}
}
