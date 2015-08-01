using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
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
                new[] {new RectangleF(413, 39, 6, 6), new RectangleF(420, 39, 6, 6)},
                new[] {new RectangleF(410, 39, 6, 6), new RectangleF(417, 39, 6, 6), new RectangleF(424, 39, 6, 6),},
                new[] { new RectangleF(404, 39, 6, 6), new RectangleF(415, 39, 6, 6), new RectangleF(422, 39, 6, 6),
                    new RectangleF(429, 39, 6, 6)} };

            //CARDS
            Common.DeckPath = @"Data\Deck\";
            
            Common.FlopCard1Rect = new RectangleF(253, 175, 7, 4);
            Common.FlopCard2Rect = new RectangleF(317, 175, 7, 4);
            Common.FlopCard3Rect = new RectangleF(381, 175, 7, 4);
            Common.TurnCardRect = new RectangleF(445, 175, 7, 4);
            Common.RiverCardRect = new RectangleF(509, 175, 7, 4);

            Hero.Card1Rect = new RectangleF(378, 368, 7, 4);
            Hero.Card2Rect = new RectangleF(393, 372, 7, 4);
            //TODO Init LEftPlayer and RightPlayer cards if needs to

            //BUTTON
            Common.ButtonColor = Color.FromArgb(203, 196, 56);

            Hero.ButtonPoint        = new PixelPoint(396, 345);
            LeftPlayer.ButtonPoint  = new PixelPoint(142, 175);
            RightPlayer.ButtonPoint = new PixelPoint(667, 175);

            //BLINDS
            Common.Blinds.DigitsPath = @"Data\BlindsDigits\";
            Common.Blinds.DigitsPointColor = Color.FromArgb(255,255,255);
            Common.Blinds.DigitsPosPoints = new[] { new PixelPoint(234,414)};
            Common.Blinds.DigitsRectMass =   new[] {new RectangleF(262,414,8,5)};
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
                {120, Color.FromArgb(5, 5, 5)},
                {240, Color.FromArgb(5, 5, 5)},
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
            LeftPlayer.PlayerStatusPointHand = new PixelPoint(190, 137);
            RightPlayer.PlayerStatusPointHand = new PixelPoint(617, 139);

            
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
             new[] {new RectangleF(397, 456, 6, 6), new RectangleF(404, 456, 6, 6) }, 
             new[] {new RectangleF(393, 456, 6, 6), new RectangleF(400, 456, 6, 6), new RectangleF(407, 456, 6, 6)},
             new[] {new RectangleF(388, 456, 6, 6), new RectangleF(399, 456, 6, 6),
                    new RectangleF(406, 456, 6, 6), new RectangleF(413, 456, 6, 6)}};

            LeftPlayer.StackDigPosPoints = new[] { new PixelPoint(51, 130), new PixelPoint(47, 130), new PixelPoint(42, 130) };
            LeftPlayer.StackDigitsRectMass = new [] {
             new[] {new RectangleF(55, 131, 6, 6), new RectangleF(62, 131, 6, 6) }, 
             new[] {new RectangleF(51, 131, 6, 6), new RectangleF(58, 131, 6, 6), new RectangleF(65, 131, 6, 6)},
             new[] {new RectangleF(46, 131, 6, 6), new RectangleF(57, 131, 6, 6), 
                    new RectangleF(64, 131, 6, 6), new RectangleF(71, 131, 6, 6)}};

            RightPlayer.StackDigPosPoints = new[] { new PixelPoint(740, 130), new PixelPoint(736, 130), new PixelPoint(731, 130) };
            RightPlayer.StackDigitsRectMass = new[] {
             new[] {new RectangleF(744, 131, 6, 6), new RectangleF(751, 131, 6, 6)},
             new[] {new RectangleF(740, 131, 6, 6), new RectangleF(747, 131, 6, 6), new RectangleF(754, 131, 6, 6)},
             new[] {new RectangleF(735, 131, 6, 6), new RectangleF(747, 131, 6, 6),
                    new RectangleF(754, 131, 6, 6), new RectangleF(761, 131, 6, 6)}};
            

            //PLAYER BET
            Common.BetDigitsPath = @"Data\BetDigits\";
            Common.BetDigitsColor = Color.FromArgb(255, 246, 207);

            Hero.BetDigPosPoints = new[] {
                new PixelPoint(452, 320), new PixelPoint(446, 320), new PixelPoint(440, 320),
                new PixelPoint(429, 320), new PixelPoint(423, 320), new PixelPoint(417, 320),
                new PixelPoint(406, 320), new PixelPoint(400, 320), new PixelPoint(394, 320)};
            Hero.BetDigitsRectMass = new[] {

                new[] {new RectangleF(428, 320, 6, 6), new RectangleF(437, 320, 6, 6), new RectangleF(443, 320, 6, 6),new RectangleF(449, 320, 6, 6)},
                new[] {new RectangleF(431, 320, 6, 6), new RectangleF(437, 320, 6, 6), new RectangleF(443, 320, 6, 6)},
                new[] {new RectangleF(431, 320, 6, 6), new RectangleF(437, 320, 6, 6)},
                new[] {new RectangleF(405, 320, 6, 6), new RectangleF(414, 320, 6, 6), new RectangleF(420, 320, 6, 6),new RectangleF(426, 320, 6, 6)},
                new[] {new RectangleF(408, 320, 6, 6), new RectangleF(414, 320, 6, 6), new RectangleF(420, 320, 6, 6)},
                new[] {new RectangleF(408, 320, 6, 6), new RectangleF(414, 320, 6, 6)},
                new[] {new RectangleF(382, 320, 6, 6), new RectangleF(391, 320, 6, 6), new RectangleF(397, 320, 6, 6),new RectangleF(403, 320, 6, 6)},
                new[] {new RectangleF(385, 320, 6, 6), new RectangleF(391, 320, 6, 6), new RectangleF(397, 320, 6, 6)},
                new[] {new RectangleF(385, 320, 6, 6),new RectangleF(391, 320, 6, 6), },
            };
             
            LeftPlayer.BetDigPosPoints = new[] {
                new PixelPoint(260, 188), new PixelPoint(254, 188), new PixelPoint(248, 188),
                new PixelPoint(240, 188), new PixelPoint(231, 188), new PixelPoint(225, 188),
                new PixelPoint(214, 188), new PixelPoint(208, 188), new PixelPoint(202, 188)};

            LeftPlayer.BetDigitsRectMass = new[] {

                new[] {new RectangleF(239, 188, 6, 6), new RectangleF(248, 188, 6, 6), new RectangleF(254, 188, 6, 6),new RectangleF(260, 188, 6, 6)},
                new[] {new RectangleF(239, 188, 6, 6), new RectangleF(245, 188, 6, 6), new RectangleF(251, 188, 6, 6)},
                new[] {new RectangleF(239, 188, 6, 6), new RectangleF(245, 188, 6, 6)},
                new[] {new RectangleF(216, 188, 6, 6), new RectangleF(225, 188, 6, 6), new RectangleF(231, 188, 6, 6),new RectangleF(237, 188, 6, 6)},
                new[] {new RectangleF(216, 188, 6, 6), new RectangleF(222, 188, 6, 6), new RectangleF(228, 188, 6, 6)},
                new[] {new RectangleF(216, 188, 6, 6), new RectangleF(222, 188, 6, 6)},
                new[] {new RectangleF(189, 188, 6, 6), new RectangleF(199, 188, 6, 6), new RectangleF(205, 188, 6, 6),new RectangleF(211, 188, 6, 6)},
                new[] {new RectangleF(193, 188, 6, 6), new RectangleF(199, 188, 6, 6), new RectangleF(205, 188, 6, 6)},
                new[] {new RectangleF(193, 188, 6, 6),new RectangleF(199, 188, 6, 6), },
            };


            RightPlayer.BetDigPosPoints = new[] {
                new PixelPoint(596, 195), new PixelPoint(573, 195), new PixelPoint(550, 195)};

            RightPlayer.BetDigitsRectMass = new[] {
                new[] {new RectangleF(572, 195, 6, 6), new RectangleF(581, 195, 6, 6), new RectangleF(587, 195, 6, 6), new RectangleF(593, 195, 6, 6)},
                new[] {new RectangleF(549, 195, 6, 6), new RectangleF(558, 195, 6, 6), new RectangleF(564, 195, 6, 6),new RectangleF(570, 195, 6, 6)},
                new[] {new RectangleF(526, 195, 6, 6), new RectangleF(535, 195, 6, 6), new RectangleF(541, 195, 6, 6),new RectangleF(547, 195, 6, 6)},};
            


            //LINE
            Common.LineLettersDictionary = new Dictionary<Color, string>();
            Common.LineLettersDictionary.Add(Color.FromArgb(128,128,128), "|");
            Common.LineLettersDictionary.Add(Color.FromArgb(64, 128, 128), "f");
            Common.LineLettersDictionary.Add(Color.FromArgb(192,192,192), "x");
            Common.LineLettersDictionary.Add(Color.FromArgb(255, 128, 255), "l");
            Common.LineLettersDictionary.Add(Color.FromArgb(255, 255, 0), "c");
            Common.LineLettersDictionary.Add(Color.FromArgb(255, 128, 64), "b");
            Common.LineLettersDictionary.Add(Color.FromArgb(255, 128, 128), "r");

            Common.LinePixelPositions = new[] {
                new PixelPoint(7, 8), new PixelPoint(15, 8), new PixelPoint(23, 8), new PixelPoint(31, 8),
                new PixelPoint(39, 8), new PixelPoint(47, 8), new PixelPoint(55, 8), new PixelPoint(63, 8),
                new PixelPoint(71, 8), new PixelPoint(79, 8), new PixelPoint(87, 8), new PixelPoint(95, 8)
            };

            Hero.LinePixelPositions = CountLinePixelPositions(Common.LinePixelPositions, new PixelPoint(462,403));
            LeftPlayer.LinePixelPositions = CountLinePixelPositions(Common.LinePixelPositions, new PixelPoint(230, 81));
            RightPlayer.LinePixelPositions = CountLinePixelPositions(Common.LinePixelPositions, new PixelPoint(468, 82));


            //STATISTICS
            Common.StatsDigitsPath = @"Data\StatsDigits\";
            Common.StatsDigitsColor = Color.FromArgb(20, 175, 196);

            //LEFT
              //PREFLOP
            LeftPlayer.PF_BTN_STEAL_StatDigPosPoints = new[] { new PixelPoint(21, 195), new PixelPoint(16, 195) };
            LeftPlayer.PF_BTN_STEAL_StatDigitsRectMass = new[] {
                new [] {new RectangleF(10,190,5,4), new RectangleF(15,190,5,4) }, 
                new [] {new RectangleF(10,190,5,4)}
                };

            LeftPlayer.PF_SB_STEAL_StatDigPosPoints = new[] { new PixelPoint(21, 206), new PixelPoint(16, 206) };
            LeftPlayer.PF_SB_STEAL_StatDigitsRectMass = new[] {
                new [] {new RectangleF(10,201,5,4), new RectangleF(15,201,5,4) }, 
                new [] {new RectangleF(10,201,5,4)}
                };

            LeftPlayer.PF_LIMP_FOLD_StatDigPosPoints = new[] { new PixelPoint(21, 216), new PixelPoint(16, 216) };
            LeftPlayer.PF_LIMP_FOLD_StatDigitsRectMass = new[] {
                new [] {new RectangleF(10,211,5,4), new RectangleF(15,211,5,4) }, 
                new [] {new RectangleF(10,211,5,4)}
                };

            LeftPlayer.PF_FOLD_3BET_StatDigPosPoints = new[] { new PixelPoint(21, 227), new PixelPoint(16, 227) };
            LeftPlayer.PF_FOLD_3BET_StatDigitsRectMass = new[] {
                new [] {new RectangleF(10,222,5,4), new RectangleF(15,222,5,4) }, 
                new [] {new RectangleF(10,222,5,4)}
                };

            LeftPlayer.PF_SB_3BET_VS_BTN_StatDigPosPoints = new[] { new PixelPoint(21, 237), new PixelPoint(16, 237) };
            LeftPlayer.PF_SB_3BET_VS_BTN_StatDigitsRectMass = new[] {
                new [] {new RectangleF(10,232,5,4), new RectangleF(15,232,5,4) }, 
                new [] {new RectangleF(10,232,5,4)}
                };

            LeftPlayer.PF_BB_3BET_VS_BTN_StatDigPosPoints = new[] { new PixelPoint(21, 248), new PixelPoint(16, 248) };
            LeftPlayer.PF_BB_3BET_VS_BTN_StatDigitsRectMass = new[] {
                new [] {new RectangleF(10,243,5,4), new RectangleF(15,243,5,4) }, 
                new [] {new RectangleF(10,243,5,4)}
                };

            LeftPlayer.PF_BB_3BET_VS_SB_StatDigPosPoints = new[] { new PixelPoint(21, 258), new PixelPoint(16, 258) };
            LeftPlayer.PF_BB_3BET_VS_SB_StatDigitsRectMass = new[] {
                new [] {new RectangleF(10,253,5,4), new RectangleF(15,253,5,4) }, 
                new [] {new RectangleF(10,253,5,4)}
                };


            LeftPlayer.PF_BB_DEF_VS_SBSTEAL_StatDigPosPoints = new[] { new PixelPoint(21, 269), new PixelPoint(16, 269) };
            LeftPlayer.PF_BB_DEF_VS_SBSTEAL_StatDigitsRectMass = new[] {
                new [] {new RectangleF(10,264,5,4), new RectangleF(15,264,5,4) }, 
                new [] {new RectangleF(10,264,5,4)}
                };

            LeftPlayer.PF_SB_OPENMINRAISE_StatDigPosPoints = new[] { new PixelPoint(21, 279), new PixelPoint(16, 279) };
            LeftPlayer.PF_SB_OPENMINRAISE_StatDigitsRectMass = new[] {
                new [] {new RectangleF(10,274,5,4), new RectangleF(15,274,5,4) }, 
                new [] {new RectangleF(10,274,5,4)}
                };

            LeftPlayer.PF_OPENPUSH_StatDigPosPoints = new[] { new PixelPoint(21, 290), new PixelPoint(16, 290) };
            LeftPlayer.PF_OPENPUSH_StatDigitsRectMass = new[] {
                new [] {new RectangleF(10,285,5,4), new RectangleF(15,285,5,4) }, 
                new [] {new RectangleF(10,285,5,4)}
                };

            //FLOP
            LeftPlayer.F_CBET_StatDigPosPoints = new[] { new PixelPoint(43, 195), new PixelPoint(38, 195) };
            LeftPlayer.F_CBET_StatDigitsRectMass = new[] {
                new [] {new RectangleF(32,190,5,4), new RectangleF(37,190,5,4) }, 
                new [] {new RectangleF(32,190,5,4)}
                };

            LeftPlayer.F_BET_LPOT_StatDigPosPoints = new[] { new PixelPoint(43, 205), new PixelPoint(38, 205) };
            LeftPlayer.F_BET_LPOT_StatDigitsRectMass = new[] {
                new [] {new RectangleF(32,200,5,4), new RectangleF(37,201,5,4) }, 
                new [] {new RectangleF(32,200,5,4)}
                };

            LeftPlayer.F_CBET_FOLDRAISE_StatDigPosPoints = new[] { new PixelPoint(43, 216), new PixelPoint(38, 216) };
            LeftPlayer.F_CBET_FOLDRAISE_StatDigitsRectMass = new[] {
                new [] {new RectangleF(32,211,5,4), new RectangleF(37,211,5,4) }, 
                new [] {new RectangleF(32,211,5,4)}
                };

            LeftPlayer.F_FOLD_CBET_StatDigPosPoints = new[] { new PixelPoint(43, 226), new PixelPoint(38, 226) };
            LeftPlayer.F_FOLD_CBET_StatDigitsRectMass = new[] {
                new [] {new RectangleF(32,221,5,4), new RectangleF(37,221,5,4) }, 
                new [] {new RectangleF(32,221,5,4)}
                };

            LeftPlayer.F_RAISE_CBET_StatDigPosPoints = new[] { new PixelPoint(43, 237), new PixelPoint(38, 237) };
            LeftPlayer.F_RAISE_CBET_StatDigitsRectMass = new[] {
                new [] {new RectangleF(32,232,5,4), new RectangleF(37,232,5,4) }, 
                new [] {new RectangleF(32,232,5,4)}
                };

            LeftPlayer.F_DONK_StatDigPosPoints = new[] { new PixelPoint(43, 247), new PixelPoint(38, 247) };
            LeftPlayer.F_DONK_StatDigitsRectMass = new[] {
                new [] {new RectangleF(32,242,5,4), new RectangleF(37,242,5,4) }, 
                new [] {new RectangleF(32,242,5,4)}
                };

            LeftPlayer.F_DONK_FOLDRAISE_StatDigPosPoints = new[] { new PixelPoint(43, 258), new PixelPoint(38, 258) };
            LeftPlayer.F_DONK_FOLDRAISE_StatDigitsRectMass = new[] {
                new [] {new RectangleF(32,253,5,4), new RectangleF(37,253,5,4) }, 
                new [] {new RectangleF(32,253,5,4)}
                };

            //RIGHT
            //PREFLOP

            RightPlayer.PF_BTN_STEAL_StatDigPosPoints = new[] { new PixelPoint(726, 195), new PixelPoint(721, 195) };
            RightPlayer.PF_BTN_STEAL_StatDigitsRectMass = new[] {
                new [] {new RectangleF(715,190,5,4), new RectangleF(720,190,5,4) }, 
                new [] {new RectangleF(715,190,5,4)}
                };

            RightPlayer.PF_SB_STEAL_StatDigPosPoints = new[] { new PixelPoint(726, 206), new PixelPoint(721, 206) };
            RightPlayer.PF_SB_STEAL_StatDigitsRectMass = new[] {
                new [] {new RectangleF(715,201,5,4), new RectangleF(720,201,5,4) }, 
                new [] {new RectangleF(715,201,5,4)}
                };

            RightPlayer.PF_LIMP_FOLD_StatDigPosPoints = new[] { new PixelPoint(726, 216), new PixelPoint(721, 216) };
            RightPlayer.PF_LIMP_FOLD_StatDigitsRectMass = new[] {
                new [] {new RectangleF(715,211,5,4), new RectangleF(720,211,5,4) }, 
                new [] {new RectangleF(715,211,5,4)}
                };

            RightPlayer.PF_FOLD_3BET_StatDigPosPoints = new[] { new PixelPoint(726, 227), new PixelPoint(721, 227) };
            RightPlayer.PF_FOLD_3BET_StatDigitsRectMass = new[] {
                new [] {new RectangleF(715,222,5,4), new RectangleF(720,222,5,4) }, 
                new [] {new RectangleF(715,222,5,4)}
                };

            RightPlayer.PF_SB_3BET_VS_BTN_StatDigPosPoints = new[] { new PixelPoint(726, 237), new PixelPoint(721, 237) };
            RightPlayer.PF_SB_3BET_VS_BTN_StatDigitsRectMass = new[] {
                new [] {new RectangleF(715,232,5,4), new RectangleF(720,232,5,4) }, 
                new [] {new RectangleF(715,232,5,4)}
                };

            RightPlayer.PF_BB_3BET_VS_BTN_StatDigPosPoints = new[] { new PixelPoint(726, 248), new PixelPoint(721, 248) };
            RightPlayer.PF_BB_3BET_VS_BTN_StatDigitsRectMass = new[] {
                new [] {new RectangleF(715,243,5,4), new RectangleF(720,243,5,4) }, 
                new [] {new RectangleF(715,243,5,4)}
                };

            RightPlayer.PF_BB_3BET_VS_SB_StatDigPosPoints = new[] { new PixelPoint(726, 258), new PixelPoint(721, 258) };
            RightPlayer.PF_BB_3BET_VS_SB_StatDigitsRectMass = new[] {
                new [] {new RectangleF(715,253,5,4), new RectangleF(720,253,5,4) }, 
                new [] {new RectangleF(715,253,5,4)}
                };

            RightPlayer.PF_BB_DEF_VS_SBSTEAL_StatDigPosPoints = new[] { new PixelPoint(726, 269), new PixelPoint(721, 269) };
            RightPlayer.PF_BB_DEF_VS_SBSTEAL_StatDigitsRectMass = new[] {
                new [] {new RectangleF(715,264,5,4), new RectangleF(720,264,5,4) }, 
                new [] {new RectangleF(715,264,5,4)}
                };

            RightPlayer.PF_SB_OPENMINRAISE_StatDigPosPoints = new[] { new PixelPoint(726, 279), new PixelPoint(721, 279) };
            RightPlayer.PF_SB_OPENMINRAISE_StatDigitsRectMass = new[] {
                new [] {new RectangleF(715,274,5,4), new RectangleF(720,274,5,4) }, 
                new [] {new RectangleF(715,274,5,4)}
                };

            RightPlayer.PF_OPENPUSH_StatDigPosPoints = new[] { new PixelPoint(726, 290), new PixelPoint(721, 290) };
            RightPlayer.PF_OPENPUSH_StatDigitsRectMass = new[] {
                new [] {new RectangleF(715,285,5,4), new RectangleF(720,285,5,4) }, 
                new [] {new RectangleF(715,285,5,4)}
                };

            //RIGHT FLOP
            RightPlayer.F_CBET_StatDigPosPoints = new[] { new PixelPoint(748, 195), new PixelPoint(743, 195) };
            RightPlayer.F_CBET_StatDigitsRectMass = new[] {
                new [] {new RectangleF(737,190,5,4), new RectangleF(742,190,5,4) }, 
                new [] {new RectangleF(737,190,5,4)}
                };

            RightPlayer.F_BET_LPOT_StatDigPosPoints = new[] { new PixelPoint(748, 206), new PixelPoint(743, 206) };
            RightPlayer.F_BET_LPOT_StatDigitsRectMass = new[] {
                new [] {new RectangleF(737,201,5,4), new RectangleF(742,201,5,4) }, 
                new [] {new RectangleF(737,201,5,4)}
                };

            RightPlayer.F_CBET_FOLDRAISE_StatDigPosPoints = new[] { new PixelPoint(748, 216), new PixelPoint(743, 216) };
            RightPlayer.F_CBET_FOLDRAISE_StatDigitsRectMass = new[] {
                new [] {new RectangleF(737,211,5,4), new RectangleF(742,211,5,4) }, 
                new [] {new RectangleF(737,211,5,4)}
                };

            RightPlayer.F_FOLD_CBET_StatDigPosPoints = new[] { new PixelPoint(748, 227), new PixelPoint(743, 227) };
            RightPlayer.F_FOLD_CBET_StatDigitsRectMass = new[] {
                new [] {new RectangleF(737,222,5,4), new RectangleF(742,222,5,4) }, 
                new [] {new RectangleF(737,222,5,4)}
                };

            RightPlayer.F_RAISE_CBET_StatDigPosPoints = new[] { new PixelPoint(748, 237), new PixelPoint(743, 237) };
            RightPlayer.F_RAISE_CBET_StatDigitsRectMass = new[] {
                new [] {new RectangleF(737,232,5,4), new RectangleF(742,232,5,4) }, 
                new [] {new RectangleF(737,232,5,4)}
                };

            RightPlayer.F_DONK_StatDigPosPoints = new[] { new PixelPoint(748, 248), new PixelPoint(743, 248) };
            RightPlayer.F_DONK_StatDigitsRectMass = new[] {
                new [] {new RectangleF(737,243,5,4), new RectangleF(742,243,5,4) }, 
                new [] {new RectangleF(737,243,5,4)}
                };

            RightPlayer.F_DONK_FOLDRAISE_StatDigPosPoints = new[] { new PixelPoint(748, 258), new PixelPoint(743, 258) };
            RightPlayer.F_DONK_FOLDRAISE_StatDigitsRectMass = new[] {
                new [] {new RectangleF(737,253,5,4), new RectangleF(742,253,5,4) }, 
                new [] {new RectangleF(737,253,5,4)}
                };

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
            PixelPoint playerCornerPixelPoint) {
            PixelPoint[] playerPixelPointsMass = new PixelPoint[commonLinePixelPositions.Length];

            for (int i = 0; i < commonLinePixelPositions.Length; i ++) {
                 playerPixelPointsMass[i] = new PixelPoint(commonLinePixelPositions[i].X + playerCornerPixelPoint.X,
                     commonLinePixelPositions[i].Y + playerCornerPixelPoint.Y);
            }
            
            return  playerPixelPointsMass;

        }

        
    }
}
