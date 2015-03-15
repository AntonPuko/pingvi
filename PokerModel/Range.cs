using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace PokerModel {
    
    public class Range {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlArray]
        public List<RangeHand> Hands { get; set; }

        public Range() {
            
        }

        public Range(string name, string range) {
            Name = name;
            Hands = new List<RangeHand>();

            using (RangeParser rp = new RangeParser()) {
               Hands = rp.Parse(range);
            } 
        }

        public bool CheckHand(Hand hand) {
            var hnd = Hands.FirstOrDefault(h => h == hand);
            return hnd != null;
        }

        public double CheckHandPlayability(Hand hand) {
            var hnd = Hands.FirstOrDefault(h => h == hand);
            if (hnd == null) return 0;
            return hnd.Playability;
        }
    

       


    }
}
