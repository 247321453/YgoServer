/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/11/3
 * 时间: 14:09
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Timers;
using YGOCore.Net;
using AsyncServer;
using OcgWrapper.Enums;

namespace YGOCore.Game
{	
	/// <summary>
	/// Description of GameTimer.
	/// </summary>
	public class GameTimer
	{
		public GameTimer(GameRoom room)
		{
			this.room = room;
			SideTimer = new MyTimer(30 * 1000, SIDE_TIMEOUT);
			SideTimer.AutoReset = true;
			SideTimer.Elapsed += new ElapsedEventHandler(SideTimer_Elapsed);
			
			StartingTimer = new MyTimer(10 * 1000, STARTING_TIMEOUT);
			StartingTimer.AutoReset = true;
			StartingTimer.Elapsed+=new ElapsedEventHandler(StartingTimer_Elapsed);
			
			HandTimer = new MyTimer(15 * 1000, Hand_TIMEOUT);
			HandTimer.AutoReset = true;
			HandTimer.Elapsed +=new ElapsedEventHandler(HandTimer_Elapsed);
		}

		public void Close(){
			SideTimer.Stop();
			SideTimer.Close();
			StartingTimer.Stop();
			StartingTimer.Close();
		}
		
		#region Starting
		public void StartStartingTimer(){
			StartingTimer.Start();
		}
		public void StopStartingTimer(){
			StartingTimer.Stop();
		}
		void StartingTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if(!(sender is MyTimer)) return;
			MyTimer timer = (MyTimer)sender;
			if(room.State != GameState.Starting) {
				timer.CheckStop();
				return;
			}
			if (room.IsTpSelect)
			{
				long second = timer.Second;
				long maxtime = timer.MaxTime;
				if ((maxtime-second) == 15 || (maxtime-second) < 6)
				{
					room.ServerMessage(string.Format(Messages.MSG_TIP_TIME, (maxtime - second)));
				}
			}
			
			if(timer.CheckStop()){
				room.Surrender(room.Players[room.m_startplayer], 3, true);
				room.State = GameState.End;
				room.End();
			}
		}
		#endregion

		#region side
		void SideTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if(!(sender is MyTimer)) return;
			MyTimer timer = (MyTimer)sender;
			if(room.State != GameState.Side) {
				timer.CheckStop();
				return;
			}
			long second = timer.Second;
			long maxtime = timer.MaxTime;
			if(second == (maxtime-30) || second == (maxtime-60)){
				room.ServerMessage(string.Format(Messages.MSG_TIP_TIME, (maxtime - second)));
			}
			if(timer.CheckStop()){
				if (!room.IsReady[0] && !room.IsReady[1])
				{
					room.State = GameState.End;
					room.End();
					return;
				}
				room.Surrender(!room.IsReady[0] ? room.Players[0]:room.Players[1],3,true);
				room.State = GameState.End;
				room.End();
			}
		}
		public void StartSideTimer(){
			SideTimer.Start();
		}
		public void StopSideTimer(){
			SideTimer.Stop();
		}
		#endregion

		public void StartHandTimer(){
			HandTimer.Start();
		}
		public void StopHandTimer(){
			HandTimer.Stop();
		}
		void HandTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if(!(sender is MyTimer)) return;
			MyTimer timer = (MyTimer)sender;
			if(room.State != GameState.Hand) {
				timer.CheckStop();
				return;
			}
			long second = timer.Second;
			long maxtime = timer.MaxTime;
			int currentTick = (int)(maxtime - second);

			if (currentTick == 30 || currentTick == 15)
			{
				room.ServerMessage(string.Format(Messages.MSG_TIP_TIME, currentTick));
			}

			if(timer.CheckStop()){
				if (room.m_handResult[0]!= 0)
					room.Surrender(room.Players[1], 3, true);
				else if (room.m_handResult[1] != 0)
					room.Surrender(room.Players[0], 3, true);
				else
				{
					room.State = GameState.End;
					room.End();
					return;
				}

				if (room.m_handResult[0] == 0 && room.m_handResult[1] == 0)
				{
					room.State = GameState.End;
					room.End();
					return;
				}
				else
					room.Surrender(room.Players[1 - room.m_lastresponse], 3, true);
			}
		}
		/// <summary>
		/// 换side
		/// </summary>
		private const int SIDE_TIMEOUT = 120;
		/// <summary>
		/// 
		/// </summary>
		private const int STARTING_TIMEOUT = 30;
		private const int Hand_TIMEOUT = 60;
		private GameRoom room;
		private MyTimer SideTimer;
		private MyTimer StartingTimer;
		private MyTimer HandTimer;
	}
}
