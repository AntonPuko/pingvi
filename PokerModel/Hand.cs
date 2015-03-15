using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PokerModel {
    
    public class Hand {
        [XmlElement]
        public Card FirstCard { get; set; }
        [XmlElement]
        public Card SecondCard { get; set; }

        [XmlAttribute]
        public string Name {
            get {
                return FirstCard.Name + SecondCard.Name;
            } //TODO придумать, как правильнее записать set
            set { _name = value; }
        }
        [XmlAttribute]
        public bool IsSuited {
            get { return FirstCard.Suit == SecondCard.Suit; } 
             }

        private string _name;

        public Hand(Card card1, Card card2) {
            //исключить повторы карт 
            if (card1.Rang >= card2.Rang) {
                if (card1.Rang == card2.Rang) {
                    if (card1.Suit <= card2.Suit) {
                        FirstCard = card1;
                        SecondCard = card2;
                    }
                    else {
                        FirstCard = card2;
                        SecondCard = card1;
                    }
                } else {
                    FirstCard = card1;
                    SecondCard = card2;
                }
            } else {
                FirstCard = card2;
                SecondCard = card1;
            }
        }

        public Hand() {
            
        }

    }
}
