/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/15
 * 时间: 9:16
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;

namespace GameClient
{
	public class RoomInfo{
		public RoomInfo(int port,string name){
			Name = name;
			Port = port;
		}
		public RoomInfo(string name,int port){
			Name = name;
			Port = port;
		}
		public string Name;
		public int Port;
		public override string ToString(){
			return Port +":"+Name;
		}
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if(obj is RoomInfo)
            {
                RoomInfo r = (RoomInfo)obj;
                if (r.ToString() == ToString())
                {
                    return true;
                }
            }
            return false;
        }
    }
	/// <summary>
	/// Description of PlayerInfo.
	/// </summary>
	public class PlayerInfo
	{
		public PlayerInfo(){
			
		}
		public PlayerInfo(string name)
		{
			Name = name;
		}
		public string Name;
		public readonly List<RoomInfo> Rooms = new List<RoomInfo>();
	}
}
