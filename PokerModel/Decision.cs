using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PokerModel
{
    public class Decision
    {
        [XmlAttribute]
        public int Value { get; set; }
        [XmlAttribute]
        public double Size { get; set; }
        [XmlAttribute]
        public int Probability { get; set; }

        public Decision() {
            
        }

        public Decision(int value , double size,int probabilty) {
            Value = value;
            Size = size;
            Probability = probabilty;
        }
    }
}
