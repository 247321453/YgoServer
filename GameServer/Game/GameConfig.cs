using System;

namespace YGOCore.Game
{
	public class GameConfig
	{
		public GameConfig(){
			
		}
        //	private static Regex NameRegex=new Regex("^[0-9A-Za-z_\\s.]+$");
        public HostInfo Info;
        public int LfList
        {
            get
            {
                return BanlistManager.GetIndex(Info.LfList);
            }
            set
            {
                    Info.LfList= BanlistManager.Gethash(value);
            }
        }
        public string BanList
        {
            get
            {
                return BanlistManager.GetName(LfList);
            }
            set
            {
                Info.LfList = BanlistManager.Gethash(value);
            }
        }
        //012
        public int Rule
        {
            get
            {
                return Info.Rule;
            }
            set
            {
                Info.Rule = Convert.ToByte(value);
            }
        }
        //012
        public int Mode
        {
            get
            {
                return Info.Mode;
            }
            set
            {
                Info.Mode = Convert.ToByte(value);
            }
        }
        public bool EnablePriority
        {
            get
            {
                return Convert.ToBoolean(Info.EnablePriority);
            }
            set
            {
                Info.EnablePriority = Convert.ToByte(value);
            }
        }
        public bool NoCheckDeck
        {
            get
            {
                return Convert.ToBoolean(Info.NoCheckDeck);
            }
            set
            {
                Info.NoCheckDeck = Convert.ToByte(value);
            }
        }
        public bool NoShuffleDeck
        {
            get
            {
                return Convert.ToBoolean(Info.NoShuffleDeck);
            }
            set
            {
                Info.NoShuffleDeck = Convert.ToByte(value);
            }
        }
        public int StartLp
        {
            get
            {
                return Convert.ToInt32(Info.StartLp);
            }
            set
            {
                Info.StartLp = Convert.ToUInt32(value);
            }
        }
        public int StartHand
        {
            get
            {
                return Info.StartHand;
            }
            set
            {
                Info.StartHand = Convert.ToByte(value);
            }
        }
        public int DrawCount
        {
            get
            {
                return Info.DrawCount;
            }
            set
            {
                Info.DrawCount = Convert.ToByte(value);
            }
        }
        public int GameTimer
        {
            get
            {
                return Info.GameTimer;
            }
            set
            {
                Info.GameTimer = Convert.ToByte(value);
            }
        }
        public string Name { get; set; }
		public bool IsRandom {get; set;}
		public bool IsMatch {get{return Mode == 1;}}
		public bool IsTag {get{return  Mode == 2;}}
		public bool IsStart{get;set;}
		public bool HasPassword(){
			return Name!=null && Name.IndexOf("$")>=0;
		}
		
		public int Parse(string gameinfo){
			int head=gameinfo.IndexOf("#");
			if(head < 0){
				IsRandom = false;
				Name = gameinfo;
				return -1;
			}else if(head==0 && gameinfo.Length > 1){
				Name = gameinfo.Substring(1);
				if(!string.IsNullOrEmpty(Name)){
					IsRandom = false;
					return -1;
				}
			}
			try{
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
			return head;
		}
	}
}