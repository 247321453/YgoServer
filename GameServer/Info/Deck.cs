﻿using System.Collections.Generic;
using OcgWrapper;
using OcgWrapper.Enums;

namespace YGOCore.Game
{
	public class Deck
	{
		public IList<int> Main { get; private set; }
		public IList<int> Extra { get; private set; }
		public IList<int> Side { get; private set; }
		
		public List<int> Alls{get;private set;}

		public Deck()
		{
			Main = new List<int>();
			Extra = new List<int>();
			Side = new List<int>();
			Alls =new List<int>();
		}

		public void AddMain(int cardId)
		{
			Card card = Card.Get(cardId);
			if (card == null)
				return;
			if ((card.Data.Type & (int)CardType.Token) != 0)
				return;
			if ((card.Data.Type & 0x802040) != 0)
			{
				if (Extra.Count < 15)
					Extra.Add(cardId);
			}
			else
			{
				if (Main.Count < 60)
					Main.Add(cardId);
			}
			if(!Alls.Contains(cardId)){
				Alls.Add(cardId);
			}
		}

		public void AddSide(int cardId)
		{
			Card card = Card.Get(cardId);
			if (card == null)
				return;
			if ((card.Data.Type & (int)CardType.Token) != 0)
				return;
			if (Side.Count < 15)
				Side.Add(cardId);
			if(!Alls.Contains(cardId)){
				Alls.Add(cardId);
			}
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ban"></param>
        /// <param name="ocg"></param>
        /// <param name="tcg"></param>
        /// <returns>1数量，2空卡，卡片规则不符合</returns>
		public int Check(Banlist ban, bool ocg, bool tcg)
		{
			if (Main.Count < 40 || Main.Count > 60 || Extra.Count > 15 || Side.Count > 15)
				return 1;

			IDictionary<int, int> cards = new Dictionary<int, int>();

			IList<int>[] stacks = new IList<int>[] { Main, Extra, Side };
			foreach (IList<int> stack in stacks)
			{
				foreach (int id in stack)
				{
					Card card = Card.Get(id);
                    if(card == null)
                    {
                        return 2;
                    }
					AddToCards(cards, card);
					if (!ocg && card.Ot == 1 || !tcg && card.Ot == 2)
						return id;
				}
			}

			if (ban == null)
				return 0;

			foreach (KeyValuePair<int, int> pair in cards)
			{
				int max = ban.GetQuantity(pair.Key);
				if (pair.Value > max)
					return pair.Key;
			}

			return 0;
		}

		public bool Check(Deck deck)
		{
			if (deck.Main.Count != Main.Count || deck.Extra.Count != Extra.Count)
				return false;

			IDictionary<int, int> cards = new Dictionary<int, int>();
			IDictionary<int, int> ncards = new Dictionary<int, int>();
			IList<int>[] stacks = new IList<int>[] { Main, Extra, Side };
			foreach (IList<int> stack in stacks)
			{
				foreach (int id in stack)
				{
					if (!cards.ContainsKey(id))
						cards.Add(id, 1);
					else
						cards[id]++;
				}
			}
			stacks = new IList<int>[] { deck.Main, deck.Extra, deck.Side };
			foreach (IList<int> stack in stacks)
			{
				foreach (int id in stack)
				{
					if (!ncards.ContainsKey(id))
						ncards.Add(id, 1);
					else
						ncards[id]++;
				}
			}
			foreach (KeyValuePair<int, int> pair in cards)
			{
				if (!ncards.ContainsKey(pair.Key))
					return false;
				if (ncards[pair.Key] != pair.Value)
					return false;
			}
			return true;
		}

		private static void AddToCards(IDictionary<int, int> cards, Card card)
		{
			int id = card.Id;
			if (card.Data.Alias != 0)
				id = card.Data.Alias;
			if (cards.ContainsKey(id))
				cards[id]++;
			else
				cards.Add(id, 1);
		}
	}
}