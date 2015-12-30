using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PokerModel
{
    public class GtoRangeHand : Hand
    {
        [XmlArray]
        public List<Decision> Decisions { get; set; }

        public GtoRangeHand()  {

        }

        public GtoRangeHand(Card card1, Card card2) : base(card1, card2) {
            Decisions = initDecisions();
        }

        private List<Decision> initDecisions() {
            var decList = new List<Decision>
            {
                new Decision(0, 0, 0),
                new Decision(0, 0, 0),
                new Decision(0, 0, 0),
                new Decision(0, 0, 0)
            };
            return decList;
        } 
    }
}
