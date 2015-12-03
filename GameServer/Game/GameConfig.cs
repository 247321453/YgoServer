using System;

namespace YGOCore.Game
{
	public class GameConfig
	{
		public GameConfig(){
			
		}
		//	private static Regex NameRegex=new Regex("^[0-9A-Za-z_\\s.]+$");
		public int LfList { get; set; }
		public string BanList{get; set;}
		//012
		public int Rule { get; set; }
		//012
		public int Mode { get; set; }
		public bool EnablePriority { get; set; }
		public bool NoCheckDeck { get; set; }
		public bool NoShuffleDeck { get; set; }
		public int StartLp { get; set; }
		public int StartHand { get; set; }
		public int DrawCount { get; set; }
		public int GameTimer { get; set; }
		public string Name { get; set; }
		public bool IsRandom {get; set;}
		public bool IsMatch {get{return Mode == 1;}}
		public bool IsTag {get{return  Mode == 2;}}
		public bool IsStart{get;set;}
		public string RoomString{get; set;}
		public bool HasPassword(){
			return (RoomString!=null && RoomString.Contains("$")) || (Name != null && Name.Contains("$"));
		}
		
		public void Parse(string gameinfo){
            if(RoomString == null)
            {
                RoomString = gameinfo;
            }
			int head=gameinfo.IndexOf("#");
			if(head < 0){
				//xxxx
				IsRandom = false;
				Name = gameinfo;
				return;
			}else if(head==0 && gameinfo.Length > 1){
				//#xxx
				Name = gameinfo.Substring(1);
				IsRandom = false;
				return;
			}
			try{
				//xxxx#xxxx
				//xxxx#xxxx$xxx
				int index=0;
				string tmp=gameinfo.Substring(index++,1);
				int tmpInt=0;
				Mode = ("t"==tmp||"T"==tmp)?2:(("m"==tmp||"M"==tmp)?1:0);
				if(IsTag){
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
					StartLp = tmpInt>0?tmpInt*4000:(IsTag?16000:8000);
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
			}catch(Exception){
				
			}
		}
	}
}