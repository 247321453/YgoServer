using System;
using OcgWrapper.Managers;
using OcgWrapper.Enums;

namespace OcgWrapper
{
	public class Card
	{
		public struct CardData
		{
			public Int32 Code;
			public Int32 Alias;
			public Int64 Setcode;
			public Int32 Type;
			public Int32 Level;
			public Int32 Attribute;
			public Int32 Race;
			public Int32 Attack;
			public Int32 Defense;
			public Int32 LScale;
			public Int32 RScale;
		}

		public int Id { get; private set; }
		public int Ot { get; private set; }
		public CardData Data { get; private set; }
		public int AliasId { get{return Data.Alias;} }
		public int Type { get{return Data.Type;}  }
		public int Level { get{return Data.Level&0xff;} }
		public int Attribute { get{return Data.Attribute;} }
		public int Race { get{return Data.Race;} }
		public int Atk { get{return Data.Attack;} }
		public int Def { get{return  Data.Defense;} }
        public string Name { get; private set; }
        public string Description { get;private set; }
		
		public static Card Get(int id)
		{
			return CardsManager.GetCard(id);
		}

		public delegate void LoadCardEventHandler(int cardId, ref CardData card);
		public static event LoadCardEventHandler LoadCard;

		internal static void OnLoadCard(int cardId, ref CardData card)
		{
			LoadCardEventHandler handler = LoadCard;
			if (handler != null) handler(cardId, ref card);
		}

		internal Card(CardData data, int ot, string name,string desc)
		{
			this.Id = data.Code;
			this.Ot = ot;
			this.Data = data;
			this.Name=name;
			this.Description=desc;
		}
		
        public Card(int id)
        {
            Id = id;
            Data=new Card.CardData();
        }
		public bool HasType(CardType type)
		{
			return ((Type & (int)type) != 0);
		}

		public bool IsExtraCard()
		{
			return (HasType(CardType.Fusion) || HasType(CardType.Synchro) || HasType(CardType.Xyz));
		}
	}
}