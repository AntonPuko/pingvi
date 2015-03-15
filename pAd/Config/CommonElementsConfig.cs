using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pingvi.Config;

namespace Pingvi
{
   public class CommonElementsConfig {
       

        public RectangleF FlopCard1Rect { get; set; }
        public RectangleF FlopCard2Rect { get; set; }
        public RectangleF FlopCard3Rect { get; set; }
        public RectangleF TurnCardRect { get; set; }
        public RectangleF RiverCardRect { get; set; }

        public string DeckPath { get; set; }
        public List<Bitmap> DeckList { get; set; } 


       //blinds

        public BlindsElementsConfig Blinds { get; set; }
     


        //Colors
        public Color FishColor { get; set; }
        public Color WeakRegColor { get; set; }
        public Color GoodRegColor { get; set; }
        public Color UberRegColor { get; set; }

        public Color RockColor { get; set; }
        public Color ManiacColor { get; set; }
       

        public Color InHandColor { get; set; }
        public Color InGameColor { get; set; }
        public Color SitOutColor { get; set; }

        public Color ButtonColor { get; set; }

        public string[] BlindMassStrings { get; set; }

        public string StackDigitsPath { get; set; }
        public List<Bitmap> StackDigitsList { get; set; }
        public Color StackDigitsColor { get; set; }

        public string PotDigitsPath { get; set; }
        public List<Bitmap> PotDigitsList { get; set; }
        public Color PotDigitsColor { get; set; }

        public PixelPoint[] PotDigPosPoints { get; set; }
        public RectangleF[][] PotDigitsRectMass { get; set; }

        public string BetDigitsPath { get; set; }
        public List<Bitmap> BetDigitsList { get; set; }
        public Color BetDigitsColor { get; set; }

        public string TableNumberDigitsPath { get; set; }
        public List<Bitmap> TableNumberDigitsList { get; set; }
        public Color TableNumberDigitsColor { get; set; }
        public PixelPoint[] TableNumberDigPosPoints { get; set; }
        public RectangleF[][] TableNumberDigitsRectMass { get; set; }

        public string StatsDigitsPath { get; set; }
        public List<Bitmap> StatsDigitsList { get; set; }
        public Color StatsDigitsColor { get; set; }





        public CommonElementsConfig() {
           Blinds = new BlindsElementsConfig();
        }

       

    }
}
