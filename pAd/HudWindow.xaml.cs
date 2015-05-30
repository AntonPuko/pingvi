﻿using System;
using System.CodeDom;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using PokerModel;
using Color = System.Windows.Media.Color;

namespace Pingvi
{
    /// <summary>
    /// Interaction logic for HudWindow.xaml
    /// </summary>
    public partial class HudWindow : Window {

        private ScreenTableManager _tableManager;
        private Bitmap _tableBitmap;

        private DispatcherTimer _topTimer;

        public HudWindow(ScreenTableManager tableManager) {
            _tableManager = tableManager;
            _tableManager.NewBitmap += OnNewBitmap;
            InitializeComponent();
          //  InitTimer();
        }

        private void InitTimer() {
            _topTimer = new DispatcherTimer();
            _topTimer.Interval = TimeSpan.FromMilliseconds(100);
            _topTimer.Tick += OnTopTimerTick;
            _topTimer.Start();
        }

        private void OnTopTimerTick(object sender, EventArgs e) {
          //  if(this.IsActive == false)
           // this.Activate();
        }

        private void OnNewBitmap(Bitmap bmp) {
            _tableBitmap = bmp;
        }
        /*
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            DragMove();
        } */

        public void OnNewDecisionInfo(DecisionInfo decisionInfo) {
            
            ShowEffectiveStack(decisionInfo.LineInfo.Elements.EffectiveStack);
            ShowRelativePosition(decisionInfo.LineInfo.Elements.HeroPlayer.RelativePosition);

            ClearStats();
            DecisionRun.Text = "";
            PotTypeRun.Foreground = new SolidColorBrush(Color.FromRgb(125, 125, 125));
            PotTypeRun.Text = "_";
            AdditionalInfoLabel.Content = "";

            var currentStreet = decisionInfo.LineInfo.Elements.CurrentStreet;
            if (currentStreet == CurrentStreet.Preflop) {
                ShowPreflopDecision(decisionInfo);
            }
            else {
                ShowPotType(decisionInfo.LineInfo.PotType);
                if (currentStreet == CurrentStreet.Flop) {
                    ShowFlopInfoStats(decisionInfo);
                }
                if (currentStreet == CurrentStreet.Turn) {
                    ShowTurnInfoStats(decisionInfo);
                }
                if (currentStreet == CurrentStreet.River) {
                    ShowRiverInfoStats(decisionInfo);
                }
            }



        }





        private void ShowEffectiveStack(double effStack) {
            //TODO придумать другую цветовую схему, это сливается в одно, нужно более ярко выделять близкие к пушам стеки
            if (effStack == 0) StackLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            if (effStack > 0.0 && effStack <= 8)
                StackLabel.Foreground = new SolidColorBrush(Color.FromRgb(36, 116, 246));
            if (effStack > 8 && effStack <= 13)
                StackLabel.Foreground = new SolidColorBrush(Color.FromRgb(14, 188, 211));
            if (effStack > 13 && effStack <= 16)
                StackLabel.Foreground = new SolidColorBrush(Color.FromRgb(27, 234, 195));
            if (effStack > 16 && effStack <= 20)
                StackLabel.Foreground = new SolidColorBrush(Color.FromRgb(14, 211, 112));
            if (effStack > 20 && effStack <= 100)
                StackLabel.Foreground = new SolidColorBrush(Color.FromRgb(37, 246, 76));

            StackLabel.Content = "S: " + effStack.ToString("#.#");
        }

        private void ShowPotOdds(double potOdds) {
            AdditionalInfoLabel.Content = potOdds == 0.0 ? "-" : "p:" + potOdds.ToString("##.#");
            if (potOdds == 0.0) AdditionalInfoLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            if (potOdds > 0.0 && potOdds <= 20) AdditionalInfoLabel.Foreground = new SolidColorBrush(Color.FromRgb(12, 255, 48));
            if (potOdds > 20 && potOdds <= 34) AdditionalInfoLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 220, 0));
            if (potOdds > 34 && potOdds <= 68) AdditionalInfoLabel.Foreground = new SolidColorBrush(Color.FromRgb(232, 92, 0));
            if (potOdds > 68 && potOdds <= 101) AdditionalInfoLabel.Foreground = new SolidColorBrush(Color.FromRgb(204, 0, 33));
        }

        private void ShowBetToPot(double betToPot) {
            AdditionalInfoLabel.Content = betToPot == 0.0 ? "-" : "b:" +betToPot.ToString("###");
            if (betToPot == 0.0) AdditionalInfoLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            if (betToPot > 0.0 && betToPot <= 30) AdditionalInfoLabel.Foreground = new SolidColorBrush(Color.FromRgb(12, 255, 48));
            if (betToPot > 30 && betToPot <= 49) AdditionalInfoLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 200, 0));
            if (betToPot > 49 && betToPot <= 65) AdditionalInfoLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 120, 30));
            if (betToPot > 65 && betToPot <= 89) AdditionalInfoLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 120, 120));
            if (betToPot > 89 && betToPot <= 999) AdditionalInfoLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 50, 50));
        }

        private void ShowRelativePosition(HeroRelativePosition relPosition) {
            switch (relPosition){
                case HeroRelativePosition.None: HudBorder.Background = new SolidColorBrush(Color.FromRgb(42, 42, 42)); break;
                case HeroRelativePosition.InPosition: HudBorder.Background = new SolidColorBrush(Color.FromRgb(42, 70, 42)); break;
                case HeroRelativePosition.OutOfPosition: HudBorder.Background = new SolidColorBrush(Color.FromRgb(70, 42, 0)); break;
            }
        }

        private void ShowPreflopDecision(DecisionInfo decisionInfo) {
            var decision = decisionInfo.PreflopDecision;
            var effStack = decisionInfo.LineInfo.Elements.EffectiveStack;
            var heroPreflopState = decisionInfo.LineInfo.HeroPreflopState;

            switch (decision) {
                case PreflopDecision.Fold:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 85, 85));
                    DecisionRun.Text = "F 11";
                    break;
                case PreflopDecision.Limp:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                    DecisionRun.Text = "Lim 22 ";
                    break;
                case PreflopDecision.OpenRaise:
                switch (heroPreflopState)
                {
                    case HeroPreflopState.FacingLimp: {
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(150, 200, 255));
                        var sbVsBbEffStack = decisionInfo.LineInfo.Elements.SbBtnEffStack;
                        if (decisionInfo.LineInfo.Elements.HeroPlayer.Position == PlayerPosition.Sb) {
                            if (sbVsBbEffStack >= 20) DecisionRun.Text = "IS 4 ";
                            else if (sbVsBbEffStack < 20 && sbVsBbEffStack > 13) DecisionRun.Text = "IS 3 ";
                            else if (sbVsBbEffStack <= 13) DecisionRun.Text = "IS 2 ";
                        }
                        else {
                            if (effStack <= 16) DecisionRun.Text = "IS 2 ";
                            else if (effStack > 16 && effStack <= 20) DecisionRun.Text = "IS 2.5 ";
                            else if (effStack > 20) DecisionRun.Text = "IS 3 ";
                        }
                        break;
                    }
                    default: 
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(55, 240, 255));
                        DecisionRun.Text = "OR 3 ";
                        break;
                }
                break;
                case PreflopDecision.Call:
                switch (heroPreflopState) {
                    case HeroPreflopState.FacingLimp:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                        DecisionRun.Text = "CL 2";
                        break;
                    case HeroPreflopState.FacingOpen:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                        DecisionRun.Text = "CO 43";
                        break;
                    case HeroPreflopState.FacingISOvsLimp:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                        DecisionRun.Text = "CLR 322";
                        break;
                    case HeroPreflopState.FacingPushVsLimp:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                        DecisionRun.Text = "CP 22";
                        break;
                    case HeroPreflopState.FacingLimpIsoShove:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                        DecisionRun.Text = "CP 222";
                        break;
                    case HeroPreflopState.FacingOpenPush:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                        DecisionRun.Text = "CP 6";
                        break;
                    case HeroPreflopState.FacingPushSqueeze:  
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                        DecisionRun.Text = "CP 3";
                        break;
                    case HeroPreflopState.FacingPushVsOpen: 
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                        DecisionRun.Text = "CP 3";
                        break;
                    case HeroPreflopState.FacingPushToIso:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                        DecisionRun.Text = "CP 236";
                        break;
                    case HeroPreflopState.FacingPush: 
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                        DecisionRun.Text = "CP 6";
                        break;
                }
                break;
                case PreflopDecision._3Bet:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(100, 100, 255));
                    DecisionRun.Text = "3bet 5";
                    break;
                case PreflopDecision.Push:
                switch (heroPreflopState) {
                    case HeroPreflopState.Open:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(5, 5, 255));
                        DecisionRun.Text = "OP 63";
                        break;
                    case HeroPreflopState.FacingLimp:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(5, 5, 255));
                        DecisionRun.Text = "LP 62";
                        break;
                    case HeroPreflopState.FacingISOvsLimp:
                                DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(5, 5, 255));
                                DecisionRun.Text = "PtLR 26";
                                break;
                    case HeroPreflopState.FacingOpen:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(5, 5, 255));
                        DecisionRun.Text = "OP 63";
                        break;
                }
                break;

                case PreflopDecision.None:
                    ShowPotOdds(decisionInfo.PotOdds);
                break;

            }


          
        }

        private void ShowPotType(PotType potType) {
            switch (potType) {
                case PotType.Limped:
                    PotTypeRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 125, 255));
                    PotTypeRun.Text = "L|";
                    break;
                case PotType.IsoLimped:
                    PotTypeRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 150, 100));
                    PotTypeRun.Text = "I|";
                    break;
                case PotType.Raised:
                    PotTypeRun.Foreground = new SolidColorBrush(Color.FromRgb(125, 225, 255));
                    PotTypeRun.Text = "R|";
                    break;
                case PotType.Reraised:
                    PotTypeRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 80, 80));
                    PotTypeRun.Text = "3|";
                    break;
            }
        }

        
   
        private void ShowFlopInfoStats(DecisionInfo decisionInfo) {
            var heroFlopState = decisionInfo.LineInfo.HeroFlopState;
            var opponent = decisionInfo.LineInfo.Elements.HuOpp;
            double? stat1, stat2;

            switch (heroFlopState) {
                case HeroFlopState.Donk:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(130, 160, 180));
                    DecisionRun.Text = "DB";
                    break;

                case HeroFlopState.FacingDonk:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(125, 225, 255));
                    DecisionRun.Text = "vsDB";

                    StatName1Run.Text = "db";
                    stat1 = opponent.Stats.F_DONK;
                    Stat1ValRun.Foreground = PeekStatColor(stat1, 10, 30);
                    Stat1ValRun.Text = stat1 == null ? "-" : stat1.ToString();

                    StatName2Run.Text = "fr";
                    stat2 = opponent.Stats.F_DONK_FOLDRAISE;
                    Stat2ValRun.Foreground = PeekStatColor(stat2, 40, 70);
                    Stat2ValRun.Text = stat2 == null ? "-" : stat2.ToString();

                    PeekStatNameColor(stat1, StatName1Run);
                    PeekStatNameColor(stat2, StatName2Run);

                    ShowBetToPot(opponent.BetToPot);
                    break;

                case HeroFlopState.Bet:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 200, 130));
                    DecisionRun.Text = "BT";

                    StatName1Run.Text = "fb";
                    stat1 = opponent.Stats.F_FOLD_CBET;

                    Stat1ValRun.Foreground = PeekStatColor(stat1, 45, 60);
                    Stat1ValRun.Text = stat1 == null ? "-" : stat1.ToString();

                    StatName2Run.Text = "rb";
                    stat2 = opponent.Stats.F_RAISE_CBET;
                    Stat2ValRun.Foreground = PeekStatColor(stat2, 10, 25);
                    Stat2ValRun.Text = stat2 == null ? "-" : stat2.ToString();

                    PeekStatNameColor(stat1, StatName1Run);
                    PeekStatNameColor(stat2, StatName2Run);
                    break;

                case HeroFlopState.FacingBet:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 200, 200));
                    DecisionRun.Text = "vBT";

                    StatName1Run.Text = "cb";
                    stat1 = opponent.Stats.F_CBET;
                    Stat1ValRun.Foreground = PeekStatColor(stat1, 40, 70);
                    Stat1ValRun.Text = stat1 == null ? "-" : stat1.ToString();

                    StatName2Run.Text = "fr";
                    stat2 = opponent.Stats.F_CBET_FOLDRAISE;
                    Stat2ValRun.Foreground = PeekStatColor(stat2, 40, 70);
                    Stat2ValRun.Text = stat2 == null ? "-" : stat2.ToString();
                    PeekStatNameColor(stat1, StatName1Run);
                    PeekStatNameColor(stat2, StatName2Run);

                    ShowBetToPot(opponent.BetToPot);
                    break;
                
                case HeroFlopState.FacingBetVsMissCbet:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 150, 150));
                    DecisionRun.Text = "vBx";

                    StatName1Run.Text = "cb";
                    stat1 = opponent.Stats.F_CBET;
                    Stat1ValRun.Foreground = PeekStatColor(stat1, 40, 70);
                    Stat1ValRun.Text = stat1 == null ? "-" : stat1.ToString();

                    StatName2Run.Text = "fr";
                    stat2 = opponent.Stats.F_CBET_FOLDRAISE;
                    Stat2ValRun.Foreground = PeekStatColor(stat2, 40, 70);
                    Stat2ValRun.Text = stat2 == null ? "-" : stat2.ToString();
                    PeekStatNameColor(stat1, StatName1Run);
                    PeekStatNameColor(stat2, StatName2Run);

                    ShowBetToPot(opponent.BetToPot);
                    break;

                case HeroFlopState.Cbet:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 177, 130));
                    DecisionRun.Text = "CB";

                    StatName1Run.Text = "fb";
                    stat1 = opponent.Stats.F_FOLD_CBET;

                    Stat1ValRun.Foreground = PeekStatColor(stat1, 45, 60);
                    Stat1ValRun.Text = stat1 == null ? "-" : stat1.ToString();

                    StatName2Run.Text = "rb";
                    stat2 = opponent.Stats.F_RAISE_CBET;
                    Stat2ValRun.Foreground = PeekStatColor(stat2, 10, 25);
                    Stat2ValRun.Text = stat2 == null ? "-" : stat2.ToString();

                    PeekStatNameColor(stat1, StatName1Run);
                    PeekStatNameColor(stat2, StatName2Run);
                    break;
                case HeroFlopState.FacingCbet:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 125, 125));
                    DecisionRun.Text = "vCB";

                    StatName1Run.Text = "cb";
                    stat1 = opponent.Stats.F_CBET;
                    Stat1ValRun.Foreground = PeekStatColor(stat1, 40, 70);
                    Stat1ValRun.Text = stat1 == null ? "-" : stat1.ToString();

                    StatName2Run.Text = "fr";
                    stat2 = opponent.Stats.F_CBET_FOLDRAISE;
                    Stat2ValRun.Foreground = PeekStatColor(stat2, 40, 70);
                    Stat2ValRun.Text = stat2 == null ? "-" : stat2.ToString();
                    PeekStatNameColor(stat1, StatName1Run);
                    PeekStatNameColor(stat2, StatName2Run);

                    ShowBetToPot(opponent.BetToPot);
                    break;

                case HeroFlopState.FacingMissCbet:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(200, 200, 125));
                    DecisionRun.Text = "mCB";

                    StatName1Run.Text = "cb";
                    stat1 = opponent.Stats.F_CBET;
                    Stat1ValRun.Foreground = PeekStatColor(stat1, 40, 70);
                    Stat1ValRun.Text = stat1 == null ? "-" : stat1.ToString();
                    break;

                case HeroFlopState.FacingReraise:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 80, 80));
                    DecisionRun.Text = "vRs";
                    
                    StatName1Run.Text = "rb";
                    stat1 = opponent.Stats.F_RAISE_CBET;
                    Stat1ValRun.Foreground = PeekStatColor(stat1, 10, 25);
                    Stat1ValRun.Text = stat1 == null ? "-" : stat1.ToString();

                    PeekStatNameColor(stat1, StatName1Run);

                    ShowPotOdds(decisionInfo.PotOdds);
                    break;
            }
        }

        private void ShowTurnInfoStats(DecisionInfo decisionInfo) {
            var heroTurnState = decisionInfo.LineInfo.HeroTurnState;
            var opponent = decisionInfo.LineInfo.Elements.HuOpp;

            switch (heroTurnState) {
                case HeroTurnState.Donk:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(130, 160, 180));
                    DecisionRun.Text = "DB";
                    break;
                    
                case HeroTurnState.Donk2:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(130, 190, 190));
                    DecisionRun.Text = "2DB";
                    break;

                case HeroTurnState.FacingDonk:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(125, 225, 255));
                    DecisionRun.Text = "vDB";
                    break;
                case HeroTurnState.FacingDonk2:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(125, 225, 255));
                    DecisionRun.Text = "v2DB";
                    break;

                case HeroTurnState.Bet2:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 180, 130));
                    DecisionRun.Text = "2BT";
                    break;
                case HeroTurnState.DelayBet:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 220, 130));
                    DecisionRun.Text = "dBT";
                    break;
                case HeroTurnState.BetAfterFReraise:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 170, 130));
                    DecisionRun.Text = "vCBrf";
                    break;
                case HeroTurnState.BetVsMissFCb:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 220, 130));
                    DecisionRun.Text = "vBTmf";
                    break;
                case HeroTurnState.BetVsMissTCb:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 220, 130));
                    DecisionRun.Text = "vBTxt";
                    break;
                case HeroTurnState.FacingBet2:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 125, 125));
                    DecisionRun.Text = "v2BT";
                    break;
                case HeroTurnState.Cbet2:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 150, 90));
                    DecisionRun.Text = "2CB";
                    break;
                case HeroTurnState.DelayCbet:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 177, 120));
                    DecisionRun.Text = "dCB";
                    break;
                case HeroTurnState.FacingCbet2:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 80, 80));
                    DecisionRun.Text = "v2CB";
                    break;
                case HeroTurnState.FacingCheckAfterFlopReraise:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(200, 220, 130));
                    DecisionRun.Text = "vXfR";
                    break;
                case HeroTurnState.FacingDelayBet:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 180, 180));
                    DecisionRun.Text = "vdBT";
                    break;
                case HeroTurnState.FacingDelayCbet:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 125, 125));
                    DecisionRun.Text = "vdCB";
                    break;
                case HeroTurnState.FacingReraise:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                    DecisionRun.Text = "vRE";
                    break;
            }
        }

        private void ShowRiverInfoStats(DecisionInfo decisionInfo) {
            var heroRiverState = decisionInfo.LineInfo.HeroRiverState;
            var opponent = decisionInfo.LineInfo.Elements.HuOpp;

            switch (heroRiverState) {
                case HeroRiverState.Donk:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(130, 160, 180));
                    DecisionRun.Text = "DB";
                    break;

                case HeroRiverState.Donk3:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(130, 190, 190));
                    DecisionRun.Text = "3DB";
                    break;

                case HeroRiverState.FacingDonk:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(125, 225, 255));
                    DecisionRun.Text = "vDB";
                    break;
                case HeroRiverState.FacingDonk3:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(125, 225, 255));
                    DecisionRun.Text = "v3DB";
                    break;
                case HeroRiverState.Cbet3:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 150, 90));
                    DecisionRun.Text = "3CB";
                    break;
                case HeroRiverState.FacingBet3:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 125, 125));
                    DecisionRun.Text = "v3BT";
                    break;
                case HeroRiverState.FacingCbet3:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 80, 80));
                    DecisionRun.Text = "v3CB";
                    break;
            
            }
        }
        
        private void ClearStats() {
            StatName1Run.Text = "";
            Stat1ValRun.Text = "";
            StatName2Run.Text = "";
            Stat2ValRun.Text = "";
        }

        private void PeekStatNameColor(double? statValue, Run statDefRun) {
            if (statValue == null || statValue == 0) statDefRun.Foreground = new SolidColorBrush(Color.FromRgb(180, 180, 180));
            else statDefRun.Foreground = new SolidColorBrush(Color.FromRgb(250, 250, 250));
        }

        private SolidColorBrush PeekStatColor(double? stat ,int lim1, int lim2) {
            if (stat == null || stat == 0) return new SolidColorBrush(Color.FromRgb(125, 125, 125));
            if (stat > 0 && stat <= lim1) return new SolidColorBrush(Color.FromRgb(0, 255, 0));
            if (stat > lim1  && stat <= lim2) return new SolidColorBrush(Color.FromRgb(255, 255, 175));
            if (stat > lim2) return new SolidColorBrush(Color.FromRgb(255, 0, 0));
            return new SolidColorBrush(Color.FromRgb(125, 125, 125));
        }

        private void StackLabel_MouseDown(object sender, MouseButtonEventArgs e) {
            if (_tableBitmap == null) return;
            const string path = @"P:\screens\";
            _tableBitmap.Save(path + DateTime.Now.ToString("s").Replace(':',' ') + ".bmp");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            //register activate hotkey
            const uint activateHotkeyVLC = 86; // keycode of V
            WINAPI.RegisterHotKey(this, activateHotkeyVLC);
            ComponentDispatcher.ThreadPreprocessMessage += ComponentDispatcher_ThreadPreprocessMessage;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            WINAPI.UnregisterHotKey(this);
        }

        private uint WM_KEYUP = 0x0101;

  
        void ComponentDispatcher_ThreadPreprocessMessage(ref MSG msg, ref bool handled)
        {
            /*
            if (msg.message == WM_KEYUP) {
                Debug.WriteLine(msg.wParam);
            }
             */
            if (msg.message == WINAPI.WM_HOTKEY) {
               // MessageBox.Show("fdsdfs");
                this.Activate();
            }
        }

       
         

    }
         
}
