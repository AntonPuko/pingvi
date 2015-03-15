﻿using System;
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

            InitializeComponent();


        }
    

        private void StartButton_Click(object sender, RoutedEventArgs e) {
            _tableManager.Start();
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
                                          "Type: {5} \n\nSB_3BETvsBTN: {6}\nBB_3BETvsBTN: {7}\nBB_3BETvsSB: {8}" +
                                                    "\nBB_DEFvsSBSTEAL: {9}\nFoldCBIP: {10}\nFlopRaiseCBIP: {11}\nFlopFoldCBOOP: {12}" +
                                                    "\nFlopRaiseCBOOP: {13}\nFlopCbet: {14}\nFlopCbFoldRIP: {15}\nFlopCbFoldROOP: {16}" +
                                                    "\nFlopDonkBet: {17}\nFlopDonkFold: {18} ",
            elements.LeftPlayer.Status, elements.LeftPlayer.Position,
            elements.LeftPlayer.CurrentStack, elements.LeftPlayer.Bet,
            elements.LeftPlayer.Stack, elements.LeftPlayer.Type,
            elements.LeftPlayer.Stats.SB_3BETvsBTN, elements.LeftPlayer.Stats.BB_3BETvsBTN,
            elements.LeftPlayer.Stats.BB_3BETvsSB,
            elements.LeftPlayer.Stats.BB_DEFvsSBSTEAL,elements.LeftPlayer.Stats.FlopFoldCBIP,
            elements.LeftPlayer.Stats.FlopRaiseCBIP, elements.LeftPlayer.Stats.FlopFoldCBOOP,
            elements.LeftPlayer.Stats.FlopRaiseCBOOP, elements.LeftPlayer.Stats.FlopCbet,
            elements.LeftPlayer.Stats.FlopCbFoldRIP, elements.LeftPlayer.Stats.FlopCbFoldROOP,
            elements.LeftPlayer.Stats.FlopDonkBet, elements.LeftPlayer.Stats.FlopDonkFold);

            RightPlayerLabel.Content = String.Format("Status: {0}\nPosition: {1}\nCurStack: {2}\nBet: {3}\nStack: {4}\n" +
                                          "Type: {5} \n\nSB_3BETvsBTN: {6}\nBB_3BETvsBTN: {7}\nBB_3BETvsSB: {8}" +
                                                    "\nBB_DEFvsSBSTEAL: {9}\nFoldCBIP: {10}\nFlopRaiseCBIP: {11}\nFlopFoldCBOOP: {12}" +
                                                    "\nFlopRaiseCBOOP: {13}\nFlopCbet: {14}\nFlopCbFoldRIP: {15}\nFlopCbFoldROOP: {16}" +
                                                    "\nFlopDonkBet: {17}\nFlopDonkFold: {18} ",
                       elements.RightPlayer.Status, elements.RightPlayer.Position,
                       elements.RightPlayer.CurrentStack, elements.RightPlayer.Bet,
                       elements.RightPlayer.Stack, elements.RightPlayer.Type,
                       elements.RightPlayer.Stats.SB_3BETvsBTN, elements.RightPlayer.Stats.BB_3BETvsBTN,
                       elements.RightPlayer.Stats.BB_3BETvsSB,
                       elements.RightPlayer.Stats.BB_DEFvsSBSTEAL, elements.RightPlayer.Stats.FlopFoldCBIP,
                       elements.RightPlayer.Stats.FlopRaiseCBIP, elements.RightPlayer.Stats.FlopFoldCBOOP,
                       elements.RightPlayer.Stats.FlopRaiseCBOOP, elements.RightPlayer.Stats.FlopCbet,
                       elements.RightPlayer.Stats.FlopCbFoldRIP, elements.RightPlayer.Stats.FlopCbFoldROOP,
                       elements.RightPlayer.Stats.FlopDonkBet, elements.RightPlayer.Stats.FlopDonkFold);

            SituationLabel.Content = String.Format("EffStack: {0} :: HeroRole: {1} :: HeroStatePreflop: {2} :: RelativePos: {3}",
                elements.EffectiveStack, elements.HeroPlayer.Role, elements.HeroPlayer.StatePreflop, elements.HeroPlayer.RelativePosition);

        }

   


        private void BborBucksButton_Click(object sender, RoutedEventArgs e) {
           
        }
    }
}
