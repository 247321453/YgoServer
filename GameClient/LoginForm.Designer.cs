/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/7
 * 时间: 20:34
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
namespace GameClient
{
	partial class LoginForm
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
            this.btn_login = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_username = new System.Windows.Forms.TextBox();
            this.tb_password = new System.Windows.Forms.TextBox();
            this.chb_record = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_game = new System.Windows.Forms.Button();
            this.btn_single = new System.Windows.Forms.Button();
            this.btn_addai = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_login
            // 
            this.btn_login.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_login.Location = new System.Drawing.Point(229, 27);
            this.btn_login.Name = "btn_login";
            this.btn_login.Size = new System.Drawing.Size(87, 54);
            this.btn_login.TabIndex = 4;
            this.btn_login.Text = "登录";
            this.btn_login.UseVisualStyleBackColor = true;
            this.btn_login.Click += new System.EventHandler(this.Button_Login_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "用户名";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "密码";
            // 
            // tb_username
            // 
            this.tb_username.Location = new System.Drawing.Point(61, 26);
            this.tb_username.Name = "tb_username";
            this.tb_username.Size = new System.Drawing.Size(162, 21);
            this.tb_username.TabIndex = 0;
            // 
            // tb_password
            // 
            this.tb_password.Location = new System.Drawing.Point(61, 66);
            this.tb_password.Name = "tb_password";
            this.tb_password.PasswordChar = '●';
            this.tb_password.Size = new System.Drawing.Size(162, 21);
            this.tb_password.TabIndex = 1;
            // 
            // chb_record
            // 
            this.chb_record.AutoSize = true;
            this.chb_record.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chb_record.Location = new System.Drawing.Point(61, 100);
            this.chb_record.Name = "chb_record";
            this.chb_record.Size = new System.Drawing.Size(70, 16);
            this.chb_record.TabIndex = 2;
            this.chb_record.Text = "记住密码";
            this.chb_record.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_game);
            this.groupBox1.Controls.Add(this.btn_single);
            this.groupBox1.Controls.Add(this.btn_addai);
            this.groupBox1.Location = new System.Drawing.Point(5, 126);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(319, 67);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "离线功能";
            // 
            // btn_game
            // 
            this.btn_game.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_game.Location = new System.Drawing.Point(215, 20);
            this.btn_game.Name = "btn_game";
            this.btn_game.Size = new System.Drawing.Size(100, 37);
            this.btn_game.TabIndex = 5;
            this.btn_game.Text = "启动游戏";
            this.btn_game.UseVisualStyleBackColor = true;
            this.btn_game.Click += new System.EventHandler(this.Btn_Game_Click);
            // 
            // btn_single
            // 
            this.btn_single.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_single.Location = new System.Drawing.Point(110, 20);
            this.btn_single.Name = "btn_single";
            this.btn_single.Size = new System.Drawing.Size(100, 37);
            this.btn_single.TabIndex = 5;
            this.btn_single.Text = "残局模式";
            this.btn_single.UseVisualStyleBackColor = true;
            this.btn_single.Click += new System.EventHandler(this.Btn_Single_Click);
            // 
            // btn_addai
            // 
            this.btn_addai.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_addai.Location = new System.Drawing.Point(6, 20);
            this.btn_addai.Name = "btn_addai";
            this.btn_addai.Size = new System.Drawing.Size(100, 37);
            this.btn_addai.TabIndex = 5;
            this.btn_addai.Text = "添加局域网AI";
            this.btn_addai.UseVisualStyleBackColor = true;
            this.btn_addai.Click += new System.EventHandler(this.Btn_addaiClick);
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Location = new System.Drawing.Point(229, 93);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(87, 30);
            this.button1.TabIndex = 5;
            this.button1.Text = "设置游戏路径";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Btn_SetGamePath_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(328, 199);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chb_record);
            this.Controls.Add(this.tb_password);
            this.Controls.Add(this.tb_username);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_login);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "登录";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		private System.Windows.Forms.Button btn_addai;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox chb_record;
		private System.Windows.Forms.TextBox tb_password;
		private System.Windows.Forms.TextBox tb_username;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btn_login;
        private System.Windows.Forms.Button btn_game;
        private System.Windows.Forms.Button btn_single;
        private System.Windows.Forms.Button button1;
    }
}
