/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/8/30
 * 时间: 11:00
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;

namespace OcgWrapper
{
	/// <summary>
	/// Description of WinInfo.
	/// </summary>
	public class WinInfo
	{
		public 	const string SQL_Table="CREATE TABLE IF NOT EXISTS wins ("
			+"id INTEGER PRIMARY KEY AUTOINCREMENT,"
			+"room TEXT,"
			+"win INTEGER,"
			+"reason INTEGER,"
			+"replay TEXT,"
			+"player0 TEXT,"
			+"player1 TEXT,"
			+"player2 TEXT,"
			+"player3 TEXT,"
			+"force INTEGER,"
			+"endtime TimeStamp NOT NULL DEFAULT CURRENT_TIMESTAMP"
			+");";
		public string room;
		public int win;
		public int reason;
		public string replay;
		public string player0;
		public string player1;
		public string player2="";
		public string player3="";
		public string date="";
		public bool force=false;
		
		public WinInfo(string room,int win,int reason,string replay,
		               string player0,string player1,
		               string player2="",string player3="",
		               bool force=false)
		{
			this.room=room;
			this.win=win;
			this.reason=reason;
			this.replay=replay;
			this.player0=player0;
			this.player1=player1;
			this.player2=player2;
			this.player3=player3;
			this.force=force;
			this.date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
		}
		private static string reText(string txt){
			if(txt==null){
				return "";
			}
			return txt.Replace("'","''");
		}
		public string getSQL(){
			string sql="INSERT INTO wins(room,win,reason,replay,player0,player1,player2,player3,force,endtime) VALUES("
				+"'"+reText(room)+"',"
				+win+","
				+reason+","
				+"'"+reText(replay)+"',"
				+"'"+reText(player0)+"',"
				+"'"+reText(player1)+"',"
				+"'"+reText(player2)+"',"
				+"'"+reText(player3)+"',"
				+(force?"1":"0")+","
				+"'"+date+"'"
				+");";
			return sql;
		}
	}
}