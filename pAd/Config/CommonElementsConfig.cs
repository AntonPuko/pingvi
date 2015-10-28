using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Imaging;
using AForge.Neuro;
using Pingvi.Config;

namespace Pingvi
{
   public class CommonElementsConfig {
       

        public Rectangle FlopCard1Rect { get; set; }
        public Rectangle FlopCard2Rect { get; set; }
        public Rectangle FlopCard3Rect { get; set; }
        public Rectangle TurnCardRect { get; set; }
        public Rectangle RiverCardRect { get; set; }

        public string DeckPath { get; set; }
        public List<Bitmap> DeckList { get; set; }
        public List<UnmanagedImage> DeckListUnmanaged { get; set; } 

       


       //blinds

        public BlindsElementsConfig Blinds { get; set; }



       //LINE
        public PixelPoint[] LinePixelPositions { get; set; }
        public Dictionary<Color, string> LineLettersColorDictionary { get; set; }



       //alterline
        public Rectangle[] LineRectPositions { get; set; }
        public string LineNetworkPath { get; set; }
        public ActivationNetwork LineNetwork { get; set; }

        public Dictionary<int,string> LineLettersNumbersDictionary { get; set; } 

       


        //Colors
        public Color FishColor { get; set; }
        public Color WeakRegColor { get; set; }
        public Color GoodRegColor { get; set; }
        public Color GoodRegColor2 { get; set; }
        public Color GoodRegColor3 { get; set; }
        public Color GoodRegColor4 { get; set; }
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
        public List<UnmanagedImage> StackDigitsListUnmanaged { get; set; }
        public Color StackDigitsColor { get; set; }

        public string PotDigitsPath { get; set; }
        public List<Bitmap> PotDigitsList { get; set; }

        public List<UnmanagedImage> PotDigitsListUnmanaged { get; set; } 

        public Color PotDigitsColor { get; set; }

        public PixelPoint[] PotDigPosPoints { get; set; }
        public Rectangle[][] PotDigitsRectMass { get; set; }

        public string BetDigitsPath { get; set; }
        public List<Bitmap> BetDigitsList { get; set; }
        public List<UnmanagedImage> BetDigitsListUnmanaged { get; set; }
        public Color BetDigitsColor { get; set; }



        public string StatsDigitsPath { get; set; }
        public List<Bitmap> StatsDigitsList { get; set; }
        public List<UnmanagedImage> StatsDigitsListUnmanaged { get; set; } 
        public Color StatsDigitsColor { get; set; }


        public Dictionary<int, Color> MultiplierColors { get; set; }
        public PixelPoint MultiplierPixelPoint { get; set; }


        public Color MultiplierColorX2 { get; set; }
        public Color MultiplierColorX4 { get; set; }
        public Color MultiplierColorX6 { get; set; }
        public Color MultiplierColorX10 { get; set; }
        public Color MultiplierColorX25 { get; set; }
        public Color MultiplierColorX120 { get; set; }
        public Color MultiplierColorX240 { get; set; }
        public Color MultiplierColorX3600 { get; set; }

        public CommonElementsConfig() {
           Blinds = new BlindsElementsConfig();
        }

       

    }
}
