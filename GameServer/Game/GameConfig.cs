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
		public int Rule { get; set; }
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
		public bool HasPassword(){
			return Name!=null && Name.IndexOf("$")>=0;
		}
		public string GetString(){
			return "";
		}
	}
}