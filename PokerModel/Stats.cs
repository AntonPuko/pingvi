using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerModel {
    public struct Stats {

        //FIRST PANEL
        public double? PfBtnSteal;
        public double? PfSbSteal;
        public double? PfOpenpush;
        public double? PfLimpFold;
        public double? PfLimpReraise; 
        public double? PfFold_3Bet;
        public double? PfBbDefVsSbsteal;
        public double? PfRaiseLimper;
        public double? PfSb_3BetVsBtn;
        public double? PfBb_3BetVsBtn;
        public double? PfBb_3BetVsSb;

        //SECOND PANEL
        public double? FCbet;
        public double? Cbet;
        public double? RCbet;
        public double? FFoldCbet;
        public double? FoldCbet;
        public double? RFoldCbet;
        public double? FCbetFoldraise;
        public double? CbetFoldraise;
        public double? FRaiseBet; 
        public double? RaiseBet; 
        public double? FLpSteal;


        //THIRD PANEL
        public double? FLpFoldVsSteal;
        public double? FLpFoldVsXr;
        public double? FCheckfoldOop;
        public double? SkipfFoldVsTProbe;
        public double? RSkiptFoldVsRProbe;
        public double? FDonk;
        public double? Donk;
        public double? FDonkFoldraise;

    }
}
