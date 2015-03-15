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
        public double Playability { get; set; }
        public RangeHand(Card card1, Card card2) : base(card1, card2) {
            Playability = 0.0;
        }

        public RangeHand() {
            
        }


    

    }
}
