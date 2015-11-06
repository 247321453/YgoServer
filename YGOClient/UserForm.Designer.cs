/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/9/3
 * 时间: 19:26
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
namespace YGOClient
{
	partial class UserForm
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserForm));
			this.cb_username = new System.Windows.Forms.ComboBox();
			this.label_user = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tb_gamepath = new System.Windows.Forms.TextBox();
			this.btn_game = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.tb_args = new System.Windows.Forms.TextBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.tb_password = new System.Windows.Forms.TextBox();
			this.btn_delete = new System.Windows.Forms.Button();
			this.chkb_record = new System.Windows.Forms.CheckBox();
			this.btn_ok = new System.Windows.Forms.Button();
			this.cb_server = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// cb_username
			// 
			this.cb_username.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.cb_username.FormattingEnabled = true;
			this.cb_username.ItemHeight = 12;
			this.cb_username.Location = new System.Drawing.Point(72, 24);
			this.cb_username.Name = "cb_username";
			this.cb_username.Size = new System.Drawing.Size(160, 20);
			this.cb_username.TabIndex = 1;
			this.cb_username.SelectedIndexChanged += new System.EventHandler(this.Cb_usernameSelectedIndexChanged);
			this.cb_username.TextChanged += new System.EventHandler(this.Cb_usernameTextChanged);
			// 
			// label_user
			// 
			this.label_user.AutoSize = true;
			this.label_user.Location = new System.Drawing.Point(12, 26);
			this.label_user.Name = "label_user";
			this.label_user.Size = new System.Drawing.Size(41, 12);
			this.label_user.TabIndex = 6;
			this.label_user.Text = "用户名";
			this.toolTip1.SetToolTip(this.label_user, "必填");
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 67);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(29, 12);
			this.label2.TabIndex = 6;
			this.label2.Text = "密码";
			this.toolTip1.SetToolTip(this.label2, "可选，如果服务端需要认证，则必填");
			// 
			// tb_gamepath
			// 
			this.tb_gamepath.Location = new System.Drawing.Point(72, 104);
			this.tb_gamepath.MaxLength = 256;
			this.tb_gamepath.Name = "tb_gamepath";
			this.tb_gamepath.Size = new System.Drawing.Size(164, 21);
			this.tb_gamepath.TabIndex = 4;
			this.tb_gamepath.TextChanged += new System.EventHandler(this.Tb_gamepathTextChanged);
			// 
			// btn_game
			// 
			this.btn_game.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btn_game.Location = new System.Drawing.Point(240, 104);
			this.btn_game.Name = "btn_game";
			this.btn_game.Size = new System.Drawing.Size(75, 23);
			this.btn_game.TabIndex = 4;
			this.btn_game.Text = "浏览";
			this.btn_game.UseVisualStyleBackColor = true;
			this.btn_game.Click += new System.EventHandler(this.Btn_gameClick);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 108);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(53, 12);
			this.label1.TabIndex = 6;
			this.label1.Text = "游戏程序";
			this.toolTip1.SetToolTip(this.label1, "必填，一般是ygopro.exe");
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(8, 148);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(53, 12);
			this.label3.TabIndex = 6;
			this.label3.Text = "游戏参数";
			this.toolTip1.SetToolTip(this.label3, "可选，比如使用diy卡片");
			// 
			// tb_args
			// 
			this.tb_args.Location = new System.Drawing.Point(72, 144);
			this.tb_args.MaxLength = 256;
			this.tb_args.Name = "tb_args";
			this.tb_args.Size = new System.Drawing.Size(240, 21);
			this.tb_args.TabIndex = 5;
			// 
			// tb_password
			// 
			this.tb_password.Location = new System.Drawing.Point(72, 68);
			this.tb_password.MaxLength = 16;
			this.tb_password.Name = "tb_password";
			this.tb_password.PasswordChar = '*';
			this.tb_password.Size = new System.Drawing.Size(164, 21);
			this.tb_password.TabIndex = 2;
			// 
			// btn_delete
			// 
			this.btn_delete.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btn_delete.ForeColor = System.Drawing.Color.DarkRed;
			this.btn_delete.Location = new System.Drawing.Point(240, 24);
			this.btn_delete.Name = "btn_delete";
			this.btn_delete.Size = new System.Drawing.Size(75, 23);
			this.btn_delete.TabIndex = 6;
			this.btn_delete.Text = "删除";
			this.btn_delete.UseVisualStyleBackColor = true;
			this.btn_delete.Click += new System.EventHandler(this.Btn_deleteClick);
			// 
			// chkb_record
			// 
			this.chkb_record.AutoSize = true;
			this.chkb_record.Location = new System.Drawing.Point(240, 72);
			this.chkb_record.Name = "chkb_record";
			this.chkb_record.Size = new System.Drawing.Size(72, 16);
			this.chkb_record.TabIndex = 3;
			this.chkb_record.Text = "记住密码";
			this.chkb_record.UseVisualStyleBackColor = true;
			// 
			// btn_ok
			// 
			this.btn_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btn_ok.Enabled = false;
			this.btn_ok.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btn_ok.Location = new System.Drawing.Point(99, 243);
			this.btn_ok.Name = "btn_ok";
			this.btn_ok.Size = new System.Drawing.Size(120, 40);
			this.btn_ok.TabIndex = 0;
			this.btn_ok.Text = "确定";
			this.btn_ok.UseVisualStyleBackColor = true;
			this.btn_ok.Click += new System.EventHandler(this.Btn_okClick);
			// 
			// cb_server
			// 
			this.cb_server.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cb_server.FormattingEnabled = true;
			this.cb_server.Location = new System.Drawing.Point(72, 184);
			this.cb_server.Name = "cb_server";
			this.cb_server.Size = new System.Drawing.Size(238, 20);
			this.cb_server.TabIndex = 6;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(11, 188);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(41, 12);
			this.label4.TabIndex = 6;
			this.label4.Text = "服务器";
			// 
			// UserForm
			// 
			this.ClientSize = new System.Drawing.Size(322, 295);
			this.Controls.Add(this.cb_server);
			this.Controls.Add(this.btn_ok);
			this.Controls.Add(this.chkb_record);
			this.Controls.Add(this.btn_delete);
			this.Controls.Add(this.tb_password);
			this.Controls.Add(this.tb_args);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btn_game);
			this.Controls.Add(this.tb_gamepath);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label_user);
			this.Controls.Add(this.cb_username);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "UserForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "用户";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UserFormFormClosing);
			this.Load += new System.EventHandler(this.UserFormLoad);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox cb_server;
		private System.Windows.Forms.Button btn_ok;
		private System.Windows.Forms.CheckBox chkb_record;
		private System.Windows.Forms.Button btn_delete;
		private System.Windows.Forms.TextBox tb_password;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.TextBox tb_args;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btn_game;
		private System.Windows.Forms.TextBox tb_gamepath;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label_user;
		private System.Windows.Forms.ComboBox cb_username;
	}
}
