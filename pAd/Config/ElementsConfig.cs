using System;
using System.Collections.Generic;

using System.Drawing;
using AForge.Neuro;
using Color = System.Drawing.Color;

namespace Pingvi {
    //TODO сделать XML реализацию
    public class ElementsConfig {

        public CommonElementsConfig Common { get; set; }
        public HeroElementsConfig Hero { get; set; }
        public PlayerElementsConfig LeftPlayer { get; set; }
        public PlayerElementsConfig RightPlayer { get; set; }

        public PixelPoint HudWindowsPoint { get; set; }

        public Color[] HudWindowColors { get; set; }

        public  ElementsConfig() {
            Common = new CommonElementsConfig();
            Hero = new HeroElementsConfig();
            LeftPlayer = new PlayerElementsConfig();
            RightPlayer = new PlayerElementsConfig();
            InitElements();
            InitLists();

        }

        private void InitElements() {

            //pot
            Common.PotDigitsPath = @"Data\PotDigits\";
            Common.PotDigitsColor = Color.FromArgb(0, 0, 0);
            Common.PotDigPosPoints = new[] { new PixelPoint(409, 38), new PixelPoint(406, 38), new PixelPoint(400, 38) };
            Common.PotDigitsRectMass = new[] {
                new[] {new Rectangle(413, 39, 6, 6), new Rectangle(420, 39, 6, 6)},
                new[] {new Rectangle(410, 39, 6, 6), new Rectangle(417, 39, 6, 6), new Rectangle(424, 39, 6, 6),},
                new[] { new Rectangle(404, 39, 6, 6), new Rectangle(415, 39, 6, 6), new Rectangle(422, 39, 6, 6),
                    new Rectangle(429, 39, 6, 6)} };

            //CARDS
            Common.DeckPath = @"Data\Deck\";
            
            Common.FlopCard1Rect = new Rectangle(253, 175, 7, 4);
            Common.FlopCard2Rect = new Rectangle(317, 175, 7, 4);
            Common.FlopCard3Rect = new Rectangle(381, 175, 7, 4);
            Common.TurnCardRect = new Rectangle(445, 175, 7, 4);
            Common.RiverCardRect = new Rectangle(509, 175, 7, 4);

            Hero.Card1Rect = new Rectangle(378, 368, 7, 4);
            Hero.Card2Rect = new Rectangle(393, 372, 7, 4);
            //TODO Init LEftPlayer and RightPlayer cards if needs to

            //BUTTON
            Common.ButtonColor = Color.FromArgb(203, 196, 56);

            Hero.ButtonPoint        = new PixelPoint(396, 345);
            LeftPlayer.ButtonPoint  = new PixelPoint(142, 175);
            RightPlayer.ButtonPoint = new PixelPoint(667, 175);

            //BLINDS
            Common.Blinds.DigitsPath = @"Data\BlindsDigits\";
            Common.Blinds.DigitsPointColor = Color.FromArgb(255,255,255);
            Common.Blinds.DigitsPosPoints = new[] { new PixelPoint(235,417)};
            Common.Blinds.DigitsRectMass =   new[] {new Rectangle(262,414,8,5)};
            Common.Blinds.BigBlindsDoubleMass = new[]{20.0, 30.0, 40.0,60.0,80.0,100.0,120.0};
            Common.BlindMassStrings = new[] { @"$10/$20", @"$15/$30", @"$20/$40", @"$30/$60",
                @"$40/$80", @"$50/$100", @"$60/$120", @"$75/$150", @"$90/$180" };


            
            //PLAYER TYPE
            //TODO переписать метод определения типа игрока, сделать расширяемым
            LeftPlayer.PlayerTypePoint = new PixelPoint(144, 92);
            RightPlayer.PlayerTypePoint = new PixelPoint(663, 93);

            Common.FishColor    = Color.FromArgb(47, 220, 47);
            Common.WeakRegColor = Color.FromArgb(47, 134, 220);
            Common.GoodRegColor = Color.FromArgb(220, 134, 47);
            Common.GoodRegColor2 = Color.FromArgb(220, 47, 47);
            Common.GoodRegColor3 = Color.FromArgb(134, 47, 47);
            Common.GoodRegColor4 = Color.FromArgb(100, 53, 6);
            Common.UberRegColor = Color.FromArgb(134, 47, 47);
            Common.RockColor = Color.FromArgb(54, 53, 6);
            Common.ManiacColor = Color.FromArgb(86, 23, 100);

            //TOURNEY MULTIPLIER
            Common.MultiplierPixelPoint = new PixelPoint(791,390);
            Common.MultiplierColors = new Dictionary<int, Color> {
                {2, Color.FromArgb(4, 24, 47)},
                {4, Color.FromArgb(0, 60, 76)},
                {6, Color.FromArgb(0, 56, 32)},
                {10, Color.FromArgb(0, 79, 0)},
                {25, Color.FromArgb(55, 31, 76)},
                {120, Color.FromArgb(76, 7, 7)},
                {240, Color.FromArgb(76, 7, 7)},
                {3600, Color.FromArgb(5, 5, 5)}
            };


            //PLAYER STATUS
            Common.InGameColor = Color.FromArgb(194, 194, 194);
            Common.InHandColor    = Color.FromArgb(230,230,230);
            Common.SitOutColor = Color.FromArgb(192, 192, 192);

            Hero.PlayerStatusPointGame        = new PixelPoint(396, 473);
            LeftPlayer.PlayerStatusPointGame = new PixelPoint(55, 144);
            RightPlayer.PlayerStatusPointGame = new PixelPoint(755, 144);

            Hero.PlayerStatusPointHand = new PixelPoint(375, 443);
            LeftPlayer.PlayerStatusPointHand = new PixelPoint(181, 155);
            RightPlayer.PlayerStatusPointHand = new PixelPoint(602, 145);

            
            Hero.PlayerStatusPointSitOut = new PixelPoint(369, 456);
            LeftPlayer.PlayerStatusPointSitOut = new PixelPoint(27, 131);
            RightPlayer.PlayerStatusPointSitOut = new PixelPoint(716, 131);


            //HERO TURN
            Hero.IsTurnPoint = new PixelPoint(452, 542);
            Hero.IsTurnColor = Color.FromArgb(250, 207, 63);

            //PLAYER STACK
            Common.StackDigitsPath = @"Data\StackDigits\";
            Common.StackDigitsColor = Color.FromArgb(192, 192, 192);

            Hero.StackDigPosPoints = new[] { new PixelPoint(393, 455), new PixelPoint(389, 455), new PixelPoint(384, 455) };
            Hero.StackDigitsRectMass = new[] {
             new[] {new Rectangle(397, 456, 6, 6), new Rectangle(404, 456, 6, 6) }, 
             new[] {new Rectangle(393, 456, 6, 6), new Rectangle(400, 456, 6, 6), new Rectangle(407, 456, 6, 6)},
             new[] {new Rectangle(388, 456, 6, 6), new Rectangle(399, 456, 6, 6),
                    new Rectangle(406, 456, 6, 6), new Rectangle(413, 456, 6, 6)}};

            LeftPlayer.StackDigPosPoints = new[] { new PixelPoint(51, 130), new PixelPoint(47, 130), new PixelPoint(42, 130) };
            LeftPlayer.StackDigitsRectMass = new [] {
             new[] {new Rectangle(55, 131, 6, 6), new Rectangle(62, 131, 6, 6) }, 
             new[] {new Rectangle(51, 131, 6, 6), new Rectangle(58, 131, 6, 6), new Rectangle(65, 131, 6, 6)},
             new[] {new Rectangle(46, 131, 6, 6), new Rectangle(57, 131, 6, 6), 
                    new Rectangle(64, 131, 6, 6), new Rectangle(71, 131, 6, 6)}};

            RightPlayer.StackDigPosPoints = new[] { new PixelPoint(740, 130), new PixelPoint(736, 130), new PixelPoint(731, 130) };
            RightPlayer.StackDigitsRectMass = new[] {
             new[] {new Rectangle(744, 131, 6, 6), new Rectangle(751, 131, 6, 6)},
             new[] {new Rectangle(740, 131, 6, 6), new Rectangle(747, 131, 6, 6), new Rectangle(754, 131, 6, 6)},
             new[] {new Rectangle(735, 131, 6, 6), new Rectangle(747, 131, 6, 6),
                    new Rectangle(754, 131, 6, 6), new Rectangle(761, 131, 6, 6)}};
            

            //PLAYER BET
            Common.BetDigitsPath = @"Data\BetDigits\";
            Common.BetDigitsColor = Color.FromArgb(255, 246, 207);

            Hero.BetDigPosPoints = new[] {
                new PixelPoint(452, 320), new PixelPoint(446, 320), new PixelPoint(440, 320),
                new PixelPoint(429, 320), new PixelPoint(423, 320), new PixelPoint(417, 320),
                new PixelPoint(406, 320), new PixelPoint(400, 320), new PixelPoint(394, 320)};
            Hero.BetDigitsRectMass = new[] {
                new[] {new Rectangle(428, 320, 6, 6), new Rectangle(437, 320, 6, 6), new Rectangle(443, 320, 6, 6),new Rectangle(449, 320, 6, 6)},
                new[] {new Rectangle(431, 320, 6, 6), new Rectangle(437, 320, 6, 6), new Rectangle(443, 320, 6, 6)},
                new[] {new Rectangle(431, 320, 6, 6), new Rectangle(437, 320, 6, 6)},
                new[] {new Rectangle(405, 320, 6, 6), new Rectangle(414, 320, 6, 6), new Rectangle(420, 320, 6, 6),new Rectangle(426, 320, 6, 6)},
                new[] {new Rectangle(408, 320, 6, 6), new Rectangle(414, 320, 6, 6), new Rectangle(420, 320, 6, 6)},
                new[] {new Rectangle(408, 320, 6, 6), new Rectangle(414, 320, 6, 6)},
                new[] {new Rectangle(382, 320, 6, 6), new Rectangle(391, 320, 6, 6), new Rectangle(397, 320, 6, 6),new Rectangle(403, 320, 6, 6)},
                new[] {new Rectangle(385, 320, 6, 6), new Rectangle(391, 320, 6, 6), new Rectangle(397, 320, 6, 6)},
                new[] {new Rectangle(385, 320, 6, 6),new Rectangle(391, 320, 6, 6), },
            };
             
            LeftPlayer.BetDigPosPoints = new[] {
                new PixelPoint(263, 188), new PixelPoint(254, 188), new PixelPoint(248, 188),
                new PixelPoint(240, 188), new PixelPoint(231, 188), new PixelPoint(225, 188),
                new PixelPoint(214, 188), new PixelPoint(208, 188), new PixelPoint(202, 188)};

            LeftPlayer.BetDigitsRectMass = new[] {
                new[] {new Rectangle(239, 188, 6, 6), new Rectangle(248, 188, 6, 6), new Rectangle(254, 188, 6, 6),new Rectangle(260, 188, 6, 6)},
                new[] {new Rectangle(239, 188, 6, 6), new Rectangle(245, 188, 6, 6), new Rectangle(251, 188, 6, 6)},
                new[] {new Rectangle(239, 188, 6, 6), new Rectangle(245, 188, 6, 6)},
                new[] {new Rectangle(216, 188, 6, 6), new Rectangle(225, 188, 6, 6), new Rectangle(231, 188, 6, 6),new Rectangle(237, 188, 6, 6)},
                new[] {new Rectangle(216, 188, 6, 6), new Rectangle(222, 188, 6, 6), new Rectangle(228, 188, 6, 6)},
                new[] {new Rectangle(216, 188, 6, 6), new Rectangle(222, 188, 6, 6)},
                new[] {new Rectangle(189, 188, 6, 6), new Rectangle(199, 188, 6, 6), new Rectangle(205, 188, 6, 6),new Rectangle(211, 188, 6, 6)},
                new[] {new Rectangle(193, 188, 6, 6), new Rectangle(199, 188, 6, 6), new Rectangle(205, 188, 6, 6)},
                new[] {new Rectangle(193, 188, 6, 6),new Rectangle(199, 188, 6, 6), },
            };


            RightPlayer.BetDigPosPoints = new[] {
                new PixelPoint(596, 195), new PixelPoint(573, 195), new PixelPoint(550, 195)};

            RightPlayer.BetDigitsRectMass = new[] {
                new[] {new Rectangle(572, 195, 6, 6), new Rectangle(581, 195, 6, 6), new Rectangle(587, 195, 6, 6), new Rectangle(593, 195, 6, 6)},
                new[] {new Rectangle(549, 195, 6, 6), new Rectangle(558, 195, 6, 6), new Rectangle(564, 195, 6, 6),new Rectangle(570, 195, 6, 6)},
                new[] {new Rectangle(526, 195, 6, 6), new Rectangle(535, 195, 6, 6), new Rectangle(541, 195, 6, 6),new Rectangle(547, 195, 6, 6)},};
            

            //LINE
            Common.LineLettersDictionary = new Dictionary<Color, string>();
            Common.LineLettersDictionary.Add(Color.FromArgb(128, 128, 128), "|");
            Common.LineLettersDictionary.Add(Color.FromArgb(64, 128, 128), "f");
            Common.LineLettersDictionary.Add(Color.FromArgb(192, 192, 192), "x");
            Common.LineLettersDictionary.Add(Color.FromArgb(255, 128, 255), "l");
            Common.LineLettersDictionary.Add(Color.FromArgb(255, 255, 0), "c");
            Common.LineLettersDictionary.Add(Color.FromArgb(255, 128, 64), "b");
            Common.LineLettersDictionary.Add(Color.FromArgb(255, 128, 128), "r");

            Common.LinePixelPositions = new[] {
                new PixelPoint(7, 8), new PixelPoint(16, 8), new PixelPoint(25, 8), new PixelPoint(33, 8),
                new PixelPoint(42, 8), new PixelPoint(51, 8), new PixelPoint(59, 8), new PixelPoint(69, 8),
                new PixelPoint(77, 8), new PixelPoint(84, 8)};

            Hero.LinePixelPositions = CountLinePixelPositions(Common.LinePixelPositions, new PixelPoint(462, 405));
            LeftPlayer.LinePixelPositions = CountLinePixelPositions(Common.LinePixelPositions, new PixelPoint(226, 82));
            RightPlayer.LinePixelPositions = CountLinePixelPositions(Common.LinePixelPositions, new PixelPoint(468, 82));



            //LINE ALTER
            Common.LineNetworkPath = @"Data\network.net";
            Common.LineNetwork = Network.Load(Common.LineNetworkPath) as ActivationNetwork;

            Common.LineRectPositions = new[] {
                new Rectangle(4,10,7,9), new Rectangle(12,10,7,9), new Rectangle(20,10,7,9), new Rectangle(28,10,7,9),
                new Rectangle(36,10,7,9), new Rectangle(44,10,7,9), new Rectangle(52,10,7,9), new Rectangle(60,10,7,9),
                new Rectangle(68,10,7,9), new Rectangle(74,10,7,9)};

            Hero.LineRectPosition = CountPlayerLineRectPositions(Common.LineRectPositions, new PixelPoint(462, 405));
            LeftPlayer.LineRectPosition = CountPlayerLineRectPositions(Common.LineRectPositions, new PixelPoint(225, 82));
            RightPlayer.LineRectPosition = CountPlayerLineRectPositions(Common.LineRectPositions, new PixelPoint(467, 82));


            //STATISTICS
            Common.StatsDigitsPath = @"Data\StatsDigits\";
            Common.StatsDigitsColor = Color.FromArgb(25, 70, 42);

            //LEFT
              //FRIST PANEL
            LeftPlayer.PF_BTN_STEAL_StatDigPosPoints = new[] { new PixelPoint(21, 197), new PixelPoint(16, 197) };
            LeftPlayer.PF_BTN_STEAL_StatDigitsRectMass = new[] {
                new [] {new Rectangle(10,190,5,4), new Rectangle(15,190,5,4) }, 
                new [] {new Rectangle(10,190,5,4)}};

            LeftPlayer.PF_SB_STEAL_StatDigPosPoints = new[] { new PixelPoint(21, 208), new PixelPoint(16, 208) };
            LeftPlayer.PF_SB_STEAL_StatDigitsRectMass = new[] {
                new [] {new Rectangle(10,201,5,4), new Rectangle(15,201,5,4) }, 
                new [] {new Rectangle(10,201,5,4)}};

            LeftPlayer.PF_OPENPUSH_StatDigPosPoints = new[] { new PixelPoint(21, 218), new PixelPoint(16, 218) };
            LeftPlayer.PF_OPENPUSH_StatDigitsRectMass = new[] {
                new [] {new Rectangle(10,211,5,4), new Rectangle(15,211,5,4) }, 
                new [] {new Rectangle(10,211,5,4)}};

            LeftPlayer.PF_LIMP_FOLD_StatDigPosPoints = new[] { new PixelPoint(21, 229), new PixelPoint(16, 229) };
            LeftPlayer.PF_LIMP_FOLD_StatDigitsRectMass = new[] {
                new [] {new Rectangle(10,222,5,4), new Rectangle(15,222,5,4) }, 
                new [] {new Rectangle(10,222,5,4)}};

            LeftPlayer.PF_LIMP_RERAISE_StatDigPosPoints = new[] { new PixelPoint(21, 239), new PixelPoint(16, 239) };
            LeftPlayer.PF_LIMP_RERAISE_StatDigitsRectMass = new[] {
                new [] {new Rectangle(10,232,5,4), new Rectangle(15,232,5,4) }, 
                new [] {new Rectangle(10,232,5,4)}};

            LeftPlayer.PF_FOLD_3BET_StatDigPosPoints = new[] { new PixelPoint(21, 250), new PixelPoint(16, 250) };
            LeftPlayer.PF_FOLD_3BET_StatDigitsRectMass = new[] {
                new [] {new Rectangle(10,243,5,4), new Rectangle(15,243,5,4) }, 
                new [] {new Rectangle(10,243,5,4)}};

            LeftPlayer.PF_BB_DEF_VS_SBSTEAL_StatDigPosPoints = new[] { new PixelPoint(21, 260), new PixelPoint(16, 260) };
            LeftPlayer.PF_BB_DEF_VS_SBSTEAL_StatDigitsRectMass = new[] {
                new [] {new Rectangle(10,253,5,4), new Rectangle(15,253,5,4) }, 
                new [] {new Rectangle(10,253,5,4)}};


            LeftPlayer.PF_RAISE_LIMPER_StatDigPosPoints = new[] { new PixelPoint(21, 271), new PixelPoint(16, 271) };
            LeftPlayer.PF_RAISE_LIMPER_StatDigitsRectMass = new[] {
                new [] {new Rectangle(10,264,5,4), new Rectangle(15,264,5,4) }, 
                new [] {new Rectangle(10,264,5,4)}};

            LeftPlayer.PF_SB_3BET_VS_BTN_StatDigPosPoints = new[] { new PixelPoint(21, 281), new PixelPoint(16, 281) };
            LeftPlayer.PF_SB_3BET_VS_BTN_StatDigitsRectMass = new[] {
                new [] {new Rectangle(10,274,5,4), new Rectangle(15,274,5,4) }, 
                new [] {new Rectangle(10,274,5,4)}};

            LeftPlayer.PF_BB_3BET_VS_BTN_StatDigPosPoints = new[] { new PixelPoint(21, 292), new PixelPoint(16, 292) };
            LeftPlayer.PF_BB_3BET_VS_BTN_StatDigitsRectMass = new[] {
                new [] {new Rectangle(10,285,5,4), new Rectangle(15,285,5,4) }, 
                new [] {new Rectangle(10,285,5,4)}};

            LeftPlayer.PF_BB_3BET_VS_SB_StatDigPosPoints = new[] { new PixelPoint(21, 302), new PixelPoint(16, 302) };
            LeftPlayer.PF_BB_3BET_VS_SB_StatDigitsRectMass = new[] {
                new [] {new Rectangle(10,295,5,4), new Rectangle(15,295,5,4) }, 
                new [] {new Rectangle(10,295,5,4)}};

            //SECOND PANEL
            LeftPlayer.F_CBET_StatDigPosPoints = new[] { new PixelPoint(43, 197), new PixelPoint(38, 197) };
            LeftPlayer.F_CBET_StatDigitsRectMass = new[] {
                new [] {new Rectangle(32,190,5,4), new Rectangle(37,190,5,4) }, 
                new [] {new Rectangle(32,190,5,4)}};
            
            LeftPlayer.T_CBET_StatDigPosPoints = new[] { new PixelPoint(43, 208), new PixelPoint(38, 208) };
            LeftPlayer.T_CBET_StatDigitsRectMass = new[] {
                new [] {new Rectangle(32,201,5,4), new Rectangle(37,201,5,4) }, 
                new [] {new Rectangle(32,201,5,4)}};
           
            LeftPlayer.R_CBET_StatDigPosPoints = new[] { new PixelPoint(43, 218), new PixelPoint(38, 218) };
            LeftPlayer.R_CBET_StatDigitsRectMass = new[] {
                new [] {new Rectangle(32,211,5,4), new Rectangle(37,211,5,4) }, 
                new [] {new Rectangle(32,211,5,4)}};
            
            LeftPlayer.F_FOLD_CBET_StatDigPosPoints = new[] { new PixelPoint(43, 229), new PixelPoint(38, 229) };
            LeftPlayer.F_FOLD_CBET_StatDigitsRectMass = new[] {
                new [] {new Rectangle(32,222,5,4), new Rectangle(37,222,5,4) }, 
                new [] {new Rectangle(32,222,5,4)}};
          
            LeftPlayer.T_FOLD_CBET_StatDigPosPoints = new[] { new PixelPoint(43, 239), new PixelPoint(38, 239) };
            LeftPlayer.T_FOLD_CBET_StatDigitsRectMass = new[] {
                new [] {new Rectangle(32,232,5,4), new Rectangle(37,232,5,4) }, 
                new [] {new Rectangle(32,232,5,4)}};
            
            LeftPlayer.R_FOLD_CBET_StatDigPosPoints = new[] { new PixelPoint(43, 250), new PixelPoint(38, 250) };
            LeftPlayer.R_FOLD_CBET_StatDigitsRectMass = new[] {
                new [] {new Rectangle(32,243,5,4), new Rectangle(37,243,5,4) }, 
                new [] {new Rectangle(32,243,5,4)}};
           
            LeftPlayer.F_CBET_FOLDRAISE_StatDigPosPoints = new[] { new PixelPoint(43, 260), new PixelPoint(38, 260) };
            LeftPlayer.F_CBET_FOLDRAISE_StatDigitsRectMass = new[] {
                new [] {new Rectangle(32,253,5,4), new Rectangle(37,253,5,4) }, 
                new [] {new Rectangle(32,253,5,4)}};
            
            LeftPlayer.T_CBET_FOLDRAISE_StatDigPosPoints = new[] { new PixelPoint(43, 271), new PixelPoint(38, 271) };
            LeftPlayer.T_CBET_FOLDRAISE_StatDigitsRectMass = new[] {
                new [] {new Rectangle(32,264,5,4), new Rectangle(37,264,5,4) }, 
                new [] {new Rectangle(32,264,5,4)}};
            
            LeftPlayer.F_RAISE_BET_StatDigPosPoints = new[] { new PixelPoint(43, 281), new PixelPoint(38, 281) };
            LeftPlayer.F_RAISE_BET_StatDigitsRectMass = new[] {
                new [] {new Rectangle(32,274,5,4), new Rectangle(37,274,5,4) }, 
                new [] {new Rectangle(32,274,5,4)}};
       
            LeftPlayer.T_RAISE_BET_StatDigPosPoints = new[] { new PixelPoint(43, 292), new PixelPoint(38, 292) };
            LeftPlayer.T_RAISE_BET_StatDigitsRectMass = new[] {
                new [] {new Rectangle(32,285,5,4), new Rectangle(37,285,5,4) }, 
                new [] {new Rectangle(32,285,5,4)}};
            
            LeftPlayer.F_LP_STEAL_StatDigPosPoints = new[] { new PixelPoint(43, 302), new PixelPoint(38, 302) };
            LeftPlayer.F_LP_STEAL_StatDigitsRectMass = new[] {
                new [] {new Rectangle(32,295,5,4), new Rectangle(37,295,5,4) }, 
                new [] {new Rectangle(32,295,5,4)}};

            //THIRD PANEL
            LeftPlayer.F_LP_FOLD_VS_STEAL_StatDigPosPoints = new[] { new PixelPoint(65, 197), new PixelPoint(60, 197) };
            LeftPlayer.F_LP_FOLD_VS_STEAL_StatDigitsRectMass = new[] {
                new [] {new Rectangle(54,190,5,4), new Rectangle(59,190,5,4) }, 
                new [] {new Rectangle(54,190,5,4)}};
            
            LeftPlayer.F_LP_FOLD_VS_XR_StatDigPosPoints = new[] { new PixelPoint(65, 208), new PixelPoint(60, 20) };
            LeftPlayer.F_LP_FOLD_VS_XR_StatDigitsRectMass = new[] {
                new [] {new Rectangle(54,201,5,4), new Rectangle(59,201,5,4) }, 
                new [] {new Rectangle(54,201,5,4)}};
         
            LeftPlayer.F_CHECKFOLD_OOP_StatDigPosPoints = new[] { new PixelPoint(65, 218), new PixelPoint(60, 218) };
            LeftPlayer.F_CHECKFOLD_OOP_StatDigitsRectMass = new[] {
                new [] {new Rectangle(54,211,5,4), new Rectangle(59,211,5,4) }, 
                new [] {new Rectangle(54,211,5,4)}};
            
            LeftPlayer.T_SKIPF_FOLD_VS_T_PROBE_StatDigPosPoints = new[] { new PixelPoint(65, 229), new PixelPoint(60, 229) };
            LeftPlayer.T_SKIPF_FOLD_VS_T_PROBE_StatDigitsRectMass = new[] {
                new [] {new Rectangle(54,222,5,4), new Rectangle(59,222,5,4) }, 
                new [] {new Rectangle(54,222,5,4)}};
            
            LeftPlayer.R_SKIPT_FOLD_VS_R_PROBE_StatDigPosPoints = new[] { new PixelPoint(65, 239), new PixelPoint(60, 239) };
            LeftPlayer.R_SKIPT_FOLD_VS_R_PROBE_StatDigitsRectMass = new[] {
                new [] {new Rectangle(54,232,5,4), new Rectangle(59,232,5,4) }, 
                new [] {new Rectangle(54,232,5,4)}};
            
            LeftPlayer.F_DONK_StatDigPosPoints = new[] { new PixelPoint(65, 250), new PixelPoint(60, 250) };
            LeftPlayer.F_DONK_StatDigitsRectMass = new[] {
                new [] {new Rectangle(54,243,5,4), new Rectangle(59,243,5,4) }, 
                new [] {new Rectangle(54,243,5,4)}};
           
            LeftPlayer.T_DONK_StatDigPosPoints = new[] { new PixelPoint(65, 260), new PixelPoint(60, 260) };
            LeftPlayer.T_DONK_StatDigitsRectMass = new[] {
                new [] {new Rectangle(54,253,5,4), new Rectangle(59,253,5,4) }, 
                new [] {new Rectangle(54,253,5,4)}};
          
            LeftPlayer.F_DONK_FOLDRAISE_StatDigPosPoints = new[] { new PixelPoint(65, 271), new PixelPoint(60, 271) };
            LeftPlayer.F_DONK_FOLDRAISE_StatDigitsRectMass = new[] {
                new [] {new Rectangle(54,264,5,4), new Rectangle(59,264,5,4) }, 
                new [] {new Rectangle(54,264,5,4)}};
            
            //RIGHT
            //FIRST PANEL
            RightPlayer.PF_BTN_STEAL_StatDigPosPoints = new[] { new PixelPoint(726, 197), new PixelPoint(721, 197) };
            RightPlayer.PF_BTN_STEAL_StatDigitsRectMass = new[] {
                new [] {new Rectangle(715,190,5,4), new Rectangle(720,190,5,4) }, 
                new [] {new Rectangle(715,190,5,4)}};

            RightPlayer.PF_SB_STEAL_StatDigPosPoints = new[] { new PixelPoint(726, 207), new PixelPoint(721, 207) };
            RightPlayer.PF_SB_STEAL_StatDigitsRectMass = new[] {
                new [] {new Rectangle(715,200,5,4), new Rectangle(720,200,5,4) }, 
                new [] {new Rectangle(715,200,5,4)}};
      
            RightPlayer.PF_OPENPUSH_StatDigPosPoints = new[] { new PixelPoint(726, 218), new PixelPoint(721, 218) };
            RightPlayer.PF_OPENPUSH_StatDigitsRectMass = new[] {
                new [] {new Rectangle(715,211,5,4), new Rectangle(720,211,5,4) }, 
                new [] {new Rectangle(715,211,5,4)}};
          
            RightPlayer.PF_LIMP_FOLD_StatDigPosPoints = new[] { new PixelPoint(726, 228), new PixelPoint(721, 228) };
            RightPlayer.PF_LIMP_FOLD_StatDigitsRectMass = new[] {
                new [] {new Rectangle(715,221,5,4), new Rectangle(720,221,5,4) }, 
                new [] {new Rectangle(715,221,5,4)}};
           
            RightPlayer.PF_LIMP_RERAISE_StatDigPosPoints = new[] { new PixelPoint(726, 239), new PixelPoint(721, 239) };
            RightPlayer.PF_LIMP_RERAISE_StatDigitsRectMass = new[] {
                new [] {new Rectangle(715,232,5,4), new Rectangle(720,232,5,4) }, 
                new [] {new Rectangle(715,232,5,4)}};
            
            RightPlayer.PF_FOLD_3BET_StatDigPosPoints = new[] { new PixelPoint(726, 249), new PixelPoint(721, 249) };
            RightPlayer.PF_FOLD_3BET_StatDigitsRectMass = new[] {
                new [] {new Rectangle(715,242,5,4), new Rectangle(720,242,5,4) }, 
                new [] {new Rectangle(715,242,5,4)}};
            
            RightPlayer.PF_BB_DEF_VS_SBSTEAL_StatDigPosPoints = new[] { new PixelPoint(726, 260), new PixelPoint(721, 260) };
            RightPlayer.PF_BB_DEF_VS_SBSTEAL_StatDigitsRectMass = new[] {
                new [] {new Rectangle(715,253,5,4), new Rectangle(720,253,5,4) }, 
                new [] {new Rectangle(715,253,5,4)}};
            
            RightPlayer.PF_RAISE_LIMPER_StatDigPosPoints = new[] { new PixelPoint(726, 270), new PixelPoint(721, 270) };
            RightPlayer.PF_RAISE_LIMPER_StatDigitsRectMass = new[] {
                new [] {new Rectangle(715,263,5,4), new Rectangle(720,263,5,4) }, 
                new [] {new Rectangle(715,263,5,4)}};
            
            RightPlayer.PF_SB_3BET_VS_BTN_StatDigPosPoints = new[] { new PixelPoint(726, 281), new PixelPoint(721, 281) };
            RightPlayer.PF_SB_3BET_VS_BTN_StatDigitsRectMass = new[] {
                new [] {new Rectangle(715,274,5,4), new Rectangle(720,274,5,4) }, 
                new [] {new Rectangle(715,274,5,4)}};
            
            RightPlayer.PF_BB_3BET_VS_BTN_StatDigPosPoints = new[] { new PixelPoint(726, 291), new PixelPoint(721, 291) };
            RightPlayer.PF_BB_3BET_VS_BTN_StatDigitsRectMass = new[] {
                new [] {new Rectangle(715,284,5,4), new Rectangle(720,284,5,4) }, 
                new [] {new Rectangle(715,284,5,4)}};
            
            RightPlayer.PF_BB_3BET_VS_SB_StatDigPosPoints = new[] { new PixelPoint(726, 302), new PixelPoint(721, 302) };
            RightPlayer.PF_BB_3BET_VS_SB_StatDigitsRectMass = new[] {
                new [] {new Rectangle(715,295,5,4), new Rectangle(720,295,5,4) }, 
                new [] {new Rectangle(715,295,5,4)}};

            //RIGHT SECOND PANEL
            RightPlayer.F_CBET_StatDigPosPoints = new[] { new PixelPoint(748, 197), new PixelPoint(743, 197) };
            RightPlayer.F_CBET_StatDigitsRectMass = new[] {
                new [] {new Rectangle(737,190,5,4), new Rectangle(742,190,5,4) }, 
                new [] {new Rectangle(737,190,5,4)}};
            
            RightPlayer.T_CBET_StatDigPosPoints = new[] { new PixelPoint(748, 207), new PixelPoint(743, 207) };
            RightPlayer.T_CBET_StatDigitsRectMass = new[] {
                new [] {new Rectangle(737,200,5,4), new Rectangle(742,200,5,4) }, 
                new [] {new Rectangle(737,200,5,4)}};
            
            RightPlayer.R_CBET_StatDigPosPoints = new[] { new PixelPoint(748, 218), new PixelPoint(743, 218) };
            RightPlayer.R_CBET_StatDigitsRectMass = new[] {
                new [] {new Rectangle(737,211,5,4), new Rectangle(742,211,5,4) }, 
                new [] {new Rectangle(737,211,5,4)}};
            
            RightPlayer.F_FOLD_CBET_StatDigPosPoints = new[] { new PixelPoint(748, 228), new PixelPoint(743, 228) };
            RightPlayer.F_FOLD_CBET_StatDigitsRectMass = new[] {
                new [] {new Rectangle(737,221,5,4), new Rectangle(742,221,5,4) }, 
                new [] {new Rectangle(737,221,5,4)}};
            
            RightPlayer.T_FOLD_CBET_StatDigPosPoints = new[] { new PixelPoint(748, 239), new PixelPoint(743, 239) };
            RightPlayer.T_FOLD_CBET_StatDigitsRectMass = new[] {
                new [] {new Rectangle(737,232,5,4), new Rectangle(742,232,5,4) }, 
                new [] {new Rectangle(737,232,5,4)}};
            
            RightPlayer.R_FOLD_CBET_StatDigPosPoints = new[] { new PixelPoint(748, 249), new PixelPoint(743, 249) };
            RightPlayer.R_FOLD_CBET_StatDigitsRectMass = new[] {
                new [] {new Rectangle(737,242,5,4), new Rectangle(742,242,5,4) }, 
                new [] {new Rectangle(737,242,5,4)}};
            
            RightPlayer.F_CBET_FOLDRAISE_StatDigPosPoints = new[] { new PixelPoint(748, 260), new PixelPoint(743, 260) };
            RightPlayer.F_CBET_FOLDRAISE_StatDigitsRectMass = new[] {
                new [] {new Rectangle(737,253,5,4), new Rectangle(742,253,5,4) }, 
                new [] {new Rectangle(737,253,5,4)}};
            
            RightPlayer.T_CBET_FOLDRAISE_StatDigPosPoints = new[] { new PixelPoint(748, 270), new PixelPoint(743, 270) };
            RightPlayer.T_CBET_FOLDRAISE_StatDigitsRectMass = new[] {
                new [] {new Rectangle(737,263,5,4), new Rectangle(742,263,5,4) }, 
                new [] {new Rectangle(737,263,5,4)}};
            
            RightPlayer.F_RAISE_BET_StatDigPosPoints = new[] { new PixelPoint(748, 281), new PixelPoint(743, 281) };
            RightPlayer.F_RAISE_BET_StatDigitsRectMass = new[] {
                new [] {new Rectangle(737,274,5,4), new Rectangle(742,274,5,4) }, 
                new [] {new Rectangle(737,274,5,4)}};
            
            RightPlayer.T_RAISE_BET_StatDigPosPoints = new[] { new PixelPoint(748, 291), new PixelPoint(743, 291) };
            RightPlayer.T_RAISE_BET_StatDigitsRectMass = new[] {
                new [] {new Rectangle(737,284,5,4), new Rectangle(742,284,5,4) }, 
                new [] {new Rectangle(737,284,5,4)}};
            
            RightPlayer.F_LP_STEAL_StatDigPosPoints = new[] { new PixelPoint(748, 302), new PixelPoint(743, 302) };
            RightPlayer.F_LP_STEAL_StatDigitsRectMass = new[] {
                new [] {new Rectangle(737,295,5,4), new Rectangle(742,295,5,4) }, 
                new [] {new Rectangle(737,295,5,4)}};

            //RIGHT THIRD PANEL
            RightPlayer.F_LP_FOLD_VS_STEAL_StatDigPosPoints = new[] { new PixelPoint(770, 197), new PixelPoint(765, 197) };
            RightPlayer.F_LP_FOLD_VS_STEAL_StatDigitsRectMass = new[] {
                new [] {new Rectangle(759,190,5,4), new Rectangle(764,190,5,4) }, 
                new [] {new Rectangle(759,190,5,4)}};
            
            RightPlayer.F_LP_FOLD_VS_XR_StatDigPosPoints = new[] { new PixelPoint(770, 207), new PixelPoint(765, 207) };
            RightPlayer.F_LP_FOLD_VS_XR_StatDigitsRectMass = new[] {
                new [] {new Rectangle(759,200,5,4), new Rectangle(764,200,5,4) }, 
                new [] {new Rectangle(759,200,5,4)}};
            
            RightPlayer.F_CHECKFOLD_OOP_StatDigPosPoints = new[] { new PixelPoint(770, 218), new PixelPoint(765, 218) };
            RightPlayer.F_CHECKFOLD_OOP_StatDigitsRectMass = new[] {
                new [] {new Rectangle(759,211,5,4), new Rectangle(764,211,5,4) }, 
                new [] {new Rectangle(759,211,5,4)}};
            
            RightPlayer.T_SKIPF_FOLD_VS_T_PROBE_StatDigPosPoints = new[] { new PixelPoint(770, 228), new PixelPoint(765, 228) };
            RightPlayer.T_SKIPF_FOLD_VS_T_PROBE_StatDigitsRectMass = new[] {
                new [] {new Rectangle(759,221,5,4), new Rectangle(764,221,5,4) }, 
                new [] {new Rectangle(759,221,5,4)}};
            
            RightPlayer.R_SKIPT_FOLD_VS_R_PROBE_StatDigPosPoints = new[] { new PixelPoint(770, 239), new PixelPoint(765, 239) };
            RightPlayer.R_SKIPT_FOLD_VS_R_PROBE_StatDigitsRectMass = new[] {
                new [] {new Rectangle(759,232,5,4), new Rectangle(764,232,5,4) }, 
                new [] {new Rectangle(759,232,5,4)}};
            
            RightPlayer.F_DONK_StatDigPosPoints = new[] { new PixelPoint(770, 249), new PixelPoint(765, 249) };
            RightPlayer.F_DONK_StatDigitsRectMass = new[] {
                new [] {new Rectangle(759,242,5,4), new Rectangle(764,242,5,4) }, 
                new [] {new Rectangle(759,242,5,4)}};
            
            RightPlayer.T_DONK_StatDigPosPoints = new[] { new PixelPoint(770, 260), new PixelPoint(765, 260) };
            RightPlayer.T_DONK_StatDigitsRectMass = new[] {
                new [] {new Rectangle(759,253,5,4), new Rectangle(764,253,5,4) }, 
                new [] {new Rectangle(759,253,5,4)}};
            
            RightPlayer.F_DONK_FOLDRAISE_StatDigPosPoints = new[] { new PixelPoint(770, 270), new PixelPoint(765, 270) };
            RightPlayer.F_DONK_FOLDRAISE_StatDigitsRectMass = new[] {
                new [] {new Rectangle(759,263,5,4), new Rectangle(764,263,5,4) }, 
                new [] {new Rectangle(759,263,5,4)}};



      

        }

        private void InitLists() {
            InitDeckList();
            InitBlindsList();
            InitStackDigitsList();
            InitBetDigitsList();
            InitPotDigitsList();
            InitStatsDigitsList();
        
        }


        
        //TODO сделать универсальный метод для инициализации листов.
        private void InitDeckList() {
            Common.DeckList = new List<Bitmap>();
            for (int i = 1; i <= 52; i++) {
                Bitmap bmp = new Bitmap(String.Format(@"{0}{1}.bmp",Common.DeckPath, i));
                Common.DeckList.Add(bmp);
            }
        }

        
        private void InitBlindsList() {
            Common.Blinds.DigitsList = new List<Bitmap>();
            for (int i = 0; i <= 5; i++) {
                Bitmap bmpActive = new Bitmap(String.Format(@"{0}{1}.bmp", Common.Blinds.DigitsPath, i));
                Common.Blinds.DigitsList.Add(bmpActive);
            }
        }
         

        private void InitStackDigitsList() {
            Common.StackDigitsList = new List<Bitmap>();
            for (int i = 0; i <= 9; i++) {
                Bitmap bmp = new Bitmap(String.Format(@"{0}{1}.bmp", Common.StackDigitsPath, i));
                Common.StackDigitsList.Add(bmp);
            }
        }

        private void InitBetDigitsList() {
            Common.BetDigitsList = new List<Bitmap>();
            for (int i = 0; i <= 9; i++) {
                Bitmap bmp = new Bitmap(String.Format(@"{0}{1}.bmp", Common.BetDigitsPath, i));
                Common.BetDigitsList.Add(bmp);
            }
           
        }

        private void InitPotDigitsList()
        {
            Common.PotDigitsList = new List<Bitmap>();
            for (int i = 0; i <= 9; i++) {
                Bitmap bmp = new Bitmap(String.Format(@"{0}{1}.bmp", Common.PotDigitsPath, i));
                Common.PotDigitsList.Add(bmp);
            }

        }

        private void InitStatsDigitsList()
        {
            Common.StatsDigitsList = new List<Bitmap>();
            for (int i = 0; i <= 9; i++)
            {
                Bitmap bmp = new Bitmap(String.Format(@"{0}{1}.bmp", Common.StatsDigitsPath, i));
                Common.StatsDigitsList.Add(bmp);
            }

        }

        private PixelPoint[] CountLinePixelPositions(PixelPoint[] commonLinePixelPositions,
          PixelPoint playerCornerPixelPoint){
            PixelPoint[] playerPixelPointsMass = new PixelPoint[commonLinePixelPositions.Length];

            for (int i = 0; i < commonLinePixelPositions.Length; i++)
            {
                playerPixelPointsMass[i] = new PixelPoint(commonLinePixelPositions[i].X + playerCornerPixelPoint.X,
                    commonLinePixelPositions[i].Y + playerCornerPixelPoint.Y);
            }

            return playerPixelPointsMass;

        }
           
            
        

        private Rectangle[] CountPlayerLineRectPositions(Rectangle[] commonLineRectPositions,
            PixelPoint playerCornerPixelPoint) {
            
            Rectangle[] playerLineRectPositionMass = new Rectangle[commonLineRectPositions.Length];
            
            for (int i = 0; i < commonLineRectPositions.Length; i++) {
                playerLineRectPositionMass[i] = new Rectangle(commonLineRectPositions[i].X + playerCornerPixelPoint.X,
                     commonLineRectPositions[i].Y + playerCornerPixelPoint.Y,
                     commonLineRectPositions[i].Width, commonLineRectPositions[i].Height);
            }
            return playerLineRectPositionMass;
        }

        
    }
}
