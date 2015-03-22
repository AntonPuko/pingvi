using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PokerModel
{
    
    public class RangeHand : Hand
    {
        [XmlAttribute]
        public int D1 { get; set; }
        [XmlAttribute]
        public int D2 { get; set; }
        [XmlAttribute]
        public double S1 { get; set; }

        public RangeHand(Card card1, Card card2) : base(card1, card2) {
            D1 = 0;
            D2 = 0;
            S1 = 0.0;
        }

        public RangeHand() {
            
        }


    

    }
}
