using System;
using System.CodeDom;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
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

        private ScreenTableManager _tableManager; //for making screenshot
        private Bitmap _tableBitmap;

        public Action MakeScreenShotClick;

        public HudWindow() {
            InitializeComponent();
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
            if (betToPot > 0.0 && betToPot <= 0.3) AdditionalInfoLabel.Foreground = new SolidColorBrush(Color.FromRgb(12, 255, 48));
            if (betToPot > 0.30 && betToPot <= 0.49) AdditionalInfoLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 200, 0));
            if (betToPot > 0.49 && betToPot <= 0.65) AdditionalInfoLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 120, 30));
            if (betToPot > 0.65 && betToPot <= 0.89) AdditionalInfoLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 120, 120));
            if (betToPot > 0.89 && betToPot <= 9.99) AdditionalInfoLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 50, 50));
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
                switch (heroPreflopState)
                {
                    case HeroPreflopState.FacingLimp: {
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(150, 200, 255));
                        var sbVsbtnEffStack = decisionInfo.LineInfo.Elements.SbBtnEffStack;

                        if (decisionInfo.LineInfo.Elements.HeroPlayer.Position == PlayerPosition.Sb) {
                            if (sbVsbtnEffStack >= 20) DecisionRun.Text = "___IS 4 ";
                            else if (sbVsbtnEffStack < 20 && sbVsbtnEffStack > 13) DecisionRun.Text = "___IS 3 ";
                            else if (sbVsbtnEffStack <= 13) DecisionRun.Text = "___IS 2 ";
                        }
                        else {
                             if (effStack <= 16) DecisionRun.Text = "___IS 2 ";
                             else if (effStack > 16 && effStack <= 20) DecisionRun.Text = "___IS 2.5 ";
                             else if (effStack > 20) DecisionRun.Text = "___IS 3 ";
                        }
                        break;
                    }
                    default: 
                        DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(55, 240, 255));
                        DecisionRun.Text = "___OR 3 ";
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

                    StatName1Run.Text = "db";
                    if(opponent !=null )stat1 = opponent.Stats.F_DONK;
                    Stat1ValRun.Foreground = PeekStatColor(stat1, 10, 30);
                    Stat1ValRun.Text = stat1 == null ? "-" : stat1.ToString();

                    StatName2Run.Text = "fr";
                    if (opponent != null) stat2 = opponent.Stats.F_DONK_FOLDRAISE;
                    Stat2ValRun.Foreground = PeekStatColor(stat2, 40, 70);
                    Stat2ValRun.Text = stat2 == null ? "-" : stat2.ToString();

                    PeekStatNameColor(stat1, StatName1Run);
                    PeekStatNameColor(stat2, StatName2Run);

                    if (opponent != null) ShowBetToPot(opponent.BetToPot);
                    break;

                case HeroFlopState.Bet:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(137, 217, 150));
                    DecisionRun.Text = "BT";

                    StatName1Run.Text = "fb";
                    if (opponent != null) stat1 = opponent.Stats.F_FOLD_CBET;

                    Stat1ValRun.Foreground = PeekStatColor(stat1, 45, 60);
                    Stat1ValRun.Text = stat1 == null ? "-" : stat1.ToString();

                    StatName2Run.Text = "rb";
                    if (opponent != null) stat2 = opponent.Stats.F_RAISE_CBET;
                    Stat2ValRun.Foreground = PeekStatColor(stat2, 10, 25);
                    Stat2ValRun.Text = stat2 == null ? "-" : stat2.ToString();

                    PeekStatNameColor(stat1, StatName1Run);
                    PeekStatNameColor(stat2, StatName2Run);
                    break;

                case HeroFlopState.FacingBet:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(235, 223, 158));
                    DecisionRun.Text = "vBT";

                    StatName1Run.Text = "cb";
                    if (opponent != null) stat1 = opponent.Stats.F_CBET;
                    Stat1ValRun.Foreground = PeekStatColor(stat1, 40, 70);
                    Stat1ValRun.Text = stat1 == null ? "-" : stat1.ToString();

                    StatName2Run.Text = "fr";
                    if (opponent != null) stat2 = opponent.Stats.F_CBET_FOLDRAISE;
                    Stat2ValRun.Foreground = PeekStatColor(stat2, 40, 70);
                    Stat2ValRun.Text = stat2 == null ? "-" : stat2.ToString();
                    PeekStatNameColor(stat1, StatName1Run);
                    PeekStatNameColor(stat2, StatName2Run);

                    if (opponent != null) ShowBetToPot(opponent.BetToPot);
                    break;
                
                case HeroFlopState.FacingBetVsMissCbet:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 150, 150));
                    DecisionRun.Text = "vBx";

                    StatName1Run.Text = "cb";
                    if (opponent != null) stat1 = opponent.Stats.F_CBET;
                    Stat1ValRun.Foreground = PeekStatColor(stat1, 40, 70);
                    Stat1ValRun.Text = stat1 == null ? "-" : stat1.ToString();

                    StatName2Run.Text = "fr";
                    if (opponent != null) stat2 = opponent.Stats.F_CBET_FOLDRAISE;
                    Stat2ValRun.Foreground = PeekStatColor(stat2, 40, 70);
                    Stat2ValRun.Text = stat2 == null ? "-" : stat2.ToString();
                    PeekStatNameColor(stat1, StatName1Run);
                    PeekStatNameColor(stat2, StatName2Run);

                    if (opponent != null) ShowBetToPot(opponent.BetToPot);
                    break;

                case HeroFlopState.Cbet:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 177, 130));
                    DecisionRun.Text = "CBet";

                    StatName1Run.Text = "fb";
                    if (opponent != null) stat1 = opponent.Stats.F_FOLD_CBET;

                    Stat1ValRun.Foreground = PeekStatColor(stat1, 45, 60);
                    Stat1ValRun.Text = stat1 == null ? "-" : stat1.ToString();

                    StatName2Run.Text = "rb";
                    if (opponent != null) stat2 = opponent.Stats.F_RAISE_CBET;
                    Stat2ValRun.Foreground = PeekStatColor(stat2, 10, 25);
                    Stat2ValRun.Text = stat2 == null ? "-" : stat2.ToString();

                    PeekStatNameColor(stat1, StatName1Run);
                    PeekStatNameColor(stat2, StatName2Run);
                    break;
                case HeroFlopState.FacingCbet:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 125, 125));
                    DecisionRun.Text = "vCB";

                    StatName1Run.Text = "cb";
                    if (opponent != null) stat1 = opponent.Stats.F_CBET;
                    Stat1ValRun.Foreground = PeekStatColor(stat1, 40, 70);
                    Stat1ValRun.Text = stat1 == null ? "-" : stat1.ToString();

                    StatName2Run.Text = "fr";
                    if (opponent != null) stat2 = opponent.Stats.F_CBET_FOLDRAISE;
                    Stat2ValRun.Foreground = PeekStatColor(stat2, 40, 70);
                    Stat2ValRun.Text = stat2 == null ? "-" : stat2.ToString();
                    PeekStatNameColor(stat1, StatName1Run);
                    PeekStatNameColor(stat2, StatName2Run);

                    if (opponent != null) ShowBetToPot(opponent.BetToPot);
                    break;

                case HeroFlopState.FacingMissCbet:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(200, 200, 125));
                    DecisionRun.Text = "vmCB";

                    StatName1Run.Text = "cb";
                    if (opponent != null) stat1 = opponent.Stats.F_CBET;
                    Stat1ValRun.Foreground = PeekStatColor(stat1, 40, 70);
                    Stat1ValRun.Text = stat1 == null ? "-" : stat1.ToString();
                    break;

                case HeroFlopState.FacingReraise:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 80, 80));
                    DecisionRun.Text = "vRE";
                    
                    StatName1Run.Text = "rb";
                    if (opponent != null) stat1 = opponent.Stats.F_RAISE_CBET;
                    Stat1ValRun.Foreground = PeekStatColor(stat1, 10, 25);
                    Stat1ValRun.Text = stat1 == null ? "-" : stat1.ToString();

                     if(opponent !=null )PeekStatNameColor(stat1, StatName1Run);

                     if (opponent != null) ShowPotOdds(decisionInfo.PotOdds);
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
                    if (opponent != null) ShowBetToPot(opponent.BetToPot);
                    break;
                case HeroTurnState.FacingDonk2:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(125, 225, 255));
                    DecisionRun.Text = "v2DB";
                    if (opponent != null) ShowBetToPot(opponent.BetToPot);
                    break;

                case HeroTurnState.Bet2:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 180, 130));
                    DecisionRun.Text = "2BT";
                    break;
                case HeroTurnState.DelayBet:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 220, 130));
                    DecisionRun.Text = "DBT";
                    break;
                case HeroTurnState.BetAfterReraiseFlop:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 190, 130));
                    DecisionRun.Text = "BaRF";
                    break;
                case HeroTurnState.VsMissFCb:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(200, 200, 125));
                    DecisionRun.Text = "vMFcb";
                    break;
                case HeroTurnState.vsMissTCb:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(200, 200, 125));
                    DecisionRun.Text = "vMTcb";
                    break;
                case HeroTurnState.FacingBet2:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 180, 180));
                    DecisionRun.Text = "v2BT";
                    if (opponent != null) ShowBetToPot(opponent.BetToPot);
                    break;
                case HeroTurnState.Cbet2:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 150, 90));
                    DecisionRun.Text = "2CBet";
                    break;
                case HeroTurnState.DelayCbet:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 177, 120));
                    DecisionRun.Text = "DCB";
                    break;
                case HeroTurnState.FacingCbet2:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 50, 50));
                    DecisionRun.Text = "v2CB";
                    if (opponent != null) ShowBetToPot(opponent.BetToPot);
                    break;
                case HeroTurnState.FacingCheckAfterFlopReraise:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(200, 220, 130));
                    DecisionRun.Text = "vXfR";
                    break;
                case HeroTurnState.FacingDelayBet:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 180, 180));
                    DecisionRun.Text = "vDbt";
                    if (opponent != null) ShowBetToPot(opponent.BetToPot);
                    break;
                case HeroTurnState.FacingDelayCbet:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 150, 150));
                    DecisionRun.Text = "vDCB";
                    if (opponent != null) ShowBetToPot(opponent.BetToPot);
                    break;
                case HeroTurnState.FacingReraise:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                    DecisionRun.Text = "vRE";
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
                    if (opponent != null) ShowBetToPot(opponent.BetToPot);
                    break;
                case HeroRiverState.FacingDonk3:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(125, 225, 255));
                    DecisionRun.Text = "v3DB";
                    if (opponent != null) ShowBetToPot(opponent.BetToPot);
                    break;
                case HeroRiverState.Cbet3:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 150, 90));
                    DecisionRun.Text = "3CB";
                    break;
                case HeroRiverState.FacingBet3:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 125, 125));
                    DecisionRun.Text = "v3BT";
                    if (opponent != null) ShowBetToPot(opponent.BetToPot);
                    break;
                case HeroRiverState.FacingCbet3:
                    DecisionRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 50, 50));
                    DecisionRun.Text = "v3CB";
                    if (opponent != null) ShowBetToPot(opponent.BetToPot);
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
         
}
