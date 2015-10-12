using System;
using System.CodeDom;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Pingvi.TableCatchers;
using PokerModel;
using Color = System.Windows.Media.Color;

namespace Pingvi
{
    /// <summary>
    /// Interaction logic for HWindow.xaml
    /// </summary>
    public partial class HWindow : Window {

        private ITableCatcher _tableCather;


        public Action MakeScreenShotClick;

        public HWindow(ITableCatcher tableCatcher)
        {
            _tableCather = tableCatcher;
            InitializeComponent(); 
        }

        protected override void OnClosed(EventArgs e) {
            _tableCather.Stop();
            
        }


        public void OnNewDecisionInfo(DecisionInfo decisionInfo) {
      
           

            ShowEffectiveStack(decisionInfo.LineInfo.Elements.EffectiveStack);
            ShowRelativePosition(decisionInfo.LineInfo.Elements.HeroPlayer.RelativePosition);

            ClearStats();
            DecisionRun.Text = "";
            PotTypeLabel.Foreground = new SolidColorBrush(Color.FromRgb(125, 125, 125));
            PotTypeLabel.Content = "_";
            AdditionalInfoLabel.Content = "";

            ShowHeroPreflopStatus(decisionInfo.LineInfo.HeroPreflopStatus);

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

        public void ShowHeroPreflopStatus(HeroPreflopStatus status) {
            switch (status) {
                case HeroPreflopStatus.Agressor:
                    HeroStatusRun.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 255));
                    HeroStatusRun.Text = "A";
                    break;
                case HeroPreflopStatus.Defender:
                    HeroStatusRun.Foreground = new SolidColorBrush(Color.FromRgb(232, 218, 0));
                    HeroStatusRun.Text = "D";
                    break;
                case HeroPreflopStatus.Limper:
                    HeroStatusRun.Foreground = new SolidColorBrush(Color.FromRgb(225, 175, 232));
                    HeroStatusRun.Text = "L";
                    break;
                case HeroPreflopStatus.None:
                    HeroStatusRun.Foreground = new SolidColorBrush(Color.FromRgb(120, 120, 120));
                    HeroStatusRun.Text = "_";
                    break;

            }
            
        }

        private void ShowEffectiveStack(double effStack) {
            //TODO придумать другую цветовую схему, это сливается в одно, нужно более ярко выделять близкие к пушам стеки
            if (effStack == 0) StackLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            if (effStack > 0.0 && effStack <= 9.5)
                StackLabel.Foreground = new SolidColorBrush(Color.FromRgb(36, 116, 246));
            if (effStack > 9.5 && effStack <= 17.5)
                StackLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 199, 80));
            if (effStack > 17.5 && effStack <= 100)
                StackLabel.Foreground = new SolidColorBrush(Color.FromRgb(103, 210, 90));

            StackLabel.Content = "S:" + effStack.ToString("#.#");
        }

        private void ShowPotOdds(double potOdds) {
            AdditionalInfoLabel.Content = potOdds == 0.0 ? "-" : "o:" + potOdds.ToString("##");
            if (potOdds == 0.0) AdditionalInfoLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            if (potOdds > 0.0 && potOdds <= 20) AdditionalInfoLabel.Foreground = new SolidColorBrush(Color.FromRgb(12, 255, 48));
            if (potOdds > 20 && potOdds <= 34) AdditionalInfoLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 220, 0));
            if (potOdds > 34 && potOdds <= 68) AdditionalInfoLabel.Foreground = new SolidColorBrush(Color.FromRgb(232, 92, 0));
            if (potOdds > 68 && potOdds <= 101) AdditionalInfoLabel.Foreground = new SolidColorBrush(Color.FromRgb(204, 0, 33));
        }

        private void ShowBetToPot(double betToPot) {
            AdditionalInfoLabel.Content = betToPot == 0.0 ? "-" : "b:" +betToPot.ToString("0.0");
            if (betToPot == 0.0) AdditionalInfoLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            if (betToPot > 0.0 && betToPot <= 0.39) AdditionalInfoLabel.Foreground = new SolidColorBrush(Color.FromRgb(12, 255, 48));
            if (betToPot > 0.39 && betToPot <= 0.65) AdditionalInfoLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 120, 30));
            if (betToPot > 0.65 && betToPot <= 0.89) AdditionalInfoLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 85, 85));
            if (betToPot > 0.89 && betToPot <= 9.99) AdditionalInfoLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
     
        }

        private void ShowRelativePosition(HeroRelativePosition relPosition) {
            switch (relPosition){
                case HeroRelativePosition.None: HudBorder.Background = new SolidColorBrush(Color.FromRgb(42, 42, 42)); break;
                case HeroRelativePosition.InPosition: HudBorder.Background = new SolidColorBrush(Color.FromRgb(42, 70, 42)); break;
                case HeroRelativePosition.OutOfPosition: HudBorder.Background = new SolidColorBrush(Color.FromRgb(70, 42, 0)); break;
            }
        }
        private void ShowPotType(PotType potType)
        {
            switch (potType)
            {
                case PotType.Limped:
                    PotTypeLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 125, 255));
                    PotTypeLabel.Content = "Lm";
                    break;
                case PotType.IsoLimped:
                    PotTypeLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 150, 100));
                    PotTypeLabel.Content = "Is";
                    break;
                case PotType.Raised:
                    PotTypeLabel.Foreground = new SolidColorBrush(Color.FromRgb(125, 225, 255));
                    PotTypeLabel.Content = "Rs";
                    break;
                case PotType.Reraised:
                    PotTypeLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 80, 80));
                    PotTypeLabel.Content = "3b";
                    break;
            }
        }

        private void ShowPreflopDecision(DecisionInfo decisionInfo) {
            var decision = decisionInfo.PreflopDecision;
            var effStack = decisionInfo.LineInfo.Elements.EffectiveStack;
            var heroPreflopState = decisionInfo.LineInfo.HeroPreflopState;
            var opponent = decisionInfo.LineInfo.Elements.HuOpp;

            switch (decision) {
                case PreflopDecision.Fold:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 85, 85));
                    DecisionRun.Text = "___F 11";
                    break;
                case PreflopDecision.Limp:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                    DecisionRun.Text = "___Lim 22 ";
                    break;
                case PreflopDecision.OpenRaise:
                switch (heroPreflopState) {
                    case HeroPreflopState.FacingLimp: {
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(150, 200, 255));

                        var sbVsbtnEffStack = decisionInfo.LineInfo.Elements.SbBtnEffStack;
                        if (decisionInfo.LineInfo.Elements.HeroPlayer.Position == PlayerPosition.Sb) {
                            if (sbVsbtnEffStack >= 20) DecisionRun.Text = "___IS 4 ";
                            else if (sbVsbtnEffStack < 20 && sbVsbtnEffStack > 13) DecisionRun.Text = "___IS 3 ";
                            else if (sbVsbtnEffStack <= 13) DecisionRun.Text = "___IS 3 ";
                        }
                        else {
                             if (effStack <= 11) DecisionRun.Text = "___IS 2 ";
                             else if (effStack > 11 && effStack <= 16) DecisionRun.Text = "___IS 2.5 ";
                             else if (effStack > 16) DecisionRun.Text = "___IS 3 ";
                        }
                        break;
                    }
                    default: 
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(55, 240, 255));
                        /*
                        if (decisionInfo.LineInfo.Elements.HeroPlayer.RelativePosition ==
                            HeroRelativePosition.OutOfPosition) {
                            if (effStack > 15) DecisionRun.Text = "___OR 2.25 ";
                            else DecisionRun.Text = "___OR";
                        }
                        else {                            
                        }*/
                        DecisionRun.Text = "___OR  ";
                        break;
                }
                break;
                case PreflopDecision.Call:
                switch (heroPreflopState) {
                    case HeroPreflopState.FacingLimp:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                        DecisionRun.Text = "___CL 2";
                        break;
                    case HeroPreflopState.FacingOpen:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                        DecisionRun.Text = "___CO 43";
                        break;
                    case HeroPreflopState.FacingISOvsLimp:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                        DecisionRun.Text = "___CLR 322";
                        break;
                    case HeroPreflopState.FacingPushVsLimp:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                        DecisionRun.Text = "___CP 22";
                        break;
                    case HeroPreflopState.FacingLimpIsoShove:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                        DecisionRun.Text = "___CP 222";
                        break;
                    case HeroPreflopState.FacingOpenPush:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                        DecisionRun.Text = "___CP 6";
                        break;
                    case HeroPreflopState.FacingPushSqueeze:  
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                        DecisionRun.Text = "___CP 3";
                        break;
                    case HeroPreflopState.FacingPushVsOpen: 
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                        DecisionRun.Text = "___CP 3";
                        break;
                    case HeroPreflopState.FacingPushToIso:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                        DecisionRun.Text = "___CP 236";
                        break;
                    case HeroPreflopState.FacingPush: 
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                        DecisionRun.Text = "___CP 6";
                        break;
                }
                break;
                case PreflopDecision._3Bet:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(100, 100, 255));
                    DecisionRun.Text = "___3bet 5";
                    break;
                case PreflopDecision.Push:
                switch (heroPreflopState) {
                    case HeroPreflopState.Open:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(5, 5, 255));
                        DecisionRun.Text = "___OP 63";
                        break;
                    case HeroPreflopState.FacingLimp:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(5, 5, 255));
                        DecisionRun.Text = "___LP 62";
                        break;
                    case HeroPreflopState.FacingISOvsLimp:
                                DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(5, 5, 255));
                                DecisionRun.Text = "___PtLR 26";
                                break;
                    case HeroPreflopState.FacingOpen:
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(5, 5, 255));
                        DecisionRun.Text = "___OP 63";
                        break;
                }
                break;

                case PreflopDecision.None: {
                    switch (heroPreflopState) {
                        case HeroPreflopState.Facing3Bet:
                            DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255,190,190));
                            DecisionRun.Text = "v3bet";
                            if (opponent == null) break;
                            DecisionRun.Text += " " + opponent.Bet.ToString("#.#");
                            break;
                        case HeroPreflopState.FacingISOvsLimp:
                            DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(222, 222, 0));
                            DecisionRun.Text = "_vlISO";
                             if (opponent == null) break;
                            DecisionRun.Text += " " + opponent.Bet.ToString("#.#");
                            SetStat1(opponent.Stats.PF_RAISE_LIMPER, "rl", 34, 59);
                            break;
                    }
                    
                }
                
                    ShowPotOdds(decisionInfo.PotOdds);
                break;

            }


          
        }

        private void ShowFlopInfoStats(DecisionInfo decisionInfo) {
            var heroFlopState = decisionInfo.LineInfo.HeroFlopState;
            var opponent = decisionInfo.LineInfo.Elements.HuOpp;
            double? stat1 = null, stat2 = null;

            switch (heroFlopState) {
                case HeroFlopState.Donk:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(130, 160, 180));
                    DecisionRun.Text = "DB";
                    break;

                case HeroFlopState.FacingDonk:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(125, 225, 255));
                    DecisionRun.Text = "vsDB";

                    if (opponent == null) break;
                    SetStat1(opponent.Stats.F_DONK, "db", 10,30);
                    SetStat2(opponent.Stats.F_DONK_FOLDRAISE, "fr", 40, 70);
                    ShowBetToPot(opponent.BetToPot);
                    break;

                case HeroFlopState.Bet:
                    if (decisionInfo.LineInfo.Elements.HeroPlayer.RelativePosition == HeroRelativePosition.InPosition) {
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(137, 217, 150));
                        DecisionRun.Text = "BtIP";
                    } else {
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(123, 156, 150));
                        DecisionRun.Text = "BtOp";
                        if (opponent != null) {
                            SetStat1(opponent.Stats.F_LP_FOLD_VS_STEAL, "fb", 45, 60);
                        }
                    }
                    if (decisionInfo.LineInfo.Elements.HeroPlayer.Position == PlayerPosition.Sb){
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 205, 130));
                        DecisionRun.Text = "Bcb";
                        if (opponent != null) {
                            SetStat1(opponent.Stats.F_FOLD_CBET, "fb", 45, 60);
                            SetStat2(opponent.Stats.F_RAISE_BET, "rb", 14, 25);
                        }
                    }
                    break;

                case HeroFlopState.FacingBet:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(235, 223, 158));
                    if (decisionInfo.LineInfo.Elements.HeroPlayer.RelativePosition == HeroRelativePosition.InPosition) {
                        DecisionRun.Text = "VsBIP";
                        if (opponent == null) break;
                        SetStat1(opponent.Stats.F_DONK, "db", 40, 70);
                        ShowBetToPot(opponent.BetToPot);
                    }
                    else {
                        DecisionRun.Text = "VsBOp";
                        if (opponent == null) break;
                        SetStat1(opponent.Stats.F_LP_STEAL, "B", 40, 70);
                        SetStat1(opponent.Stats.F_LP_FOLD_VS_XR, "fr", 40, 70);
                        ShowBetToPot(opponent.BetToPot);
                    }
                    break;
                case HeroFlopState.FacingBetVsMissCbet:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 150, 150));
                    DecisionRun.Text = "vBx";
                    if (opponent == null) break;
                    ShowBetToPot(opponent.BetToPot);
                    break;
                case HeroFlopState.Cbet:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 177, 130));
                    DecisionRun.Text = "CBet";

                    if (opponent == null) break;
                    SetStat1(opponent.Stats.F_CBET, "fb", 45, 60);
                    SetStat2(opponent.Stats.F_RAISE_BET, "rb", 14, 25);
                    break;
                case HeroFlopState.FacingCbet:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 125, 125));
                    DecisionRun.Text = "vCB";

                    if (opponent == null) break;

                    if (decisionInfo.LineInfo.PotType == PotType.Raised) {
                        var steal = opponent.Position == PlayerPosition.Button ? opponent.Stats.PF_BTN_STEAL : opponent.Stats.PF_SB_STEAL;
                        var cbetStrenght = new CbetStrength(decisionInfo.LineInfo.Elements.CurrentStreet, steal, opponent.Stats.F_CBET);

                        StatName1Run.Text = cbetStrenght.StreetValue;
                        StatName1Run.Background = new SolidColorBrush(cbetStrenght.Color);
                        SetStat2(opponent.Stats.F_CBET_FOLDRAISE, "fr", 40, 65);
                    }
                    ShowBetToPot(opponent.BetToPot);
                    break;

                case HeroFlopState.FacingMissCbet:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(200, 200, 125));
                    DecisionRun.Text = "vmCB";

                    if (opponent == null) break;
                    SetStat1(opponent.Stats.F_CHECKFOLD_OOP, "xf", 40,65);
                    break;

                case HeroFlopState.FacingReraise:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 80, 80));
                    DecisionRun.Text = "vRE";

                    if (opponent == null) break;
                    SetStat1(opponent.Stats.F_RAISE_BET,"rb", 14, 25);
                    ShowPotOdds(decisionInfo.PotOdds);
                    break;
            }
        }

        private void ShowTurnInfoStats(DecisionInfo decisionInfo) {
            var heroTurnState = decisionInfo.LineInfo.HeroTurnState;
            var opponent = decisionInfo.LineInfo.Elements.HuOpp;

            switch (heroTurnState) {
                case HeroTurnState.Donk:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(160, 160, 160));
                    DecisionRun.Text = "Dk";
                    
                    break;
                    
                case HeroTurnState.Donk2:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(130, 190, 190));
                    DecisionRun.Text = "2DB";
                    break;

                case HeroTurnState.FacingDonk:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(125, 225, 255));
                    DecisionRun.Text = "vDB";
                    if (opponent == null) break;
                    SetStat1(opponent.Stats.T_DONK, "td", 15,50);
                    ShowBetToPot(opponent.BetToPot);
                    break;
                case HeroTurnState.FacingDonk2:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(125, 225, 255));
                    DecisionRun.Text = "v2DB";
                    if (opponent == null) break;
                    SetStat1(opponent.Stats.T_DONK, "td", 15,50);
                    ShowBetToPot(opponent.BetToPot);
                    break;

                case HeroTurnState.Bet2:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 180, 130));
                    DecisionRun.Text = "2BT";
                    if (opponent == null) break;
                    SetStat1(opponent.Stats.T_FOLD_CBET, "fcb", 40,60);
                    break;

                case HeroTurnState.DelayBet:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(222, 235, 125));
                    DecisionRun.Text = "vMFb";
                    break;
                case HeroTurnState.BetAfterReraiseFlop:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 190, 130));
                    DecisionRun.Text = "BaRF";
                    break;
                case HeroTurnState.VsMissFCb:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(200, 200, 125));
                    DecisionRun.Text = "vMFcb";
                    if (opponent == null) break;
                    SetStat1(opponent.Stats.T_SKIPF_FOLD_VS_T_PROBE, "fp", 40, 60);
                    break;
                case HeroTurnState.vsMissTCb:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(200, 200, 125));
                    DecisionRun.Text = "vMTcb";
                    if (opponent == null) break;
                    SetStat1(opponent.Stats.T_SKIPF_FOLD_VS_T_PROBE, "fp", 40, 60);
                    break;
                case HeroTurnState.FacingBet2:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 180, 180));
                    DecisionRun.Text = "v2BT";
                    if (opponent == null) break;
                    var st = opponent.Position == PlayerPosition.Button ? opponent.Stats.PF_BTN_STEAL : opponent.Stats.PF_SB_STEAL;
                    var cbstr = new CbetStrength(decisionInfo.LineInfo.Elements.CurrentStreet, st, opponent.Stats.F_LP_STEAL, opponent.Stats.T_CBET);
                    StatName1Run.Text = cbstr.StreetValue;
                    StatName1Run.Background = new SolidColorBrush(cbstr.Color);
                    SetStat2(opponent.Stats.T_CBET_FOLDRAISE, "fr", 40, 65);
                    ShowBetToPot(opponent.BetToPot);
                    break;
                case HeroTurnState.Cbet2:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 150, 90));
                    DecisionRun.Text = "2CBet";
                    if (opponent == null) break;

                    SetStat2(opponent.Stats.T_FOLD_CBET, "fb", 40, 65);
                    SetStat2(opponent.Stats.T_RAISE_BET, "rb", 5, 20);
                    break;
                case HeroTurnState.DelayCbet:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 177, 120));
                    DecisionRun.Text = "DCB";
                    break;
                case HeroTurnState.FacingCbet2:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 50, 50));
                    DecisionRun.Text = "v2CB";

                    if (opponent == null) break;
                    var steal = opponent.Position == PlayerPosition.Button
                            ? opponent.Stats.PF_BTN_STEAL : opponent.Stats.PF_SB_STEAL;
                    var cbetStrenght = new CbetStrength(decisionInfo.LineInfo.Elements.CurrentStreet, steal, opponent.Stats.F_CBET, opponent.Stats.T_CBET);
                    StatName1Run.Text = cbetStrenght.StreetValue;
                    StatName1Run.Background = new SolidColorBrush(cbetStrenght.Color);
                    ShowBetToPot(opponent.BetToPot);
                    break;
                case HeroTurnState.FacingCheckAfterFlopReraise:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(200, 220, 130));
                    DecisionRun.Text = "vXfR";
                    break;
                case HeroTurnState.FacingDelayBet:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 180, 180));
                    DecisionRun.Text = "vDbt";
                    if (opponent == null) break;
                    ShowBetToPot(opponent.BetToPot);
                    break;
                case HeroTurnState.FacingDelayCbet:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 150, 150));
                    DecisionRun.Text = "vDCB";
                    if (opponent == null) break;
                    ShowBetToPot(opponent.BetToPot);
                    break;
                case HeroTurnState.FacingReraise:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                    DecisionRun.Text = "vRE";
                    if (opponent == null) break;
                    
                    SetStat1(opponent.Stats.T_RAISE_BET,"rb", 5, 15);
                    break;
                case HeroTurnState.VsMissDonk2:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(217, 222, 255));
                    DecisionRun.Text = "vMDK2";
                    break;
                case HeroTurnState.FacingCbetAfterFlopReraise:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                    DecisionRun.Text = "vCbaFR";
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
                    if (opponent == null) break;
                    ShowBetToPot(opponent.BetToPot);
                    break;
                case HeroRiverState.FacingDonk3:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(125, 225, 255));
                    DecisionRun.Text = "v3DB";
                    if (opponent == null) break;
                    ShowBetToPot(opponent.BetToPot);
                    break;
                case HeroRiverState.Cbet3:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 150, 90));
                    DecisionRun.Text = "3CB";
                    if (opponent == null) break;
                    SetStat1(opponent.Stats.R_FOLD_CBET, "fcb", 33, 60);
                    break;
                case HeroRiverState.FacingBet3:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 125, 125));
                    DecisionRun.Text = "v3BT";
                    if (opponent == null) break;
                    var st = opponent.Position == PlayerPosition.Button ? opponent.Stats.PF_BTN_STEAL : opponent.Stats.PF_SB_STEAL;
                    var cbstr = new CbetStrength(decisionInfo.LineInfo.Elements.CurrentStreet, st, opponent.Stats.F_LP_STEAL, opponent.Stats.T_CBET, opponent.Stats.R_CBET);
                    StatName1Run.Text = cbstr.StreetValue;
                    StatName1Run.Background = new SolidColorBrush(cbstr.Color);
                    ShowBetToPot(opponent.BetToPot);
                    break;

                case HeroRiverState.FacingCbet3:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 50, 50));
                    DecisionRun.Text = "v3CB";
                    if (opponent == null) break;
                    var steal = opponent.Position == PlayerPosition.Button ? opponent.Stats.PF_BTN_STEAL : opponent.Stats.PF_SB_STEAL;
                    var cbetStrenght = new CbetStrength(decisionInfo.LineInfo.Elements.CurrentStreet, steal, opponent.Stats.F_CBET, opponent.Stats.T_CBET, opponent.Stats.R_CBET);
                    StatName1Run.Text = cbetStrenght.StreetValue;
                    StatName1Run.Background = new SolidColorBrush(cbetStrenght.Color);
                    ShowBetToPot(opponent.BetToPot);
                    break;
            }
        }
        
        private void ClearStats() {
            StatName1Run.Text = "";
            StatName1Run.Foreground = new SolidColorBrush(Color.FromRgb(255,255,255));
            StatName2Run.Foreground = new SolidColorBrush(Color.FromRgb(255,255,255));

            StatName1Run.Background = new SolidColorBrush(Color.FromArgb(0,0,0,0));
            StatName2Run.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            
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


        private void SetStat1(double? stat, string name, int colorSeparator1, int colorSeperator2) {
            StatName1Run.Text = name;
            if (stat == null || stat == 0) StatName1Run.Foreground = new SolidColorBrush(Color.FromRgb(125, 125, 125));
            Stat1ValRun.Text = stat == null ? "-" : stat.ToString();
            Stat1ValRun.Foreground = PeekStatColor(stat, colorSeparator1, colorSeperator2);
            
        }

        private void SetStat2(double? stat, string name, int colorSeparator1, int colorSeperator2) {
            StatName2Run.Text = name;
            if (stat == null || stat == 0) StatName2Run.Foreground = new SolidColorBrush(Color.FromRgb(125, 125, 125));
            Stat2ValRun.Text = stat == null ? "-" : stat.ToString();
            Stat2ValRun.Foreground = PeekStatColor(stat, colorSeparator1, colorSeperator2);
            
        }
                        
        private void StackLabel_MouseDown(object sender, MouseButtonEventArgs e) {
            if (MakeScreenShotClick != null) {
                MakeScreenShotClick();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            //register activate hotkey
           // const uint activateHotkeyVLC = 86; // keycode of V
         //   WINAPI.RegisterHotKey(this, activateHotkeyVLC);
         //   ComponentDispatcher.ThreadPreprocessMessage += ComponentDispatcher_ThreadPreprocessMessage;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
        //   WINAPI.UnregisterHotKey(this);

            foreach (var p in Process.GetProcesses()) {
                if (p.ProcessName == "Pingvi") p.Kill();
            }
        }

       // private uint WM_KEYUP = 0x0101;

  
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

    class CbetStrength {
        public Color Color
        {
            get
            {
                if (StreetValue == "n" || StreetValue == "") return Color.FromRgb(42, 42, 42);
                if (StreetValue == "p" || StreetValue == "p!")
                {
                    byte redC = DoubleToByte((510 - 510 * (_pfSteal / 100.0)));
                    byte greenC = DoubleToByte((510 * (_pfSteal / 100.0)));
                    return Color.FromRgb(redC, greenC, 0);
                }
                if (StreetValue == "_f" || StreetValue == "f!") {
                    byte redC = DoubleToByte((510 - 510 * (_pfSteal / 100.0) * (_fCbet / 100.0)));
                    byte greenC = DoubleToByte((510 * (_pfSteal / 100.0) * (_fCbet / 100.0)));
                    return Color.FromRgb(redC, greenC, 0);
                }
                if (StreetValue == "_t" || StreetValue == "t!") {
                    byte redC = DoubleToByte((510 - 510 * (_pfSteal / 100.0) * (_fCbet / 100.0) * (_tCbet / 100.0)));
                    byte greenC = DoubleToByte((510 * (_pfSteal / 100.0) * (_fCbet / 100.0) * (_tCbet / 100.0)));
                    return Color.FromRgb(redC, greenC, 0);
                }
                if (StreetValue == "_r") {
                    byte redC = DoubleToByte((510 - 510 * (_pfSteal / 100.0) * (_fCbet / 100.0) * (_tCbet / 100.0) * (_rCbet / 100.0)));
                    byte greenC = DoubleToByte((510 * (_pfSteal / 100.0) * (_fCbet / 100.0) * (_tCbet / 100.0) * (_rCbet / 100.0)));
                    return Color.FromRgb(redC, greenC, 0);
                }
                return Color.FromRgb(42, 42, 42);
            }
        }
        private CurrentStreet _currentStreet;
        private double? _pfSteal;
        private double? _fCbet;
        private double? _tCbet;
        private double? _rCbet;

        public string StreetValue
        {
            get
            {
                switch (_currentStreet) {
                    case CurrentStreet.Preflop: return "n";
                    case CurrentStreet.Flop:
                        if (_pfSteal != null && _fCbet != null) return "_f";
                        if (_pfSteal != null && _fCbet == null) return "p!";
                        return "n";
                    case CurrentStreet.Turn:
                        if (_pfSteal != null && _fCbet != null && _tCbet != null) return "_t";
                        if (_pfSteal != null && _fCbet != null) return "f!";
                        return "n";
                    case CurrentStreet.River:
                        if (_pfSteal != null && _fCbet != null && _tCbet != null && _rCbet != null) return "_r";
                        if (_pfSteal != null && _fCbet != null && _tCbet != null) return "t!";
                        if (_pfSteal != null && _fCbet != null) return "f!";
                        return "n";
                }
                return "";
            }
        }

        public CbetStrength(CurrentStreet currentStreet, double? pfSteal, double? fCbet) {
            _currentStreet = currentStreet;
            _pfSteal = pfSteal;
            _fCbet = fCbet;
        }

        public CbetStrength(CurrentStreet currentStreet, double? pfSteal, double? fCbet, double? tCbet)
        {
            _currentStreet = currentStreet;
            _pfSteal = pfSteal;
            _fCbet = fCbet;
            _tCbet = tCbet;
        }

        public CbetStrength(CurrentStreet currentStreet, double? pfSteal, double? fCbet, double? tCbet, double? rCbet)
        {
            _currentStreet = currentStreet;
            _pfSteal = pfSteal;
            _fCbet = fCbet;
            _tCbet = tCbet;
            _rCbet = rCbet;
        }

        private byte DoubleToByte(double? value) {
            if (value == null) return 0;
            var val = (int) value;
            if (val > 255) val = 255;
            if (val < 0) val = 0;
            string valStr = val.ToString();
            return Byte.Parse(valStr);
        }
    }
         
}
