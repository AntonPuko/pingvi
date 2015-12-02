using System.Drawing;

namespace Pingvi
{
    public class PlayerElementsConfig
    {
        public Rectangle Card1Rect { get; set; }
        public Rectangle Card2Rect { get; set; }

        public PixelPoint PlayerTypePoint { get; set; }
        public PixelPoint PlayerStatusPointGame { get; set; }
        public PixelPoint PlayerStatusPointHand { get; set; }
        public PixelPoint PlayerStatusPointSitOut { get; set; }
        public PixelPoint ButtonPoint { get; set; }

        public PixelPoint[] StackDigPosPoints { get; set; }
        public Rectangle[][] StackDigitsRectMass { get; set; }

        public PixelPoint[] BetDigPosPoints { get; set; }
        public Rectangle[][] BetDigitsRectMass { get; set; }


        //LINE

        public PixelPoint[] LinePixelPositions { get; set; }
        //ALTER LINE
        public Rectangle[] LineRectPosition { get; set; }


        //STATS
        //FIRST PANEL
        public PixelPoint[] PfBtnStealStatDigPosPoints { get; set; }
        public Rectangle[][] PfBtnStealStatDigitsRectMass { get; set; }

        public PixelPoint[] PfSbStealStatDigPosPoints { get; set; }
        public Rectangle[][] PfSbStealStatDigitsRectMass { get; set; }

        public PixelPoint[] PfOpenpushStatDigPosPoints { get; set; }
        public Rectangle[][] PfOpenpushStatDigitsRectMass { get; set; }

        public PixelPoint[] PfLimpFoldStatDigPosPoints { get; set; }
        public Rectangle[][] PfLimpFoldStatDigitsRectMass { get; set; }

        public PixelPoint[] PfLimpReraiseStatDigPosPoints { get; set; }
        public Rectangle[][] PfLimpReraiseStatDigitsRectMass { get; set; }

        public PixelPoint[] PfFold_3BetStatDigPosPoints { get; set; }
        public Rectangle[][] PfFold_3BetStatDigitsRectMass { get; set; }

        public PixelPoint[] PfBbDefVsSbstealStatDigPosPoints { get; set; }
        public Rectangle[][] PfBbDefVsSbstealStatDigitsRectMass { get; set; }

        public PixelPoint[] PfRaiseLimperStatDigPosPoints { get; set; }
        public Rectangle[][] PfRaiseLimperStatDigitsRectMass { get; set; }

        public PixelPoint[] PfSb_3BetVsBtnStatDigPosPoints { get; set; }
        public Rectangle[][] PfSb_3BetVsBtnStatDigitsRectMass { get; set; }

        public PixelPoint[] PfBb_3BetVsBtnStatDigPosPoints { get; set; }
        public Rectangle[][] PfBb_3BetVsBtnStatDigitsRectMass { get; set; }

        public PixelPoint[] PfBb_3BetVsSbStatDigPosPoints { get; set; }
        public Rectangle[][] PfBb_3BetVsSbStatDigitsRectMass { get; set; }

        //SECOND PANEL
        public PixelPoint[] FCbetStatDigPosPoints { get; set; }
        public Rectangle[][] FCbetStatDigitsRectMass { get; set; }

        public PixelPoint[] CbetStatDigPosPoints { get; set; }
        public Rectangle[][] CbetStatDigitsRectMass { get; set; }

        public PixelPoint[] RCbetStatDigPosPoints { get; set; }
        public Rectangle[][] RCbetStatDigitsRectMass { get; set; }

        public PixelPoint[] FFoldCbetStatDigPosPoints { get; set; }
        public Rectangle[][] FFoldCbetStatDigitsRectMass { get; set; }

        public PixelPoint[] FoldCbetStatDigPosPoints { get; set; }
        public Rectangle[][] FoldCbetStatDigitsRectMass { get; set; }

        public PixelPoint[] RFoldCbetStatDigPosPoints { get; set; }
        public Rectangle[][] RFoldCbetStatDigitsRectMass { get; set; }

        public PixelPoint[] FCbetFoldraiseStatDigPosPoints { get; set; }
        public Rectangle[][] FCbetFoldraiseStatDigitsRectMass { get; set; }

        public PixelPoint[] CbetFoldraiseStatDigPosPoints { get; set; }
        public Rectangle[][] CbetFoldraiseStatDigitsRectMass { get; set; }

        public PixelPoint[] FRaiseBetStatDigPosPoints { get; set; }
        public Rectangle[][] FRaiseBetStatDigitsRectMass { get; set; }

        public PixelPoint[] RaiseBetStatDigPosPoints { get; set; }
        public Rectangle[][] RaiseBetStatDigitsRectMass { get; set; }

        public PixelPoint[] FLpStealStatDigPosPoints { get; set; }
        public Rectangle[][] FLpStealStatDigitsRectMass { get; set; }

        //THIRD PANEL
        public PixelPoint[] FLpFoldVsStealStatDigPosPoints { get; set; }
        public Rectangle[][] FLpFoldVsStealStatDigitsRectMass { get; set; }

        public PixelPoint[] FLpFoldVsXrStatDigPosPoints { get; set; }
        public Rectangle[][] FLpFoldVsXrStatDigitsRectMass { get; set; }

        public PixelPoint[] FCheckfoldOopStatDigPosPoints { get; set; }
        public Rectangle[][] FCheckfoldOopStatDigitsRectMass { get; set; }

        public PixelPoint[] SkipfFoldVsTProbeStatDigPosPoints { get; set; }
        public Rectangle[][] SkipfFoldVsTProbeStatDigitsRectMass { get; set; }

        public PixelPoint[] RSkiptFoldVsRProbeStatDigPosPoints { get; set; }
        public Rectangle[][] RSkiptFoldVsRProbeStatDigitsRectMass { get; set; }

        public PixelPoint[] FDonkStatDigPosPoints { get; set; }
        public Rectangle[][] FDonkStatDigitsRectMass { get; set; }

        public PixelPoint[] DonkStatDigPosPoints { get; set; }
        public Rectangle[][] DonkStatDigitsRectMass { get; set; }

        public PixelPoint[] FDonkFoldraiseStatDigPosPoints { get; set; }
        public Rectangle[][] FDonkFoldraiseStatDigitsRectMass { get; set; }
    }
}