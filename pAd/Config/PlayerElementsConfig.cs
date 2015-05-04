using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pingvi {
    public class PlayerElementsConfig {

        public RectangleF Card1Rect { get; set; }
        public RectangleF Card2Rect { get; set; }

        public PixelPoint PlayerTypePoint { get; set; }
        public PixelPoint PlayerStatusPointGame { get; set; }
        public PixelPoint PlayerStatusPointHand { get; set; }
        public PixelPoint PlayerStatusPointSitOut { get; set; }
        public PixelPoint ButtonPoint { get; set; }

        public PixelPoint[] StackDigPosPoints { get; set; }
        public RectangleF[][] StackDigitsRectMass { get; set; }

        public PixelPoint[] BetDigPosPoints { get; set; }
        public RectangleF[][] BetDigitsRectMass { get; set; }


        //STATS

        //PREFLOP

        
        public PixelPoint[] PF_BTN_STEAL_StatDigPosPoints { get; set; }
        public RectangleF[][] PF_BTN_STEAL_StatDigitsRectMass { get; set; }

        public PixelPoint[] PF_SB_STEAL_StatDigPosPoints { get; set; }
        public RectangleF[][] PF_SB_STEAL_StatDigitsRectMass { get; set; }

        public PixelPoint[] PF_LIMP_FOLD_StatDigPosPoints { get; set; }
        public RectangleF[][] PF_LIMP_FOLD_StatDigitsRectMass { get; set; }

        public PixelPoint[] PF_FOLD_3BET_StatDigPosPoints { get; set; }
        public RectangleF[][] PF_FOLD_3BET_StatDigitsRectMass { get; set; }

        public PixelPoint[] PF_SB_3BET_VS_BTN_StatDigPosPoints { get; set; }
        public RectangleF[][] PF_SB_3BET_VS_BTN_StatDigitsRectMass { get; set; }

        public PixelPoint[] PF_BB_3BET_VS_BTN_StatDigPosPoints { get; set; }
        public RectangleF[][] PF_BB_3BET_VS_BTN_StatDigitsRectMass { get; set; }

        public PixelPoint[] PF_BB_3BET_VS_SB_StatDigPosPoints { get; set; }
        public RectangleF[][] PF_BB_3BET_VS_SB_StatDigitsRectMass { get; set; }

        public PixelPoint[] PF_BB_DEF_VS_SBSTEAL_StatDigPosPoints { get; set; }
        public RectangleF[][] PF_BB_DEF_VS_SBSTEAL_StatDigitsRectMass { get; set; }

        public PixelPoint[] PF_SB_OPENMINRAISE_StatDigPosPoints { get; set; }
        public RectangleF[][] PF_SB_OPENMINRAISE_StatDigitsRectMass { get; set; }

        //FLOP

        public PixelPoint[] F_CBET_StatDigPosPoints { get; set; }
        public RectangleF[][] F_CBET_StatDigitsRectMass { get; set; }

        public PixelPoint[] F_BET_LPOT_StatDigPosPoints { get; set; }
        public RectangleF[][] F_BET_LPOT_StatDigitsRectMass { get; set; }

        public PixelPoint[] F_CBET_FOLDRAISE_StatDigPosPoints { get; set; }
        public RectangleF[][] F_CBET_FOLDRAISE_StatDigitsRectMass { get; set; }

        public PixelPoint[] F_FOLD_CBET_StatDigPosPoints { get; set; }
        public RectangleF[][] F_FOLD_CBET_StatDigitsRectMass { get; set; }

        public PixelPoint[] F_RAISE_CBET_StatDigPosPoints { get; set; }
        public RectangleF[][] F_RAISE_CBET_StatDigitsRectMass { get; set; }

        public PixelPoint[] F_DONK_StatDigPosPoints { get; set; }
        public RectangleF[][] F_DONK_StatDigitsRectMass { get; set; }

        public PixelPoint[] F_DONK_FOLDRAISE_StatDigPosPoints { get; set; }
        public RectangleF[][] F_DONK_FOLDRAISE_StatDigitsRectMass { get; set; }

     



    

       

    }
}
