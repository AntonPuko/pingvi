using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pingvi {
    //TODO сделать XML реализацию
    public class ElementsConfig {

        public CommonElementsConfig Common { get; set; }
        public HeroElementsConfig Hero { get; set; }
        public PlayerElementsConfig LeftPlayer { get; set; }
        public PlayerElementsConfig RightPlayer { get; set; }

        public  ElementsConfig() {
            Common = new CommonElementsConfig();
            Hero = new HeroElementsConfig();
            LeftPlayer = new PlayerElementsConfig();
            RightPlayer = new PlayerElementsConfig();
            InitElements();
            InitLists();

        }

        private void InitElements() {

            Common.TableNumberDigitsPath = @"Data\TableNumberDigits\";
            Common.TableNumberDigitsColor = Color.FromArgb(255, 255, 255);
            Common.TableNumberDigPosPoints = new[] {new PixelPoint(665, 7), new PixelPoint(660, 7)};
            Common.TableNumberDigitsRectMass = new[] {
                new[] {new RectangleF(648, 7, 10, 8), new RectangleF(659, 7, 10, 8)},
                new[] {new RectangleF(654, 7, 10, 8)},
            };

            //pot
            Common.PotDigitsPath = @"Data\PotDigits\";
            Common.PotDigitsColor = Color.FromArgb(0, 0, 0);
            Common.PotDigPosPoints = new[] {new PixelPoint(413, 41), new PixelPoint(410, 41), new PixelPoint(404,41) };
            Common.PotDigitsRectMass = new[] {
                new[] {new RectangleF(417,42,6,6),new RectangleF(424,42,6,6)},
                new[] {new RectangleF(414,42,6,6),  new RectangleF(421,42,6,6),new RectangleF(428,42,6,6), },
                new[] {new RectangleF(408,42,6,6), new RectangleF(419,42,6,6), new RectangleF(426,42,6,6), new RectangleF(433,42,6,6)}
            };
            

            //CARDS
            Common.DeckPath = @"Data\Deck\";

            Common.FlopCard1Rect = new RectangleF(257, 178, 7, 4);
            Common.FlopCard2Rect = new RectangleF(321, 178, 7, 4);
            Common.FlopCard3Rect = new RectangleF(385, 178, 7, 4);
            Common.TurnCardRect = new RectangleF(449, 178, 7, 4);
            Common.RiverCardRect = new RectangleF(513, 178, 7, 4);

            Hero.Card1Rect = new RectangleF(382, 372, 7, 4);
            Hero.Card2Rect = new RectangleF(397, 376, 7, 4);
            //TODO Init LEftPlayer and RightPlayer cards if needs to

            //BUTTON
            Common.ButtonColor = Color.FromArgb(203, 196, 56);

            Hero.ButtonPoint        = new PixelPoint(399, 349);
            LeftPlayer.ButtonPoint  = new PixelPoint(144, 178);
            RightPlayer.ButtonPoint = new PixelPoint(671, 178);

            //BLINDS
            Common.Blinds.DigitsPath = @"Data\BlindsDigits\";
         
            Common.Blinds.DigitsPointColor = Color.FromArgb(211,179,145);

            Common.Blinds.DigitsPosPoints = new[] { new PixelPoint(133,13), new PixelPoint(139, 13)};
            Common.Blinds.DigitsRectMass =   new[] {new RectangleF(177,8,11,5), new RectangleF(183,8,11,5) };
            Common.Blinds.SmallBlindsDoubleMass = new[]{10.0, 15.0, 20.0,30.0,40.0,50.0,60.0};
            
       
            Common.BlindMassStrings = new[] { @"$10/$20", @"$15/$30", @"$20/$40", @"$30/$60",
                @"$40/$80", @"$50/$100", @"$60/$120", @"$75/$150", @"$90/$180" };
            //TODO переписать метод определения типа игрока, сделать расширяемым
            //PLAYER TYPE
            Common.FishColor    = Color.FromArgb(6, 100, 6);
            Common.WeakRegColor = Color.FromArgb(6, 53, 100);
            Common.GoodRegColor = Color.FromArgb(100, 53, 6);
            Common.UberRegColor = Color.FromArgb(100, 6, 6);
            Common.RockColor = Color.FromArgb(54, 53, 6);
            Common.ManiacColor = Color.FromArgb(86, 23, 100);

            LeftPlayer.PlayerTypePoint  = new PixelPoint(145, 100);
            RightPlayer.PlayerTypePoint = new PixelPoint(666, 101);
            //TODO Init Hero Player type if needs to

            //PLAYER STATUS
            Common.InHandColor    = Color.FromArgb(230,230,230);
            Common.InGameColor    = Color.FromArgb(194, 194, 194);
            Common.SitOutColor = Color.FromArgb(192, 192, 192);
            

            Hero.PlayerStatusPointGame        = new PixelPoint(396, 473);
            LeftPlayer.PlayerStatusPointGame = new PixelPoint(55, 147);
            RightPlayer.PlayerStatusPointGame = new PixelPoint(755, 147);

            Hero.PlayerStatusPointHand = new PixelPoint(394, 451);
            LeftPlayer.PlayerStatusPointHand = new PixelPoint(189, 149);
            RightPlayer.PlayerStatusPointHand = new PixelPoint(617, 142);

            
            Hero.PlayerStatusPointSitOut = new PixelPoint(394, 451);
            LeftPlayer.PlayerStatusPointSitOut = new PixelPoint(64, 146);
            RightPlayer.PlayerStatusPointSitOut = new PixelPoint(756, 146);

            //PLAYER STACK
            Common.StackDigitsPath = @"Data\StackDigits\";
            
            Common.StackDigitsColor = Color.FromArgb(192, 192, 192);

            Hero.StackDigPosPoints = new[] { new PixelPoint(397, 459), new PixelPoint(393, 459), new PixelPoint(388, 459) };
            Hero.StackDigitsRectMass = new[] {
             new[] {new RectangleF(401, 460, 6, 6), new RectangleF(408, 460, 6, 6) }, 
             new[] {new RectangleF(397, 460, 6, 6), new RectangleF(404, 460, 6, 6), new RectangleF(411, 460, 6, 6)},
             new[] {new RectangleF(392, 460, 6, 6), new RectangleF(403, 460, 6, 6),
                    new RectangleF(410, 460, 6, 6), new RectangleF(417, 460, 6, 6)}};
       
            LeftPlayer.StackDigPosPoints = new[] {new PixelPoint(54, 133), new PixelPoint(50, 133), new PixelPoint(45, 133)};
            LeftPlayer.StackDigitsRectMass = new [] {
             new[] {new RectangleF(58, 134, 6, 6), new RectangleF(65, 134, 6, 6) }, 
             new[] {new RectangleF(54, 134, 6, 6), new RectangleF(61, 134, 6, 6), new RectangleF(68, 138, 6, 6)},
             new[] {new RectangleF(49, 134, 6, 6), new RectangleF(60, 134, 6, 6), 
                    new RectangleF(67, 134, 6, 6), new RectangleF(74, 134, 6, 6)}};
          
            RightPlayer.StackDigPosPoints = new[] {new PixelPoint(745, 133), new PixelPoint(741, 133), new PixelPoint(736, 133)};
            RightPlayer.StackDigitsRectMass = new[] {
             new[] {new RectangleF(749, 134, 6, 6), new RectangleF(756, 134, 6, 6)},
             new[] {new RectangleF(745, 134, 6, 6), new RectangleF(752, 134, 6, 6), new RectangleF(759, 134, 6, 6)},
             new[] {new RectangleF(740, 134, 6, 6), new RectangleF(751, 134, 6, 6),
                    new RectangleF(758, 134, 6, 6), new RectangleF(765, 134, 6, 6)}};


            //PLAYER BET
            Common.BetDigitsPath = @"Data\BetDigits\";
            Common.BetDigitsColor = Color.FromArgb(255, 246, 207);

            Hero.BetDigPosPoints = new[] {
                new PixelPoint(456, 324), new PixelPoint(450, 324), new PixelPoint(444, 324),
                new PixelPoint(433, 324), new PixelPoint(427, 324), new PixelPoint(421, 324),
                new PixelPoint(410, 324), new PixelPoint(404, 324), new PixelPoint(398, 324)};
            Hero.BetDigitsRectMass = new[] {

                new[] {new RectangleF(432, 324, 6, 6), new RectangleF(441, 324, 6, 6), new RectangleF(447, 324, 6, 6),new RectangleF(453, 324, 6, 6)},
                new[] {new RectangleF(435, 324, 6, 6), new RectangleF(441, 324, 6, 6), new RectangleF(447, 324, 6, 6)},
                new[] {new RectangleF(435, 324, 6, 6), new RectangleF(441, 324, 6, 6)},
                new[] {new RectangleF(409, 324, 6, 6), new RectangleF(418, 324, 6, 6), new RectangleF(424, 324, 6, 6),new RectangleF(430, 324, 6, 6)},
                new[] {new RectangleF(412, 324, 6, 6), new RectangleF(418, 324, 6, 6), new RectangleF(424, 324, 6, 6)},
                new[] {new RectangleF(412, 324, 6, 6), new RectangleF(418, 324, 6, 6)},
                new[] {new RectangleF(386, 324, 6, 6), new RectangleF(395, 324, 6, 6), new RectangleF(401, 324, 6, 6),new RectangleF(407, 324, 6, 6)},
                new[] {new RectangleF(389, 324, 6, 6), new RectangleF(395, 324, 6, 6), new RectangleF(401, 324, 6, 6)},
                new[] {new RectangleF(389, 324, 6, 6),new RectangleF(395, 324, 6, 6), },
            };


            LeftPlayer.BetDigPosPoints = new[] {
                new PixelPoint(266, 191), new PixelPoint(257, 191), new PixelPoint(251, 191),
                new PixelPoint(243, 191), new PixelPoint(234, 191), new PixelPoint(228, 191),
                new PixelPoint(217, 191), new PixelPoint(211, 191), new PixelPoint(205, 191)};

       

            LeftPlayer.BetDigitsRectMass = new[] {

                new[] {new RectangleF(242, 191, 6, 6), new RectangleF(251, 191, 6, 6), new RectangleF(257, 191, 6, 6),new RectangleF(263, 191, 6, 6)},
                new[] {new RectangleF(242, 191, 6, 6), new RectangleF(248, 191, 6, 6), new RectangleF(254, 191, 6, 6)},
                new[] {new RectangleF(242, 191, 6, 6), new RectangleF(248, 191, 6, 6)},
                new[] {new RectangleF(219, 191, 6, 6), new RectangleF(228, 191, 6, 6), new RectangleF(234, 191, 6, 6),new RectangleF(240, 191, 6, 6)},
                new[] {new RectangleF(219, 191, 6, 6), new RectangleF(225, 191, 6, 6), new RectangleF(231, 191, 6, 6)},
                new[] {new RectangleF(219, 191, 6, 6), new RectangleF(225, 191, 6, 6)},
                new[] {new RectangleF(193, 191, 6, 6), new RectangleF(202, 191, 6, 6), new RectangleF(208, 191, 6, 6),new RectangleF(214, 191, 6, 6)},
                new[] {new RectangleF(196, 191, 6, 6), new RectangleF(202, 191, 6, 6), new RectangleF(208, 191, 6, 6)},
                new[] {new RectangleF(196, 191, 6, 6),new RectangleF(202, 191, 6, 6), },
            };

            RightPlayer.BetDigPosPoints = new[] {
                new PixelPoint(601, 198), new PixelPoint(578, 198), new PixelPoint(555, 198)};


            RightPlayer.BetDigitsRectMass = new[] {
                new[] {new RectangleF(577, 198, 6, 6), new RectangleF(586, 198, 6, 6), new RectangleF(592, 198, 6, 6), new RectangleF(598, 198, 6, 6)},
                new[] {new RectangleF(554, 198, 6, 6), new RectangleF(563, 198, 6, 6), new RectangleF(569, 198, 6, 6),new RectangleF(575, 198, 6, 6)},
                new[] {new RectangleF(531, 198, 6, 6), new RectangleF(540, 198, 6, 6), new RectangleF(546, 198, 6, 6),new RectangleF(552, 198, 6, 6)},};
            

            //STATISTICS
            Common.StatsDigitsPath = @"Data\StatsDigits\";
            Common.StatsDigitsColor = Color.FromArgb(164, 90, 42);

            //LEFT
              //PREFLOP

            LeftPlayer.PF_BTN_STEAL_StatDigPosPoints = new[] { new PixelPoint(26, 215), new PixelPoint(21, 215), new PixelPoint(16, 215) };
            LeftPlayer.PF_BTN_STEAL_StatDigitsRectMass = new[] {
                new [] {new RectangleF(10,208,5,4), new RectangleF(15,208,5,4), new RectangleF(20,208,5,4) },
                new [] {new RectangleF(10,208,5,4), new RectangleF(15,208,5,4) }, 
                new [] {new RectangleF(10,208,5,4)}
                };

            LeftPlayer.PF_SB_STEAL_StatDigPosPoints = new[] { new PixelPoint(26, 227), new PixelPoint(21, 227), new PixelPoint(16, 227) };
            LeftPlayer.PF_SB_STEAL_StatDigitsRectMass = new[] {
                new [] {new RectangleF(10,220,5,4), new RectangleF(15,220,5,4), new RectangleF(20,220,5,4) },
                new [] {new RectangleF(10,220,5,4), new RectangleF(15,220,5,4) }, 
                new [] {new RectangleF(10,220,5,4)}
                };

            LeftPlayer.PF_LIMP_FOLD_StatDigPosPoints = new[] { new PixelPoint(26, 239), new PixelPoint(21, 239), new PixelPoint(16, 239) };
            LeftPlayer.PF_LIMP_FOLD_StatDigitsRectMass = new[] {
                new [] {new RectangleF(10,232,5,4), new RectangleF(15,232,5,4), new RectangleF(20,232,5,4) },
                new [] {new RectangleF(10,232,5,4), new RectangleF(15,232,5,4) }, 
                new [] {new RectangleF(10,232,5,4)}
                };

            LeftPlayer.PF_FOLD_3BET_StatDigPosPoints = new[] { new PixelPoint(26, 251), new PixelPoint(21, 251), new PixelPoint(16, 251) };
            LeftPlayer.PF_FOLD_3BET_StatDigitsRectMass = new[] {
                new [] {new RectangleF(10,244,5,4), new RectangleF(15,244,5,4), new RectangleF(20,244,5,4) },
                new [] {new RectangleF(10,244,5,4), new RectangleF(15,244,5,4) }, 
                new [] {new RectangleF(10,244,5,4)}
                };

            LeftPlayer.PF_SB_3BET_VS_BTN_StatDigPosPoints = new[] { new PixelPoint(26, 263), new PixelPoint(21, 263), new PixelPoint(16, 263) };
            LeftPlayer.PF_SB_3BET_VS_BTN_StatDigitsRectMass = new[] {
                new [] {new RectangleF(10,256,5,4), new RectangleF(15,256,5,4), new RectangleF(20,244,5,4) },
                new [] {new RectangleF(10,256,5,4), new RectangleF(15,256,5,4) }, 
                new [] {new RectangleF(10,256,5,4)}
                };

            LeftPlayer.PF_BB_3BET_VS_BTN_StatDigPosPoints = new[] { new PixelPoint(26, 275), new PixelPoint(21, 275), new PixelPoint(16, 275) };
            LeftPlayer.PF_BB_3BET_VS_BTN_StatDigitsRectMass = new[] {
                new [] {new RectangleF(10,268,5,4), new RectangleF(15,268,5,4), new RectangleF(20,268,5,4) },
                new [] {new RectangleF(10,268,5,4), new RectangleF(15,268,5,4) }, 
                new [] {new RectangleF(10,268,5,4)}
                };

            LeftPlayer.PF_BB_3BET_VS_SB_StatDigPosPoints = new[] { new PixelPoint(26, 287), new PixelPoint(21, 287), new PixelPoint(16, 287) };
            LeftPlayer.PF_BB_3BET_VS_SB_StatDigitsRectMass = new[] {
                new [] {new RectangleF(10,280,5,4), new RectangleF(15,280,5,4), new RectangleF(20,280,5,4) },
                new [] {new RectangleF(10,280,5,4), new RectangleF(15,280,5,4) }, 
                new [] {new RectangleF(10,280,5,4)}
                };


            LeftPlayer.PF_BB_VS_SBSTEAL_FOLD_StatDigPosPoints = new[] { new PixelPoint(26, 299), new PixelPoint(21, 299), new PixelPoint(16, 299) };
            LeftPlayer.PF_BB_VS_SBSTEAL_FOLD_StatDigitsRectMass = new[] {
                new [] {new RectangleF(10,292,5,4), new RectangleF(15,292,5,4), new RectangleF(20,292,5,4) },
                new [] {new RectangleF(10,292,5,4), new RectangleF(15,292,5,4) }, 
                new [] {new RectangleF(10,292,5,4)}
                };

            //не работает сейчас( в пт4)
            LeftPlayer.PF_SB_OPENMINRAISE_StatDigPosPoints = new[] { new PixelPoint(19, 282), new PixelPoint(14, 282) };
            LeftPlayer.PF_SB_OPENMINRAISE_StatDigitsRectMass = new[] {
                new [] {new RectangleF(8,277,5,4), new RectangleF(13,277,5,4) }, 
                new [] {new RectangleF(8,277,5,4)}
                };
             

            //FLOP
            LeftPlayer.F_CBET_StatDigPosPoints = new[] { new PixelPoint(47, 215), new PixelPoint(42, 215), new PixelPoint(37, 215) };
            LeftPlayer.F_CBET_StatDigitsRectMass = new[] {
                new [] {new RectangleF(31,208,5,4), new RectangleF(36,208,5,4), new RectangleF(41,208,5,4) },
                new [] {new RectangleF(31,208,5,4), new RectangleF(36,208,5,4) }, 
                new [] {new RectangleF(31,208,5,4)}
                };

            LeftPlayer.F_BET_LPOT_StatDigPosPoints = new[] { new PixelPoint(47, 227), new PixelPoint(42, 227), new PixelPoint(37, 227) };
            LeftPlayer.F_BET_LPOT_StatDigitsRectMass = new[] {
                new [] {new RectangleF(31,220,5,4), new RectangleF(36,220,5,4), new RectangleF(41,220,5,4) },
                new [] {new RectangleF(31,220,5,4), new RectangleF(36,220,5,4) }, 
                new [] {new RectangleF(31,220,5,4)}
                };

            LeftPlayer.F_CBET_FOLDRAISE_StatDigPosPoints = new[] { new PixelPoint(47, 239), new PixelPoint(42, 239), new PixelPoint(37, 239) };
            LeftPlayer.F_CBET_FOLDRAISE_StatDigitsRectMass = new[] {
                new [] {new RectangleF(31,232,5,4), new RectangleF(36,232,5,4), new RectangleF(41,232,5,4) },
                new [] {new RectangleF(31,232,5,4), new RectangleF(36,232,5,4) }, 
                new [] {new RectangleF(31,232,5,4)}
                };

            LeftPlayer.F_FOLD_CBET_StatDigPosPoints = new[] { new PixelPoint(47, 251), new PixelPoint(42, 251), new PixelPoint(37, 251) };
            LeftPlayer.F_FOLD_CBET_StatDigitsRectMass = new[] {
                new [] {new RectangleF(31,244,5,4), new RectangleF(36,244,5,4), new RectangleF(41,244,5,4) },
                new [] {new RectangleF(31,244,5,4), new RectangleF(36,244,5,4) }, 
                new [] {new RectangleF(31,244,5,4)}
                };

            LeftPlayer.F_RAISE_CBET_StatDigPosPoints = new[] { new PixelPoint(47, 263), new PixelPoint(42, 263), new PixelPoint(37, 263) };
            LeftPlayer.F_RAISE_CBET_StatDigitsRectMass = new[] {
                new [] {new RectangleF(31,256,5,4), new RectangleF(36,256,5,4), new RectangleF(41,256,5,4) },
                new [] {new RectangleF(31,256,5,4), new RectangleF(36,256,5,4) }, 
                new [] {new RectangleF(31,256,5,4)}
                };

            LeftPlayer.F_DONK_StatDigPosPoints = new[] { new PixelPoint(47, 275), new PixelPoint(42, 275), new PixelPoint(37, 275) };
            LeftPlayer.F_DONK_StatDigitsRectMass = new[] {
                new [] {new RectangleF(31,268,5,4), new RectangleF(36,268,5,4), new RectangleF(41,268,5,4) },
                new [] {new RectangleF(31,268,5,4), new RectangleF(36,268,5,4) }, 
                new [] {new RectangleF(31,268,5,4)}
                };

            LeftPlayer.F_DONK_FOLDRAISE_StatDigPosPoints = new[] { new PixelPoint(47, 287), new PixelPoint(42, 287), new PixelPoint(37, 287) };
            LeftPlayer.F_DONK_FOLDRAISE_StatDigitsRectMass = new[] {
                new [] {new RectangleF(31,280,5,4), new RectangleF(36,280,5,4), new RectangleF(41,280,5,4) },
                new [] {new RectangleF(31,280,5,4), new RectangleF(36,280,5,4) }, 
                new [] {new RectangleF(31,280,5,4)}
                };

            //RIGHT
            //PREFLOP

            RightPlayer.PF_BTN_STEAL_StatDigPosPoints = new[] { new PixelPoint(738, 215), new PixelPoint(733, 215), new PixelPoint(728, 215) };
            RightPlayer.PF_BTN_STEAL_StatDigitsRectMass = new[] {
                new [] {new RectangleF(722,208,5,4), new RectangleF(727,208,5,4), new RectangleF(732,208,5,4) },
                new [] {new RectangleF(722,208,5,4), new RectangleF(727,208,5,4) }, 
                new [] {new RectangleF(722,208,5,4)}
                };

            RightPlayer.PF_SB_STEAL_StatDigPosPoints = new[] { new PixelPoint(738, 227), new PixelPoint(733, 227), new PixelPoint(728, 227) };
            RightPlayer.PF_SB_STEAL_StatDigitsRectMass = new[] {
                new [] {new RectangleF(722,220,5,4), new RectangleF(727,220,5,4), new RectangleF(732,220,5,4) },
                new [] {new RectangleF(722,220,5,4), new RectangleF(727,220,5,4) }, 
                new [] {new RectangleF(722,220,5,4)}
                };

            RightPlayer.PF_LIMP_FOLD_StatDigPosPoints = new[] { new PixelPoint(738, 239), new PixelPoint(733, 239), new PixelPoint(728, 239) };
            RightPlayer.PF_LIMP_FOLD_StatDigitsRectMass = new[] {
                new [] {new RectangleF(722,232,5,4), new RectangleF(727,232,5,4), new RectangleF(732,232,5,4) },
                new [] {new RectangleF(722,232,5,4), new RectangleF(727,232,5,4) }, 
                new [] {new RectangleF(722,232,5,4)}
                };

            RightPlayer.PF_FOLD_3BET_StatDigPosPoints = new[] { new PixelPoint(738, 251), new PixelPoint(733, 251), new PixelPoint(728, 251) };
            RightPlayer.PF_FOLD_3BET_StatDigitsRectMass = new[] {
                new [] {new RectangleF(722,244,5,4), new RectangleF(727,244,5,4), new RectangleF(732,244,5,4) },
                new [] {new RectangleF(722,244,5,4), new RectangleF(727,244,5,4) }, 
                new [] {new RectangleF(722,244,5,4)}
                };

            RightPlayer.PF_SB_3BET_VS_BTN_StatDigPosPoints = new[] { new PixelPoint(738, 263), new PixelPoint(733, 263), new PixelPoint(728, 263) };
            RightPlayer.PF_SB_3BET_VS_BTN_StatDigitsRectMass = new[] {
                new [] {new RectangleF(722,256,5,4), new RectangleF(727,256,5,4), new RectangleF(732,256,5,4) },
                new [] {new RectangleF(722,256,5,4), new RectangleF(727,256,5,4) }, 
                new [] {new RectangleF(722,256,5,4)}
                };

            RightPlayer.PF_BB_3BET_VS_BTN_StatDigPosPoints = new[] { new PixelPoint(738, 275), new PixelPoint(733, 275), new PixelPoint(728, 275) };
            RightPlayer.PF_BB_3BET_VS_BTN_StatDigitsRectMass = new[] {
                new [] {new RectangleF(722,268,5,4), new RectangleF(727,268,5,4), new RectangleF(732,268,5,4) },
                new [] {new RectangleF(722,268,5,4), new RectangleF(727,268,5,4) }, 
                new [] {new RectangleF(722,268,5,4)}
                };

            RightPlayer.PF_BB_3BET_VS_SB_StatDigPosPoints = new[] { new PixelPoint(738, 287), new PixelPoint(733, 287), new PixelPoint(728, 287) };
            RightPlayer.PF_BB_3BET_VS_SB_StatDigitsRectMass = new[] {
                new [] {new RectangleF(722,280,5,4), new RectangleF(727,280,5,4), new RectangleF(732,280,5,4) },
                new [] {new RectangleF(722,280,5,4), new RectangleF(727,280,5,4) }, 
                new [] {new RectangleF(722,280,5,4)}
                };

            RightPlayer.PF_BB_VS_SBSTEAL_FOLD_StatDigPosPoints = new[] { new PixelPoint(738, 299), new PixelPoint(733, 299), new PixelPoint(728, 299) };
            RightPlayer.PF_BB_VS_SBSTEAL_FOLD_StatDigitsRectMass = new[] {
                new [] {new RectangleF(722,292,5,4), new RectangleF(727,292,5,4), new RectangleF(732,292,5,4) },
                new [] {new RectangleF(722,292,5,4), new RectangleF(727,292,5,4) }, 
                new [] {new RectangleF(722,292,5,4)}
                };

            //не работает в пт4
            RightPlayer.PF_SB_OPENMINRAISE_StatDigPosPoints = new[] { new PixelPoint(731, 282), new PixelPoint(726, 282) };
            RightPlayer.PF_SB_OPENMINRAISE_StatDigitsRectMass = new[] {
                new [] {new RectangleF(720,277,5,4), new RectangleF(725,277,5,4) }, 
                new [] {new RectangleF(720,277,5,4)}
                };

            //RIGHT FLOP
            RightPlayer.F_CBET_StatDigPosPoints = new[] { new PixelPoint(759, 215), new PixelPoint(754, 215), new PixelPoint(749, 215) };
            RightPlayer.F_CBET_StatDigitsRectMass = new[] {
                new [] {new RectangleF(743,208,5,4), new RectangleF(748,208,5,4), new RectangleF(753,208,5,4) },
                new [] {new RectangleF(743,208,5,4), new RectangleF(748,208,5,4) }, 
                new [] {new RectangleF(743,208,5,4)}
                };

            RightPlayer.F_BET_LPOT_StatDigPosPoints = new[] { new PixelPoint(759, 227), new PixelPoint(754, 227), new PixelPoint(749, 227) };
            RightPlayer.F_BET_LPOT_StatDigitsRectMass = new[] {
                new [] {new RectangleF(743,220,5,4), new RectangleF(748,220,5,4), new RectangleF(753,220,5,4) },
                new [] {new RectangleF(743,220,5,4), new RectangleF(748,220,5,4) }, 
                new [] {new RectangleF(743,220,5,4)}
                };

            RightPlayer.F_CBET_FOLDRAISE_StatDigPosPoints = new[] { new PixelPoint(759, 239), new PixelPoint(754, 239), new PixelPoint(749, 239) };
            RightPlayer.F_CBET_FOLDRAISE_StatDigitsRectMass = new[] {
                new [] {new RectangleF(743,232,5,4), new RectangleF(748,232,5,4), new RectangleF(753,232,5,4) },
                new [] {new RectangleF(743,232,5,4), new RectangleF(748,232,5,4) }, 
                new [] {new RectangleF(743,232,5,4)}
                };

            RightPlayer.F_FOLD_CBET_StatDigPosPoints = new[] { new PixelPoint(759, 251), new PixelPoint(754, 251), new PixelPoint(749, 251) };
            RightPlayer.F_FOLD_CBET_StatDigitsRectMass = new[] {
                new [] {new RectangleF(743,244,5,4), new RectangleF(748,244,5,4), new RectangleF(753,244,5,4) },
                new [] {new RectangleF(743,244,5,4), new RectangleF(748,244,5,4) }, 
                new [] {new RectangleF(743,244,5,4)}
                };

            RightPlayer.F_RAISE_CBET_StatDigPosPoints = new[] { new PixelPoint(759, 263), new PixelPoint(754, 263), new PixelPoint(749, 263) };
            RightPlayer.F_RAISE_CBET_StatDigitsRectMass = new[] {
                new [] {new RectangleF(743,256,5,4), new RectangleF(748,256,5,4), new RectangleF(753,256,5,4) },
                new [] {new RectangleF(743,256,5,4), new RectangleF(748,256,5,4) }, 
                new [] {new RectangleF(743,256,5,4)}
                };

            RightPlayer.F_DONK_StatDigPosPoints = new[] { new PixelPoint(759, 275), new PixelPoint(754, 275), new PixelPoint(749, 275) };
            RightPlayer.F_DONK_StatDigitsRectMass = new[] {
                new [] {new RectangleF(743,268,5,4), new RectangleF(748,268,5,4), new RectangleF(753,268,5,4) },
                new [] {new RectangleF(743,268,5,4), new RectangleF(748,268,5,4) }, 
                new [] {new RectangleF(743,268,5,4)}
                };

            RightPlayer.F_DONK_FOLDRAISE_StatDigPosPoints = new[] { new PixelPoint(759, 287), new PixelPoint(754, 287), new PixelPoint(749, 287) };
            RightPlayer.F_DONK_FOLDRAISE_StatDigitsRectMass = new[] {
                new [] {new RectangleF(743,280,5,4), new RectangleF(748,280,5,4), new RectangleF(753,280,5,4) },
                new [] {new RectangleF(743,280,5,4), new RectangleF(748,280,5,4) }, 
                new [] {new RectangleF(743,280,5,4)}
                };







           

            


            //HERO TURN
            Hero.IsTurnPoint = new PixelPoint(456,543);
            Hero.IsTurnColor = Color.FromArgb(250, 207, 63);


        }

        private void InitLists() {
            InitTableNumberDigitsList();
            InitDeckList();
            InitBlindsList();
            InitStackDigitsList();
            InitBetDigitsList();
            InitPotDigitsList();
            InitStatsDigitsList();
        }
        //TODO сделать универсальный метод для инициализации листов.
        private void InitTableNumberDigitsList() {
            Common.TableNumberDigitsList = new List<Bitmap>();
            for (int i = 0; i <= 9; i++){
                Bitmap bmp = new Bitmap(String.Format(@"{0}{1}.bmp", Common.TableNumberDigitsPath, i));
                Common.TableNumberDigitsList.Add(bmp);
            }
        }

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

        
    }
}
