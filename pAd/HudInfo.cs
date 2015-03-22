using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokerModel;

namespace Pingvi
{
    public enum DecisionPreflop {
        None,Fold,Limp,OpenRaise,Call, _3Bet,Push
    }
    public class HudInfo {
        public DecisionPreflop DecisionPreflop { get; set; }
        public double EffectiveStack { get; set; }
        public double HandRangeStat { get; set; }
        public HeroStatePreflop HeroStatePreflop { get; set; }

        public HeroStatePostflop HeroStateFlop { get; set; }
        public CurrentStreet CurrentStreet { get; set; }
        public double CurrentPot { get; set; }
        public HeroRelativePosition HeroRelativePosition { get; set; }

        public Player Opponent { get; set; }

        public double PotOdds { get; set; }

    }
}
