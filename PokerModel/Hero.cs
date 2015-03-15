using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerModel
{
    public enum HeroRole {
        None, Opener,Defender
    }
    public enum HeroStatePreflop {
        None, Open, FacingLimp, FacingOpen, Facing3Bet, FacingPush
    }

    public enum HeroStatePostflop {
        None, Cbet, FacingCbet, FacingDonk, FacingBetToCheck, FacingRaiseToCbet, FacingCheckRaise, MissedCbet
    }

    public enum HeroRelativePosition {
        None, InPosition, OutOfPosition
    }
    public  class Hero : Player {
        public HeroRole Role { get; set; }
        public HeroStatePreflop StatePreflop { get; set; }
        public HeroStatePostflop StatePostflop { get; set; }
        public HeroRelativePosition RelativePosition { get;set; }
        public bool IsHeroTurn { get; set; }
        public Hero(string name) : base(name) {
        }
    }
}
