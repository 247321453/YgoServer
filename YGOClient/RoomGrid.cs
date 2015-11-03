/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/9/4
 * 时间: 13:54
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.ComponentModel;
using System.Drawing;
using YGOCore.Game;
using YGOClient;

namespace System.Windows.Forms
{
	public class RoomBlock : FlowLayoutPanel
	{
		RoomInfo m_room;
		Client m_client;
		Server m_server;
		public RoomBlock(Client client, Server server,RoomInfo room):base(){
			this.m_room=room;
			this.m_client=client;
			this.m_server=server;
			Init(room);
		}

		private void Init(RoomInfo room){
			this.Size=new Size(186, 200);
			if(room!=null){
				this.SuspendLayout();
				Label label = new Label();
				
				label.TextAlign = ContentAlignment.MiddleCenter;
				label.Size = new Size(182, 30);
				//label.Location = new Point(3,3);
				label.ForeColor = Color.White;

				if(room.Warring){
					label.BackColor=Color.FromArgb(0xee, 0xdd, 0, 0);
					label.Text = "[有坑]"+room.RoomName;
				}else{
					if(room.NeedPass){
						//label.BackColor=Color.FromArgb(0xdd, 0, 0x68, 0x8b);
						label.BackColor=Color.FromArgb(0xee, 0xee, 0xad, 0x0e);
						label.Text = room.RoomName;
					}else{
						label.BackColor=Color.FromArgb(0xdd, 0, 0x64, 0);
						label.Text = room.RoomName;
					}
				}
				if(room.IsStart){
					label.Text+=" 【决斗中】";
				}else{
					label.Text+=" 【等待中】";
				}
				this.Controls.Add(label);
				
				Label label2 = new Label();
				label2.Text = "禁限卡表："+(room.Lflist==0?"OCG":"TCG");
				label2.TextAlign = ContentAlignment.MiddleLeft;
				label2.Padding = new Padding(8, 2, 2, 2);
				label2.Size = new Size(182, 26);
				this.Controls.Add(label2);
				
				Label label3 = new Label();
				label3.Text = "允许卡片："+(room.Rule==0?"OCG":(room.Rule==1?"TCG":"OCG&TCG"));
				label3.TextAlign = ContentAlignment.MiddleLeft;
				label3.Padding = new Padding(8, 2, 2, 2);
				label3.Size = new Size(182, 26);
				this.Controls.Add(label3);
				
				Label label4 = new Label();
				label4.Text = "决斗模式："+(room.Mode==2?"双打模式":(room.Mode==1?"比赛模式":"单局模式"));
				label4.TextAlign = ContentAlignment.MiddleLeft;
				label4.Padding = new Padding(8, 2, 2, 2);
				label4.Size = new Size(182, 26);
				this.Controls.Add(label4);
				
				Label label5 = new Label();
				label5.Text = "玩家1："+((room.players!=null && room.players.Count>0)?room.players[0]:"--");
				label5.TextAlign = ContentAlignment.MiddleLeft;
				label5.Padding = new Padding(8, 2, 2, 2);
				label5.Size = new Size(182, 26);
				this.Controls.Add(label5);
				
				if(room.Mode==2){
					Label label7 = new Label();
					label7.Text = "玩家2："+((room.players!=null && room.players.Count>2)?room.players[2]:"--");
					label7.TextAlign = ContentAlignment.MiddleLeft;
					label7.Padding = new Padding(8, 2, 2, 2);
					label7.Size = new Size(182, 26);
					this.Controls.Add(label7);
				}else{
					Label label6 = new Label();
					label6.Text = "玩家2："+((room.players!=null && room.players.Count>1)?room.players[1]:"--");
					label6.TextAlign = ContentAlignment.MiddleLeft;
					label6.Padding = new Padding(8, 2, 2, 2);
					label6.Size = new Size(182, 26);
					this.Controls.Add(label6);
					
				}
				
				Button join=new Button();
				join.FlatStyle = FlatStyle.Popup;
				join.Size=new Size(180, 32);
				if(room.NeedPass){
					join.Text="输入密码";
					join.ForeColor =Color.DarkRed;
					join.BackColor = Color.Gray;
				}else{
					join.Text="加入房间";
				}
				join.Click += new EventHandler(join_Click);
				join.BackColor= SystemColors.Control;
				this.Controls.Add(join);
				this.ResumeLayout(true);
			}
			this.BackColor = Color.White;
		}

		void join_Click(object sender, EventArgs e)
		{
			if(m_server==null||m_client==null||m_client.m_user==null){
				MessageBox.Show("错误：信息为空。");
				return;
			}
			string pass="";
			if(m_room.NeedPass){
				using(InputDialog input=new InputDialog("请输入密码", true)){
					if(input.ShowDialog()==DialogResult.OK){
						pass +="$"+input.InputText;
					}else{
						return;
					}
				}
			}
			RoomTool.Start(m_server, m_client.m_user, m_room.RoomName+pass);
		}
	}
	public class RoomGrid : FlowLayoutPanel
	{
		#region rooms list
		public void SetRooms(Client client, Server server,RoomInfo[] rooms){
			if(rooms==null){
				rooms=new RoomInfo[0];
			}
			if (!this.InvokeRequired)
			{
				AddRooms(client,server,rooms);
			}
			else
			{
				BeginInvoke(new Action(()=>{
				                       	AddRooms(client,server,rooms);
				                       })
				           );
			}
		}
		
		
		private void AddRooms(Client client, Server server,RoomInfo[] rooms){
			this.SuspendLayout();
			this.Controls.Clear();
			int i=0;
			//MessageBox.Show("共有"+rooms.Length+"房间");
			foreach(RoomInfo room in rooms){
				i++;
				AddRoom(client,server,room, i==rooms.Length);
			}
			if(rooms.Length==0){
				this.ResumeLayout(false);
			}
		}
		public void AddRoom(Client client, Server server,RoomInfo room, bool isLast){
			RoomBlock block=new RoomBlock(client,server,room);
			this.Controls.Add(block);
			if(isLast){
				this.ResumeLayout(true);
			}
		}
		#endregion
	}
}
