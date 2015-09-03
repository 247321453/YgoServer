/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/9/3
 * 时间: 17:10
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace YGOClient
{
	/// <summary>
	/// Description of RoomForm.
	/// </summary>
	public partial class RoomForm : Form
	{
		Client m_client;
		public RoomForm(Client client)
		{
			InitializeComponent();
			this.m_client=client;
		}
		
		void RoomFormFormClosing(object sender, FormClosingEventArgs e)
		{
			if(!m_client.isClose){
				e.Cancel=true;
				Hide();
			}
		}
	}
}
