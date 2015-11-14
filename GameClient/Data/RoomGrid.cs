/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/9/4
 * 时间: 13:54
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.Drawing;

using YGOCore;
using YGOCore.Game;
using GameClient;

namespace System.Windows.Forms
{
	
	#region room
	public class RoomBlock : FlowLayoutPanel
	{
		RoomGrid parent;
		GameConfig2 config;
		Label lb_statu;
		string RoomName;
		public RoomBlock(RoomGrid parent, GameConfig2 config){
			this.parent=parent;
			this.config = config;
			Init();
		}
		
		#region init
		private void Init(){
			RoomName = Password.OnlyName(config.Name);
			this.Tag = config.DeulPort+":"+RoomName;
			this.Size=new Size(186, 186);
			if(config!=null){
				this.SuspendLayout();
				Label lb_title = new Label();
				lb_title.TextAlign = ContentAlignment.MiddleCenter;
				lb_title.Size = new Size(182, 30);
				//label.Location = new Point(3,3);
				lb_title.ForeColor = Color.White;
				lb_title.Text = RoomName;
				if(config.NoCheckDeck | config.NoShuffleDeck){
					lb_title.BackColor=Color.FromArgb(0xee, 0xdd, 0, 0);
				}else{
					if(config.HasPassword()){
						//label.BackColor=Color.FromArgb(0xdd, 0, 0x68, 0x8b);
						lb_title.BackColor=Color.FromArgb(0xee, 0xee, 0xad, 0x0e);
					}else{
						lb_title.BackColor=Color.FromArgb(0xdd, 0, 0x64, 0);
					}
				}
				lb_statu = new Label();
				lb_statu.TextAlign = ContentAlignment.MiddleCenter;
				lb_statu.Size = new Size(182, 30);
				this.Controls.Add(lb_title);
				this.Controls.Add(lb_statu);
				
				Label label2 = new Label();
				label2.Text = "禁限卡表："+config.BanList;
				label2.TextAlign = ContentAlignment.MiddleLeft;
				label2.Padding = new Padding(8, 2, 2, 2);
				label2.Size = new Size(182, 26);
				this.Controls.Add(label2);
				
				Label label3 = new Label();
				label3.Text = "允许卡片："+(config.Rule==0?"OCG":(config.Rule==1?"TCG":"OCG&TCG"));
				label3.TextAlign = ContentAlignment.MiddleLeft;
				label3.Padding = new Padding(8, 2, 2, 2);
				label3.Size = new Size(182, 26);
				this.Controls.Add(label3);
				
				Label label4 = new Label();
				label4.Text = "决斗模式："+(config.Mode==2?"双打模式":(config.Mode==1?"比赛模式":"单局模式"));
				label4.TextAlign = ContentAlignment.MiddleLeft;
				label4.Padding = new Padding(8, 2, 2, 2);
				label4.Size = new Size(182, 26);
				this.Controls.Add(label4);
				
				Button join=new Button();
				join.FlatStyle = FlatStyle.Popup;
				join.Size=new Size(180, 32);
				if(config.HasPassword()){
					join.Text="输入密码";
					join.ForeColor =Color.DarkRed;
					join.BackColor = Color.Gray;
				}else{
					join.Text="加入房间";
				}
				join.Click += delegate {
					if(config.HasPassword()){
						string pass = Password.GetPwd(config.Name);
						using(InputDialog input=new InputDialog("请输入密码", true)){
							if(input.ShowDialog()==DialogResult.OK){
								if(pass == input.InputText){
									if(parent!=null)parent.JoinRoom(config.Name);
								}else{
									MessageBox.Show("密码不正确");
								}
							}
						}
					}else{
						if(parent!=null)parent.JoinRoom(config.Name);
					}
				};
				join.BackColor= SystemColors.Control;
				StartGame(config.IsStart);
				this.Controls.Add(join);
				this.ResumeLayout(true);
			}
			this.BackColor = Color.White;
		}
		#endregion
		
		public void StartGame(bool start){
			if(lb_statu!=null){
				if(start){
					lb_statu.Text="【决斗中】";
				}else{
					lb_statu.Text="【等待中】";
				}
			}
		}
	}
	#endregion
	
	#region panel
	public class RoomGrid : FlowLayoutPanel
	{
		private Client client;
		private readonly SortedList<string, GameConfig> Rooms=new SortedList<string, GameConfig>();
		private readonly byte[] _lock =new byte[0];
		public void SetClient(Client client){
			this.client=client;
		}
		
		public void JoinRoom(string room){
			if(client!=null){
				client.JoinRoom(room);
			}
		}
		public void ClearRooms(){
			lock(Rooms){
				Rooms.Clear();
			}
		}
		#region allrooms
		public void OnRoomList(List<GameConfig2> configs){
			lock(Rooms){
				foreach(GameConfig2 config in configs){
					string name = Password.OnlyName(config.Name);
					if(!Rooms.ContainsKey(name))
						Rooms.Add(name, config);
				}
			}
			BeginInvoke(new Action(
				()=>{
					lock(_lock)
						UpdateAll(configs);
				})
			           );
		}
		
		private void UpdateAll(List<GameConfig2> configs){
			this.SuspendLayout();
			this.Controls.Clear();
			//MessageBox.Show("共有"+rooms.Length+"房间");
			foreach(GameConfig2 config in configs){
				AddRoom(config);
			}
			this.ResumeLayout(true);
		}
		#endregion
		
		#region close
		public void OnClose(int port, string name){
			name = port+":"+name;
			lock(Rooms){
				if(Rooms.ContainsKey(name)){
					Rooms.Remove(name);
				}
			}
			BeginInvoke(new Action(
				()=>{
					lock(_lock)
						RemoveRoom(name);
				})
			           );
		}
		private void RemoveRoom(string name){
			for(int i= Controls.Count-1;i>=0;i--){
				Control c = Controls[i];
				if(c is RoomBlock && c.Tag!=null){
					if(c.Tag.ToString() == name){
						this.Controls.Remove(c);
					}
				}
			}
		}
		#endregion
		
		#region start
		public void OnStart(int port, string name){
			name = port+":"+name;
			lock(Rooms){
				if(Rooms.ContainsKey(name)){
					Rooms[name].IsStart = true;
				}
			}
			BeginInvoke(new Action(
				()=>{
					lock(_lock)
						StartRoom(name);
				})
			           );
		}
		private void StartRoom(string name){
			for(int i= Controls.Count-1;i>=0;i--){
				Control c = Controls[i];
				if(c is RoomBlock && c.Tag!=null){
					if(c.Tag.ToString() == name){
						((RoomBlock)c).StartGame(true);
					}
				}
			}
		}
		#endregion
		
		#region create
		public void OnCreate(GameConfig2 config){
			string name = Password.OnlyName(config.Name);
			lock(Rooms){
				if(Rooms.ContainsKey(name)){
					Rooms[name] = config;
				}else{
					Rooms.Add(name, config);
				}
			}
			BeginInvoke(new Action(
				()=>{
					lock(_lock)
						AddRoom(config);
				})
			           );
		}

		private void AddRoom(GameConfig2 config){
			RoomBlock b = new RoomBlock(this, config);
			Controls.Add(b);
		}
		#endregion
		
	}
	#endregion
}
