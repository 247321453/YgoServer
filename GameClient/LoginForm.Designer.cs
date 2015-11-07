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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
			this.btn_login = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tb_username = new System.Windows.Forms.TextBox();
			this.tb_password = new System.Windows.Forms.TextBox();
			this.chb_record = new System.Windows.Forms.CheckBox();
			this.chb_autologin = new System.Windows.Forms.CheckBox();
			this.btn_register = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.cb_serverlist = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// btn_login
			// 
			this.btn_login.Location = new System.Drawing.Point(229, 66);
			this.btn_login.Name = "btn_login";
			this.btn_login.Size = new System.Drawing.Size(75, 54);
			this.btn_login.TabIndex = 0;
			this.btn_login.Text = "登录";
			this.btn_login.UseVisualStyleBackColor = true;
			this.btn_login.Click += new System.EventHandler(this.Button1Click);
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
			this.tb_username.TabIndex = 2;
			// 
			// tb_password
			// 
			this.tb_password.Location = new System.Drawing.Point(61, 66);
			this.tb_password.Name = "tb_password";
			this.tb_password.PasswordChar = '●';
			this.tb_password.Size = new System.Drawing.Size(162, 21);
			this.tb_password.TabIndex = 3;
			// 
			// chb_record
			// 
			this.chb_record.AutoSize = true;
			this.chb_record.Location = new System.Drawing.Point(61, 100);
			this.chb_record.Name = "chb_record";
			this.chb_record.Size = new System.Drawing.Size(72, 16);
			this.chb_record.TabIndex = 4;
			this.chb_record.Text = "记住密码";
			this.chb_record.UseVisualStyleBackColor = true;
			// 
			// chb_autologin
			// 
			this.chb_autologin.AutoSize = true;
			this.chb_autologin.Location = new System.Drawing.Point(151, 100);
			this.chb_autologin.Name = "chb_autologin";
			this.chb_autologin.Size = new System.Drawing.Size(72, 16);
			this.chb_autologin.TabIndex = 4;
			this.chb_autologin.Text = "自动登录";
			this.chb_autologin.UseVisualStyleBackColor = true;
			// 
			// btn_register
			// 
			this.btn_register.Location = new System.Drawing.Point(229, 24);
			this.btn_register.Name = "btn_register";
			this.btn_register.Size = new System.Drawing.Size(75, 23);
			this.btn_register.TabIndex = 5;
			this.btn_register.Text = "注册";
			this.btn_register.UseVisualStyleBackColor = true;
			this.btn_register.Click += new System.EventHandler(this.Do_RegisterButton_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 133);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(41, 12);
			this.label3.TabIndex = 1;
			this.label3.Text = "服务器";
			// 
			// cb_serverlist
			// 
			this.cb_serverlist.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cb_serverlist.FormattingEnabled = true;
			this.cb_serverlist.Location = new System.Drawing.Point(61, 131);
			this.cb_serverlist.Name = "cb_serverlist";
			this.cb_serverlist.Size = new System.Drawing.Size(162, 20);
			this.cb_serverlist.TabIndex = 6;
			this.cb_serverlist.SelectedIndexChanged += new System.EventHandler(this.Do_ServerList_SelectedIndexChanged);
			// 
			// LoginForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(325, 164);
			this.Controls.Add(this.cb_serverlist);
			this.Controls.Add(this.btn_register);
			this.Controls.Add(this.chb_autologin);
			this.Controls.Add(this.chb_record);
			this.Controls.Add(this.tb_password);
			this.Controls.Add(this.tb_username);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btn_login);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LoginForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "登录";
			this.Load += new System.EventHandler(this.LoginFormLoad);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.ComboBox cb_serverlist;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btn_register;
		private System.Windows.Forms.CheckBox chb_autologin;
		private System.Windows.Forms.CheckBox chb_record;
		private System.Windows.Forms.TextBox tb_password;
		private System.Windows.Forms.TextBox tb_username;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btn_login;
	}
}
