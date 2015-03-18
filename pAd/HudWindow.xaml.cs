using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
        
        public void  OnNewHudInfo(HudInfo hudInfo) {

            ClearStats();
            HeroStateRun.Text = "";

            PotOddsLabel.Content = hudInfo.PotOdds == 0.0 ? "-" : hudInfo.PotOdds.ToString("##.#");
            if (hudInfo.PotOdds == 0.0) PotOddsLabel.Foreground = new SolidColorBrush(Color.FromRgb(255,255,255));
            if (hudInfo.PotOdds > 0.0 && hudInfo.PotOdds <= 20) PotOddsLabel.Foreground = new SolidColorBrush(Color.FromRgb(12, 255, 48));
            if (hudInfo.PotOdds > 20 && hudInfo.PotOdds <= 34) PotOddsLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 194, 0));
            if (hudInfo.PotOdds > 34 && hudInfo.PotOdds <= 68) PotOddsLabel.Foreground = new SolidColorBrush(Color.FromRgb(232, 92, 0));
            if (hudInfo.PotOdds > 68 && hudInfo.PotOdds <= 101) PotOddsLabel.Foreground = new SolidColorBrush(Color.FromRgb(204, 0, 33));


            //TODO придумать другую цветовую схему, это сливается в одно, нужно более ярко выделять близкие к пушам стеки
            if(hudInfo.EffectiveStack == 0) StackLabel.Foreground = new SolidColorBrush(Color.FromRgb(255,255,255));
            if (hudInfo.EffectiveStack > 0.0 && hudInfo.EffectiveStack <= 8)
                StackLabel.Foreground = new SolidColorBrush(Color.FromRgb(36, 116, 246));
            if (hudInfo.EffectiveStack > 8 && hudInfo.EffectiveStack <= 13)
                StackLabel.Foreground = new SolidColorBrush(Color.FromRgb(14, 188, 211));
            if (hudInfo.EffectiveStack > 13 && hudInfo.EffectiveStack <= 16)
                StackLabel.Foreground = new SolidColorBrush(Color.FromRgb(27, 234, 195));
            if (hudInfo.EffectiveStack > 16 && hudInfo.EffectiveStack <= 20)
                StackLabel.Foreground = new SolidColorBrush(Color.FromRgb(14, 211, 112));
            if (hudInfo.EffectiveStack > 20 && hudInfo.EffectiveStack <= 100)
                StackLabel.Foreground = new SolidColorBrush(Color.FromRgb(37, 246, 76));
    

            StackLabel.Content = "S: " + hudInfo.EffectiveStack.ToString("#.#");
        

            switch (hudInfo.HeroRelativePosition) {
                    case HeroRelativePosition.None: HudBorder.Background = new SolidColorBrush(Color.FromRgb(42,42,42)); break;
                    case HeroRelativePosition.InPosition: HudBorder.Background = new SolidColorBrush(Color.FromRgb(42, 70, 42)); break;
                    case HeroRelativePosition.OutOfPosition: HudBorder.Background = new SolidColorBrush(Color.FromRgb(70, 42 , 0)); break;
            }



            if (hudInfo.CurrentStreet == CurrentStreet.Preflop) {
                switch (hudInfo.Decision) {
                    case Decision.Fold:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 85, 85));
                        DecisionRun.Text = "F " + hudInfo.HandPlayability.ToString("#.#");
                        break;
                    case Decision.Limp:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                        DecisionRun.Text = "Lim 22 ";
                        break;
                    case Decision.OpenPush:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(5, 5, 255));
                        DecisionRun.Text = "OP " + hudInfo.HandPlayability.ToString("#.#");
                        break;
                    case Decision.PushToOpen:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(5, 5, 255));
                        DecisionRun.Text = "OP " + hudInfo.HandPlayability.ToString("#.#");
                        break;
                    case Decision.OpenRaise:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(55, 240, 255));
                        DecisionRun.Text = "OR " + hudInfo.HandPlayability.ToString("#.#");
                        break;
                    case Decision.PushToLimp:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(5, 5, 255));
                        DecisionRun.Text = "LP " + hudInfo.HandPlayability.ToString("#.#");
                        break;
                    case Decision.RaiseToLimp:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(55, 240, 255));
                        DecisionRun.Text = "LR " + hudInfo.HandPlayability.ToString("#.#");
                        break;
                    case Decision.CallToOpen:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                        DecisionRun.Text = "CO " + hudInfo.HandPlayability.ToString("#.#");
                        break;
                    case Decision.CallToPush:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                        DecisionRun.Text = "CP " + hudInfo.HandPlayability.ToString("#.#");
                        break;
                    case Decision.CallToLimp:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                        DecisionRun.Text = "CL " + hudInfo.HandPlayability.ToString("#.#");
                        break;
                    case Decision._3Bet4:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(100, 100, 255));
                        DecisionRun.Text = "3b4 " + hudInfo.HandPlayability.ToString("#.#");
                        break;
                    case Decision._3Bet45:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(170, 170, 255));
                        DecisionRun.Text = "3b5 " + hudInfo.HandPlayability.ToString("#.#");
                        break;
                    default:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(125, 125, 125));
                        DecisionRun.Text = "" + hudInfo.HandPlayability.ToString("#.#");
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
        

    }
}
