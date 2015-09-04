/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/9/4
 * 时间: 17:18
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace YGOClient
{
	/// <summary>
	/// Description of InputDialog.
	/// </summary>
	public partial class InputDialog : Form
	{
		string input="";
		public string InputText{get{return input;}}
		public InputDialog(string title,bool hidetext=false)
		{
			InitializeComponent();
			if(!string.IsNullOrEmpty(title)){
				Text=title;
			}
			if(hidetext){
				textBox1.PasswordChar='*';
			}
		}
		
		void Btn_okClick(object sender, EventArgs e)
		{
			input = textBox1.Text;
		}
	}
}
