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
            Common.StatsDigitsColor = Color.FromArgb(20, 175, 196);

            //done
            LeftPlayer.SB_3BETvsBTN_StatDigPosPoints = new[] { new PixelPoint(19, 198), new PixelPoint(14, 198) };

            LeftPlayer.SB_3BETvsBTN_StatDigitsRectMass = new[] {
                new [] {new RectangleF(8,193,5,4), new RectangleF(13,193,5,4) }, 
                new [] {new RectangleF(8,193,5,4)}
                };
            //done
            LeftPlayer.BB_3BETvsBTN_StatDigPosPoints = new[] { new PixelPoint(19, 209), new PixelPoint(14, 209) };

            LeftPlayer.BB_3BETvsBTN_StatDigitsRectMass = new[] {
                new [] {new RectangleF(8,204,5,4), new RectangleF(13,204,5,4) }, 
                new [] {new RectangleF(8,204,5,4)}
                };
            //done
            LeftPlayer.BB_3BETvsSB_StatDigPosPoints = new[] { new PixelPoint(19, 219), new PixelPoint(14, 219) };

            LeftPlayer.BB_3BETvsSB_StatDigitsRectMass = new[] {
                new [] {new RectangleF(8,214,5,4), new RectangleF(13,214,5,4) }, 
                new [] {new RectangleF(8,214,5,4)}
                };

            //done
            LeftPlayer.BB_DEFvsSBSTEAL_StatDigPosPoints = new[] { new PixelPoint(19, 230), new PixelPoint(14, 230) };

            LeftPlayer.BB_DEFvsSBSTEAL_StatDigitsRectMass = new[] {
                new [] {new RectangleF(8,225,5,4), new RectangleF(13,225,5,4) }, 
                new [] {new RectangleF(8,225,5,4)}
                };
            //done
            LeftPlayer.FlopFoldCBIP_StatDigPosPoints = new[] { new PixelPoint(19, 240), new PixelPoint(14, 240) };

            LeftPlayer.FlopFoldCBIP_StatDigitsRectMass = new[] {
                new [] {new RectangleF(8,235,5,4), new RectangleF(13,235,5,4) }, 
                new [] {new RectangleF(8,235,5,4)}
                };
            //done
            LeftPlayer.FlopRaiseCBIP_StatDigPosPoints = new[] { new PixelPoint(19, 251), new PixelPoint(14, 251) };

            LeftPlayer.FlopRaiseCBIP_StatDigitsRectMass = new[] {
                new [] {new RectangleF(8,246,5,4), new RectangleF(13,246,5,4) }, 
                new [] {new RectangleF(8,246,5,4)}
                };

            LeftPlayer.FlopFoldCBOOP_StatDigPosPoints = new[] { new PixelPoint(19, 261), new PixelPoint(14, 261) };

            LeftPlayer.FlopFoldCBOOP_StatDigitsRectMass = new[] {
                new [] {new RectangleF(8,256,5,4), new RectangleF(13,256,5,4) }, 
                new [] {new RectangleF(8,256,5,4)}
                };



            LeftPlayer.FlopRaiseCBOOP_StatDigPosPoints = new[] { new PixelPoint(19, 272), new PixelPoint(14, 272) };

            LeftPlayer.FlopRaiseCBOOP_StatDigitsRectMass = new[] {
                new [] {new RectangleF(8,267,5,4), new RectangleF(13,267,5,4) }, 
                new [] {new RectangleF(8,267,5,4)}
                };

            LeftPlayer.FlopCbet_StatDigPosPoints = new[] { new PixelPoint(19, 282), new PixelPoint(14, 282) };

            LeftPlayer.FlopCbet_StatDigitsRectMass = new[] {
                new [] {new RectangleF(8,277,5,4), new RectangleF(13,277,5,4) }, 
                new [] {new RectangleF(8,277,5,4)}
                };

            LeftPlayer.FlopCbFoldRIP_StatDigPosPoints = new[] { new PixelPoint(19, 293), new PixelPoint(14, 293) };

            LeftPlayer.FlopCbFoldRIP_StatDigitsRectMass = new[] {
                new [] {new RectangleF(8,288,5,4), new RectangleF(13,288,5,4) }, 
                new [] {new RectangleF(8,288,5,4)}
                };

            LeftPlayer.FlopCbFoldROOP_StatDigPosPoints = new[] { new PixelPoint(19, 303), new PixelPoint(14, 303) };

            LeftPlayer.FlopCbFoldROOP_StatDigitsRectMass = new[] {
                new [] {new RectangleF(8,298,5,4), new RectangleF(13,298,5,4) }, 
                new [] {new RectangleF(8,298,5,4)}
                };


            LeftPlayer.FlopDonkBet_StatDigPosPoints = new[] { new PixelPoint(19, 314), new PixelPoint(14, 314) };

            LeftPlayer.FlopDonkBet_StatDigitsRectMass = new[] {
                new [] {new RectangleF(8,309,5,4), new RectangleF(13,309,5,4) }, 
                new [] {new RectangleF(8,309,5,4)}
                };

            LeftPlayer.FlopDonkFold_StatDigPosPoints = new[] { new PixelPoint(19, 324), new PixelPoint(14, 324) };

            LeftPlayer.FlopDonkFold_StatDigitsRectMass = new[] {
                new [] {new RectangleF(8,319,5,4), new RectangleF(13,319,5,4) }, 
                new [] {new RectangleF(8,319,5,4)}
                };



            RightPlayer.SB_3BETvsBTN_StatDigPosPoints = new[] { new PixelPoint(777, 198), new PixelPoint(772, 198) };

            RightPlayer.SB_3BETvsBTN_StatDigitsRectMass = new[] {
                new [] {new RectangleF(766,193,5,4), new RectangleF(771,193,5,4) }, 
                new [] {new RectangleF(766,193,5,4)}
                };
            //done
            RightPlayer.BB_3BETvsBTN_StatDigPosPoints = new[] { new PixelPoint(777, 209), new PixelPoint(772, 209) };

            RightPlayer.BB_3BETvsBTN_StatDigitsRectMass = new[] {
                new [] {new RectangleF(766,204,5,4), new RectangleF(771,204,5,4) }, 
                new [] {new RectangleF(766,204,5,4)}
                };
            //done
            RightPlayer.BB_3BETvsSB_StatDigPosPoints = new[] { new PixelPoint(777, 219), new PixelPoint(772, 219) };

            RightPlayer.BB_3BETvsSB_StatDigitsRectMass = new[] {
                new [] {new RectangleF(766,214,5,4), new RectangleF(771,214,5,4) }, 
                new [] {new RectangleF(766,214,5,4)}
                };

            //done
            RightPlayer.BB_DEFvsSBSTEAL_StatDigPosPoints = new[] { new PixelPoint(777, 230), new PixelPoint(772, 230) };

            RightPlayer.BB_DEFvsSBSTEAL_StatDigitsRectMass = new[] {
                new [] {new RectangleF(766,225,5,4), new RectangleF(771,225,5,4) }, 
                new [] {new RectangleF(766,225,5,4)}
                };
            //done
            RightPlayer.FlopFoldCBIP_StatDigPosPoints = new[] { new PixelPoint(777, 240), new PixelPoint(772, 240) };

            RightPlayer.FlopFoldCBIP_StatDigitsRectMass = new[] {
                new [] {new RectangleF(766,235,5,4), new RectangleF(771,235,5,4) }, 
                new [] {new RectangleF(766,235,5,4)}
                };
            //done
            RightPlayer.FlopRaiseCBIP_StatDigPosPoints = new[] { new PixelPoint(777, 251), new PixelPoint(772, 251) };

            RightPlayer.FlopRaiseCBIP_StatDigitsRectMass = new[] {
                new [] {new RectangleF(766,246,5,4), new RectangleF(771,246,5,4) }, 
                new [] {new RectangleF(766,246,5,4)}
                };
            //done
            RightPlayer.FlopFoldCBOOP_StatDigPosPoints = new[] { new PixelPoint(777, 261), new PixelPoint(772, 261) };

            RightPlayer.FlopFoldCBOOP_StatDigitsRectMass = new[] {
                new [] {new RectangleF(766,256,5,4), new RectangleF(771,256,5,4) }, 
                new [] {new RectangleF(766,256,5,4)}
                };

            //done
            RightPlayer.FlopRaiseCBOOP_StatDigPosPoints = new[] { new PixelPoint(777, 272), new PixelPoint(772, 272) };

            RightPlayer.FlopRaiseCBOOP_StatDigitsRectMass = new[] {
                new [] {new RectangleF(766,267,5,4), new RectangleF(771,267,5,4) }, 
                new [] {new RectangleF(766,267,5,4)}
                };
            //done
            RightPlayer.FlopCbet_StatDigPosPoints = new[] { new PixelPoint(777, 282), new PixelPoint(772, 282) };

            RightPlayer.FlopCbet_StatDigitsRectMass = new[] {
                new [] {new RectangleF(766,277,5,4), new RectangleF(771,277,5,4) }, 
                new [] {new RectangleF(766,277,5,4)}
                };
            //done
            RightPlayer.FlopCbFoldRIP_StatDigPosPoints = new[] { new PixelPoint(777, 293), new PixelPoint(772, 293) };

            RightPlayer.FlopCbFoldRIP_StatDigitsRectMass = new[] {
                new [] {new RectangleF(766,288,5,4), new RectangleF(771,288,5,4) }, 
                new [] {new RectangleF(766,288,5,4)}
                };
            
            RightPlayer.FlopCbFoldROOP_StatDigPosPoints = new[] { new PixelPoint(777, 303), new PixelPoint(772, 303) };

            RightPlayer.FlopCbFoldROOP_StatDigitsRectMass = new[] {
                new [] {new RectangleF(766,298,5,4), new RectangleF(771,298,5,4) }, 
                new [] {new RectangleF(766,298,5,4)}
                };

            RightPlayer.FlopDonkBet_StatDigPosPoints = new[] { new PixelPoint(777, 314), new PixelPoint(772, 314) };

            RightPlayer.FlopDonkBet_StatDigitsRectMass = new[] {
                new [] {new RectangleF(766,309,5,4), new RectangleF(771,309,5,4) }, 
                new [] {new RectangleF(766,309,5,4)}
                };
            
            RightPlayer.FlopDonkFold_StatDigPosPoints = new[] { new PixelPoint(777, 324), new PixelPoint(772, 324) };

            RightPlayer.FlopDonkFold_StatDigitsRectMass = new[] {
                new [] {new RectangleF(766,319,5,4), new RectangleF(771,319,5,4) }, 
                new [] {new RectangleF(766,319,5,4)}
                };

            


            //HERO TURN
            Hero.IsTurnPoint = new PixelPoint(447,568);
            Hero.IsTurnColor = Color.FromArgb(171, 139, 49);


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
