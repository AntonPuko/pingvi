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
        public PixelPoint[] SB_3BETvsBTN_StatDigPosPoints { get; set; }
        public RectangleF[][] SB_3BETvsBTN_StatDigitsRectMass { get; set; }

        public PixelPoint[] BB_3BETvsBTN_StatDigPosPoints { get; set; }
        public RectangleF[][] BB_3BETvsBTN_StatDigitsRectMass { get; set; }

        public PixelPoint[] BB_3BETvsSB_StatDigPosPoints { get; set; }
        public RectangleF[][] BB_3BETvsSB_StatDigitsRectMass { get; set; }

        public PixelPoint[] BB_DEFvsSBSTEAL_StatDigPosPoints { get; set; }
        public RectangleF[][] BB_DEFvsSBSTEAL_StatDigitsRectMass { get; set; }

        public PixelPoint[] FlopFoldCBIP_StatDigPosPoints { get; set; }
        public RectangleF[][] FlopFoldCBIP_StatDigitsRectMass { get; set; }

        public PixelPoint[] FlopRaiseCBIP_StatDigPosPoints { get; set; }
        public RectangleF[][] FlopRaiseCBIP_StatDigitsRectMass { get; set; }

        public PixelPoint[] FlopFoldCBOOP_StatDigPosPoints { get; set; }
        public RectangleF[][] FlopFoldCBOOP_StatDigitsRectMass { get; set; }

        public PixelPoint[] FlopRaiseCBOOP_StatDigPosPoints { get; set; }
        public RectangleF[][] FlopRaiseCBOOP_StatDigitsRectMass { get; set; }

        public PixelPoint[] FlopCbet_StatDigPosPoints { get; set; }
        public RectangleF[][] FlopCbet_StatDigitsRectMass { get; set; }

        public PixelPoint[] FlopCbFoldRIP_StatDigPosPoints { get; set; }
        public RectangleF[][] FlopCbFoldRIP_StatDigitsRectMass { get; set; }

        public PixelPoint[] FlopCbFoldROOP_StatDigPosPoints { get; set; }
        public RectangleF[][] FlopCbFoldROOP_StatDigitsRectMass { get; set; }

        public PixelPoint[] FlopDonkBet_StatDigPosPoints { get; set; }
        public RectangleF[][] FlopDonkBet_StatDigitsRectMass { get; set; }

        public PixelPoint[] FlopDonkFold_StatDigPosPoints { get; set; }
        public RectangleF[][] FlopDonkFold_StatDigitsRectMass { get; set; }



    

       

    }
}
