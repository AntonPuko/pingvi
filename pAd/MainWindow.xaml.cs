using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PokerModel;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Pingvi
{
    public partial class MainWindow : Window {

        private readonly ScreenTableManager _tableManager;

        public MainWindow() {
            _tableManager = new ScreenTableManager();
            var hudWindow = new HudWindow(_tableManager);
            var elementManager = new ElementsManager();
            var lineManager = new LineManager();
            var decisionManager = new DecisionManager();

            _tableManager.NewBitmap += elementManager.OnNewBitmap;
            elementManager.NewElements += lineManager.OnNewElements;
            lineManager.NewLineInfo += decisionManager.OnNewLineInfo;
            decisionManager.NewDecisionInfo += hudWindow.OnNewDecisionInfo;
            decisionManager.NewDecisionInfo += OnNewDecisionInfo;

            hudWindow.Show();
            InitializeComponent();

        }
    

        private void StartButton_Click(object sender, RoutedEventArgs e) {
            _tableManager.Start();
        }


        private void OnNewDecisionInfo(DecisionInfo decisionInfo) {

            var elements = decisionInfo.LineInfo.Elements;

            CommonLabel.Content = "";
            RangeLabel.Content = "";
            RangeLabel.Content = decisionInfo.PreflopRangeChosen;    

            CommonLabel.Content = String.Format("TABN: {0} DECK: {1} {2} {3} {4} {5} :: Str: {6} :: BL: {7}/{8} :: POT: {9} :: LINE: {10}",
                elements.TableNumber,
                elements.FlopCard1.Name, elements.FlopCard2.Name, elements.FlopCard3.Name,
                elements.TurnCard.Name, elements.RiverCard.Name,
                elements.CurrentStreet, elements.SbAmt, elements.BbAmt, elements.TotalPot, decisionInfo.LineInfo.FinalCompositeLine);


            HeroLabel.Content =
                String.Format("Status: {0}\nPosition: {1}\nLine: {2}\nCurStack: {3}\nBet: {4}\nStack: {5}\n" +
                              "IsHeroTurn: {6}\nHeroHand: {7}\n HeroStatePreflop: {8}\nHeroStatePostflop: {9}",
                    elements.HeroPlayer.Status, elements.HeroPlayer.Position,
                    elements.HeroPlayer.Line,
                    elements.HeroPlayer.CurrentStack, elements.HeroPlayer.Bet,
                    elements.HeroPlayer.Stack, elements.HeroPlayer.IsHeroTurn,
                    elements.HeroPlayer.Hand.Name, elements.HeroPlayer.StatePreflop, elements.HeroPlayer.StatePostflop);

            LeftPlayerLabel.Content = String.Format("Status: {0}\nPosition: {1}\nLine: {2}\nCurStack: {3}\nBet: {4}\nStack: {5}\n" +
                                          "Type: {6} \n\nPF_BTN_STEAL: {7}\nPF_SB_STEAL: {8}\nPF_LIMP_FOLD: {9}" +
                                                    "\nPF_FOLD_3BET: {10}\nPF_SB_3BET_VS_BTN: {11}\nPF_BB_3BET_VS_BTN: {12}" +
                                                    "\nPF_BB_3BET_VS_SB: {13}\nPF_BB_DEF_VS_SBSTEAL: {14}\n" +
                                                    "\nF_CBET: {15}\nF_BET_LPOT: {16}\nF_CBET_FOLDRAISE: {17}" +
                                                    "\nF_FOLD_CBET: {18}\nF_RAISE_CBET: {19}\nF_DONK: {20}" +
                                                    "\nF_DONK_FOLDRAISE: {21}" +
                                                    "\nPF_OPENMINRAISE: {22}",
            elements.LeftPlayer.Status, elements.LeftPlayer.Position,
            elements.LeftPlayer.Line,
            elements.LeftPlayer.CurrentStack, elements.LeftPlayer.Bet,
            elements.LeftPlayer.Stack, elements.LeftPlayer.Type,
            elements.LeftPlayer.Stats.PF_BTN_STEAL, elements.LeftPlayer.Stats.PF_SB_STEAL,
            elements.LeftPlayer.Stats.PF_LIMP_FOLD, elements.LeftPlayer.Stats.PF_FOLD_3BET,
            elements.LeftPlayer.Stats.PF_SB_3BET_VS_BTN, elements.LeftPlayer.Stats.PF_BB_3BET_VS_BTN,
            elements.LeftPlayer.Stats.PF_BB_3BET_VS_SB, elements.LeftPlayer.Stats.PF_BB_DEF_VS_SBSTEAL,
            elements.LeftPlayer.Stats.F_CBET, elements.LeftPlayer.Stats.F_BET_LPOT,
            elements.LeftPlayer.Stats.F_CBET_FOLDRAISE, elements.LeftPlayer.Stats.F_FOLD_CBET,
            elements.LeftPlayer.Stats.F_RAISE_CBET, elements.LeftPlayer.Stats.F_DONK,
            elements.LeftPlayer.Stats.F_DONK_FOLDRAISE, elements.LeftPlayer.Stats.PF_SB_OPENMINRAISE);

            RightPlayerLabel.Content = String.Format("Status: {0}\nPosition: {1}\nLine: {2}\nCurStack: {3}\nBet: {4}\nStack: {5}\n" +
                                          "Type: {6} \n\nPF_BTN_STEAL: {7}\nPF_SB_STEAL: {8}\nPF_LIMP_FOLD: {9}" +
                                                    "\nPF_FOLD_3BET: {10}\nPF_SB_3BET_VS_BTN: {11}\nPF_BB_3BET_VS_BTN: {12}" +
                                                    "\nPF_BB_3BET_VS_SB: {13}\nPF_BB_DEF_VS_SBSTEAL: {14}\n" +
                                                    "\nF_CBET: {15}\nF_BET_LPOT: {16}\nF_CBET_FOLDRAISE: {17}" +
                                                    "\nF_FOLD_CBET: {18}\nF_RAISE_CBET: {19}\nF_DONK: {20}" +
                                                    "\nF_DONK_FOLDRAISE: {21}" +
                                                    "\nPF_OPENMINRAISE: {22}",
           elements.RightPlayer.Status, elements.RightPlayer.Position,
           elements.RightPlayer.Line,
           elements.RightPlayer.CurrentStack, elements.RightPlayer.Bet,
           elements.RightPlayer.Stack, elements.RightPlayer.Type,
           elements.RightPlayer.Stats.PF_BTN_STEAL, elements.RightPlayer.Stats.PF_SB_STEAL,
           elements.RightPlayer.Stats.PF_LIMP_FOLD, elements.RightPlayer.Stats.PF_FOLD_3BET,
           elements.RightPlayer.Stats.PF_SB_3BET_VS_BTN, elements.RightPlayer.Stats.PF_BB_3BET_VS_BTN,
           elements.RightPlayer.Stats.PF_BB_3BET_VS_SB, elements.RightPlayer.Stats.PF_BB_DEF_VS_SBSTEAL,
           elements.RightPlayer.Stats.F_CBET, elements.RightPlayer.Stats.F_BET_LPOT,
           elements.RightPlayer.Stats.F_CBET_FOLDRAISE, elements.RightPlayer.Stats.F_FOLD_CBET,
           elements.RightPlayer.Stats.F_RAISE_CBET, elements.RightPlayer.Stats.F_DONK,
           elements.RightPlayer.Stats.F_DONK_FOLDRAISE, elements.RightPlayer.Stats.PF_SB_OPENMINRAISE);

            SituationLabel.Content = String.Format("EffStack: {0} :: HeroRole: {1} :: HeroStatePreflop: {2} :: RelativePos: {3} " +
                                                   "\nPotType: {4} :: PfState: {5} :: FlopState: {6} :: TurnState: {7} :: RiverState: {8}",
                elements.EffectiveStack, elements.HeroPlayer.Role, elements.HeroPlayer.StatePreflop, elements.HeroPlayer.RelativePosition,
                decisionInfo.LineInfo.PotType, decisionInfo.LineInfo.HeroPreflopState, decisionInfo.LineInfo.HeroFlopState, 
                decisionInfo.LineInfo.HeroTurnState, decisionInfo.LineInfo.HeroRiverState);

        }

   


     
    }
}
