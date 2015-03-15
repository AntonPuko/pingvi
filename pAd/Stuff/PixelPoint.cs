using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Pingvi {
    /// <summary>
    /// Represent pixel coordinates on screen
    /// </summary>
    public class PixelPoint {
        [XmlAttribute]
        public int X { get; set; }
        [XmlAttribute]
        public int Y { get;set; }
        public PixelPoint() {
            
        }

        public PixelPoint(int x, int y) {
            X = x;
            Y = y;
            
        }
    }
}
