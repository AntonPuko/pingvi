using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerModel {
    public struct Stats {

        //FIRST PANEL
        public double? PF_BTN_STEAL;
        public double? PF_SB_STEAL;
        public double? PF_OPENPUSH;
        public double? PF_LIMP_FOLD;
        public double? PF_LIMP_RERAISE; 
        public double? PF_FOLD_3BET;
        public double? PF_BB_DEF_VS_SBSTEAL;
        public double? PF_RAISE_LIMPER;
        public double? PF_SB_3BET_VS_BTN;
        public double? PF_BB_3BET_VS_BTN;
        public double? PF_BB_3BET_VS_SB;

        //SECOND PANEL
        public double? F_CBET;
        public double? T_CBET;
        public double? R_CBET;
        public double? F_FOLD_CBET;
        public double? T_FOLD_CBET;
        public double? R_FOLD_CBET;
        public double? F_CBET_FOLDRAISE;
        public double? T_CBET_FOLDRAISE;
        public double? F_RAISE_BET; 
        public double? T_RAISE_BET; 
        public double? F_LP_STEAL;


        //THIRD PANEL
        public double? F_LP_FOLD_VS_STEAL;
        public double? F_LP_FOLD_VS_XR;
        public double? F_CHECKFOLD_OOP;
        public double? T_SKIPF_FOLD_VS_T_PROBE;
        public double? R_SKIPT_FOLD_VS_R_PROBE;
        public double? F_DONK;
        public double? T_DONK;
        public double? F_DONK_FOLDRAISE;

    }
}
