using System.Collections.Generic;
using PokerModel;

namespace Pingvi {
    public enum ButtonPosition {
        None,
        Hero,
        Left,
        Right
    }

    public enum CurrentStreet {
        Preflop,
        Flop,
        Turn,
        River
    }


    public class Elements {
        public Elements() {
            HeroPlayer = new Hero("Hero");
            LeftPlayer = new Player("LeftPlayer");
            RightPlayer = new Player("RightPlayer");
            EffectiveStack = 0.0;
            BbAmt = 0.0;
            SbAmt = 0.0;
        }

        public Card FlopCard1 { get; set; }
        public Card FlopCard2 { get; set; }
        public Card FlopCard3 { get; set; }
        public Card TurnCard { get; set; }
        public Card RiverCard { get; set; }
        public int? TourneyMultiplier { get; set; }
        public bool IsHU { get; set; }
        public Player HuOpp { get; set; }
        public double SbBtnEffStack { get; set; }
        public double TotalPot { get; set; }
        public CurrentStreet CurrentStreet { get; set; }
        public Hero HeroPlayer { get; set; }
        public Player LeftPlayer { get; set; }
        public Player RightPlayer { get; set; }
        public double BbAmt { get; set; }
        public double SbAmt { get; set; }
        public List<Player> ActivePlayers { get; set; }
        public List<Player> InGamePlayers { get; set; }
        public List<Player> SitOutPlayers { get; set; }
        public double EffectiveStack { get; set; }
        public ButtonPosition ButtonPosition { get; set; }
        public string BtnLine { get; set; }
        public string SbLine { get; set; }
        public string BbLine { get; set; }
    }
}