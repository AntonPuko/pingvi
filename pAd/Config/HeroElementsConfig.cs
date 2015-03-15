using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pingvi
{
    public class HeroElementsConfig : PlayerElementsConfig {
        public PixelPoint IsTurnPoint { get; set; }
        public Color IsTurnColor { get; set; }
    }
}
