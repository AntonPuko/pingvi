using System.Collections.Generic;
using System.Xml.Serialization;

namespace PokerModel
{
    public class GtoRange
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlArray]
        public List<GtoRangeHand> Hands { get; set; }

        public GtoRange() {
            
        }

        public GtoRange(string name, string range) {
            Name = name;
            Hands = new List<GtoRangeHand>();
            using (GtoRangeParser rp = new GtoRangeParser())
            {
                Hands = rp.Parse(range);
            }
        }


    }
}