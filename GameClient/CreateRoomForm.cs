/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/9/4
 * 时间: 16:22
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GameClient
{
	/// <summary>
	/// Description of CreateRoomForm.
	/// </summary>
	public partial class CreateRoomForm : Form
	{
		Client client;
		public CreateRoomForm(Client client)
		{
			InitializeComponent();
			this.client=client;
			cb_banlist.SelectedIndex=0;
			cb_draw.SelectedIndex=0;
			cb_hand.SelectedIndex=4;
			cb_lp.SelectedIndex=1;
			cb_mode.SelectedIndex=0;
			cb_rule.SelectedIndex=2;
			cb_timeout.SelectedIndex=2;
			chk_sp1.Checked=false;
			chk_sp2.Checked=false;
			chk_sp3.Checked=false;
			tb_name.Text="";
			tb_password.Text="";
		}
		
		private string GetRoomString(){
			string cmd="";
			try{
				int index=cb_mode.SelectedIndex;
				if(index==1){
					cmd+="M";
				}else if(index==2){
					cmd+="T";
				}else{
					cmd+="S";
				}
				index = cb_rule.SelectedIndex;
				cmd += ""+index;
				index = cb_banlist.SelectedIndex;
				cmd += ""+index;
				index = cb_timeout.SelectedIndex+1;
				cmd += ""+index;
				cmd += chk_sp1.Checked?"T":"F";
				cmd += chk_sp2.Checked?"T":"F";
				cmd += chk_sp3.Checked?"T":"F";
				index = cb_lp.SelectedIndex+1;
				cmd += ""+index;
				index = cb_hand.SelectedIndex+1;
				cmd += ""+index;
				index = cb_draw.SelectedIndex+1;
				cmd += ""+index;
				cmd += "#"+tb_name.Text;
				if(!string.IsNullOrEmpty(tb_password.Text)){
					cmd += "$"+tb_password.Text;
				}
			}catch{}
			return cmd;
		}
		
		#region create
		void Btn_createClick(object sender, EventArgs e)
		{
			string room = GetRoomString();
			client.JoinRoom(room);
			this.Hide();
			//MessageBox.Show(room);
		}
		#endregion
				
		void Cb_lpSelectedIndexChanged(object sender, EventArgs e)
		{
			try{
				int index=cb_lp.SelectedIndex;
				if(index>=0){
					lb_lp.Text="x4000 = "+(index+1)*4000;
				}
			}catch{}
		}
	}
}
