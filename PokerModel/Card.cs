using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Serialization;

namespace PokerModel {
    public enum Suit {
        NoCard, Clubs , Spades, Hearts, Diamonds, 
    }

    public enum Rang {
        NoCard, Two ,Three,Four,Five,Six,Seven,Eight,Nine,Ten, Jack, Queen, King, Ace
    }

    public class Card {
        [XmlElement]
        public int Value { get; set; }
        [XmlElement]
        public Suit Suit { get; set; }
        [XmlElement]
        public Rang Rang { get; set; }
        [XmlElement]
        public string Name { get; set; }

        public Card() {
        }
       
        public Card(int value) {
            SetProperties(value);
        }

        public Card(string fullname) {
            
            SetProperties(fullname);
        }

        public Card(Rang rang, Suit suit) {
            Suit = suit;
            Rang = rang;
            Name = SetCardName(rang, suit);
            Value = SetCardValue(rang, suit);
        }


        private void SetProperties(int value) {
            Value = value;
            Suit  = SetSuitByValue(value);
            Rang = SetRangByValue(value);
            Name  =  SetCardName(Rang, Suit);
        }

        private void SetProperties(string fullname) {
            Name = fullname;
            Rang = SetRangByName(fullname);
            Suit = SetSuitByName(fullname);
            Value = SetCardValue(Rang, Suit);
        }

        private Suit SetSuitByName(string name) {
            if (Rang == Rang.NoCard) return Suit.NoCard;
            var suitName = name.Substring(1,1);
            if (suitName == "c") return Suit.Clubs;
            if (suitName == "s") return Suit.Spades;
            if (suitName == "h") return Suit.Hearts;
            if (suitName == "d") return Suit.Diamonds;
            return Suit.NoCard;
        }

        public static Rang SetRangByName(string name) {
            var rangName = name.Substring(0, 1);
            if (rangName == "2") return Rang.Two;
            if (rangName == "3") return Rang.Three;
            if (rangName == "4") return Rang.Four;
            if (rangName == "5") return Rang.Five;
            if (rangName == "6") return Rang.Six;
            if (rangName == "7") return Rang.Seven;
            if (rangName == "8") return Rang.Eight;
            if (rangName == "9") return Rang.Nine;
            if (rangName == "T") return Rang.Ten;
            if (rangName == "J") return Rang.Jack;
            if (rangName == "Q") return Rang.Queen;
            if (rangName == "K") return Rang.King;
            if (rangName == "A") return Rang.Ace;
            return Rang.NoCard;
        }


        private Suit SetSuitByValue(int value) {
            if(value >= 1 && value <=13) return Suit.Clubs;
            if (value > 13 && value <= 26) return Suit.Spades;
            if (value > 26 && value <= 39) return Suit.Hearts;
            if (value > 39 && value <= 52) return Suit.Diamonds;
            return Suit.NoCard;
        }

        private Rang SetRangByValue(int value) {
            if (value >= 1 && value <= 13) return (Rang) value;
            if (value > 13 && value <= 26) return (Rang) value - 13;
            if (value > 26 && value <= 39) return (Rang) value - 26;
            if (value > 39 && value <= 52) return (Rang) value - 39;
            return Rang.NoCard;
        }


        private int SetCardValue(Rang rang, Suit suit) {
            var val = (int) rang;
            if (val < 1 && val > 52) return 0;
            switch (suit) {
                case Suit.Clubs: return val;
                case Suit.Spades: return val+13;
                case Suit.Hearts: return val+26;
                case Suit.Diamonds: return val+39;
                default: return 0;
            }
        }

        private string SetCardName(Rang rang, Suit suit) {
            string rStr = SetRangName(rang);
            string sStr = SetSuitName(suit);
            return rStr + sStr;
        }

        public static string SetRangName(Rang rang) {
            string rStr;
            switch (rang)
            {
                case Rang.NoCard: rStr = ""; break;
                case Rang.Two: rStr = "2"; break;
                case Rang.Three: rStr = "3"; break;
                case Rang.Four: rStr = "4"; break;
                case Rang.Five: rStr = "5"; break;
                case Rang.Six: rStr = "6"; break;
                case Rang.Seven: rStr = "7"; break;
                case Rang.Eight: rStr = "8"; break;
                case Rang.Nine: rStr = "9"; break;
                case Rang.Ten: rStr = "T"; break;
                case Rang.Jack: rStr = "J"; break;
                case Rang.Queen: rStr = "Q"; break;
                case Rang.King: rStr = "K"; break;
                case Rang.Ace: rStr = "A"; break;
                default: rStr = ""; break;
            }
            return rStr;
        }

        public static string SetSuitName(Suit suit) {
            string sStr;
            switch (suit) {
                case Suit.NoCard: sStr = ""; break;
                case Suit.Clubs: sStr = "c"; break;
                case Suit.Spades: sStr = "s"; break;
                case Suit.Hearts: sStr = "h"; break;
                case Suit.Diamonds: sStr = "d"; break;
                default: sStr = ""; break;
            }
            return sStr;
        }

    }
}
