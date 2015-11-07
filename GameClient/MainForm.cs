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
		LoginForm m_login;
		public MainForm(LoginForm parent)
		{
			InitializeComponent();
			m_login = parent;
		}
		void MainFormLoad(object sender, EventArgs e)
		{
			if(m_login.ShowDialog() != DialogResult.OK){
				
			}
		}
	}
}
