using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private ScreenTableManager _tableManager;
        private ElementsManager _elementManager;
        private DecisionManager _decisionManager;
        private HudWindow _hudWindow;
        
   //     private VmWindow _vmWindow;
    
        public MainWindow() {

            _tableManager = new ScreenTableManager();
            _hudWindow = new HudWindow(_tableManager);
            _hudWindow.Show();
            _elementManager = new ElementsManager();
            _decisionManager = new DecisionManager();

            _tableManager.NewBitmap += _elementManager.OnNewBitmap;
            _elementManager.NewElements += OnNewElements;
            _elementManager.NewElements += _decisionManager.OnNewElements;
            _decisionManager.NewHudInfo += _hudWindow.OnNewHudInfo;


            _decisionManager.NewRangeChosen += OnNewRangeChosen;
            InitializeComponent();


        }
    

        private void StartButton_Click(object sender, RoutedEventArgs e) {
            _tableManager.Start();
        }

        private void OnNewRangeChosen(string rangeName) {
            
            RangeLabel.Content = rangeName;
        }

        private void OnNewElements(Elements elements) {
            
            CommonLabel.Content = String.Format("TABN: {0} DECK: {1} {2} {3} {4} {5} :: Str: {6} :: BL: {7}/{8} :: POT: {9}",
                elements.TableNumber,
                elements.FlopCard1.Name, elements.FlopCard2.Name, elements.FlopCard3.Name,
                elements.TurnCard.Name, elements.RiverCard.Name,
                elements.CurrentStreet, elements.SbAmt, elements.BbAmt, elements.TotalPot);
             

            HeroLabel.Content = String.Format("Status: {0}\nPosition: {1}\nCurStack: {2}\nBet: {3}\nStack: {4}\n" +
                                                  "IsHeroTurn: {5}\nHeroHand: {6}\n HeroStatePreflop: {7}\n HeroStatePostflop: {8}",
                    elements.HeroPlayer.Status, elements.HeroPlayer.Position,
                    elements.HeroPlayer.CurrentStack, elements.HeroPlayer.Bet,
                    elements.HeroPlayer.Stack, elements.HeroPlayer.IsHeroTurn,
                    elements.HeroPlayer.Hand.Name, elements.HeroPlayer.StatePreflop, elements.HeroPlayer.StatePostflop);

            LeftPlayerLabel.Content = String.Format("Status: {0}\nPosition: {1}\nCurStack: {2}\nBet: {3}\nStack: {4}\n" +
                                          "Type: {5} \n\nPF_BTN_STEAL: {6}\nPF_SB_STEAL: {7}\nPF_LIMP_FOLD: {8}" +
                                                    "\nPF_FOLD_3BET: {9}\nPF_SB_3BET_VS_BTN: {10}\nPF_BB_3BET_VS_BTN: {11}" +
                                                    "\nPF_BB_3BET_VS_SB: {12}\nPF_BB_DEF_VS_SBSTEAL: {13}\n" +
                                                    "\nF_CBET: {14}\nF_BET_LPOT: {15}\nF_CBET_FOLDRAISE: {16}" +
                                                    "\nF_FOLD_CBET: {17}\nF_RAISE_CBET: {18}\nF_DONK: {19}" +
                                                    "\nF_DONK_FOLDRAISE: {20}" +
                                                    "\nPF_OPENMINRAISE: {21}",
            elements.LeftPlayer.Status, elements.LeftPlayer.Position,
            elements.LeftPlayer.CurrentStack, elements.LeftPlayer.Bet,
            elements.LeftPlayer.Stack, elements.LeftPlayer.Type,
            elements.LeftPlayer.Stats.PF_BTN_STEAL, elements.LeftPlayer.Stats.PF_SB_STEAL,
            elements.LeftPlayer.Stats.PF_LIMP_FOLD, elements.LeftPlayer.Stats.PF_FOLD_3BET,
            elements.LeftPlayer.Stats.PF_SB_3BET_VS_BTN, elements.LeftPlayer.Stats.PF_BB_3BET_VS_BTN,
            elements.LeftPlayer.Stats.PF_BB_3BET_VS_SB, elements.LeftPlayer.Stats.PF_BB_FOLD_VS_SBSTEAL,
            elements.LeftPlayer.Stats.F_CBET, elements.LeftPlayer.Stats.F_BET_LPOT,
            elements.LeftPlayer.Stats.F_CBET_FOLDRAISE, elements.LeftPlayer.Stats.F_FOLD_CBET,
            elements.LeftPlayer.Stats.F_RAISE_CBET, elements.LeftPlayer.Stats.F_DONK,
            elements.LeftPlayer.Stats.F_DONK_FOLDRAISE, elements.LeftPlayer.Stats.PF_SB_OPENMINRAISE);

            RightPlayerLabel.Content = String.Format("Status: {0}\nPosition: {1}\nCurStack: {2}\nBet: {3}\nStack: {4}\n" +
                                         "Type: {5} \n\nPF_BTN_STEAL: {6}\nPF_SB_STEAL: {7}\nPF_LIMP_FOLD: {8}" +
                                                   "\nPF_FOLD_3BET: {9}\nPF_SB_3BET_VS_BTN: {10}\nPF_BB_3BET_VS_BTN: {11}" +
                                                   "\nPF_BB_3BET_VS_SB: {12}\nPF_BB_DEF_VS_SBSTEAL: {13}\n" +
                                                   "\nF_CBET: {14}\nF_BET_LPOT: {15}\nF_CBET_FOLDRAISE: {16}" +
                                                   "\nF_FOLD_CBET: {17}\nF_RAISE_CBET: {18}\nF_DONK: {19}" +
                                                   "\nF_DONK_FOLDRAISE: {20}" +
                                                     "\nPF_OPENMINRAISE: {21}",
           elements.RightPlayer.Status, elements.RightPlayer.Position,
           elements.RightPlayer.CurrentStack, elements.RightPlayer.Bet,
           elements.RightPlayer.Stack, elements.RightPlayer.Type,
           elements.RightPlayer.Stats.PF_BTN_STEAL, elements.RightPlayer.Stats.PF_SB_STEAL,
           elements.RightPlayer.Stats.PF_LIMP_FOLD, elements.RightPlayer.Stats.PF_FOLD_3BET,
           elements.RightPlayer.Stats.PF_SB_3BET_VS_BTN, elements.RightPlayer.Stats.PF_BB_3BET_VS_BTN,
           elements.RightPlayer.Stats.PF_BB_3BET_VS_SB, elements.RightPlayer.Stats.PF_BB_FOLD_VS_SBSTEAL,
           elements.RightPlayer.Stats.F_CBET, elements.RightPlayer.Stats.F_BET_LPOT,
           elements.RightPlayer.Stats.F_CBET_FOLDRAISE, elements.RightPlayer.Stats.F_FOLD_CBET,
           elements.RightPlayer.Stats.F_RAISE_CBET, elements.RightPlayer.Stats.F_DONK,
           elements.RightPlayer.Stats.F_DONK_FOLDRAISE, elements.RightPlayer.Stats.PF_SB_OPENMINRAISE);

            SituationLabel.Content = String.Format("EffStack: {0} :: HeroRole: {1} :: HeroStatePreflop: {2} :: RelativePos: {3}",
                elements.EffectiveStack, elements.HeroPlayer.Role, elements.HeroPlayer.StatePreflop, elements.HeroPlayer.RelativePosition);

        }

   


        private void BborBucksButton_Click(object sender, RoutedEventArgs e) {
           
        }
    }
}
