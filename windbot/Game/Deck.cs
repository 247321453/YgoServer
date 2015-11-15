using System;
using System.Collections.Generic;
using System.IO;
using OcgWrapper;
using WindBot.Game.AI;

namespace WindBot.Game.Data {
    public class Deck {
        public IList<Card> Cards { get; private set; }
        public IList<Card> ExtraCards { get; private set; }
        public IList<Card> SideCards { get; private set; }
        static Random random=new Random(Environment.TickCount);

        public Deck() {
            Cards = new List<Card>();
            ExtraCards = new List<Card>();
            SideCards = new List<Card>();
        }

        private void AddNewCard(int cardId, bool sideDeck) {
            Card newCard = Card.Get(cardId);
            if (newCard == null)
                return;

            if (!sideDeck)
                AddCard(newCard);
            else
                SideCards.Add(newCard);
        }

        private void AddCard(Card card) {
            if (card.IsExtraCard())
                ExtraCards.Add(card);
            else
                Cards.Add(card);
        }

        public static Deck Load(string name) {
            StreamReader reader = null;
            try {
                reader = new StreamReader(new FileStream("Decks/" + name+".ydk", FileMode.Open, FileAccess.Read));

                Deck deck = new Deck();
                bool side = false;

                while (!reader.EndOfStream) {
                    string line = reader.ReadLine();
                    if (line == null)
                        continue;

                    line = line.Trim();
                    if (line.StartsWith("#"))
                        continue;
                    if (line.Equals("!side")) {
                        side = true;
                        continue;
                    }

                    int id;
                    if (!int.TryParse(line, out id))
                        continue;

                    deck.AddNewCard(id, side);
                }

                reader.Close();

                if (deck.Cards.Count > 60)
                    return null;
                if (deck.ExtraCards.Count > 15)
                    return null;
                if (deck.SideCards.Count > 15)
                    return null;

                return deck;
            }
            catch (Exception e) {
            	Logger.WriteLine(e.ToString());
                if (reader != null)
                    reader.Close();
                return null;
            }
        }
    }
}