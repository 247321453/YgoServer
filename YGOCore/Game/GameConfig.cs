using System;
using System.Text;
using System.Text.RegularExpressions;

namespace YGOCore.Game
{
	public class GameConfig
	{
		private static Regex NameRegex=new Regex("^[0-9A-Za-z_\\s.]+$");
		public int LfList { get; private set; }
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
		public string Password{get;private set;}

		public GameConfig(string info)
		{
			Load(info);
//			if (info.ToLower() == "tcg" || info.ToLower() == "ocg" || info.ToLower() == "ocg/tcg" || info.ToLower() == "tcg/ocg")
//			{
//				LfList = info.ToLower() == "ocg/tcg" ? 1: info.ToLower() == "tcg/ocg" ? 0 : info.ToLower() == "ocg" ? 1 : 0;
//				Rule = info.ToLower() == "ocg/tcg" || info.ToLower() == "tcg/ocg" ? 2 : info.ToLower() == "tcg" ? 1 : 0;
//				Mode = 0;
//				EnablePriority = false;
//				NoCheckDeck = false;
//				NoShuffleDeck = false;
//				StartLp = 8000;
//				StartHand = 5;
//				DrawCount = 1;
//				GameTimer = 120;
//				Name = GameManager.RandomRoomName();
//			}
		}

		public GameConfig(GameClientPacket packet)
		{
			LfList = BanlistManager.GetIndex(packet.ReadUInt32());
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
				Name = GameManager.RandomRoomName();
		}

		public void Load(string gameinfo)
		{
			LfList = 0;
			Rule = 2;
			Mode = 0;
			EnablePriority = false;
			NoCheckDeck = false;
			NoShuffleDeck = false;
			StartLp = 8000;
			StartHand = 5;
			DrawCount = 1;
			GameTimer = 120;
			try{
				if(!string.IsNullOrEmpty(gameinfo)){
					gameinfo = gameinfo.ToLower();
					gameinfo = gameinfo.Trim();
				}
				if(string.IsNullOrEmpty(gameinfo)||gameinfo=="random"){
					//random
					Name = GameManager.RandomRoomName();
				}
				else if(NameRegex.IsMatch(gameinfo)){
					//normal
					Name = gameinfo;
				}else{
					int head=gameinfo.IndexOf("#");
					if(head<0){
						Name=gameinfo;
					}else if(head==0){
						Name=gameinfo.Substring(1);
					}else{
						int index=0;
						string tmp=gameinfo.Substring(index++,1);
						int tmpInt=0;
						Mode = "t"==tmp?2:("m"==tmp?1:0);
						if(Mode==2){
							StartLp=16000;
						}
						if(index < head){
							tmp=gameinfo.Substring(index++,1);
							Rule = "1"==tmp?1:("0"==tmp?0:2);
						}
						if(index < head){
							tmp=gameinfo.Substring(index++,1);
							int.TryParse(tmp, out tmpInt);
							GameTimer = tmpInt>0?tmpInt*60:120;
						}
						if(index < head){
							tmp=gameinfo.Substring(index++,1);
							EnablePriority=(tmp=="t"||tmp=="1");
						}
						if(index < head){
							tmp=gameinfo.Substring(index++,1);
							NoCheckDeck=(tmp=="t"||tmp=="1");
						}
						if(index < head){
							tmp=gameinfo.Substring(index++,1);
							NoShuffleDeck=(tmp=="t"||tmp=="1");
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
						if(head+1>=gameinfo.Length){
							string _name=GameManager.RandomRoomName(gameinfo);
							if(_name==null){
								Name=gameinfo+GameManager.RandomRoomName();
							}else{
								Name=_name;
							}
						}else{
							Name=gameinfo.Substring(head+1);
						}
					}
				}
			}
			catch(Exception){
				Name = GameManager.RandomRoomName();
			}
			//
			//(N|M|T)(0|1|2)(0-9)(T|F|1|0)(T|F|1|0)(T|F|1|0)(0-9)(0-9)(0-9)#name$password
			//mode,rule,outtime[minute],EnablePriority,NoCheckDeck,NoShuffleDeck,lp[*4000],hand,draw
			//N23FFF851#xxxx$123
			//N23#xxxx$123
			//N2#xxxx$123
			//N#xxxx$123
			//xxxxx$123
			//xxxx
//			LfList = 0;
//			Rule = 2;
//			Mode = 0;
//			EnablePriority = false;
//			NoCheckDeck = false;
//			NoShuffleDeck = false;
//			StartLp = 8000;
//			StartHand = 5;
//			DrawCount = 1;
//			GameTimer = 120;
//			bool isSimple=false;
//			try
//			{
//				if(gameinfo.StartsWith("N#")){
//					Mode=0;
//					isSimple=true;
//				}else if(gameinfo.StartsWith("M#")){
//					Mode=1;
//					isSimple=true;
//				}else if(gameinfo.StartsWith("T#")){
//					Mode=2;
//					StartLp = 16000;
//					isSimple=true;
//				}
//				if(isSimple){
//					if(gameinfo.EndsWith("#")){
//						string _name=GameManager.RandomRoomName(gameinfo);
//						if(_name==null){
//							Name=gameinfo+GameManager.RandomRoomName();
//						}else{
//							Name=_name;
//						}
//					}else{
//						Name=gameinfo;
//					}
//					return;
//				}
//				string rules = gameinfo.Substring(0, 6);
//
//				Rule = int.Parse(rules[0].ToString());
//				Mode = int.Parse(rules[1].ToString());
//				GameTimer = int.Parse(rules[2].ToString()) ;
//				GameTimer=(GameTimer== 0) ? 120 : (GameTimer*60);
//				EnablePriority = rules[3] == 'T' || rules[3] == '1';
//				NoCheckDeck = rules[4] == 'T' || rules[4] == '1';
//				NoShuffleDeck = rules[5] == 'T' || rules[5] == '1';
//
//				string data = gameinfo.Substring(6, gameinfo.Length - 6);
//
//				string[] list = data.Split(',');
//
//				StartLp = int.Parse(list[0]);
//				LfList = int.Parse(list[1]);
//
//				StartHand = int.Parse(list[2]);
//				DrawCount = int.Parse(list[3]);
//
//				Name = list[4];
//			}
//			catch (Exception)
//			{
//				LfList = 0;
//				Rule = 2;
//				Mode = 0;
//				EnablePriority = false;
//				NoCheckDeck = false;
//				NoShuffleDeck = false;
//				StartLp = 8000;
//				StartHand = 5;
//				DrawCount = 1;
//				GameTimer = 120;
//				if(gameinfo==null||gameinfo.Length==0){
//					Name=GameManager.RandomRoomName();
//				}else{
//					Name = gameinfo;//
//				}
//				return;
//			}
		}
	}
}