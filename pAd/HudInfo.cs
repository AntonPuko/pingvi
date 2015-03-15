using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokerModel;

namespace Pingvi
{
    public enum Decision {
        None, Fold, Limp, OpenPush, OpenRaise, CallToPush , 
        CallToOpen, CallToLimp, PushToLimp, RaiseToLimp,  _3Bet4, _3Bet45, PushToOpen,
    }
    public class HudInfo {
        public Decision Decision { get; set; }
        public double EffectiveStack { get; set; }
        public double HandPlayability { get; set; }
        public HeroStatePreflop HeroStatePreflop { get; set; }

        public HeroStatePostflop HeroStateFlop { get; set; }
        public CurrentStreet CurrentStreet { get; set; }
        public double CurrentPot { get; set; }
        public HeroRelativePosition HeroRelativePosition { get; set; }

        public Player Opponent { get; set; }

        public double PotOdds { get; set; }

    }
}
