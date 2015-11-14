/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/8/30
 * 时间: 11:00
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;

namespace OcgWrapper
{
	/// <summary>
	/// Description of WinInfo.
	/// </summary>
	public class WinInfo
	{
		private const string SQL_Table="CREATE TABLE IF NOT EXISTS wins ("
			+"id INTEGER PRIMARY KEY AUTOINCREMENT,"
			+"room TEXT,"
			+"win INTEGER,"
			+"mode INTEGER,"
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
		public int mode;
		public int reason;
		public string replay;
		public string[] players;
		public string date="";
		public bool force=false;
		
		public static string DB_FILE = "win.db";
		public static void Init(string file="win.db"){
			DB_FILE = file;
			SQLiteTool.Create(file, SQL_Table);
			List<string> cols=SQLiteTool.GetColumns(file, "wins");
			if(!cols.Contains("mode"))
				SQLiteTool.Command(file, "ALTER TABLE wins ADD COLUMN mode INTEGER NOT NULL DEFAULT 0;");
		}
		
		public WinInfo(string room,int win,int reason,string replay,
		               string[] players,
		               bool force=false)
		{
			this.room=room;
			this.win=win;
			this.reason=reason;
			this.replay=replay;
			this.players= players;
			this.force=force;
			this.date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
		}
		private static string reText(string txt){
			if(txt==null){
				return "";
			}
			return txt.Replace("'","''");
		}
		public string GetSQL(){
			string sql="INSERT INTO wins(room,win,mode,reason,replay,"
				+"player0,player1,player2,player3,"
				+"force,endtime)"
				+" VALUES("
				+"'"+reText(room)+"',"
				+win+","
				+mode+","
				+reason+","
				+"'"+reText(replay)+"',"
				+"'"+reText(players[0])+"',"
				+"'"+reText(players[1])+"',"
				+"'"+reText(players[2])+"',"
				+"'"+reText(players[3])+"',"
				+(force?"1":"0")+","
				+"'"+date+"'"
				+");";
			return sql;
		}
	}
}