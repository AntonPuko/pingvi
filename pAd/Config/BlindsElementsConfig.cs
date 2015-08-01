﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pingvi.Config
{
    public class BlindsElementsConfig {
        public Color DigitsPointColor { get; set; }
        public PixelPoint[] DigitsPosPoints { get; set; }
        public RectangleF[] DigitsRectMass { get; set; }
        public double[] BigBlindsDoubleMass { get; set; }
        public string DigitsPath { get; set; }
        public List<Bitmap> DigitsList { get; set; }
      
    }
}
