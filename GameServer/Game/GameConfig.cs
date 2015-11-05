using System;
using System.Text;
using System.Text.RegularExpressions;
using AsyncServer;
using YGOCore.Net;
using System.Collections.Generic;

namespace YGOCore.Game
{
	public class GameConfig
	{
		//	private static Regex NameRegex=new Regex("^[0-9A-Za-z_\\s.]+$");
		public int LfList { get; private set; }
		public string BanList{get;private set;}
		public int Rule { get; private set; }
		public int Mode { get; private set; }
		public bool EnablePriority { get; private set; }
		public bool NoCheckDeck { get; private set; }
		public bool NoShuffleDeck { get; private set; }
		public int StartLp { get; private set; }
		public int StartHand { get; private set; }
		public int DrawCount { get; private set; }
		public int GameTimer { get; private set; }
		public string Name { get; private set; }
		public bool IsMatch {get{return Mode == 1;}}
		public bool IsTag {get{return  Mode == 2;}}
		
		public GameConfig(string gameinfo)
		{
			LfList = 0;
			BanList = BanlistManager.Banlists[LfList].Name;
			Rule = 2;
			Mode = 0;
			EnablePriority = false;
			NoCheckDeck = false;
			NoShuffleDeck = false;
			StartLp = 8000;
			StartHand = 5;
			DrawCount = 1;
			GameTimer = 120;
			
			#region 解析
			try{
				if(!string.IsNullOrEmpty(gameinfo)){
					gameinfo = gameinfo.Trim();
				}
				if(string.IsNullOrEmpty(gameinfo)||gameinfo=="random"){
					//random
					Name = RoomManager.RandomRoomName();
				}
				else{
					int head=gameinfo.IndexOf("#");
					if(head<0){
						Name=gameinfo;
					}else if(head==0){
						Name=gameinfo.Substring(1);
					}else{
						int index=0;
						string tmp=gameinfo.Substring(index++,1);
						int tmpInt=0;
						Mode = ("t"==tmp||"T"==tmp)?2:(("m"==tmp||"M"==tmp)?1:0);
						if(Mode==2){
							StartLp=16000;
						}
						if(index < head){
							tmp=gameinfo.Substring(index++,1);
							Rule = ("1"==tmp||"t"==tmp||"T"==tmp)?1:(("0"==tmp||"o"==tmp||"O"==tmp)?0:2);
						}
						if(index < head){
							tmp=gameinfo.Substring(index++,1);
							LfList = Convert.ToInt32(tmp);//("1"==tmp||"t"==tmp||"T"==tmp)?1:0;
						}
						if(index < head){
							tmp=gameinfo.Substring(index++,1);
							int.TryParse(tmp, out tmpInt);
							GameTimer = tmpInt>0?tmpInt*60:120;
						}
						if(index < head){
							tmp=gameinfo.Substring(index++,1);
							EnablePriority=(tmp=="t"||tmp=="1"||"T"==tmp);
						}
						if(index < head){
							tmp=gameinfo.Substring(index++,1);
							NoCheckDeck=(tmp=="t"||tmp=="1"||tmp=="T");
						}
						if(index < head){
							tmp=gameinfo.Substring(index++,1);
							NoShuffleDeck=(tmp=="t"||tmp=="1"||"T"==tmp);
						}
						if(index < head){
							tmp=gameinfo.Substring(index++,1);
							int.TryParse(tmp, out tmpInt);
							StartLp = tmpInt>0?tmpInt*4000:(Mode==2?16000:8000);
						}
						if(index < head){
							tmp=gameinfo.Substring(index++,1);
							int.TryParse(tmp, out tmpInt);
							StartHand = tmpInt>0?tmpInt:5;
						}
						if(index < head){
							tmp=gameinfo.Substring(index++,1);
							int.TryParse(tmp, out tmpInt);
							DrawCount = tmpInt>0?tmpInt:1;
						}
						//M#
						//head=1
						if(head+1>=gameinfo.Length){
							string _name=RoomManager.RandomRoomName(gameinfo);
							if(_name==null){
								//条件#的随机房间名没找到，则创建一个
								Name=gameinfo + RoomManager.NewRandomRoomName();
							}else{
								//条件#的随机房间名存在，则进去，可能重复观战
								Name=_name;
							}
						}else{
							Name=gameinfo;//.Substring(head+1);
						}
					}
				}
			}
			catch(Exception e){
				Name = RoomManager.NewRandomRoomName();
				Logger.Warn("gameinfo="+e);
			}
			#endregion
		}

		public GameConfig(GameClientPacket packet)
		{
			LfList = BanlistManager.GetIndex(packet.ReadUInt32());
			BanList = BanlistManager.Banlists[LfList].Name;
			Rule = packet.ReadByte();
			Mode = packet.ReadByte();
			EnablePriority = Convert.ToBoolean(packet.ReadByte());
			NoCheckDeck = Convert.ToBoolean(packet.ReadByte());
			NoShuffleDeck = Convert.ToBoolean(packet.ReadByte());
			//C++ padding: 5 bytes + 3 bytes = 8 bytes
			for (int i = 0; i < 3; i++)
				packet.ReadByte();
			StartLp = packet.ReadInt32();
			StartHand = packet.ReadByte();
			DrawCount = packet.ReadByte();
			GameTimer = packet.ReadInt16();
			packet.ReadUnicode(20);
			Name = packet.ReadUnicode(30);
			if (string.IsNullOrEmpty(Name))
				Name = RoomManager.NewRandomRoomName();
		}
		
		public bool HasPassword(){
			return Name!=null && Name.IndexOf("$")>=0;
		}
	}
}