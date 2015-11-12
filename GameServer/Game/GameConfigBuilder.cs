/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/8
 * 时间: 10:04
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using YGOCore.Net;
using AsyncServer;

namespace YGOCore.Game
{

	public static class GameConfigBuilder
	{
		
		public static GameConfig Build(string gameinfo)
		{
			//默认值
			GameConfig config = new GameConfig();
			config.IsRandom = true;
			config.LfList = 0;
			config.BanList = BanlistManager.Banlists[0].Name;
			config.Rule = 2;
			config.Mode = 0;
			config.EnablePriority = false;
			config.NoCheckDeck = false;
			config.NoShuffleDeck = false;
			config.StartLp = 8000;
			config.StartHand = 5;
			config.DrawCount = 1;
			config.GameTimer = 120;
			#region 解析
			try{
				if(!string.IsNullOrEmpty(gameinfo)){
					gameinfo = gameinfo.Trim();
				}
				if(string.IsNullOrEmpty(gameinfo)||gameinfo=="random"){
					//random
					config.Name = RoomManager.RandomRoomName();
					return config;
				}
				int head=gameinfo.IndexOf("#");
				if(head<0){
					config.IsRandom = false;
					config.Name=gameinfo;
					return config;
				}else if(head==0){
					config.Name = gameinfo.Substring(1);
					if(!string.IsNullOrEmpty(config.Name)){
						config.IsRandom = false;
						return config;
					}
				}
				int index=0;
				string tmp=gameinfo.Substring(index++,1);
				int tmpInt=0;
				config.Mode = ("t"==tmp||"T"==tmp)?2:(("m"==tmp||"M"==tmp)?1:0);
				if(config.Mode==2){
					config.StartLp=16000;
				}
				if(index < head){
					tmp=gameinfo.Substring(index++,1);
					config.Rule = ("1"==tmp||"t"==tmp||"T"==tmp)?1:(("0"==tmp||"o"==tmp||"O"==tmp)?0:2);
				}
				if(index < head){
					tmp=gameinfo.Substring(index++,1);
					config.LfList = Convert.ToInt32(tmp);//("1"==tmp||"t"==tmp||"T"==tmp)?1:0;
				}
				if(index < head){
					tmp=gameinfo.Substring(index++,1);
					int.TryParse(tmp, out tmpInt);
					config.GameTimer = tmpInt>0?tmpInt*60:120;
				}
				if(index < head){
					tmp=gameinfo.Substring(index++,1);
					config.EnablePriority=(tmp=="t"||tmp=="1"||"T"==tmp);
				}
				if(index < head){
					tmp=gameinfo.Substring(index++,1);
					config.NoCheckDeck=(tmp=="t"||tmp=="1"||tmp=="T");
				}
				if(index < head){
					tmp=gameinfo.Substring(index++,1);
					config.NoShuffleDeck=(tmp=="t"||tmp=="1"||"T"==tmp);
				}
				if(index < head){
					tmp=gameinfo.Substring(index++,1);
					int.TryParse(tmp, out tmpInt);
					config.StartLp = tmpInt>0?tmpInt*4000:(config.IsTag?16000:8000);
				}
				if(index < head){
					tmp=gameinfo.Substring(index++,1);
					int.TryParse(tmp, out tmpInt);
					config.StartHand = tmpInt>0?tmpInt:5;
				}
				if(index < head){
					tmp=gameinfo.Substring(index++,1);
					int.TryParse(tmp, out tmpInt);
					config.DrawCount = tmpInt>0?tmpInt:1;
				}
				//M#
				//head=1
				if(head+1>=gameinfo.Length){
					string _name=RoomManager.RandomRoomName(gameinfo);
					if(_name==null){
						//条件#的随机房间名没找到，则创建一个
						config.Name=gameinfo + RoomManager.NewRandomRoomName();
					}else{
						//条件#的随机房间名存在，则进去，可能重复观战
						config.Name=_name;
					}
				}else{
					config.IsRandom = false;
					config.Name=gameinfo;//.Substring(head+1);
				}
			}
			catch(Exception e){
				config.Name = RoomManager.NewRandomRoomName();
				Logger.Warn("gameinfo="+e);
			}
			#endregion
			
			return config;
		}

		public static GameConfig Build(GameClientPacket packet)
		{
			GameConfig config =new GameConfig();
			config.LfList = BanlistManager.GetIndex(packet.ReadUInt32());
			config.BanList = BanlistManager.Banlists[config.LfList].Name;
			config.Rule = packet.ReadByte();
			config.Mode = packet.ReadByte();
			config.EnablePriority = Convert.ToBoolean(packet.ReadByte());
			config.NoCheckDeck = Convert.ToBoolean(packet.ReadByte());
			config.NoShuffleDeck = Convert.ToBoolean(packet.ReadByte());
			//C++ padding: 5 bytes + 3 bytes = 8 bytes
			for (int i = 0; i < 3; i++)
				packet.ReadByte();
			config.StartLp = packet.ReadInt32();
			config.StartHand = packet.ReadByte();
			config.DrawCount = packet.ReadByte();
			config.GameTimer = packet.ReadInt16();
			packet.ReadUnicode(20);
			config.Name = packet.ReadUnicode(30);
			if (string.IsNullOrEmpty(config.Name))
				config.Name = RoomManager.NewRandomRoomName();
			return config;
		}
		

	}
}
