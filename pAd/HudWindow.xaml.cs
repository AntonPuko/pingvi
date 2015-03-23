using System;
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


        public void OnNewHudInfo(HudInfo hudInfo) {
            ShowPotOdds(hudInfo.PotOdds);
            ShowEffectiveStack(hudInfo.EffectiveStack);
            ShowRelativePosition(hudInfo.HeroRelativePosition);

            ClearStats();
            HeroStateRun.Text = "";


            if (hudInfo.CurrentStreet == CurrentStreet.Preflop) {
                ShowPreflopDecision(hudInfo.DecisionPreflop, hudInfo.HeroStatePreflop, hudInfo.HandRangeStat);
            }
            else {
                ShowPreflopStateOnPostflop(hudInfo.HeroStatePreflop);
                if (hudInfo.CurrentStreet == CurrentStreet.Flop) {
                    ShowFlopInfoStats(hudInfo);
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
            PotOddsLabel.Content = potOdds == 0.0 ? "-" : potOdds.ToString("##.#");
            if (potOdds == 0.0) PotOddsLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            if (potOdds > 0.0 && potOdds <= 20) PotOddsLabel.Foreground = new SolidColorBrush(Color.FromRgb(12, 255, 48));
            if (potOdds > 20 && potOdds <= 34) PotOddsLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 194, 0));
            if (potOdds > 34 && potOdds <= 68) PotOddsLabel.Foreground = new SolidColorBrush(Color.FromRgb(232, 92, 0));
            if (potOdds > 68 && potOdds <= 101) PotOddsLabel.Foreground = new SolidColorBrush(Color.FromRgb(204, 0, 33));
        }

        private void ShowRelativePosition(HeroRelativePosition relPosition) {
            switch (relPosition){
                case HeroRelativePosition.None: HudBorder.Background = new SolidColorBrush(Color.FromRgb(42, 42, 42)); break;
                case HeroRelativePosition.InPosition: HudBorder.Background = new SolidColorBrush(Color.FromRgb(42, 70, 42)); break;
                case HeroRelativePosition.OutOfPosition: HudBorder.Background = new SolidColorBrush(Color.FromRgb(70, 42, 0)); break;
            }
        }

        private void ShowPreflopDecision(DecisionPreflop decision, HeroStatePreflop heroState, double heroRangeStat) {
            DecisionRun.Text = "";
            switch (decision) {
                    case DecisionPreflop.Fold:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 85, 85));
                        DecisionRun.Text = "F 1";
                        break;
                    case DecisionPreflop.Limp:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                        DecisionRun.Text = "Lim 22 ";
                        break;
                    case DecisionPreflop.OpenRaise:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(55, 240, 255));
                        DecisionRun.Text = "OR 3 ";
                        break;
                    case DecisionPreflop.Call:
                        switch (heroState) {
                            case HeroStatePreflop.FacingLimp:
                                DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                                DecisionRun.Text = "CL 42";
                                break;
                            case HeroStatePreflop.FacingOpen:
                                DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                                DecisionRun.Text = "CO 43";
                                break;
                            case HeroStatePreflop.FacingPush:
                                DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                                DecisionRun.Text = "CP 46";
                                break;
                        }
                        break;
                    case DecisionPreflop._3Bet:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(100, 100, 255));
                        DecisionRun.Text = "3bet 5";
                        break;
                    case DecisionPreflop.Push:
                        switch (heroState) {
                            case HeroStatePreflop.Open:
                                DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(5, 5, 255));
                                DecisionRun.Text = "OP 63" + heroRangeStat.ToString("#.#");
                                break;
                            case HeroStatePreflop.FacingLimp:
                                DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(5, 5, 255));
                                DecisionRun.Text = "LP 62" + heroRangeStat.ToString("#.#");
                                break;
                            case HeroStatePreflop.FacingOpen:
                                DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(5, 5, 255));
                                DecisionRun.Text = "OP 63" + heroRangeStat.ToString("#.#");
                                break;
                        }
                        break;
            }


          
        }

        private void ShowPreflopStateOnPostflop(HeroStatePreflop heroPfState) {
                DecisionRun.Text = "";
                switch (heroPfState) {
                    case HeroStatePreflop.Open:
                        HeroStateRun.Foreground = new SolidColorBrush(Color.FromRgb(240, 255, 240));
                        HeroStateRun.Text = "OPN";
                        break;
                    case HeroStatePreflop.FacingOpen:
                        HeroStateRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 232));
                        HeroStateRun.Text = "Fop";
                        break;
                    case HeroStatePreflop.FacingLimp:
                        HeroStateRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 242, 249));
                        HeroStateRun.Text = "Flp";
                        break;
                    case HeroStatePreflop.Facing3Bet:
                        HeroStateRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                        HeroStateRun.Text = "F3bt" + " -";
                        break;
                    case HeroStatePreflop.FacingPush:
                        HeroStateRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                        HeroStateRun.Text = "FPsh" + " -";
                        break;
                    case HeroStatePreflop.None:
                        HeroStateRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                        HeroStateRun.Text = "non" + " -";
                        break;
                }
        }

        private void ShowFlopInfoStats(HudInfo hudInfo) {
            if (hudInfo.Opponent == null) return;
            switch (hudInfo.HeroStateFlop) {
                case HeroStatePostflop.LimpBet: {
                    HeroStateRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 177, 130));
                    HeroStateRun.Text = "OL BT";

                    Stat1DefRun.Text = "fcb";
                    var stat1 = hudInfo.Opponent.Stats.F_FOLD_CBET;

                    Stat1ValRun.Foreground = PeekStatColor(stat1, 45, 60);
                    Stat1ValRun.Text = stat1 == 0 ? "-" : stat1.ToString("#");

                    Stat2DefRun.Text = "rcb";
                    var stat2 = hudInfo.Opponent.Stats.F_RAISE_CBET;
                    Stat2ValRun.Foreground = PeekStatColor(stat2, 10, 25);
                    Stat2ValRun.Text = stat2 == 0 ? "-" : stat2.ToString("#");

                    PeekStatDefForeground(stat1, Stat1DefRun);
                    PeekStatDefForeground(stat2, Stat2DefRun);
                    break;
                }

                case HeroStatePostflop.FacingDONKVsOpenLimp: {
                    HeroStateRun.Foreground = new SolidColorBrush(Color.FromRgb(125, 180, 255));
                    HeroStateRun.Text = "LvsDB";

                    Stat1DefRun.Text = "db";
                    var stat1 = hudInfo.Opponent.Stats.F_DONK;
                    Stat1ValRun.Foreground = PeekStatColor(stat1, 10, 30);
                    Stat1ValRun.Text = stat1 == 0 ? "-" : stat1.ToString("#");

                    Stat2DefRun.Text = "fr";
                    var stat2 = hudInfo.Opponent.Stats.F_DONK_FOLDRAISE;
                    Stat2ValRun.Foreground = PeekStatColor(stat2, 40, 70);
                    Stat2ValRun.Text = stat2 == 0 ? "-" : stat2.ToString("#");

                    PeekStatDefForeground(stat1, Stat1DefRun);
                    PeekStatDefForeground(stat2, Stat2DefRun);
                    break;
                }


                case HeroStatePostflop.Cbet:
                case HeroStatePostflop.FacingRaiseToCbet:
                {


                    if (hudInfo.HeroStatePreflop == HeroStatePreflop.FacingLimp && hudInfo.CurrentPot == 2)
                    {
                        HeroStateRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 180, 232));
                        HeroStateRun.Text = "L BT";
                    }

                    if (hudInfo.HeroStatePreflop == HeroStatePreflop.FacingLimp && hudInfo.CurrentPot > 2)
                    {
                        HeroStateRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 125, 232));
                        HeroStateRun.Text = "L CB";
                    }

                    if (hudInfo.HeroStatePreflop == HeroStatePreflop.Open)
                    {
                        HeroStateRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 177, 130));
                        HeroStateRun.Text = "O CB";
                    }


                    Stat1DefRun.Text = "fcb";
                    var stat1 = hudInfo.Opponent.Stats.F_FOLD_CBET;

                    Stat1ValRun.Foreground = PeekStatColor(stat1, 45, 60);
                    Stat1ValRun.Text = stat1 == 0 ? "-" : stat1.ToString("#");

                    Stat2DefRun.Text = "rcb";
                    var stat2 = hudInfo.Opponent.Stats.F_RAISE_CBET;
                    Stat2ValRun.Foreground = PeekStatColor(stat2, 10, 25);
                    Stat2ValRun.Text = stat2 == 0 ? "-" : stat2.ToString("#");

                    PeekStatDefForeground(stat1, Stat1DefRun);
                    PeekStatDefForeground(stat2, Stat2DefRun);
                    break;
                }

                case HeroStatePostflop.MissedCbet:
                {
                    if (hudInfo.HeroStatePreflop == HeroStatePreflop.FacingOpen ||
                        hudInfo.HeroStatePreflop == HeroStatePreflop.Facing3Bet)
                    {

                        if (hudInfo.HeroStatePreflop == HeroStatePreflop.FacingOpen)
                        {
                            HeroStateRun.Text = "MsCB";
                        }
                        else if (hudInfo.HeroStatePreflop == HeroStatePreflop.Facing3Bet)
                        {
                            HeroStateRun.Text = "3MsCB";
                        }

                        HeroStateRun.Foreground = new SolidColorBrush(Color.FromRgb(200, 200, 100));

                        Stat1DefRun.Text = "cb";
                        var stat1 = hudInfo.Opponent.Stats.F_CBET;
                        Stat1ValRun.Foreground = PeekStatColor(stat1, 55, 70);
                        Stat1ValRun.Text = stat1 == 0 ? "-" : stat1.ToString("#");
                        PeekStatDefForeground(stat1, Stat1DefRun);
                    }
                    break;
                }



                case HeroStatePostflop.FacingCbet:
                {

                    HeroStateRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 185, 185));
                    if (hudInfo.HeroStatePreflop == HeroStatePreflop.FacingOpen)
                    {
                        HeroStateRun.Text = "VsCB";
                    }
                    else if (hudInfo.HeroStatePreflop == HeroStatePreflop.Facing3Bet)
                    {
                        HeroStateRun.Text = "3VsCB";
                    }

                    Stat1DefRun.Text = "cb";
                    var stat1 = hudInfo.Opponent.Stats.F_CBET;
                    Stat1ValRun.Foreground = PeekStatColor(stat1, 40, 70);
                    Stat1ValRun.Text = stat1 == 0 ? "-" : stat1.ToString("#");


                    Stat2DefRun.Text = "fr";
                    var stat2 = hudInfo.Opponent.Stats.F_CBET_FOLDRAISE;
                    Stat2ValRun.Foreground = PeekStatColor(stat2, 40, 70);
                    Stat2ValRun.Text = stat2 == 0 ? "-" : stat2.ToString("#");
                    PeekStatDefForeground(stat1, Stat1DefRun);
                    PeekStatDefForeground(stat2, Stat2DefRun);
                    break;
                }
                case HeroStatePostflop.FacingDonk:
                {
                    HeroStateRun.Foreground = new SolidColorBrush(Color.FromRgb(125, 225, 255));
                    HeroStateRun.Text = "VsDK";

                    Stat1DefRun.Text = "db";
                    var stat1 = hudInfo.Opponent.Stats.F_DONK;
                    Stat1ValRun.Foreground = PeekStatColor(stat1, 10, 30);
                    Stat1ValRun.Text = stat1 == 0 ? "-" : stat1.ToString("#");

                    Stat2DefRun.Text = "fr";
                    var stat2 = hudInfo.Opponent.Stats.F_DONK_FOLDRAISE;
                    Stat2ValRun.Foreground = PeekStatColor(stat2, 40, 70);
                    Stat2ValRun.Text = stat2 == 0 ? "-" : stat2.ToString("#");

                    PeekStatDefForeground(stat1, Stat1DefRun);
                    PeekStatDefForeground(stat2, Stat2DefRun);
                    break;
                }
            }
        }

        /*
        public void  OnNewHudInfo(HudInfo hudInfo) {

            ClearStats();
            HeroStateRun.Text = "";


            if (hudInfo.CurrentStreet == CurrentStreet.Preflop) {
                switch (hudInfo.DecisionPreflop) {
                    case DecisionPreflop.Fold:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 85, 85));
                        DecisionRun.Text = "F " + hudInfo.HandRangeStat.ToString("#.#");
                        break;
                    case DecisionPreflop.Limp:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                        DecisionRun.Text = "Lim 22 ";
                        break;
                    case DecisionPreflop.OpenPush:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(5, 5, 255));
                        DecisionRun.Text = "OP " + hudInfo.HandRangeStat.ToString("#.#");
                        break;
                    case DecisionPreflop.PushToOpen:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(5, 5, 255));
                        DecisionRun.Text = "OP " + hudInfo.HandRangeStat.ToString("#.#");
                        break;
                    case DecisionPreflop.OpenRaise:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(55, 240, 255));
                        DecisionRun.Text = "OR " + hudInfo.HandRangeStat.ToString("#.#");
                        break;
                    case DecisionPreflop.PushToLimp:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(5, 5, 255));
                        DecisionRun.Text = "LP " + hudInfo.HandRangeStat.ToString("#.#");
                        break;
                    case DecisionPreflop.RaiseToLimp:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(55, 240, 255));
                        DecisionRun.Text = "LR " + hudInfo.HandRangeStat.ToString("#.#");
                        break;
                    case DecisionPreflop.CallToOpen:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                        DecisionRun.Text = "CO " + hudInfo.HandRangeStat.ToString("#.#");
                        break;
                    case DecisionPreflop.CallToPush:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                        DecisionRun.Text = "CP " + hudInfo.HandRangeStat.ToString("#.#");
                        break;
                    case DecisionPreflop.CallToLimp:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                        DecisionRun.Text = "CL " + hudInfo.HandRangeStat.ToString("#.#");
                        break;
                    case DecisionPreflop._3Bet4:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(100, 100, 255));
                        DecisionRun.Text = "3b4 " + hudInfo.HandRangeStat.ToString("#.#");
                        break;
                    case DecisionPreflop._3Bet45:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(170, 170, 255));
                        DecisionRun.Text = "3b5 " + hudInfo.HandRangeStat.ToString("#.#");
                        break;
                    default:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(125, 125, 125));
                        DecisionRun.Text = "" + hudInfo.HandRangeStat.ToString("#.#");
                        break;
                }
            }
            else {
                DecisionRun.Text = "";

                switch (hudInfo.HeroStatePreflop)
                {
                    case HeroStatePreflop.Open:
                        HeroStateRun.Foreground = new SolidColorBrush(Color.FromRgb(240, 255, 240));
                        HeroStateRun.Text = "OPN";
                        break;
                    case HeroStatePreflop.FacingOpen:
                        HeroStateRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 232));
                        HeroStateRun.Text = "Fop";
                        break;
                    case HeroStatePreflop.FacingLimp:
                        HeroStateRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 242, 249));
                        HeroStateRun.Text = "Flp";
                        break;
                    case HeroStatePreflop.Facing3Bet:
                        HeroStateRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                        HeroStateRun.Text = "F3bt" + " -";
                        break;
                    case HeroStatePreflop.FacingPush:
                        HeroStateRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                        HeroStateRun.Text = "FPsh" + " -";
                        break;
                    case HeroStatePreflop.None:
                        HeroStateRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                        HeroStateRun.Text = "non" + " -";
                        break;


                }



                if (hudInfo.CurrentStreet == CurrentStreet.Flop) {
                    if (hudInfo.Opponent != null) {
                        switch (hudInfo.HeroStateFlop)
                        {
                            case HeroStatePostflop.LimpBet: {
                                HeroStateRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 177, 130));
                                HeroStateRun.Text = "OL BT";

                                Stat1DefRun.Text = "fcb";
                                var stat1 = hudInfo.Opponent.Stats.F_FOLD_CBET;

                                Stat1ValRun.Foreground = PeekStatColor(stat1, 45, 60);
                                Stat1ValRun.Text = stat1 == 0 ? "-" : stat1.ToString("#");

                                Stat2DefRun.Text = "rcb";
                                var stat2 = hudInfo.Opponent.Stats.F_RAISE_CBET;
                                Stat2ValRun.Foreground = PeekStatColor(stat2, 10, 25);
                                Stat2ValRun.Text = stat2 == 0 ? "-" : stat2.ToString("#");

                                PeekStatDefForeground(stat1, Stat1DefRun);
                                PeekStatDefForeground(stat2, Stat2DefRun);
                                break;
                            }

                            case HeroStatePostflop.FacingDONKVsOpenLimp: {
                                HeroStateRun.Foreground = new SolidColorBrush(Color.FromRgb(125, 180, 255));
                                HeroStateRun.Text = "LvsDB";

                                Stat1DefRun.Text = "db";
                                var stat1 = hudInfo.Opponent.Stats.F_DONK;
                                Stat1ValRun.Foreground = PeekStatColor(stat1, 10, 30);
                                Stat1ValRun.Text = stat1 == 0 ? "-" : stat1.ToString("#");

                                Stat2DefRun.Text = "fr";
                                var stat2 = hudInfo.Opponent.Stats.F_DONK_FOLDRAISE;
                                Stat2ValRun.Foreground = PeekStatColor(stat2, 40, 70);
                                Stat2ValRun.Text = stat2 == 0 ? "-" : stat2.ToString("#");

                                PeekStatDefForeground(stat1, Stat1DefRun);
                                PeekStatDefForeground(stat2, Stat2DefRun);
                                break;
                            }


                            case HeroStatePostflop.Cbet: case HeroStatePostflop.FacingRaiseToCbet:
                                {


                                    if (hudInfo.HeroStatePreflop == HeroStatePreflop.FacingLimp && hudInfo.CurrentPot == 2) {
                                        HeroStateRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 180, 232));
                                        HeroStateRun.Text = "L BT";
                                    }

                                    if (hudInfo.HeroStatePreflop == HeroStatePreflop.FacingLimp && hudInfo.CurrentPot >2)
                                    {
                                        HeroStateRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 125, 232));
                                        HeroStateRun.Text = "L CB";
                                    }

                                    if (hudInfo.HeroStatePreflop == HeroStatePreflop.Open) {
                                        HeroStateRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 177, 130));
                                        HeroStateRun.Text = "O CB";
                                    }
                                 
                                    
                                    Stat1DefRun.Text = "fcb";
                                    var stat1 = hudInfo.Opponent.Stats.F_FOLD_CBET;

                                    Stat1ValRun.Foreground = PeekStatColor(stat1, 45, 60);
                                    Stat1ValRun.Text = stat1 == 0 ? "-" : stat1.ToString("#");

                                    Stat2DefRun.Text = "rcb";
                                    var stat2 =  hudInfo.Opponent.Stats.F_RAISE_CBET;
                                    Stat2ValRun.Foreground = PeekStatColor(stat2, 10, 25);
                                    Stat2ValRun.Text = stat2 == 0 ? "-" : stat2.ToString("#");

                                    PeekStatDefForeground(stat1, Stat1DefRun);
                                    PeekStatDefForeground(stat2, Stat2DefRun);
                                    break;
                                }

                            case HeroStatePostflop.MissedCbet: {
                                if (hudInfo.HeroStatePreflop == HeroStatePreflop.FacingOpen ||
                                    hudInfo.HeroStatePreflop == HeroStatePreflop.Facing3Bet) {

                                        if (hudInfo.HeroStatePreflop == HeroStatePreflop.FacingOpen)
                                        {
                                            HeroStateRun.Text = "MsCB";
                                        }
                                        else if (hudInfo.HeroStatePreflop == HeroStatePreflop.Facing3Bet)
                                        {
                                            HeroStateRun.Text = "3MsCB";
                                        }
                                        
                                        HeroStateRun.Foreground = new SolidColorBrush(Color.FromRgb(200, 200, 100));

                                        Stat1DefRun.Text = "cb";
                                        var stat1 = hudInfo.Opponent.Stats.F_CBET;
                                        Stat1ValRun.Foreground = PeekStatColor(stat1, 55, 70);
                                        Stat1ValRun.Text = stat1 == 0 ? "-" : stat1.ToString("#");
                                        PeekStatDefForeground(stat1, Stat1DefRun);
                                }
                                    break;
                                }



                            case HeroStatePostflop.FacingCbet:
                                {
                                    
                                    HeroStateRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 185, 185));
                                    if (hudInfo.HeroStatePreflop == HeroStatePreflop.FacingOpen) {
                                        HeroStateRun.Text = "VsCB";
                                    }
                                    else if (hudInfo.HeroStatePreflop == HeroStatePreflop.Facing3Bet) {
                                        HeroStateRun.Text = "3VsCB";
                                    }
                                    
                                    Stat1DefRun.Text = "cb";
                                    var stat1 = hudInfo.Opponent.Stats.F_CBET;
                                    Stat1ValRun.Foreground = PeekStatColor(stat1, 40, 70);
                                    Stat1ValRun.Text = stat1 == 0 ? "-" : stat1.ToString("#");
                                    

                                    Stat2DefRun.Text = "fr";
                                    var stat2 = hudInfo.Opponent.Stats.F_CBET_FOLDRAISE;
                                    Stat2ValRun.Foreground = PeekStatColor(stat2, 40, 70);
                                    Stat2ValRun.Text = stat2 == 0 ? "-" : stat2.ToString("#");
                                    PeekStatDefForeground(stat1, Stat1DefRun);
                                    PeekStatDefForeground(stat2, Stat2DefRun);
                                    break;
                                }
                            case HeroStatePostflop.FacingDonk:
                                {
                                    HeroStateRun.Foreground = new SolidColorBrush(Color.FromRgb(125, 225, 255));
                                    HeroStateRun.Text = "VsDK";
                                    
                                    Stat1DefRun.Text = "db";
                                    var stat1 = hudInfo.Opponent.Stats.F_DONK;
                                    Stat1ValRun.Foreground = PeekStatColor(stat1, 10, 30);
                                    Stat1ValRun.Text = stat1 == 0 ? "-" : stat1.ToString("#");

                                    Stat2DefRun.Text = "fr";
                                    var stat2 = hudInfo.Opponent.Stats.F_DONK_FOLDRAISE;
                                    Stat2ValRun.Foreground = PeekStatColor(stat2, 40, 70);
                                    Stat2ValRun.Text = stat2 == 0 ? "-" : stat2.ToString("#");

                                    PeekStatDefForeground(stat1, Stat1DefRun);
                                    PeekStatDefForeground(stat2, Stat2DefRun);
                                    break;
                                }
                        }
                    }
                }
                   

            
            }
           
        }
           */

        private void ClearStats() {
            Stat1DefRun.Text = "";
            Stat1ValRun.Text = "";
            Stat2DefRun.Text = "";
            Stat2ValRun.Text = "";
        }

        private void PeekStatDefForeground(double statValue, Run statDefRun) {
            if (statValue == 0) statDefRun.Foreground = new SolidColorBrush(Color.FromRgb(180, 180, 180));
            else statDefRun.Foreground = new SolidColorBrush(Color.FromRgb(250, 250, 250));
        }

        private SolidColorBrush PeekStatColor(double stat ,int lim1, int lim2) {
            if (stat == 0) return new SolidColorBrush(Color.FromRgb(125, 125, 125));
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
            const uint activateHotkeyVLC = 192; // keycode of tilda ~
            WINAPI.RegisterHotKey(this, activateHotkeyVLC);
            ComponentDispatcher.ThreadPreprocessMessage += ComponentDispatcher_ThreadPreprocessMessage;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            WINAPI.UnregisterHotKey(this);
        }

  
        void ComponentDispatcher_ThreadPreprocessMessage(ref MSG msg, ref bool handled)
        {
            if (msg.message == WINAPI.WM_HOTKEY) {
                this.Activate();
            }
        }

       
         

    }
         
}
