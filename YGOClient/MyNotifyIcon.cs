/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/9/3
 * 时间: 16:55
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace YGOClient
{
	public sealed  class MyNotifyIcon
	{
		private NotifyIcon notifyIcon;
		private ContextMenu notificationMenu;
		public Client m_client;
		
		#region Initialize icon and menu
		public MyNotifyIcon()
		{
			notifyIcon = new NotifyIcon();
			notificationMenu = new ContextMenu(InitializeMenu());
			
			notifyIcon.DoubleClick += IconDoubleClick;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MyNotifyIcon));
			notifyIcon.Icon = (Icon)resources.GetObject("$this.Icon");
			notifyIcon.ContextMenu = notificationMenu;
		
			m_client = new Client(notifyIcon);
		}
		
		private MenuItem[] InitializeMenu()
		{
			MenuItem[] menu = new MenuItem[] {
				new MenuItem("房间列表", menuShowClick),
				new MenuItem("用户信息", menuChangeUserClick),
				new MenuItem("论坛", menuAboutClick),
				new MenuItem("退出", menuExitClick)
			};
			return menu;
		}
		
		private void Show(){
			m_client.Show();
		}
		#endregion
		
		#region Main - Program entry point
		/// <summary>Program entry point.</summary>
		/// <param name="args">Command Line Arguments</param>
		[STAThread]
		public static void Main(string[] args)
		{
			if(args.Length >= 1){
				switch(args[0]){
					case "install":
						Protocol.Reg(RoomTool.PRO);
						Environment.Exit(0);
						break;
					case "uninstall":
						Protocol.UnReg(RoomTool.PRO);
						Environment.Exit(0);
						break;
					default:
						if(args[0].StartsWith(RoomTool.PRO)){
							RoomTool.Command(args[0]);
							Environment.Exit(0);
						}
						break;
				}
				
			}
			
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			
			bool isFirstInstance;
			// Please use a unique name for the mutex to prevent conflicts with other programs
			using (Mutex mtx = new Mutex(true, "YGOClient2", out isFirstInstance)) {
				if (isFirstInstance) {
					MyNotifyIcon notificationIcon = new MyNotifyIcon();
					notificationIcon.notifyIcon.Visible = true;
					notificationIcon.Show();
					Application.Run();
					notificationIcon.notifyIcon.Dispose();
				} else {
					MessageBox.Show("This is running.");
					// The application is already running
					// TODO: Display message box or change focus to existing application instance
				}
			} // releases the Mutex
		}
		#endregion
		
		#region Event Handlers
		private void menuChangeUserClick(object sender, EventArgs e){
			m_client.ChangedUser();
		}
		private void menuShowClick(object sender, EventArgs e)
		{
			m_client.Show();
		}
		
		private void menuAboutClick(object sender, EventArgs e)
		{
			try{
				System.Diagnostics.Process.Start("http://bbs.ygobbs.com/");
			}catch(Exception){}
		}
		
		private void menuExitClick(object sender, EventArgs e)
		{
			if(m_client!=null){
				m_client.isClose=true;
				m_client.Close();
			}
			Application.Exit();
		}
		
		private void IconDoubleClick(object sender, EventArgs e)
		{
			if(m_client.IsShowing()){
				m_client.Hide();
			}else{
				m_client.Show();
			}
			//MessageBox.Show("The icon was double clicked");
		}
		#endregion
	}
}
