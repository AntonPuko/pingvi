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
            /*
            //pot in bb
            Common.PotDigPosPoints = new[] {new PixelPoint(408, 41), new PixelPoint(411, 41), new PixelPoint(413, 41), new PixelPoint(417,41) };
            Common.PotDigitsRectMass = new[] {
                new[] {new RectangleF(412,42,6,6),new RectangleF(419,42,6,6), new RectangleF(430,42,6,6)},
                new[] {new RectangleF(415,42,6,6),new RectangleF(426,42,6,6)},
                new[] {new RectangleF(417,42,6,6),  new RectangleF(424,42,6,6),new RectangleF(431,42,6,6), },
                new[] {new RectangleF(421,42,6,6), new RectangleF(428,42,6,6)}
            };
            */
            Common.PotDigPosPoints = new[] { new PixelPoint(413, 41), new PixelPoint(410, 41), new PixelPoint(404, 41) };

            Common.PotDigitsRectMass = new[] {
                new[] {new RectangleF(417, 42, 6, 6), new RectangleF(424, 42, 6, 6)},
                new[] {new RectangleF(414, 42, 6, 6), new RectangleF(421, 42, 6, 6), new RectangleF(428, 42, 6, 6),},
                new[] { new RectangleF(408, 42, 6, 6), new RectangleF(419, 42, 6, 6), new RectangleF(426, 42, 6, 6),
                    new RectangleF(433, 42, 6, 6)} };

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
            Common.GoodRegColor = Color.FromArgb(101, 55, 8);
            Common.GoodRegColor2 = Color.FromArgb(100, 6, 6);
            Common.GoodRegColor3 = Color.FromArgb(54, 6, 6);
            Common.GoodRegColor4 = Color.FromArgb(100, 53, 6);
            Common.UberRegColor = Color.FromArgb(100, 5, 10);
            Common.RockColor = Color.FromArgb(54, 53, 6);
            Common.ManiacColor = Color.FromArgb(86, 23, 100);


            //TOURNEY MULTIPLIER
            Common.MultiplierPixelPoint = new PixelPoint(800,445);
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


            /*
            //stacks in bb
            Hero.StackDigPosPoints = new[] { new PixelPoint(400, 459) , new PixelPoint(397, 459), new PixelPoint(395, 459), new PixelPoint(391, 459) };
            Hero.StackDigitsRectMass = new[] {
                new[] {new RectangleF(404, 460, 6, 6), new RectangleF(410, 460, 6, 6)},
                new[] {new RectangleF(401, 460, 6, 6), new RectangleF(408, 460, 6, 6), new RectangleF(415, 460, 6, 6)},
                new[] {new RectangleF(399, 460, 6, 6), new RectangleF(410, 460, 6, 6)},
                new[] {new RectangleF(395, 460, 6, 6), new RectangleF(402, 460, 6, 6), new RectangleF(413, 460, 6, 6)}};
            */

            
             //FOR STACK IN CHIPS
            Hero.StackDigPosPoints = new[] { new PixelPoint(397, 459), new PixelPoint(393, 459), new PixelPoint(388, 459) };
            Hero.StackDigitsRectMass = new[] {
             new[] {new RectangleF(401, 460, 6, 6), new RectangleF(408, 460, 6, 6) }, 
             new[] {new RectangleF(397, 460, 6, 6), new RectangleF(404, 460, 6, 6), new RectangleF(411, 460, 6, 6)},
             new[] {new RectangleF(392, 460, 6, 6), new RectangleF(403, 460, 6, 6),
                    new RectangleF(410, 460, 6, 6), new RectangleF(417, 460, 6, 6)}};
            
            /*
            //stack in BB
            LeftPlayer.StackDigPosPoints = new[] { new PixelPoint(57, 133), new PixelPoint(54, 133), new PixelPoint(52, 133), new PixelPoint(48, 133) };

            LeftPlayer.StackDigitsRectMass = new[] {
             new[] {new RectangleF(61, 134, 6, 6), new RectangleF(67, 134, 6, 6) }, 
             new[] {new RectangleF(58, 134, 6, 6), new RectangleF(65, 134, 6, 6), new RectangleF(72, 134, 6, 6) }, 
             new[] {new RectangleF(56, 134, 6, 6), new RectangleF(67, 134, 6, 6) }, 
             new[] {new RectangleF(52, 134, 6, 6), new RectangleF(59, 134, 6, 6), new RectangleF(70, 134, 6, 6) }, 
             };
            */

            
             //FOR STACK IN CHIPS       
            LeftPlayer.StackDigPosPoints = new[] {new PixelPoint(54, 133), new PixelPoint(50, 133), new PixelPoint(45, 133)};
            LeftPlayer.StackDigitsRectMass = new [] {
             new[] {new RectangleF(58, 134, 6, 6), new RectangleF(65, 134, 6, 6) }, 
             new[] {new RectangleF(54, 134, 6, 6), new RectangleF(61, 134, 6, 6), new RectangleF(68, 138, 6, 6)},
             new[] {new RectangleF(49, 134, 6, 6), new RectangleF(60, 134, 6, 6), 
                    new RectangleF(67, 134, 6, 6), new RectangleF(74, 134, 6, 6)}};
             
            /*

            RightPlayer.StackDigPosPoints = new[] { new PixelPoint(748, 133), new PixelPoint(745, 133), new PixelPoint(743, 133), new PixelPoint(739, 133) };

            RightPlayer.StackDigitsRectMass = new[] {
                new[] {new RectangleF(752, 134, 6, 6), new RectangleF(758, 134, 6, 6) },
                new[] {new RectangleF(749, 134, 6, 6), new RectangleF(756, 134, 6, 6), new RectangleF(773, 134, 6, 6)},
                new[] {new RectangleF(747, 134, 6, 6), new RectangleF(758, 134, 6, 6) },
                new[] {new RectangleF(743, 134, 6, 6), new RectangleF(750, 134, 6, 6), new RectangleF(761, 134, 6, 6)},
            };
             */

            
             // FOR STACK IN CHIPS
            RightPlayer.StackDigPosPoints = new[] {new PixelPoint(745, 133), new PixelPoint(741, 133), new PixelPoint(736, 133)};
            RightPlayer.StackDigitsRectMass = new[] {
             new[] {new RectangleF(749, 134, 6, 6), new RectangleF(756, 134, 6, 6)},
             new[] {new RectangleF(745, 134, 6, 6), new RectangleF(752, 134, 6, 6), new RectangleF(759, 134, 6, 6)},
             new[] {new RectangleF(740, 134, 6, 6), new RectangleF(751, 134, 6, 6),
                    new RectangleF(758, 134, 6, 6), new RectangleF(765, 134, 6, 6)}};
            

            //PLAYER BET
            Common.BetDigitsPath = @"Data\BetDigits\";
            Common.BetDigitsColor = Color.FromArgb(255, 246, 207);

            /*
            Hero.BetDigPosPoints = new[] {
                new PixelPoint(453, 324),new PixelPoint(447, 324), new PixelPoint(444, 324), new PixelPoint(438, 324),
                new PixelPoint(430, 324),new PixelPoint(424, 324), new PixelPoint(421, 324), new PixelPoint(415, 324),
                new PixelPoint(407, 324),new PixelPoint(401, 324), new PixelPoint(398, 324), new PixelPoint(392, 324)};

            Hero.BetDigitsRectMass = new[] {
                new[] {new RectangleF(435, 324, 6, 6), new RectangleF(441, 324, 6, 6), new RectangleF(450, 324, 6, 6)},
                new[] {new RectangleF(435, 324, 6, 6), new RectangleF(444, 324, 6, 6) },
                new[] {new RectangleF(435, 324, 6, 6), new RectangleF(441, 324, 6, 6), new RectangleF(447, 324, 6, 6)},
                new[] {new RectangleF(435, 324, 6, 6),new RectangleF(441, 324, 6, 6) },

                new[] {new RectangleF(412, 324, 6, 6), new RectangleF(418, 324, 6, 6), new RectangleF(427, 324, 6, 6)},
                new[] {new RectangleF(412, 324, 6, 6), new RectangleF(421, 324, 6, 6) },
                new[] {new RectangleF(412, 324, 6, 6), new RectangleF(418, 324, 6, 6), new RectangleF(424, 324, 6, 6)},
                new[] {new RectangleF(412, 324, 6, 6),new RectangleF(418, 324, 6, 6) },

                new[] {new RectangleF(389, 324, 6, 6), new RectangleF(395, 324, 6, 6), new RectangleF(404, 324, 6, 6)},
                new[] {new RectangleF(389, 324, 6, 6), new RectangleF(398, 324, 6, 6) },
                new[] {new RectangleF(389, 324, 6, 6), new RectangleF(395, 324, 6, 6), new RectangleF(401, 324, 6, 6)},
                new[] {new RectangleF(389, 324, 6, 6),new RectangleF(395, 324, 6, 6) },
            };
            */



            // FOR BET IN CHIPS
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
             

            /*


            LeftPlayer.BetDigPosPoints = new[] {

                new PixelPoint(260, 191), new PixelPoint(254, 191),new PixelPoint(251, 191), new PixelPoint(245, 191),
                new PixelPoint(237, 191), new PixelPoint(231, 191),new PixelPoint(228, 191), new PixelPoint(222, 191), 
                new PixelPoint(214, 191), new PixelPoint(208, 191), new PixelPoint(205, 191), new PixelPoint(199, 191)};

            LeftPlayer.BetDigitsRectMass = new[] {


                new[] {new RectangleF(242, 191, 6, 6), new RectangleF(248, 191, 6, 6), new RectangleF(257, 191, 6, 6)},
                new[] {new RectangleF(242, 191, 6, 6), new RectangleF(251, 191, 6, 6)},
                new[] {new RectangleF(242, 191, 6, 6), new RectangleF(248, 191, 6, 6), new RectangleF(254, 191, 6, 6)},
                new[] {new RectangleF(242, 191, 6, 6), new RectangleF(248, 191, 6, 6)},
                


                new[] {new RectangleF(219, 191, 6, 6), new RectangleF(225, 191, 6, 6), new RectangleF(234, 191, 6, 6)},
                new[] {new RectangleF(219, 191, 6, 6), new RectangleF(228, 191, 6, 6)},
                new[] {new RectangleF(219, 191, 6, 6), new RectangleF(225, 191, 6, 6), new RectangleF(231, 191, 6, 6)},
                new[] {new RectangleF(219, 191, 6, 6), new RectangleF(225, 191, 6, 6)},
                
                new[] {new RectangleF(196, 191, 6, 6), new RectangleF(202, 191, 6, 6), new RectangleF(211, 191, 6, 6)},
                new[] {new RectangleF(196, 191, 6, 6), new RectangleF(205, 191, 6, 6)},
                new[] {new RectangleF(196, 191, 6, 6), new RectangleF(202, 191, 6, 6), new RectangleF(208, 191, 6, 6)},
                new[] {new RectangleF(196, 191, 6, 6),new RectangleF(202, 191, 6, 6), },
            };

            */


            
             // FOR BETS IN CHIPS
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
             
             
                 /*

            RightPlayer.BetDigPosPoints = new[] {
                new PixelPoint(594, 197), new PixelPoint(588, 197), new PixelPoint(585, 197), new PixelPoint(579, 197),
                new PixelPoint(571, 197), new PixelPoint(565, 197), new PixelPoint(562, 197), new PixelPoint(556, 197),
                new PixelPoint(548, 197), new PixelPoint(542, 197), new PixelPoint(538, 197), new PixelPoint(533, 197),
            };


            RightPlayer.BetDigitsRectMass = new[] {
                new[] {new RectangleF(598, 198, 6, 6), new RectangleF(604, 198, 6, 6), },
                new[] {new RectangleF(592, 198, 6, 6), new RectangleF(598, 198, 6, 6), new RectangleF(604, 198, 6, 6),},
                new[] {new RectangleF(589, 198, 6, 6), new RectangleF(598, 198, 6, 6), },
                new[] {new RectangleF(583, 198, 6, 6), new RectangleF(589, 198, 6, 6), new RectangleF(598, 198, 6, 6),},

                new[] {new RectangleF(575, 198, 6, 6), new RectangleF(581, 198, 6, 6), },
                new[] {new RectangleF(569, 198, 6, 6), new RectangleF(575, 198, 6, 6), new RectangleF(581, 198, 6, 6),},
                new[] {new RectangleF(566, 198, 6, 6), new RectangleF(575, 198, 6, 6), },
                new[] {new RectangleF(560, 198, 6, 6), new RectangleF(566, 198, 6, 6), new RectangleF(575, 198, 6, 6),},

                new[] {new RectangleF(552, 198, 6, 6), new RectangleF(558, 198, 6, 6), },
                new[] {new RectangleF(546, 198, 6, 6), new RectangleF(552, 198, 6, 6), new RectangleF(558, 198, 6, 6),},
                new[] {new RectangleF(543, 198, 6, 6), new RectangleF(552, 198, 6, 6), },
                new[] {new RectangleF(537, 198, 6, 6), new RectangleF(543, 198, 6, 6), new RectangleF(552, 198, 6, 6),}
                };

                 */
            
            // FOR BETS IN CHIPS
            RightPlayer.BetDigPosPoints = new[] {
                new PixelPoint(601, 198), new PixelPoint(578, 198), new PixelPoint(555, 198)};


            RightPlayer.BetDigitsRectMass = new[] {
                new[] {new RectangleF(577, 198, 6, 6), new RectangleF(586, 198, 6, 6), new RectangleF(592, 198, 6, 6), new RectangleF(598, 198, 6, 6)},
                new[] {new RectangleF(554, 198, 6, 6), new RectangleF(563, 198, 6, 6), new RectangleF(569, 198, 6, 6),new RectangleF(575, 198, 6, 6)},
                new[] {new RectangleF(531, 198, 6, 6), new RectangleF(540, 198, 6, 6), new RectangleF(546, 198, 6, 6),new RectangleF(552, 198, 6, 6)},};
            


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
                new PixelPoint(7, 7), new PixelPoint(15, 7), new PixelPoint(23, 7), new PixelPoint(31, 7),
                new PixelPoint(39, 7), new PixelPoint(47, 7), new PixelPoint(55, 7), new PixelPoint(63, 7),
                new PixelPoint(71, 7), new PixelPoint(79, 7), new PixelPoint(87, 7), new PixelPoint(95, 7)
            };

            Hero.LinePixelPositions = CountLinePixelPositions(Common.LinePixelPositions, new PixelPoint(473,414));
            LeftPlayer.LinePixelPositions = CountLinePixelPositions(Common.LinePixelPositions, new PixelPoint(349, 88));
            RightPlayer.LinePixelPositions = CountLinePixelPositions(Common.LinePixelPositions, new PixelPoint(349, 117));
            //STATISTICS
            Common.StatsDigitsPath = @"Data\StatsDigits\";
            Common.StatsDigitsColor = Color.FromArgb(20, 175, 196);

            //LEFT
              //PREFLOP

            LeftPlayer.PF_BTN_STEAL_StatDigPosPoints = new[] { new PixelPoint(19, 198), new PixelPoint(14, 198) };
            LeftPlayer.PF_BTN_STEAL_StatDigitsRectMass = new[] {
                new [] {new RectangleF(8,193,5,4), new RectangleF(13,193,5,4) }, 
                new [] {new RectangleF(8,193,5,4)}
                };

            LeftPlayer.PF_SB_STEAL_StatDigPosPoints = new[] { new PixelPoint(19, 209), new PixelPoint(14, 209) };
            LeftPlayer.PF_SB_STEAL_StatDigitsRectMass = new[] {
                new [] {new RectangleF(8,204,5,4), new RectangleF(13,204,5,4) }, 
                new [] {new RectangleF(8,204,5,4)}
                };

            LeftPlayer.PF_LIMP_FOLD_StatDigPosPoints = new[] { new PixelPoint(19, 219), new PixelPoint(14, 219) };
            LeftPlayer.PF_LIMP_FOLD_StatDigitsRectMass = new[] {
                new [] {new RectangleF(8,214,5,4), new RectangleF(13,214,5,4) }, 
                new [] {new RectangleF(8,214,5,4)}
                };

            LeftPlayer.PF_FOLD_3BET_StatDigPosPoints = new[] { new PixelPoint(19, 230), new PixelPoint(14, 230) };
            LeftPlayer.PF_FOLD_3BET_StatDigitsRectMass = new[] {
                new [] {new RectangleF(8,225,5,4), new RectangleF(13,225,5,4) }, 
                new [] {new RectangleF(8,225,5,4)}
                };
            
            LeftPlayer.PF_SB_3BET_VS_BTN_StatDigPosPoints = new[] { new PixelPoint(19, 240), new PixelPoint(14, 240) };
            LeftPlayer.PF_SB_3BET_VS_BTN_StatDigitsRectMass = new[] {
                new [] {new RectangleF(8,235,5,4), new RectangleF(13,235,5,4) }, 
                new [] {new RectangleF(8,235,5,4)}
                };

            LeftPlayer.PF_BB_3BET_VS_BTN_StatDigPosPoints = new[] { new PixelPoint(19, 251), new PixelPoint(14, 251) };
            LeftPlayer.PF_BB_3BET_VS_BTN_StatDigitsRectMass = new[] {
                new [] {new RectangleF(8,246,5,4), new RectangleF(13,246,5,4) }, 
                new [] {new RectangleF(8,246,5,4)}
                };

            LeftPlayer.PF_BB_3BET_VS_SB_StatDigPosPoints = new[] { new PixelPoint(19, 261), new PixelPoint(14, 261) };
            LeftPlayer.PF_BB_3BET_VS_SB_StatDigitsRectMass = new[] {
                new [] {new RectangleF(8,256,5,4), new RectangleF(13,256,5,4) }, 
                new [] {new RectangleF(8,256,5,4)}
                };


            LeftPlayer.PF_BB_DEF_VS_SBSTEAL_StatDigPosPoints = new[] { new PixelPoint(19, 272), new PixelPoint(14, 272) };
            LeftPlayer.PF_BB_DEF_VS_SBSTEAL_StatDigitsRectMass = new[] {
                new [] {new RectangleF(8,267,5,4), new RectangleF(13,267,5,4) }, 
                new [] {new RectangleF(8,267,5,4)}
                };

            LeftPlayer.PF_SB_OPENMINRAISE_StatDigPosPoints = new[] { new PixelPoint(19, 282), new PixelPoint(14, 282) };
            LeftPlayer.PF_SB_OPENMINRAISE_StatDigitsRectMass = new[] {
                new [] {new RectangleF(8,277,5,4), new RectangleF(13,277,5,4) }, 
                new [] {new RectangleF(8,277,5,4)}
                };

            LeftPlayer.PF_OPENPUSH_StatDigPosPoints = new[] { new PixelPoint(19, 293), new PixelPoint(14, 293) };
            LeftPlayer.PF_OPENPUSH_StatDigitsRectMass = new[] {
                new [] {new RectangleF(8,289,5,4), new RectangleF(13,289,5,4) }, 
                new [] {new RectangleF(8,289,5,4)}
                };

            //FLOP
            LeftPlayer.F_CBET_StatDigPosPoints = new[] { new PixelPoint(41, 198), new PixelPoint(36, 198) };
            LeftPlayer.F_CBET_StatDigitsRectMass = new[] {
                new [] {new RectangleF(30,193,5,4), new RectangleF(35,193,5,4) }, 
                new [] {new RectangleF(30,193,5,4)}
                };

            LeftPlayer.F_BET_LPOT_StatDigPosPoints = new[] { new PixelPoint(41, 209), new PixelPoint(36, 209) };
            LeftPlayer.F_BET_LPOT_StatDigitsRectMass = new[] {
                new [] {new RectangleF(30,204,5,4), new RectangleF(35,204,5,4) }, 
                new [] {new RectangleF(30,204,5,4)}
                };

            LeftPlayer.F_CBET_FOLDRAISE_StatDigPosPoints = new[] { new PixelPoint(41, 219), new PixelPoint(36, 219) };
            LeftPlayer.F_CBET_FOLDRAISE_StatDigitsRectMass = new[] {
                new [] {new RectangleF(30,214,5,4), new RectangleF(35,214,5,4) }, 
                new [] {new RectangleF(30,214,5,4)}
                };

            LeftPlayer.F_FOLD_CBET_StatDigPosPoints = new[] { new PixelPoint(41, 230), new PixelPoint(36, 230) };
            LeftPlayer.F_FOLD_CBET_StatDigitsRectMass = new[] {
                new [] {new RectangleF(30,225,5,4), new RectangleF(35,225,5,4) }, 
                new [] {new RectangleF(30,225,5,4)}
                };

            LeftPlayer.F_RAISE_CBET_StatDigPosPoints = new[] { new PixelPoint(41, 240), new PixelPoint(36, 240) };
            LeftPlayer.F_RAISE_CBET_StatDigitsRectMass = new[] {
                new [] {new RectangleF(30,235,5,4), new RectangleF(35,235,5,4) }, 
                new [] {new RectangleF(30,235,5,4)}
                };

            LeftPlayer.F_DONK_StatDigPosPoints = new[] { new PixelPoint(41, 251), new PixelPoint(36, 251) };
            LeftPlayer.F_DONK_StatDigitsRectMass = new[] {
                new [] {new RectangleF(30,246,5,4), new RectangleF(35,246,5,4) }, 
                new [] {new RectangleF(30,246,5,4)}
                };

            LeftPlayer.F_DONK_FOLDRAISE_StatDigPosPoints = new[] { new PixelPoint(41, 261), new PixelPoint(36, 261) };
            LeftPlayer.F_DONK_FOLDRAISE_StatDigitsRectMass = new[] {
                new [] {new RectangleF(30,256,5,4), new RectangleF(35,256,5,4) }, 
                new [] {new RectangleF(30,256,5,4)}
                };

            //RIGHT
            //PREFLOP

            RightPlayer.PF_BTN_STEAL_StatDigPosPoints = new[] { new PixelPoint(731, 198), new PixelPoint(726, 198) };
            RightPlayer.PF_BTN_STEAL_StatDigitsRectMass = new[] {
                new [] {new RectangleF(720,193,5,4), new RectangleF(725,193,5,4) }, 
                new [] {new RectangleF(720,193,5,4)}
                };

            RightPlayer.PF_SB_STEAL_StatDigPosPoints = new[] { new PixelPoint(731, 209), new PixelPoint(726, 209) };
            RightPlayer.PF_SB_STEAL_StatDigitsRectMass = new[] {
                new [] {new RectangleF(720,204,5,4), new RectangleF(725,204,5,4) }, 
                new [] {new RectangleF(720,204,5,4)}
                };

            RightPlayer.PF_LIMP_FOLD_StatDigPosPoints = new[] { new PixelPoint(731, 219), new PixelPoint(726, 219) };
            RightPlayer.PF_LIMP_FOLD_StatDigitsRectMass = new[] {
                new [] {new RectangleF(720,214,5,4), new RectangleF(725,214,5,4) }, 
                new [] {new RectangleF(720,214,5,4)}
                };

            RightPlayer.PF_FOLD_3BET_StatDigPosPoints = new[] { new PixelPoint(731, 230), new PixelPoint(726, 230) };
            RightPlayer.PF_FOLD_3BET_StatDigitsRectMass = new[] {
                new [] {new RectangleF(720,225,5,4), new RectangleF(725,225,5,4) }, 
                new [] {new RectangleF(720,225,5,4)}
                };

            RightPlayer.PF_SB_3BET_VS_BTN_StatDigPosPoints = new[] { new PixelPoint(731, 240), new PixelPoint(726, 240) };
            RightPlayer.PF_SB_3BET_VS_BTN_StatDigitsRectMass = new[] {
                new [] {new RectangleF(720,235,5,4), new RectangleF(725,235,5,4) }, 
                new [] {new RectangleF(720,235,5,4)}
                };

            RightPlayer.PF_BB_3BET_VS_BTN_StatDigPosPoints = new[] { new PixelPoint(731, 251), new PixelPoint(726, 251) };
            RightPlayer.PF_BB_3BET_VS_BTN_StatDigitsRectMass = new[] {
                new [] {new RectangleF(720,246,5,4), new RectangleF(725,246,5,4) }, 
                new [] {new RectangleF(720,246,5,4)}
                };

            RightPlayer.PF_BB_3BET_VS_SB_StatDigPosPoints = new[] { new PixelPoint(731, 261), new PixelPoint(726, 261) };
            RightPlayer.PF_BB_3BET_VS_SB_StatDigitsRectMass = new[] {
                new [] {new RectangleF(720,256,5,4), new RectangleF(725,256,5,4) }, 
                new [] {new RectangleF(720,256,5,4)}
                };

            RightPlayer.PF_BB_DEF_VS_SBSTEAL_StatDigPosPoints = new[] { new PixelPoint(731, 272), new PixelPoint(726, 272) };
            RightPlayer.PF_BB_DEF_VS_SBSTEAL_StatDigitsRectMass = new[] {
                new [] {new RectangleF(720,267,5,4), new RectangleF(725,267,5,4) }, 
                new [] {new RectangleF(720,267,5,4)}
                };

            RightPlayer.PF_SB_OPENMINRAISE_StatDigPosPoints = new[] { new PixelPoint(731, 282), new PixelPoint(726, 282) };
            RightPlayer.PF_SB_OPENMINRAISE_StatDigitsRectMass = new[] {
                new [] {new RectangleF(720,277,5,4), new RectangleF(725,277,5,4) }, 
                new [] {new RectangleF(720,277,5,4)}
                };

            RightPlayer.PF_OPENPUSH_StatDigPosPoints = new[] { new PixelPoint(731, 293), new PixelPoint(726, 293) };
            RightPlayer.PF_OPENPUSH_StatDigitsRectMass = new[] {
                new [] {new RectangleF(720,289,5,4), new RectangleF(725,289,5,4) }, 
                new [] {new RectangleF(720,289,5,4)}
                };

            //RIGHT FLOP
            RightPlayer.F_CBET_StatDigPosPoints = new[] { new PixelPoint(753, 198), new PixelPoint(748, 198) };
            RightPlayer.F_CBET_StatDigitsRectMass = new[] {
                new [] {new RectangleF(742,193,5,4), new RectangleF(747,193,5,4) }, 
                new [] {new RectangleF(742,193,5,4)}
                };

            RightPlayer.F_BET_LPOT_StatDigPosPoints = new[] { new PixelPoint(753, 209), new PixelPoint(748, 209) };
            RightPlayer.F_BET_LPOT_StatDigitsRectMass = new[] {
                new [] {new RectangleF(742,204,5,4), new RectangleF(747,204,5,4) }, 
                new [] {new RectangleF(742,204,5,4)}
                };

            RightPlayer.F_CBET_FOLDRAISE_StatDigPosPoints = new[] { new PixelPoint(753, 219), new PixelPoint(748, 219) };
            RightPlayer.F_CBET_FOLDRAISE_StatDigitsRectMass = new[] {
                new [] {new RectangleF(742,214,5,4), new RectangleF(747,214,5,4) }, 
                new [] {new RectangleF(742,214,5,4)}
                };

            RightPlayer.F_FOLD_CBET_StatDigPosPoints = new[] { new PixelPoint(753, 230), new PixelPoint(748, 230) };
            RightPlayer.F_FOLD_CBET_StatDigitsRectMass = new[] {
                new [] {new RectangleF(742,225,5,4), new RectangleF(747,225,5,4) }, 
                new [] {new RectangleF(742,225,5,4)}
                };

            RightPlayer.F_RAISE_CBET_StatDigPosPoints = new[] { new PixelPoint(753, 240), new PixelPoint(748, 240) };
            RightPlayer.F_RAISE_CBET_StatDigitsRectMass = new[] {
                new [] {new RectangleF(742,235,5,4), new RectangleF(747,235,5,4) }, 
                new [] {new RectangleF(742,235,5,4)}
                };

            RightPlayer.F_DONK_StatDigPosPoints = new[] { new PixelPoint(753, 251), new PixelPoint(748, 251) };
            RightPlayer.F_DONK_StatDigitsRectMass = new[] {
                new [] {new RectangleF(742,246,5,4), new RectangleF(747,246,5,4) }, 
                new [] {new RectangleF(742,246,5,4)}
                };

            RightPlayer.F_DONK_FOLDRAISE_StatDigPosPoints = new[] { new PixelPoint(753, 261), new PixelPoint(748, 261) };
            RightPlayer.F_DONK_FOLDRAISE_StatDigitsRectMass = new[] {
                new [] {new RectangleF(742,256,5,4), new RectangleF(747,256,5,4) }, 
                new [] {new RectangleF(742,256,5,4)}
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
