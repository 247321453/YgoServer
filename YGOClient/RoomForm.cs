/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/9/3
 * 时间: 17:10
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using System.Windows.Forms;

using YGOCore;
using YGOCore.Game;

namespace YGOClient
{
	public partial class RoomForm : Form
	{
		private Client m_client;
		private Server m_server;
		private string m_dir;
		
		private CreateRoomForm m_craeteform;
		private int m_lasttime;
		
		#region form
		public RoomForm(Client client)
		{
			InitializeComponent();
			this.fp_rooms.SetParent(client, null);
			this.m_client=client;
			m_dir=Tool.Combine(Application.StartupPath, "data");
		}
		
		void RoomFormLoad(object sender, EventArgs e)
		{
			loadServers();
		}
		
		void RoomFormFormClosing(object sender, FormClosingEventArgs e)
		{
			if(!m_client.isClose){
				e.Cancel=true;
				Hide();
			}
		}
		#endregion
		
		#region refresh room
		void Btn_refreshClick(object sender, EventArgs e)
		{
			if(m_server == null){
				return;
			}
			int now=Environment.TickCount;
			int sp=now-m_lasttime;
			if(sp<3*1000){
				MessageBox.Show(string.Format("请不要刷新太快, 请{0:N1}秒后再刷新",((3000-sp)/1000.0f)));
				return;
			}
			m_lasttime = now;
			btn_refresh.Enabled=false;
			Thread thread=new Thread(new ThreadStart(RefreshRooms));
			thread.IsBackground=true;
			thread.Start();
		}
		
		
		private void RefreshRooms(){
			string url=m_server.RoomUrl;
			if(url!=null){
				if(url.IndexOf("?")>=0){
					url += "&start=false";
				}else{
					url += "?start=false";
				}
			}
			string json = Tool.GetHtmlContentByUrl(url);
			try{
				List<RoomInfo> list=new List<RoomInfo>();
				list=Tool.Parse<List<RoomInfo>>(json);
				fp_rooms.SetRooms(list.ToArray());
			}
			catch{
				fp_rooms.SetRooms(null);
			}
			btn_refresh.BeginInvoke(new Action(()=>
			                                   {
			                                   	btn_refresh.Enabled=true;
			                                   })
			                       );
		}
		#endregion
		
		#region server
		void Cb_serverSelectedIndexChanged(object sender, EventArgs e)
		{
			try{
				int index=cb_server.SelectedIndex;
				string name=(index>=0)?cb_server.Items[index].ToString():"";
				Server srv=Server.Load(name);
				if(srv!=null){
					cb_auth.Checked=srv.NeedAuth;
					btn_reg.Enabled = srv.NeedAuth;
					m_server=srv;
					fp_rooms.SetParent(m_client, m_server);
					ConfigManager.Save(Server.TAG, name);
				}else{
					//MessageBox.Show("加载失败");
				}
			}catch(Exception){}
		}

		
		void loadServers(){
			cb_server.Items.Clear();
			if(Directory.Exists(m_dir)){
				DirectoryInfo dir=new DirectoryInfo(m_dir);
				FileInfo[] files=dir.GetFiles("*.srv");
				foreach(FileInfo info in files){
					string name=info.Name;
					name=name.Substring(0, name.Length-4);
					cb_server.Items.Add(name);
				}
			}
			if(cb_server.Items.Count>0){
				string name=ConfigManager.readString(Server.TAG);
				cb_server.SelectedIndex=0;
				int index=0;
				foreach(object o in cb_server.Items){
					if(o.ToString() == name){
						cb_server.SelectedIndex = index;
						break;
					}
					index++;
				}
			}else{
				cb_auth.Checked=false;
				m_server=null;
			}
		}
		
		void Btn_regClick(object sender, EventArgs e)
		{
			if(m_server==null){
				return;
			}
			try{
				System.Diagnostics.Process.Start(m_server.RegisterUrl);
			}catch{}
		}
		#endregion

		#region create/join
		void Btn_createClick(object sender, EventArgs e)
		{
			if(m_craeteform==null){
				m_craeteform=new CreateRoomForm(m_server, m_client.m_user);
			}else{
				m_craeteform.SetInfo(m_server, m_client.m_user);
			}
			m_craeteform.ShowDialog();
		}
		
		void Btn_joinClick(object sender, EventArgs e)
		{
			if(m_client==null){
				return;
			}
			RoomTool.Start(m_server, m_client.m_user, "random");
		}
		
		
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData){
			if(keyData == Keys.F5){
				Btn_refreshClick(null, null);
			}else if(keyData == Keys.F6){
				Btn_joinClick(null, null);
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
		#endregion
		
		//deck
		void Btn_deckClick(object sender, EventArgs e)
		{
			if(m_client==null||m_client.m_user==null){
				return;
			}
			RoomTool.RunGame(m_client.m_user.GamePath, "-d");
		}
		//replay
		void Btn_replayClick(object sender, EventArgs e)
		{
			if(m_client==null||m_client.m_user==null){
				return;
			}
			RoomTool.RunGame(m_client.m_user.GamePath, "-r");
		}
	}
}
