using System.Collections.Generic;
using System.Drawing;
using AForge.Imaging;
using AForge.Neuro;

namespace Pingvi
{
    //TODO сделать XML реализацию
    public class ElementsConfig
    {
        public ElementsConfig() {
            Common = new CommonElementsConfig();
            Hero = new HeroElementsConfig();
            LeftPlayer = new PlayerElementsConfig();
            RightPlayer = new PlayerElementsConfig();
            InitElements();
            InitLists();
        }

        public CommonElementsConfig Common { get; set; }
        public HeroElementsConfig Hero { get; set; }
        public PlayerElementsConfig LeftPlayer { get; set; }
        public PlayerElementsConfig RightPlayer { get; set; }

        public PixelPoint HudWindowsPoint { get; set; }

        public Color[] HudWindowColors { get; set; }

        private void InitElements() {
            //pot
            Common.PotDigitsPath = @"Data\PotDigits\";
            Common.PotDigitsColor = Color.FromArgb(0, 0, 0);
            /* POT IN CHIPS
            Common.PotDigPosPoints = new[] { new PixelPoint(409, 38), new PixelPoint(406, 38), new PixelPoint(400, 38) };
            Common.PotDigitsRectMass = new[] {
                new[] {new Rectangle(413, 39, 6, 6), new Rectangle(420, 39, 6, 6)},
                new[] {new Rectangle(410, 39, 6, 6), new Rectangle(417, 39, 6, 6), new Rectangle(424, 39, 6, 6),},
                new[] { new Rectangle(404, 39, 6, 6), new Rectangle(415, 39, 6, 6), new Rectangle(422, 39, 6, 6),
                    new Rectangle(429, 39, 6, 6)} };
             */
            Common.PotDigPosPoints = new[]
            {new PixelPoint(404, 38), new PixelPoint(407, 38), new PixelPoint(409, 38), new PixelPoint(413, 38)};
            Common.PotDigitsRectMass = new[]
            {
                new[] {new Rectangle(408, 39, 6, 6), new Rectangle(415, 39, 6, 6), new Rectangle(426, 39, 6, 6)},
                new[] {new Rectangle(411, 39, 6, 6), new Rectangle(422, 39, 6, 6)},
                new[] {new Rectangle(413, 39, 6, 6), new Rectangle(420, 39, 6, 6), new Rectangle(427, 39, 6, 6)},
                new[] {new Rectangle(417, 39, 6, 6), new Rectangle(424, 39, 6, 6)}
            };

            //CARDS
            Common.DeckPath = @"Data\Deck\";

            Common.FlopCard1Rect = new Rectangle(281, 189, 32, 2);
            Common.FlopCard2Rect = new Rectangle(335, 189, 32, 2);
            Common.FlopCard3Rect = new Rectangle(389, 189, 32, 2);
            Common.TurnCardRect = new Rectangle(443, 189, 32, 2);
            Common.RiverCardRect = new Rectangle(497, 189, 32, 2);

            Hero.Card1Rect = new Rectangle(368, 375, 32, 2);
            Hero.Card2Rect = new Rectangle(418, 375, 32, 2);
            //TODO Init LEftPlayer and RightPlayer cards if needs to

            //BUTTON
            Common.ButtonColor = Color.FromArgb(203, 196, 56);

            Hero.ButtonPoint = new PixelPoint(396, 345);
            LeftPlayer.ButtonPoint = new PixelPoint(142, 175);
            RightPlayer.ButtonPoint = new PixelPoint(667, 175);

            //BLINDS
            Common.Blinds.DigitsPath = @"Data\BlindsDigits\";
            Common.Blinds.DigitsPointColor = Color.FromArgb(255, 255, 255);
            Common.Blinds.DigitsPosPoints = new[] {new PixelPoint(235, 417)};
            Common.Blinds.DigitsRectMass = new[] {new Rectangle(262, 414, 8, 5)};
            Common.Blinds.BigBlindsDoubleMass = new[] {20.0, 30.0, 40.0, 60.0, 80.0, 100.0, 120.0};
            Common.BlindMassStrings = new[]
            {
                @"$10/$20", @"$15/$30", @"$20/$40", @"$30/$60",
                @"$40/$80", @"$50/$100", @"$60/$120", @"$75/$150", @"$90/$180"
            };


            //PLAYER TYPE
            //TODO переписать метод определения типа игрока, сделать расширяемым
            LeftPlayer.PlayerTypePoint = new PixelPoint(144, 92);
            RightPlayer.PlayerTypePoint = new PixelPoint(663, 93);

            Common.FishColor = Color.FromArgb(47, 220, 47);
            Common.WeakRegColor = Color.FromArgb(47, 134, 220);
            Common.GoodRegColor = Color.FromArgb(220, 134, 47);
            Common.GoodRegColor2 = Color.FromArgb(220, 47, 47);
            Common.GoodRegColor3 = Color.FromArgb(134, 47, 47);
            Common.GoodRegColor4 = Color.FromArgb(100, 53, 6);
            Common.UberRegColor = Color.FromArgb(134, 47, 47);
            Common.RockColor = Color.FromArgb(54, 53, 6);
            Common.ManiacColor = Color.FromArgb(86, 23, 100);

            //TOURNEY MULTIPLIER
            Common.MultiplierPixelPoint = new PixelPoint(791, 390);
            Common.MultiplierColors = new Dictionary<int, Color>
            {
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
            Common.InHandColor = Color.FromArgb(230, 230, 230);
            Common.SitOutColor = Color.FromArgb(192, 192, 192);

            Hero.PlayerStatusPointGame = new PixelPoint(396, 473);
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

            /* STAK IN CHIPS
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
             */

            Hero.StackDigPosPoints = new[]
            {new PixelPoint(396, 455), new PixelPoint(393, 455), new PixelPoint(391, 455), new PixelPoint(387, 455)};
            Hero.StackDigitsRectMass = new[]
            {
                new[] {new Rectangle(400, 456, 6, 6), new Rectangle(406, 456, 6, 6)},
                new[] {new Rectangle(397, 456, 6, 6), new Rectangle(404, 456, 6, 6), new Rectangle(411, 456, 6, 6)},
                new[] {new Rectangle(395, 456, 6, 6), new Rectangle(406, 456, 6, 6)},
                new[] {new Rectangle(391, 456, 6, 6), new Rectangle(398, 456, 6, 6), new Rectangle(409, 456, 6, 6)}
            };

            LeftPlayer.StackDigPosPoints = new[]
            {new PixelPoint(54, 130), new PixelPoint(51, 130), new PixelPoint(49, 130), new PixelPoint(45, 130)};
            LeftPlayer.StackDigitsRectMass = new[]
            {
                new[] {new Rectangle(58, 131, 6, 6), new Rectangle(64, 131, 6, 6)},
                new[] {new Rectangle(55, 131, 6, 6), new Rectangle(62, 131, 6, 6), new Rectangle(69, 131, 6, 6)},
                new[] {new Rectangle(53, 131, 6, 6), new Rectangle(64, 131, 6, 6)},
                new[] {new Rectangle(49, 131, 6, 6), new Rectangle(56, 131, 6, 6), new Rectangle(67, 131, 6, 6)}
            };

            RightPlayer.StackDigPosPoints = new[]
            {new PixelPoint(743, 130), new PixelPoint(740, 130), new PixelPoint(738, 130), new PixelPoint(734, 130)};
            RightPlayer.StackDigitsRectMass = new[]
            {
                new[] {new Rectangle(747, 131, 6, 6), new Rectangle(753, 131, 6, 6)},
                new[] {new Rectangle(744, 131, 6, 6), new Rectangle(751, 131, 6, 6), new Rectangle(768, 131, 6, 6)},
                new[] {new Rectangle(742, 131, 6, 6), new Rectangle(753, 131, 6, 6)},
                new[] {new Rectangle(738, 131, 6, 6), new Rectangle(745, 131, 6, 6), new Rectangle(756, 131, 6, 6)}
            };

            //PLAYER BET
            Common.BetDigitsPath = @"Data\BetDigits\";
            Common.BetDigitsColor = Color.FromArgb(255, 246, 207);

            /* BETS IN CHIPS
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
            */

            Hero.BetDigPosPoints = new[]
            {
                new PixelPoint(449, 320), new PixelPoint(443, 320), new PixelPoint(440, 320), new PixelPoint(434, 320),
                new PixelPoint(426, 320), new PixelPoint(420, 320), new PixelPoint(417, 320), new PixelPoint(411, 320),
                new PixelPoint(403, 320), new PixelPoint(397, 320), new PixelPoint(394, 320), new PixelPoint(388, 320)
            };

            Hero.BetDigitsRectMass = new[]
            {
                new[] {new Rectangle(431, 320, 6, 6), new Rectangle(437, 320, 6, 6), new Rectangle(446, 320, 6, 6)},
                new[] {new Rectangle(431, 320, 6, 6), new Rectangle(440, 320, 6, 6)},
                new[] {new Rectangle(431, 320, 6, 6), new Rectangle(437, 320, 6, 6), new Rectangle(443, 320, 6, 6)},
                new[] {new Rectangle(431, 320, 6, 6), new Rectangle(437, 320, 6, 6)},
                new[] {new Rectangle(408, 320, 6, 6), new Rectangle(414, 320, 6, 6), new Rectangle(423, 320, 6, 6)},
                new[] {new Rectangle(408, 320, 6, 6), new Rectangle(417, 320, 6, 6)},
                new[] {new Rectangle(408, 320, 6, 6), new Rectangle(414, 320, 6, 6), new Rectangle(420, 320, 6, 6)},
                new[] {new Rectangle(408, 320, 6, 6), new Rectangle(414, 320, 6, 6)},
                new[] {new Rectangle(385, 320, 6, 6), new Rectangle(391, 320, 6, 6), new Rectangle(400, 320, 6, 6)},
                new[] {new Rectangle(385, 320, 6, 6), new Rectangle(394, 320, 6, 6)},
                new[] {new Rectangle(385, 320, 6, 6), new Rectangle(391, 320, 6, 6), new Rectangle(397, 320, 6, 6)},
                new[] {new Rectangle(385, 320, 6, 6), new Rectangle(391, 320, 6, 6)}
            };

            LeftPlayer.BetDigPosPoints = new[]
            {
                new PixelPoint(257, 188), new PixelPoint(251, 188), new PixelPoint(248, 188), new PixelPoint(242, 188),
                new PixelPoint(234, 188), new PixelPoint(228, 188), new PixelPoint(225, 188), new PixelPoint(219, 188),
                new PixelPoint(211, 188), new PixelPoint(205, 188), new PixelPoint(202, 188), new PixelPoint(196, 188)
            };

            LeftPlayer.BetDigitsRectMass = new[]
            {
                new[] {new Rectangle(239, 188, 6, 6), new Rectangle(245, 188, 6, 6), new Rectangle(254, 188, 6, 6)},
                new[] {new Rectangle(239, 188, 6, 6), new Rectangle(248, 188, 6, 6)},
                new[] {new Rectangle(239, 188, 6, 6), new Rectangle(245, 188, 6, 6), new Rectangle(251, 188, 6, 6)},
                new[] {new Rectangle(239, 188, 6, 6), new Rectangle(245, 188, 6, 6)},
                new[] {new Rectangle(216, 188, 6, 6), new Rectangle(222, 188, 6, 6), new Rectangle(231, 188, 6, 6)},
                new[] {new Rectangle(216, 188, 6, 6), new Rectangle(225, 188, 6, 6)},
                new[] {new Rectangle(216, 188, 6, 6), new Rectangle(222, 188, 6, 6), new Rectangle(228, 188, 6, 6)},
                new[] {new Rectangle(216, 188, 6, 6), new Rectangle(222, 188, 6, 6)},
                new[] {new Rectangle(193, 188, 6, 6), new Rectangle(199, 188, 6, 6), new Rectangle(208, 188, 6, 6)},
                new[] {new Rectangle(193, 188, 6, 6), new Rectangle(202, 188, 6, 6)},
                new[] {new Rectangle(193, 188, 6, 6), new Rectangle(199, 188, 6, 6), new Rectangle(205, 188, 6, 6)},
                new[] {new Rectangle(193, 188, 6, 6), new Rectangle(199, 188, 6, 6)}
            };


            RightPlayer.BetDigPosPoints = new[]
            {
                new PixelPoint(589, 194), new PixelPoint(583, 194), new PixelPoint(580, 194), new PixelPoint(574, 194),
                new PixelPoint(566, 194), new PixelPoint(560, 194), new PixelPoint(557, 194), new PixelPoint(551, 194),
                new PixelPoint(543, 194), new PixelPoint(537, 194), new PixelPoint(534, 194), new PixelPoint(528, 194)
            };

            RightPlayer.BetDigitsRectMass = new[]
            {
                new[] {new Rectangle(593, 195, 6, 6), new Rectangle(599, 195, 6, 6)},
                new[] {new Rectangle(587, 195, 6, 6), new Rectangle(593, 195, 6, 6), new Rectangle(599, 195, 6, 6)},
                new[] {new Rectangle(584, 195, 6, 6), new Rectangle(593, 195, 6, 6)},
                new[] {new Rectangle(578, 195, 6, 6), new Rectangle(584, 195, 6, 6), new Rectangle(593, 195, 6, 6)},
                new[] {new Rectangle(570, 195, 6, 6), new Rectangle(576, 195, 6, 6)},
                new[] {new Rectangle(564, 195, 6, 6), new Rectangle(570, 195, 6, 6), new Rectangle(576, 195, 6, 6)},
                new[] {new Rectangle(561, 195, 6, 6), new Rectangle(570, 195, 6, 6)},
                new[] {new Rectangle(555, 195, 6, 6), new Rectangle(561, 195, 6, 6), new Rectangle(570, 195, 6, 6)},
                new[] {new Rectangle(547, 195, 6, 6), new Rectangle(553, 195, 6, 6)},
                new[] {new Rectangle(541, 195, 6, 6), new Rectangle(547, 195, 6, 6), new Rectangle(553, 195, 6, 6)},
                new[] {new Rectangle(538, 195, 6, 6), new Rectangle(547, 195, 6, 6)},
                new[] {new Rectangle(532, 195, 6, 6), new Rectangle(538, 195, 6, 6), new Rectangle(547, 195, 6, 6)}
            };


            //LINE
            Common.LineLettersColorDictionary = new Dictionary<Color, string>();
            Common.LineLettersColorDictionary.Add(Color.FromArgb(128, 128, 128), "|");
            Common.LineLettersColorDictionary.Add(Color.FromArgb(64, 128, 128), "f");
            Common.LineLettersColorDictionary.Add(Color.FromArgb(192, 192, 192), "x");
            Common.LineLettersColorDictionary.Add(Color.FromArgb(255, 128, 255), "l");
            Common.LineLettersColorDictionary.Add(Color.FromArgb(255, 255, 0), "c");
            Common.LineLettersColorDictionary.Add(Color.FromArgb(255, 128, 64), "b");
            Common.LineLettersColorDictionary.Add(Color.FromArgb(255, 128, 128), "r");

            Common.LinePixelPositions = new[]
            {
                new PixelPoint(7, 8), new PixelPoint(16, 8), new PixelPoint(25, 8), new PixelPoint(33, 8),
                new PixelPoint(42, 8), new PixelPoint(51, 8), new PixelPoint(59, 8), new PixelPoint(69, 8),
                new PixelPoint(77, 8), new PixelPoint(84, 8)
            };

            Hero.LinePixelPositions = CountLinePixelPositions(Common.LinePixelPositions, new PixelPoint(462, 405));
            LeftPlayer.LinePixelPositions = CountLinePixelPositions(Common.LinePixelPositions, new PixelPoint(226, 82));
            RightPlayer.LinePixelPositions = CountLinePixelPositions(Common.LinePixelPositions, new PixelPoint(468, 82));


            //LINE ALTER
            Common.LineNetworkPath = @"Data\network.net";
            Common.LineNetwork = Network.Load(Common.LineNetworkPath) as ActivationNetwork;

            Common.LineLettersNumbersDictionary = new Dictionary<int, string>();
            Common.LineLettersNumbersDictionary.Add(0, "f");
            Common.LineLettersNumbersDictionary.Add(1, "l");
            Common.LineLettersNumbersDictionary.Add(2, "r");
            Common.LineLettersNumbersDictionary.Add(3, "c");
            Common.LineLettersNumbersDictionary.Add(4, "x");
            Common.LineLettersNumbersDictionary.Add(5, "b");
            Common.LineLettersNumbersDictionary.Add(6, "|");
            Common.LineLettersNumbersDictionary.Add(7, "");
            Common.LineLettersNumbersDictionary.Add(8, "");


            Common.LineRectPositions = new[]
            {
                new Rectangle(4, 10, 7, 9), new Rectangle(12, 10, 7, 9), new Rectangle(20, 10, 7, 9),
                new Rectangle(28, 10, 7, 9),
                new Rectangle(36, 10, 7, 9), new Rectangle(44, 10, 7, 9), new Rectangle(52, 10, 7, 9),
                new Rectangle(60, 10, 7, 9),
                new Rectangle(68, 10, 7, 9), new Rectangle(74, 10, 7, 9)
            };

            LeftPlayer.LineRectPosition = CountPlayerLineRectPositions(Common.LineRectPositions, new PixelPoint(234, 2));
            RightPlayer.LineRectPosition = CountPlayerLineRectPositions(Common.LineRectPositions, new PixelPoint(347, 2));
            Hero.LineRectPosition = CountPlayerLineRectPositions(Common.LineRectPositions, new PixelPoint(459, 2));

            //STATISTICS
            Common.StatsDigitsPath = @"Data\StatsDigits\";

            //Common.StatsDigitsColor = Color.FromArgb(25, 70, 42);
            Common.StatsDigitsColor = Color.FromArgb(11, 157, 164);
            //LEFT
            //FRIST PANEL
            LeftPlayer.PfBtnStealStatDigPosPoints = new[] {new PixelPoint(21, 196), new PixelPoint(16, 196)};
            LeftPlayer.PfBtnStealStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(10, 190, 5, 4), new Rectangle(15, 190, 5, 4)},
                new[] {new Rectangle(10, 190, 5, 4)}
            };

            LeftPlayer.PfSbStealStatDigPosPoints = new[] {new PixelPoint(21, 207), new PixelPoint(16, 207)};
            LeftPlayer.PfSbStealStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(10, 201, 5, 4), new Rectangle(15, 201, 5, 4)},
                new[] {new Rectangle(10, 201, 5, 4)}
            };

            LeftPlayer.PfOpenpushStatDigPosPoints = new[] {new PixelPoint(21, 217), new PixelPoint(16, 217)};
            LeftPlayer.PfOpenpushStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(10, 211, 5, 4), new Rectangle(15, 211, 5, 4)},
                new[] {new Rectangle(10, 211, 5, 4)}
            };

            LeftPlayer.PfLimpFoldStatDigPosPoints = new[] {new PixelPoint(21, 228), new PixelPoint(16, 228)};
            LeftPlayer.PfLimpFoldStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(10, 222, 5, 4), new Rectangle(15, 222, 5, 4)},
                new[] {new Rectangle(10, 222, 5, 4)}
            };

            LeftPlayer.PfLimpReraiseStatDigPosPoints = new[] {new PixelPoint(21, 238), new PixelPoint(16, 238)};
            LeftPlayer.PfLimpReraiseStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(10, 232, 5, 4), new Rectangle(15, 232, 5, 4)},
                new[] {new Rectangle(10, 232, 5, 4)}
            };

            LeftPlayer.PfFold_3BetStatDigPosPoints = new[] {new PixelPoint(21, 249), new PixelPoint(16, 249)};
            LeftPlayer.PfFold_3BetStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(10, 243, 5, 4), new Rectangle(15, 243, 5, 4)},
                new[] {new Rectangle(10, 243, 5, 4)}
            };

            LeftPlayer.PfBbDefVsSbstealStatDigPosPoints = new[] {new PixelPoint(21, 259), new PixelPoint(16, 259)};
            LeftPlayer.PfBbDefVsSbstealStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(10, 253, 5, 4), new Rectangle(15, 253, 5, 4)},
                new[] {new Rectangle(10, 253, 5, 4)}
            };


            LeftPlayer.PfRaiseLimperStatDigPosPoints = new[] {new PixelPoint(21, 270), new PixelPoint(16, 270)};
            LeftPlayer.PfRaiseLimperStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(10, 264, 5, 4), new Rectangle(15, 264, 5, 4)},
                new[] {new Rectangle(10, 264, 5, 4)}
            };

            LeftPlayer.PfSb_3BetVsBtnStatDigPosPoints = new[] {new PixelPoint(21, 280), new PixelPoint(16, 280)};
            LeftPlayer.PfSb_3BetVsBtnStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(10, 274, 5, 4), new Rectangle(15, 274, 5, 4)},
                new[] {new Rectangle(10, 274, 5, 4)}
            };

            LeftPlayer.PfBb_3BetVsBtnStatDigPosPoints = new[] {new PixelPoint(21, 291), new PixelPoint(16, 291)};
            LeftPlayer.PfBb_3BetVsBtnStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(10, 285, 5, 4), new Rectangle(15, 285, 5, 4)},
                new[] {new Rectangle(10, 285, 5, 4)}
            };

            LeftPlayer.PfBb_3BetVsSbStatDigPosPoints = new[] {new PixelPoint(21, 301), new PixelPoint(16, 301)};
            LeftPlayer.PfBb_3BetVsSbStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(10, 295, 5, 4), new Rectangle(15, 295, 5, 4)},
                new[] {new Rectangle(10, 295, 5, 4)}
            };

            //SECOND PANEL
            LeftPlayer.FCbetStatDigPosPoints = new[] {new PixelPoint(43, 196), new PixelPoint(38, 196)};
            LeftPlayer.FCbetStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(32, 190, 5, 4), new Rectangle(37, 190, 5, 4)},
                new[] {new Rectangle(32, 190, 5, 4)}
            };

            LeftPlayer.CbetStatDigPosPoints = new[] {new PixelPoint(43, 207), new PixelPoint(38, 207)};
            LeftPlayer.CbetStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(32, 201, 5, 4), new Rectangle(37, 201, 5, 4)},
                new[] {new Rectangle(32, 201, 5, 4)}
            };

            LeftPlayer.RCbetStatDigPosPoints = new[] {new PixelPoint(43, 217), new PixelPoint(38, 217)};
            LeftPlayer.RCbetStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(32, 211, 5, 4), new Rectangle(37, 211, 5, 4)},
                new[] {new Rectangle(32, 211, 5, 4)}
            };

            LeftPlayer.FFoldCbetStatDigPosPoints = new[] {new PixelPoint(43, 228), new PixelPoint(38, 228)};
            LeftPlayer.FFoldCbetStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(32, 222, 5, 4), new Rectangle(37, 222, 5, 4)},
                new[] {new Rectangle(32, 222, 5, 4)}
            };

            LeftPlayer.FoldCbetStatDigPosPoints = new[] {new PixelPoint(43, 238), new PixelPoint(38, 238)};
            LeftPlayer.FoldCbetStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(32, 232, 5, 4), new Rectangle(37, 232, 5, 4)},
                new[] {new Rectangle(32, 232, 5, 4)}
            };

            LeftPlayer.RFoldCbetStatDigPosPoints = new[] {new PixelPoint(43, 249), new PixelPoint(38, 249)};
            LeftPlayer.RFoldCbetStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(32, 243, 5, 4), new Rectangle(37, 243, 5, 4)},
                new[] {new Rectangle(32, 243, 5, 4)}
            };

            LeftPlayer.FCbetFoldraiseStatDigPosPoints = new[] {new PixelPoint(43, 259), new PixelPoint(38, 259)};
            LeftPlayer.FCbetFoldraiseStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(32, 253, 5, 4), new Rectangle(37, 253, 5, 4)},
                new[] {new Rectangle(32, 253, 5, 4)}
            };

            LeftPlayer.CbetFoldraiseStatDigPosPoints = new[] {new PixelPoint(43, 270), new PixelPoint(38, 270)};
            LeftPlayer.CbetFoldraiseStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(32, 264, 5, 4), new Rectangle(37, 264, 5, 4)},
                new[] {new Rectangle(32, 264, 5, 4)}
            };

            LeftPlayer.FRaiseBetStatDigPosPoints = new[] {new PixelPoint(43, 280), new PixelPoint(38, 280)};
            LeftPlayer.FRaiseBetStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(32, 274, 5, 4), new Rectangle(37, 274, 5, 4)},
                new[] {new Rectangle(32, 274, 5, 4)}
            };

            LeftPlayer.RaiseBetStatDigPosPoints = new[] {new PixelPoint(43, 291), new PixelPoint(38, 291)};
            LeftPlayer.RaiseBetStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(32, 285, 5, 4), new Rectangle(37, 285, 5, 4)},
                new[] {new Rectangle(32, 285, 5, 4)}
            };

            LeftPlayer.FLpStealStatDigPosPoints = new[] {new PixelPoint(43, 301), new PixelPoint(38, 301)};
            LeftPlayer.FLpStealStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(32, 295, 5, 4), new Rectangle(37, 295, 5, 4)},
                new[] {new Rectangle(32, 295, 5, 4)}
            };

            //THIRD PANEL
            LeftPlayer.FLpFoldVsStealStatDigPosPoints = new[] {new PixelPoint(65, 196), new PixelPoint(60, 196)};
            LeftPlayer.FLpFoldVsStealStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(54, 190, 5, 4), new Rectangle(59, 190, 5, 4)},
                new[] {new Rectangle(54, 190, 5, 4)}
            };

            LeftPlayer.FLpFoldVsXrStatDigPosPoints = new[] {new PixelPoint(65, 207), new PixelPoint(60, 207)};
            LeftPlayer.FLpFoldVsXrStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(54, 201, 5, 4), new Rectangle(59, 201, 5, 4)},
                new[] {new Rectangle(54, 201, 5, 4)}
            };

            LeftPlayer.FCheckfoldOopStatDigPosPoints = new[] {new PixelPoint(65, 217), new PixelPoint(60, 217)};
            LeftPlayer.FCheckfoldOopStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(54, 211, 5, 4), new Rectangle(59, 211, 5, 4)},
                new[] {new Rectangle(54, 211, 5, 4)}
            };

            LeftPlayer.SkipfFoldVsTProbeStatDigPosPoints = new[] {new PixelPoint(65, 228), new PixelPoint(60, 228)};
            LeftPlayer.SkipfFoldVsTProbeStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(54, 222, 5, 4), new Rectangle(59, 222, 5, 4)},
                new[] {new Rectangle(54, 222, 5, 4)}
            };

            LeftPlayer.RSkiptFoldVsRProbeStatDigPosPoints = new[] {new PixelPoint(65, 238), new PixelPoint(60, 238)};
            LeftPlayer.RSkiptFoldVsRProbeStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(54, 232, 5, 4), new Rectangle(59, 232, 5, 4)},
                new[] {new Rectangle(54, 232, 5, 4)}
            };

            LeftPlayer.FDonkStatDigPosPoints = new[] {new PixelPoint(65, 249), new PixelPoint(60, 249)};
            LeftPlayer.FDonkStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(54, 243, 5, 4), new Rectangle(59, 243, 5, 4)},
                new[] {new Rectangle(54, 243, 5, 4)}
            };

            LeftPlayer.DonkStatDigPosPoints = new[] {new PixelPoint(65, 259), new PixelPoint(60, 259)};
            LeftPlayer.DonkStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(54, 253, 5, 4), new Rectangle(59, 253, 5, 4)},
                new[] {new Rectangle(54, 253, 5, 4)}
            };

            LeftPlayer.FDonkFoldraiseStatDigPosPoints = new[] {new PixelPoint(65, 270), new PixelPoint(60, 270)};
            LeftPlayer.FDonkFoldraiseStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(54, 264, 5, 4), new Rectangle(59, 264, 5, 4)},
                new[] {new Rectangle(54, 264, 5, 4)}
            };

            //RIGHT
            //FIRST PANEL
            RightPlayer.PfBtnStealStatDigPosPoints = new[] {new PixelPoint(726, 196), new PixelPoint(721, 196)};
            RightPlayer.PfBtnStealStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(715, 190, 5, 4), new Rectangle(720, 190, 5, 4)},
                new[] {new Rectangle(715, 190, 5, 4)}
            };

            RightPlayer.PfSbStealStatDigPosPoints = new[] {new PixelPoint(726, 206), new PixelPoint(721, 206)};
            RightPlayer.PfSbStealStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(715, 200, 5, 4), new Rectangle(720, 200, 5, 4)},
                new[] {new Rectangle(715, 200, 5, 4)}
            };

            RightPlayer.PfOpenpushStatDigPosPoints = new[] {new PixelPoint(726, 217), new PixelPoint(721, 217)};
            RightPlayer.PfOpenpushStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(715, 211, 5, 4), new Rectangle(720, 211, 5, 4)},
                new[] {new Rectangle(715, 211, 5, 4)}
            };

            RightPlayer.PfLimpFoldStatDigPosPoints = new[] {new PixelPoint(726, 227), new PixelPoint(721, 227)};
            RightPlayer.PfLimpFoldStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(715, 221, 5, 4), new Rectangle(720, 221, 5, 4)},
                new[] {new Rectangle(715, 221, 5, 4)}
            };

            RightPlayer.PfLimpReraiseStatDigPosPoints = new[] {new PixelPoint(726, 238), new PixelPoint(721, 238)};
            RightPlayer.PfLimpReraiseStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(715, 232, 5, 4), new Rectangle(720, 232, 5, 4)},
                new[] {new Rectangle(715, 232, 5, 4)}
            };

            RightPlayer.PfFold_3BetStatDigPosPoints = new[] {new PixelPoint(726, 248), new PixelPoint(721, 248)};
            RightPlayer.PfFold_3BetStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(715, 242, 5, 4), new Rectangle(720, 242, 5, 4)},
                new[] {new Rectangle(715, 242, 5, 4)}
            };

            RightPlayer.PfBbDefVsSbstealStatDigPosPoints = new[]  {new PixelPoint(726, 259), new PixelPoint(721, 259)};
            RightPlayer.PfBbDefVsSbstealStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(715, 253, 5, 4), new Rectangle(720, 253, 5, 4)},
                new[] {new Rectangle(715, 253, 5, 4)}
            };

            RightPlayer.PfRaiseLimperStatDigPosPoints = new[] {new PixelPoint(726, 269), new PixelPoint(721, 269)};
            RightPlayer.PfRaiseLimperStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(715, 263, 5, 4), new Rectangle(720, 263, 5, 4)},
                new[] {new Rectangle(715, 263, 5, 4)}
            };

            RightPlayer.PfSb_3BetVsBtnStatDigPosPoints = new[] {new PixelPoint(726, 280), new PixelPoint(721, 280)};
            RightPlayer.PfSb_3BetVsBtnStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(715, 274, 5, 4), new Rectangle(720, 274, 5, 4)},
                new[] {new Rectangle(715, 274, 5, 4)}
            };

            RightPlayer.PfBb_3BetVsBtnStatDigPosPoints = new[] {new PixelPoint(726, 290), new PixelPoint(721, 290)};
            RightPlayer.PfBb_3BetVsBtnStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(715, 284, 5, 4), new Rectangle(720, 284, 5, 4)},
                new[] {new Rectangle(715, 284, 5, 4)}
            };

            RightPlayer.PfBb_3BetVsSbStatDigPosPoints = new[] {new PixelPoint(726, 301), new PixelPoint(721, 301)};
            RightPlayer.PfBb_3BetVsSbStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(715, 295, 5, 4), new Rectangle(720, 295, 5, 4)},
                new[] {new Rectangle(715, 295, 5, 4)}
            };

            //RIGHT SECOND PANEL
            RightPlayer.FCbetStatDigPosPoints = new[] {new PixelPoint(748, 196), new PixelPoint(743, 196)};
            RightPlayer.FCbetStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(737, 190, 5, 4), new Rectangle(742, 190, 5, 4)},
                new[] {new Rectangle(737, 190, 5, 4)}
            };

            RightPlayer.CbetStatDigPosPoints = new[] {new PixelPoint(748, 206), new PixelPoint(743, 206)};
            RightPlayer.CbetStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(737, 200, 5, 4), new Rectangle(742, 200, 5, 4)},
                new[] {new Rectangle(737, 200, 5, 4)}
            };

            RightPlayer.RCbetStatDigPosPoints = new[] {new PixelPoint(748, 217), new PixelPoint(743, 217)};
            RightPlayer.RCbetStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(737, 211, 5, 4), new Rectangle(742, 211, 5, 4)},
                new[] {new Rectangle(737, 211, 5, 4)}
            };

            RightPlayer.FFoldCbetStatDigPosPoints = new[] {new PixelPoint(748, 227), new PixelPoint(743, 227)};
            RightPlayer.FFoldCbetStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(737, 221, 5, 4), new Rectangle(742, 221, 5, 4)},
                new[] {new Rectangle(737, 221, 5, 4)}
            };

            RightPlayer.FoldCbetStatDigPosPoints = new[] {new PixelPoint(748, 238), new PixelPoint(743, 238)};
            RightPlayer.FoldCbetStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(737, 232, 5, 4), new Rectangle(742, 232, 5, 4)},
                new[] {new Rectangle(737, 232, 5, 4)}
            };

            RightPlayer.RFoldCbetStatDigPosPoints = new[] {new PixelPoint(748, 248), new PixelPoint(743, 248)};
            RightPlayer.RFoldCbetStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(737, 242, 5, 4), new Rectangle(742, 242, 5, 4)},
                new[] {new Rectangle(737, 242, 5, 4)}
            };

            RightPlayer.FCbetFoldraiseStatDigPosPoints = new[] {new PixelPoint(748, 259), new PixelPoint(743, 259)};
            RightPlayer.FCbetFoldraiseStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(737, 253, 5, 4), new Rectangle(742, 253, 5, 4)},
                new[] {new Rectangle(737, 253, 5, 4)}
            };

            RightPlayer.CbetFoldraiseStatDigPosPoints = new[] {new PixelPoint(748, 269), new PixelPoint(743, 269)};
            RightPlayer.CbetFoldraiseStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(737, 263, 5, 4), new Rectangle(742, 263, 5, 4)},
                new[] {new Rectangle(737, 263, 5, 4)}
            };

            RightPlayer.FRaiseBetStatDigPosPoints = new[] {new PixelPoint(748, 280), new PixelPoint(743, 280)};
            RightPlayer.FRaiseBetStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(737, 274, 5, 4), new Rectangle(742, 274, 5, 4)},
                new[] {new Rectangle(737, 274, 5, 4)}
            };

            RightPlayer.RaiseBetStatDigPosPoints = new[] {new PixelPoint(748, 290), new PixelPoint(743, 290)};
            RightPlayer.RaiseBetStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(737, 284, 5, 4), new Rectangle(742, 284, 5, 4)},
                new[] {new Rectangle(737, 284, 5, 4)}
            };

            RightPlayer.FLpStealStatDigPosPoints = new[] {new PixelPoint(748, 301), new PixelPoint(743, 301)};
            RightPlayer.FLpStealStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(737, 295, 5, 4), new Rectangle(742, 295, 5, 4)},
                new[] {new Rectangle(737, 295, 5, 4)}
            };

            //RIGHT THIRD PANEL
            RightPlayer.FLpFoldVsStealStatDigPosPoints = new[] {new PixelPoint(770, 196), new PixelPoint(765, 196)};
            RightPlayer.FLpFoldVsStealStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(759, 190, 5, 4), new Rectangle(764, 190, 5, 4)},
                new[] {new Rectangle(759, 190, 5, 4)}
            };

            RightPlayer.FLpFoldVsXrStatDigPosPoints = new[] {new PixelPoint(770, 206), new PixelPoint(765, 206)};
            RightPlayer.FLpFoldVsXrStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(759, 200, 5, 4), new Rectangle(764, 200, 5, 4)},
                new[] {new Rectangle(759, 200, 5, 4)}
            };

            RightPlayer.FCheckfoldOopStatDigPosPoints = new[] {new PixelPoint(770, 217), new PixelPoint(765, 217)};
            RightPlayer.FCheckfoldOopStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(759, 211, 5, 4), new Rectangle(764, 211, 5, 4)},
                new[] {new Rectangle(759, 211, 5, 4)}
            };

            RightPlayer.SkipfFoldVsTProbeStatDigPosPoints = new[] {new PixelPoint(770, 227), new PixelPoint(765, 227)};
            RightPlayer.SkipfFoldVsTProbeStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(759, 221, 5, 4), new Rectangle(764, 221, 5, 4)},
                new[] {new Rectangle(759, 221, 5, 4)}
            };

            RightPlayer.RSkiptFoldVsRProbeStatDigPosPoints = new[] {new PixelPoint(770, 238), new PixelPoint(765, 238)};
            RightPlayer.RSkiptFoldVsRProbeStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(759, 232, 5, 4), new Rectangle(764, 232, 5, 4)},
                new[] {new Rectangle(759, 232, 5, 4)}
            };

            RightPlayer.FDonkStatDigPosPoints = new[] {new PixelPoint(770, 248), new PixelPoint(765, 248)};
            RightPlayer.FDonkStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(759, 242, 5, 4), new Rectangle(764, 242, 5, 4)},
                new[] {new Rectangle(759, 242, 5, 4)}
            };

            RightPlayer.DonkStatDigPosPoints = new[] {new PixelPoint(770, 259), new PixelPoint(765, 259)};
            RightPlayer.DonkStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(759, 253, 5, 4), new Rectangle(764, 253, 5, 4)},
                new[] {new Rectangle(759, 253, 5, 4)}
            };

            RightPlayer.FDonkFoldraiseStatDigPosPoints = new[] {new PixelPoint(770, 269), new PixelPoint(765, 269)};
            RightPlayer.FDonkFoldraiseStatDigitsRectMass = new[]
            {
                new[] {new Rectangle(759, 263, 5, 4), new Rectangle(764, 263, 5, 4)},
                new[] {new Rectangle(759, 263, 5, 4)}
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
            Common.DeckListUnmanaged = new List<UnmanagedImage>();
            for (var i = 1; i <= 52; i++)
            {
                var bmp = new Bitmap(string.Format(@"{0}{1}.bmp", Common.DeckPath, i));
                var unmanagedBmp = UnmanagedImage.FromManagedImage(bmp);
                Common.DeckListUnmanaged.Add(unmanagedBmp);
            }
        }


        private void InitBlindsList() {
            Common.Blinds.DigitsListUnmanaged = new List<UnmanagedImage>();
            for (var i = 0; i <= 5; i++)
            {
                var bmpActive = new Bitmap(string.Format(@"{0}{1}.bmp", Common.Blinds.DigitsPath, i));
                var bmpUnmanaged = UnmanagedImage.FromManagedImage(bmpActive);
                Common.Blinds.DigitsListUnmanaged.Add(bmpUnmanaged);
            }
        }


        private void InitStackDigitsList() {
            Common.StackDigitsListUnmanaged = new List<UnmanagedImage>();
            for (var i = 0; i <= 9; i++)
            {
                var bmp = new Bitmap(string.Format(@"{0}{1}.bmp", Common.StackDigitsPath, i));
                Common.StackDigitsListUnmanaged.Add(UnmanagedImage.FromManagedImage(bmp));
            }
        }

        private void InitBetDigitsList() {
            Common.BetDigitsListUnmanaged = new List<UnmanagedImage>();
            for (var i = 0; i <= 9; i++)
            {
                var bmp = new Bitmap(string.Format(@"{0}{1}.bmp", Common.BetDigitsPath, i));
                Common.BetDigitsListUnmanaged.Add(UnmanagedImage.FromManagedImage(bmp));
            }
        }

        private void InitPotDigitsList() {
            Common.PotDigitsListUnmanaged = new List<UnmanagedImage>();
            for (var i = 0; i <= 9; i++)
            {
                var bmp = new Bitmap(string.Format(@"{0}{1}.bmp", Common.PotDigitsPath, i));
                Common.PotDigitsListUnmanaged.Add(UnmanagedImage.FromManagedImage(bmp));
            }
        }

        private void InitStatsDigitsList() {
            Common.StatsDigitsListUnmanaged = new List<UnmanagedImage>();
            for (var i = 0; i <= 9; i++)
            {
                var bmp = new Bitmap(string.Format(@"{0}{1}.bmp", Common.StatsDigitsPath, i));
                Common.StatsDigitsListUnmanaged.Add(UnmanagedImage.FromManagedImage(bmp));
            }
        }

        private PixelPoint[] CountLinePixelPositions(PixelPoint[] commonLinePixelPositions,
            PixelPoint playerCornerPixelPoint) {
            var playerPixelPointsMass = new PixelPoint[commonLinePixelPositions.Length];

            for (var i = 0; i < commonLinePixelPositions.Length; i++)
            {
                playerPixelPointsMass[i] = new PixelPoint(commonLinePixelPositions[i].X + playerCornerPixelPoint.X,
                    commonLinePixelPositions[i].Y + playerCornerPixelPoint.Y);
            }

            return playerPixelPointsMass;
        }


        private Rectangle[] CountPlayerLineRectPositions(Rectangle[] commonLineRectPositions,
            PixelPoint playerCornerPixelPoint) {
            var playerLineRectPositionMass = new Rectangle[commonLineRectPositions.Length];

            for (var i = 0; i < commonLineRectPositions.Length; i++)
            {
                playerLineRectPositionMass[i] = new Rectangle(commonLineRectPositions[i].X + playerCornerPixelPoint.X,
                    commonLineRectPositions[i].Y + playerCornerPixelPoint.Y,
                    commonLineRectPositions[i].Width, commonLineRectPositions[i].Height);
            }
            return playerLineRectPositionMass;
        }
    }
}