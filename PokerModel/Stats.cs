using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerModel {
    public struct Stats {

        //PREFLOP
        public double? PF_BTN_STEAL; //
        public double? PF_SB_STEAL; //
        public double? PF_LIMP_FOLD; //
        public double? PF_FOLD_3BET; //
        public double? PF_SB_3BET_VS_BTN;
        public double? PF_BB_3BET_VS_BTN;
        public double? PF_BB_3BET_VS_SB;
        public double? PF_BB_FOLD_VS_SBSTEAL;
        public double? PF_SB_OPENMINRAISE;

        //FLOP
        public double? F_CBET;
        public double? F_BET_LPOT; //
        public double? F_CBET_FOLDRAISE;
        public double? F_FOLD_CBET; //
        public double? F_RAISE_CBET; //
        public double? F_DONK;
        public double? F_DONK_FOLDRAISE;

    }
}
