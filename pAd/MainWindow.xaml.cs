using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using AForge.Imaging.Filters;
using Pingvi.Stuff;
using Pingvi.TableCatchers;
using PokerModel;


namespace Pingvi
{
    public partial class MainWindow : Window {

        private readonly ITableCatcher _tableCatcher;
        private Bitmap _tableBitmap;

        private readonly ElementsManager elementManager;

        public MainWindow() {
            
            var tablePositionRect = new Rectangle(2150, 20, 800, 574);
            // var tablePositionRect = new Rectangle(230, 20, 800, 574);
            int tableFrameInterval = 100;
           // TimeSpan tableFrameIntervalSpan  = TimeSpan.FromMilliseconds(100);
            string screenShotsPath = @"P:\screens\";

            _tableCatcher = new AForgeTableCatcher(tablePositionRect, tableFrameInterval, screenShotsPath);
           // _tableCatcher = new ScreenShotTableCatcher(tablePositionRect, tableFrameIntervalSpan, screenShotsPath);
        
            var hudWindow = new HWindow(_tableCatcher);
            elementManager = new ElementsManager();
            var lineManager = new LineManager();
            var decisionManager = new DecisionManager();

            hudWindow.MakeScreenShotClick += _tableCatcher.MakeScreenShot;

           // _tableCatcher.NewTableBitmap += OnNewTableBitmap;



            //CREATE LINE WINDOWS
            var heroLineWindow = new LWindow(0) {Top = 410, Left = 2360};
            lineManager.NewLineInfo += li => Dispatcher.BeginInvoke((Action)delegate { heroLineWindow.OnNewLineInfo(li); });
            heroLineWindow.Show();

            var leftLineWindow = new LWindow(1) { Top = 120, Left = 2150 };
            lineManager.NewLineInfo += li => Dispatcher.BeginInvoke((Action)delegate { leftLineWindow.OnNewLineInfo(li); });
            leftLineWindow.Show();

            var rightLineWindow = new LWindow(2) { Top = 120, Left = 2380 };
            lineManager.NewLineInfo += li => Dispatcher.BeginInvoke((Action)delegate { rightLineWindow.OnNewLineInfo(li); });
            rightLineWindow.Show();
          



            _tableCatcher.NewTableImage += elementManager.OnNewTableImage;
            elementManager.NewElements += lineManager.OnNewElements;
            lineManager.NewLineInfo += decisionManager.OnNewLineInfo;

           // decisionManager.NewDecisionInfo += hudWindow.OnNewDecisionInfo;
           // decisionManager.NewDecisionInfo +=  OnNewDecisionInfo;
            decisionManager.NewDecisionInfo += di => Dispatcher.BeginInvoke((Action)delegate { hudWindow.OnNewDecisionInfo(di); });
            decisionManager.NewDecisionInfo += di => Dispatcher.BeginInvoke((Action)delegate { OnNewDecisionInfo(di); });

            hudWindow.Show();

            
            //create new thread for result window because of http query lags
            Thread resultWindowThread = new Thread(new ThreadStart(() => {
                var resultWindow = new RWindow();
                resultWindow.Show();
                System.Windows.Threading.Dispatcher.Run();
            }));

            resultWindowThread.SetApartmentState(ApartmentState.STA);
            resultWindowThread.IsBackground = true;
            resultWindowThread.Start();
            
             
            InitializeComponent();

        }

        private void OnNewTableBitmap(Bitmap obj) {
            _tableBitmap = obj;
        }


        private void StartButton_Click(object sender, RoutedEventArgs e) {
            _tableCatcher.Start();
        }

     

        private void OnNewDecisionInfo(DecisionInfo decisionInfo) {
        
            var elements = decisionInfo.LineInfo.Elements;
       
            CommonLabel.Content = "";
            RangeLabel.Content = "";
            RangeLabel.Content = decisionInfo.PreflopRangeChosen;    

            CommonLabel.Content = String.Format(" MULTIPLIER: x{0} DECK: {1} {2} {3} {4} {5} :: Str: {6} :: BL: {7}/{8} :: POT: {9} :: LINE: {10}",
                elements.TourneyMultiplier,
                elements.FlopCard1.Name, elements.FlopCard2.Name, elements.FlopCard3.Name,
                elements.TurnCard.Name, elements.RiverCard.Name,
                elements.CurrentStreet, elements.SbAmt, elements.BbAmt, elements.TotalPot, decisionInfo.LineInfo.FinalCompositeLine );

            HeroLabel.Content =
                String.Format("Status: {0}\nPosition: {1}\nLine: {2}\nCurStack: {3}\nBet: {4}\nStack: {5}\n" +
                              "IsHeroTurn: {6}\nHeroHand: {7}\n HeroStatePreflop: {8}\nHeroStatePostflop: {9}",
                    elements.HeroPlayer.Status, elements.HeroPlayer.Position,
                    elements.HeroPlayer.Line,
                    elements.HeroPlayer.CurrentStack, elements.HeroPlayer.Bet,
                    elements.HeroPlayer.Stack, elements.HeroPlayer.IsHeroTurn,
                    elements.HeroPlayer.Hand.Name, elements.HeroPlayer.StatePreflop, elements.HeroPlayer.StatePostflop);

            LeftPlayerLabel.Content = ShowPlayerInfo(elements.LeftPlayer) + "\n" + ShowPlayerStats(elements.LeftPlayer);

            RightPlayerLabel.Content = ShowPlayerInfo(elements.RightPlayer) + "\n" + ShowPlayerStats(elements.RightPlayer);

            SituationLabel.Content = String.Format("EffStack: {0} :: HeroRole: {1} :: HeroStatePreflop: {2} :: RelativePos: {3} " +
                                                   "\nPotType: {4} :: PfState: {5} :: FlopState: {6} :: TurnState: {7} :: RiverState: {8}",
                elements.EffectiveStack, elements.HeroPlayer.Role, elements.HeroPlayer.StatePreflop, elements.HeroPlayer.RelativePosition,
                decisionInfo.LineInfo.PotType, decisionInfo.LineInfo.HeroPreflopState, decisionInfo.LineInfo.HeroFlopState, 
                decisionInfo.LineInfo.HeroTurnState, decisionInfo.LineInfo.HeroRiverState);

        }

        private string ShowPlayerStats(Player player) {
            var stats = player.Stats;
            return string.Format("\nPF_BTN_STEAL: {0}\nPF_SB_STEAL: {1}\nPF_OPENPUSH: {2}" +
                                 "\nPF_LIMP_FOLD: {3}\nPF_LIMP_RERAISE: {4}\nPF_FOLD_3BET: {5}" +
                                 "\nPF_BB_DEF_VS_SBSTEAL: {6}\nPF_RAISE_LIMPER: {7}\nPF_SB_3BET_VS_BTN: {8}" +
                                 "\nPF_BB_3BET_VS_BTN: {9}\nPF_BB_3BET_VS_SB: {10}" +
                                 "\n\nF_CBET: {11}\nT_CBET: {12}\nR_CBET: {13}" +
                                 "\nF_FOLD_CBET: {14}\nT_FOLD_CBET: {15}\nR_FOLD_CBET: {16}" +
                                 "\nF_CBET_FOLDRAISE: {17}\nT_CBET_FOLDRAISE: {18}\nF_RAISE_BET: {19}" +
                                 "\nT_RAISE_BET: {20}\nF_LP_STEAL: {21}" +
                                 "\n\nF_LP_FOLD_VS_STEAL: {22}\nF_LP_FOLD_VS_XR: {23}\nF_CHECKFOLD_OOP: {24}" +
                                 "\nT_SKIPF_FOLD_VS_T_PROBE: {25}\nR_SKIPT_FOLD_VS_R_PROBE: {26}" +
                                 "\nF_DONK: {27}\nT_DONK: {28}\nF_DONK_FOLDRAISE: {29}",
                                 stats.PF_BTN_STEAL, stats.PF_SB_STEAL, stats.PF_OPENPUSH,
                                 stats.PF_LIMP_FOLD, stats.PF_LIMP_RERAISE, stats.PF_FOLD_3BET,
                                 stats.PF_BB_DEF_VS_SBSTEAL,stats.PF_RAISE_LIMPER, stats.PF_SB_3BET_VS_BTN,
                                 stats.PF_BB_3BET_VS_BTN, stats.PF_BB_3BET_VS_SB,
                                 stats.F_CBET, stats.T_CBET, stats.R_CBET,
                                 stats.F_FOLD_CBET, stats.T_FOLD_CBET, stats.R_FOLD_CBET,
                                 stats.F_CBET_FOLDRAISE, stats.T_CBET_FOLDRAISE, stats.F_RAISE_BET,
                                 stats.T_RAISE_BET, stats.F_LP_STEAL,
                                 stats.F_LP_FOLD_VS_STEAL, stats.F_LP_FOLD_VS_XR, stats.F_CHECKFOLD_OOP,
                                 stats.T_SKIPF_FOLD_VS_T_PROBE,stats.R_SKIPT_FOLD_VS_R_PROBE,
                                 stats.F_DONK,stats.T_DONK, stats.F_DONK_FOLDRAISE
                );
        }

        private string ShowPlayerInfo(Player player) {
            return string.Format("Status: {0}\nPosition: {1}\nLine: {2}" +
                                 "\nCurStack: {3}\nBet: {4}\nType: {5}",
                player.Status, player.Position, player.Line,
                player.CurrentStack, player.Bet, player.Type);
        }

        private void MakeButton_Click(object sender, RoutedEventArgs e) {

            /*
            var rightPRects =  elementManager.ElementConfig.RightPlayer.LineRectPosition;
            var leftPRects = elementManager.ElementConfig.LeftPlayer.LineRectPosition;
            var heroPRects = elementManager.ElementConfig.Hero.LineRectPosition;

            for (int i = 0; i < rightPRects.Length; i++) {
                var rightP = rightPRects[i];
                var leftP = leftPRects[i];
                var heroP = heroPRects[i];

                Crop filterR = new Crop(rightP);
                Crop filterL = new Crop(leftP);
                Crop filterH = new Crop(heroP);

                var bmpR = filterR.Apply(_tableBitmap);
                var bmpL = filterL.Apply(_tableBitmap);
                var bmpH = filterH.Apply(_tableBitmap);

                bmpR = Grayscale.CommonAlgorithms.BT709.Apply(bmpR);
                bmpL = Grayscale.CommonAlgorithms.BT709.Apply(bmpL);
                bmpH = Grayscale.CommonAlgorithms.BT709.Apply(bmpH);

                Threshold tfilter = new Threshold();

                bmpR = tfilter.Apply(bmpR);
                bmpL = tfilter.Apply(bmpL);
                bmpH = tfilter.Apply(bmpH);

                
                const string path = @"P:\screens\letters\";
                bmpR.Save(path + DateTime.Now.Ticks + ".bmp");
                bmpL.Save(path + DateTime.Now.Ticks + ".bmp");
                bmpH.Save(path + DateTime.Now.Ticks + ".bmp");


            }
             */

            Rectangle[] rects = new[] {
                elementManager.ElementConfig.Common.FlopCard1Rect,
                elementManager.ElementConfig.Common.FlopCard2Rect,
                elementManager.ElementConfig.Common.FlopCard3Rect,
                elementManager.ElementConfig.Common.TurnCardRect,
                elementManager.ElementConfig.Common.RiverCardRect,
            };

            const string path = @"P:\screens\cards\";

            foreach (var r in rects) {
                Crop cropfilter = new Crop(r);
                var bmp = cropfilter.Apply(_tableBitmap);
                bmp.Save(path + Array.IndexOf(rects,r) + ".bmp");
            }


        }
    }
}
