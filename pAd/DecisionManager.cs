using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using PokerModel;

namespace Pingvi
{
    public enum PlMode {
        None, More, Less
    }


    public enum   Foldto3Bet  {None, Small, Average, Big};
    public class DecisionManager {

        public event Action<HudInfo> NewHudInfo;

        private HudInfo _hudInfo;
        private bool _isNewDecision;
        private const string RangesPath = @"Data\Ranges";
        private List<Range> _rangesList;

        private Elements prevEl;

        
        public event Action<string> NewRangeChosen;

        public DecisionManager() {
            _hudInfo = new HudInfo();
            LoadRanges(RangesPath);
        }
     

        private void LoadRanges(string rangesPath) {
            _rangesList = new List<Range>();
            foreach (var file in Directory.GetFiles(rangesPath, "*.xml", SearchOption.AllDirectories)) {
                _rangesList.Add(XmlRangeHelper.Load(file));
            }
        }

        public void OnNewElements(Elements elements)
        {

            _isNewDecision = false;

            prevEl = elements;
            _hudInfo.EffectiveStack = elements.EffectiveStack;
            _hudInfo.HeroRelativePosition = elements.HeroPlayer.RelativePosition;
            _hudInfo.HeroStatePreflop = elements.HeroPlayer.StatePreflop;
            _hudInfo.HeroStateFlop = elements.HeroPlayer.StatePostflop;
            _hudInfo.CurrentStreet = elements.CurrentStreet;
            _hudInfo.CurrentPot = elements.TotalPot;
            _hudInfo.PotOdds = FindPotOdds(elements);
            _hudInfo.Opponent = elements.HuOpp;

            Hand heroHand = elements.HeroPlayer.Hand;

            //COMMON
            elements.StartRule().HeroRole(HeroRole.Opener)
               .SitOutOpp()
               .Do(e => CheckDecision(heroHand, "COMMON_OPEN_100", 0, PlMode.None));



            #region BTN
            //BTN
            //BTN OPEN


            double? SB_3BET_VS_BTN = elements.LeftPlayer.Stats.PF_SB_3BET_VS_BTN;
            double? BB_3BET_VS_BTN = elements.RightPlayer.Stats.PF_BB_3BET_VS_BTN;

            const double SB_3BET_VS_BTN_default = 20;
            const double BB_3BET_VS_BTN_default = 20;
            if (SB_3BET_VS_BTN == null) SB_3BET_VS_BTN = SB_3BET_VS_BTN_default;
            if (BB_3BET_VS_BTN == null) BB_3BET_VS_BTN = BB_3BET_VS_BTN_default;

            double? merged3betVsBTN = SB_3BET_VS_BTN + BB_3BET_VS_BTN;

            elements.StartRule().HeroPosition(PlayerPosition.Button)
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                .HeroStackBetween(0, 4)
                .Do(e => CheckDecision(heroHand, "BTN_OPEN_0-4bb", elements.HeroPlayer.Stack, PlMode.Less));

            elements.StartRule().HeroPosition(PlayerPosition.Button)
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                .HeroStackBetween(4, 6)
                .Do(e => CheckDecision(heroHand, "BTN_OPEN_4-6bb", 0, PlMode.None));

            elements.StartRule().HeroPosition(PlayerPosition.Button)
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                .HeroStackBetween(6, 8)
                .Do(e => CheckDecision(heroHand, "BTN_OPEN_6-8bb", 0, PlMode.None));

            elements.StartRule().HeroPosition(PlayerPosition.Button)
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                .HeroStackBetween(8, 13)
                .Do(e => CheckDecision(heroHand, "BTN_OPEN_8-13bb", merged3betVsBTN, PlMode.More));

            elements.StartRule().HeroPosition(PlayerPosition.Button)
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                .HeroStackBetween(13, 17)
                .Do(e => CheckDecision(heroHand, "BTN_OPEN_13-17bb", merged3betVsBTN, PlMode.More));

            elements.StartRule().HeroPosition(PlayerPosition.Button)
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                .HeroStackBetween(17, 100)
                .Do(e => CheckDecision(heroHand, "BTN_OPEN_17-100bb", 0, PlMode.None));
     
            
            //BTN CALL PUSH AFTER OPEN
            //SMALL STAKES
            //TODO ДОДЕЛАТЬ ДИАПАЗОНЫ КОЛА В МЕЛКИХ СТЕКАХ, РАЗДЕЛИТЬ В ЗАВИСИМОСТИ ОТ РАЗМЕРА ББ
            elements.StartRule().HeroPosition(PlayerPosition.Button).IsHU()
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.FacingPush)
                .EffectiveStackBetween(0, 8)
                .Do(e => CheckDecision(heroHand, "BTN_CALLPUSH_VSOPEN_0-11bb_UNK", e.EffectiveStack, PlMode.Less));
            //VS SB PUSH
            elements.StartRule().HeroPosition(PlayerPosition.Button).IsHU()
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.FacingPush)
                .EffectiveStackBetween(8, 100)
                .OppPosition(PlayerPosition.Sb)
                .Do(e => CheckDecision(heroHand, "btn_callpush_vs_SB", e.EffectiveStack, PlMode.Less));

            //VS BB PUSH
            elements.StartRule().HeroPosition(PlayerPosition.Button).IsHU()
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.FacingPush)
                .EffectiveStackBetween(8, 100)
                .OppPosition(PlayerPosition.Bb)
                .Do(e => CheckDecision(heroHand, "btn_callpush_vs_BB", e.EffectiveStack, PlMode.Less));
           

            
            
            
          
         

            #endregion


            #region SB
            //SB

            double? _3betStatBBvsSB = null;
            if (elements.HuOpp != null) _3betStatBBvsSB = elements.HuOpp.Stats.PF_BB_3BET_VS_SB;
            double? _DefStatBBvsSB = null;
            if (elements.HuOpp != null) _DefStatBBvsSB = elements.HuOpp.Stats.PF_BB_DEF_VS_SBSTEAL;
            double? _FoldCBIP = null;
            if (elements.HuOpp != null) _FoldCBIP = elements.HuOpp.Stats.F_FOLD_CBET;

            //SB OPEN COMMON
            //vs BIG STACK
            elements.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open).HeroRelativePosition(HeroRelativePosition.InPosition)
                .EffectiveStackBetween(0, 7)
                .VsBigStack()
                .Do(e => CheckDecision(heroHand, "SB_OPENPUSH_CHEBUKOV", e.EffectiveStack, PlMode.Less));

            elements.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open).HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(0, 8)
                .VsBigStack()
                .Do(e => CheckDecision(heroHand, "SB_OPENPUSH_CHEBUKOV", e.EffectiveStack, PlMode.Less));

            //vs other
            elements.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open).HeroRelativePosition(HeroRelativePosition.InPosition)
                .EffectiveStackBetween(0, 7)
                .Do(e => CheckDecision(heroHand, "SB_OPENPUSH_NASH", e.EffectiveStack, PlMode.Less));

            elements.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open).HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(0, 8)
                .Do(e => CheckDecision(heroHand, "SB_OPENPUSH_NASH", e.EffectiveStack, PlMode.Less));

            //SB VS BB OPEN 2max HU

            elements.StartRule().HeroPosition(PlayerPosition.Sb).HeroRelativePosition(HeroRelativePosition.InPosition)
                   .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                   .EffectiveStackBetween(7, 8)
                   .Do(e => CheckDecision(heroHand, "SB_VS_BB_OPEN_7-8bb_2max_UNK", 0, PlMode.None));

            elements.StartRule().HeroPosition(PlayerPosition.Sb).HeroRelativePosition(HeroRelativePosition.InPosition)
                   .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                   .EffectiveStackBetween(8, 9)
                   .Do(e => CheckDecision(heroHand, "SB_VS_BB_OPEN_8-9bb_2max_UNK", 0, PlMode.None));


            if (_3betStatBBvsSB > 35 || (_DefStatBBvsSB > 65 && _FoldCBIP < 43)) {
                //vs LAG
                elements.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(9, 12)
                    .Do(e => CheckDecision(heroHand, "SB_VS_BB_OPEN_9-12bb_2max_AGR", 0, PlMode.None));

                elements.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(12, 14)
                    .Do(e => CheckDecision(heroHand, "SB_VS_BB_OPEN_12-14bb_2max_AGR", 0, PlMode.None));

                elements.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(14, 16)
                    .Do(e => CheckDecision(heroHand, "SB_VS_BB_OPEN_14-16bb_2max_AGR", 0, PlMode.None));

                elements.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(16, 20)
                    .Do(e => CheckDecision(heroHand, "SB_VS_BB_OPEN_16-20bb_2max_AGR", 0, PlMode.None));

                elements.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(20, 100)
                    .Do(e => CheckDecision(heroHand, "SB_VS_BB_OPEN_20-100bb_2max_AGR", 0, PlMode.None));
            }
            else {
                //vs UNK
                elements.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(9, 11)
                    .Do(e => CheckDecision(heroHand, "SB_VS_BB_OPEN_9-11bb_2max_UNK", 0, PlMode.None));

                elements.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(11, 13)
                    .Do(e => CheckDecision(heroHand, "SB_VS_BB_OPEN_11-13bb_2max_UNK", 0, PlMode.None));

                elements.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(13, 16)
                    .Do(e => CheckDecision(heroHand, "SB_VS_BB_OPEN_13-16bb_2max_UNK", 0, PlMode.None));

                elements.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(16, 20)
                    .Do(e => CheckDecision(heroHand, "SB_VS_BB_OPEN_16-20bb_2max_UNK", 0, PlMode.None));

                elements.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(20, 100)
                    .Do(e => CheckDecision(heroHand, "SB_VS_BB_OPEN_20-100bb_2max_UNK", 0, PlMode.None));

            }


            //SB OPEN VS BB 3MAX
            if (_3betStatBBvsSB == null || _3betStatBBvsSB < 35) {
                //VS UNK
                elements.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(8, 10)
                    .Do(e => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_8-10bb_UNK", 0, PlMode.None));

                elements.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(10, 13)
                    .Do(e => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_10-13bb_UNK", 0, PlMode.None));

                elements.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(13, 15)
                    .Do(e => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_13-15bb_UNK", 0, PlMode.None));

                elements.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(15, 20)
                    .Do(e => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_15-20bb_UNK", 0, PlMode.None));

                elements.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(20, 100)
                    .Do(e => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_20-100bb_UNK", 0, PlMode.None));
            }
            else {
                //VS LAG
                elements.StartRule()
                   .HeroPosition(PlayerPosition.Sb)
                   .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                   .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                   .EffectiveStackBetween(8, 10)
                   .Do(e => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_8-10bb_AGR", 0, PlMode.None));

                elements.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(10, 13)
                    .Do(e => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_10-13bb_AGR", 0, PlMode.None));

                elements.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(13, 15)
                    .Do(e => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_13-15bb_AGR", 0, PlMode.None));

                elements.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(15, 20)
                    .Do(e => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_15-20bb_AGR", 0, PlMode.None));

                elements.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(20, 100)
                    .Do(e => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_20-100bb_AGR", 0, PlMode.None));
            }
            
            //SB CALL PUSH AFTER LIMP

            elements.StartRule().HeroPosition(PlayerPosition.Sb)
             .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.FacingPushVsLimp)
             .EffectiveStackBetween(0, 9)
             .Do(e => CheckDecision(heroHand, "SB_CALLPUSH_VSLIMP_7_9bb_UNK", e.EffectiveStack, PlMode.Less));

            elements.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.FacingPushVsLimp)
                .EffectiveStackBetween(9, 12)
                .Do(e => CheckDecision(heroHand, "SB_CALLPUSH_VSLIMP_9-12bb_UNK", e.EffectiveStack, PlMode.Less));

            elements.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.FacingPushVsLimp)
                .EffectiveStackBetween(12, 14)
                .Do(e => CheckDecision(heroHand, "SB_CALLPUSH_VSLIMP_12-14bb_UNK", null, PlMode.None));

            elements.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.FacingPushVsLimp)
                .EffectiveStackBetween(14, 16)
                .Do(e => CheckDecision(heroHand, "SB_CALLPUSH_VSLIMP_14-16bb_UNK", null, PlMode.None));

            //SB DEF VS MINR After LImp
            elements.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.FacingRaiseVsLimp)
                .EffectiveStackBetween(7, 9)
                .Do(e => CheckDecision(heroHand, "SB_DefVSMinRaise_AfterLIMP_IP_7-9bb_UNK", null, PlMode.None));

            elements.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.FacingRaiseVsLimp)
                .EffectiveStackBetween(9, 12)
                .Do(e => CheckDecision(heroHand, "SB_DefVSMinRaise_AfterLIMP_IP_9-12bb_UNK", null, PlMode.None));

            elements.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.FacingRaiseVsLimp)
                .EffectiveStackBetween(12, 14)
                .Do(e => CheckDecision(heroHand, "SB_DefVSMinRaise_AfterLIMP_IP_12-14bb_UNK", null, PlMode.None));

            elements.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.FacingRaiseVsLimp)
                .EffectiveStackBetween(14, 16)
                .Do(e => CheckDecision(heroHand, "SB_DefVSMinRaise_AfterLIMP_IP_14-16bb_UNK", null, PlMode.None));



            //SB CALL PUSH AFTER OPEN
            //SB CALL PUSH AFTER VS UNK
            if (_3betStatBBvsSB == null)
            {
                elements.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.FacingPush)
                    .EffectiveStackBetween(0, 9)
                    .Do(e => CheckDecision(heroHand, "SB_CALLPUSH_VSOPEN_0-11bb_UNK", e.EffectiveStack, PlMode.Less));

                elements.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.FacingPush)
                    .EffectiveStackBetween(9, 100)
                    .Do(e => CheckDecision(heroHand, "SB_CALLPUSH_AFTEROPEN_9-100_UNK_NEW", e.EffectiveStack, PlMode.Less));

            }
            else
            {
                elements.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.FacingPush)
                    .EffectiveStackBetween(0, 9)
                    .Do(e => CheckDecision(heroHand, "SB_CALLPUSH_VSOPEN_0-11bb_UNK", e.EffectiveStack, PlMode.Less));

                elements.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.FacingPush)
                    .EffectiveStackBetween(9, 13)
                    .Do(e => CheckDecision(heroHand, "SB_CALLPuSH_VSOPEN_10-13bb_EXPL", _3betStatBBvsSB, PlMode.More));

                elements.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.FacingPush)
                    .EffectiveStackBetween(13, 20)
                    .Do(e => CheckDecision(heroHand, "SB_CALLPuSH_VSOPEN_13-20bb_EXPL", _3betStatBBvsSB, PlMode.More));

                elements.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.FacingPush)
                    .EffectiveStackBetween(20, 100)
                    .Do(e => CheckDecision(heroHand, "SB_CALLPuSH_VSOPEN_20bb+_EXPL", _3betStatBBvsSB, PlMode.More));
            }

            //SB DEFEND VS BTN OPEN
            double? _btnOpenSteal = null;
            var buttonOpener = elements.ActivePlayers.FirstOrDefault(p => p.Position == PlayerPosition.Button);
            if (buttonOpener != null) _btnOpenSteal = buttonOpener.Stats.PF_BTN_STEAL;


            elements.StartRule().HeroPosition(PlayerPosition.Sb)
               .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen)
               .OppBetSizeMinRaise()
               .EffectiveStackBetween(8, 12)
               .Do(e => CheckDecision(heroHand, "SB_FacingMinRaise_MP_8_12bb", 0, PlMode.None));

            elements.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(12, 15)
                .Do(e => CheckDecision(heroHand, "SB_FacingMinRaise_MP_12_15bb", 0, PlMode.None));

            if (_btnOpenSteal == null)
            {
                elements.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen)
                    .OppBetSizeMinRaise()
                    .EffectiveStackBetween(15, 17)
                    .Do(e => CheckDecision(heroHand, "SB_FacingMinRaise_MP_15_17bb_UNK", 0, PlMode.None));

                elements.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen)
                    .OppBetSizeMinRaise()
                    .EffectiveStackBetween(17, 19)
                    .Do(e => CheckDecision(heroHand, "SB_FacingMinRaise_MP_17_20bb_UNK", 0, PlMode.None));

            }
            else
            {
                elements.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen)
                    .OppBetSizeMinRaise()
                    .EffectiveStackBetween(15, 17)
                    .Do(e => CheckDecision(heroHand, "SB_FacingMinRaise_MP_15_17bb_EXPL", _btnOpenSteal, PlMode.More));

                elements.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen)
                    .OppBetSizeMinRaise()
                    .EffectiveStackBetween(17, 19)
                    .Do(e => CheckDecision(heroHand, "SB_FacingMinRaise_MP_17_20bb_EXPl", _btnOpenSteal, PlMode.More));

            }


            elements.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(19, 100)
                .Do(e => CheckDecision(heroHand, "SB_VS_BTN_OPEN_MP_19-100bb", 0, PlMode.None));


            //BB
            //BB FACING LIMP HU
            //BB FACING LIMP COMMON HU

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingLimp).IsHU()
                .EffectiveStackBetween(0, 4)
                .Do(e => CheckDecision(heroHand, "BB_FacingLimp_HU_0_4bb", 0, PlMode.None));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingLimp).IsHU()
                .EffectiveStackBetween(4, 6)
                .Do(e => CheckDecision(heroHand, "BB_FacingLimp_HU_4_6bb", 0, PlMode.None));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingLimp).IsHU()
                .EffectiveStackBetween(6, 8)
                .Do(e => CheckDecision(heroHand, "BB_FacingLimp_HU_6_8bb", 0, PlMode.None));


            //BB FACING LIMP IP
            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingLimp).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .EffectiveStackBetween(8, 10)
                .Do(e => CheckDecision(heroHand, "BB_FacingLimp_HU_IP_8_10bb", 0, PlMode.None));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingLimp).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .EffectiveStackBetween(10, 13)
                .Do(e => CheckDecision(heroHand, "BB_FacingLimp_HU_IP_10_13bb", 0, PlMode.None));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingLimp).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .EffectiveStackBetween(13, 15)
                .Do(e => CheckDecision(heroHand, "BB_FacingLimp_HU_IP_13_15bb", 0, PlMode.None));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingLimp).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .EffectiveStackBetween(15, 100)
                .Do(e => CheckDecision(heroHand, "BB_FacingLimp_HU_IP_15bb+", 0, PlMode.None));

            //BB FACING LIMP OOP   

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingLimp).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(8, 10)
                .Do(e => CheckDecision(heroHand, "BB_FacingLimp_HU_OOP_8_10bb", 0, PlMode.None));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingLimp).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(10, 13)
                .Do(e => CheckDecision(heroHand, "BB_FacingLimp_HU_OOP_10_13bb", 0, PlMode.None));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingLimp).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(13, 16)
                .Do(e => CheckDecision(heroHand, "BB_FacingLimp_HU_OOP_13_16bb", 0, PlMode.None));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingLimp).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(16, 100)
                .Do(e => CheckDecision(heroHand, "BB_FacingLimp_HU_OOP_16bb+", 0, PlMode.None));


            double? openRaise = null;
            double? foldTo3bet = null;
            
            if (elements.HuOpp != null)
            {
                if (elements.HuOpp.Position == PlayerPosition.Button) 
                    openRaise = elements.HuOpp.Stats.PF_BTN_STEAL;
                else if (elements.HuOpp.Position == PlayerPosition.Sb)
                    openRaise = elements.HuOpp.Stats.PF_SB_OPENMINRAISE;
                //в хаде ХМ2 сейчас вместо фолд ту 3бет ИП , стоит фолд ту 3бет ТОТАЛ!!!
                foldTo3bet = elements.HuOpp.Stats.PF_FOLD_3BET_IP;

                //const double defaultOpenRaise = 72;
                // if (openRaise == null) openRaise = defaultOpenRaise;
                const double defaultFoldTo3Bet = 74;
                if (foldTo3bet == null) foldTo3bet = defaultFoldTo3Bet;
                if (elements.HuOpp.Stack >= 2*elements.HeroPlayer.Stack) foldTo3bet -= 5;

            }

            Foldto3Bet fto3bCategory = ChooseFoldTo3betCategory(foldTo3bet, elements);


            //BB VS BTN OPEN 3MAX
            if (openRaise == null) {
                //VS UNK
                elements.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSize(2)
                    .EffectiveStackBetween(0, 3)
                    .Do(e => CheckDecision(heroHand, "BB_VS_BTN_OPEN_0-3_SMALLSTEAL", 0, PlMode.None));

                elements.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSize(2)
                    .EffectiveStackBetween(3, 7)
                    .Do(e => CheckDecision(heroHand, "BB_VS_BTN_OPEN_3-7_SMALLSTEAL", 0, PlMode.None));

                elements.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSize(2)
                    .EffectiveStackBetween(7, 10)
                    .Do(e => CheckDecision(heroHand, "BB_VS_BTN_OPEN_7-10_SMALLSTEAL", 0, PlMode.None));

                elements.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSize(2)
                    .EffectiveStackBetween(10, 13)
                    .Do(e => CheckDecision(heroHand, "BB_VS_BTN_OPEN_10-13_BIGSTEAL", 0, PlMode.None));

                elements.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSizeMinRaise()
                    .EffectiveStackBetween(13, 16)
                    .Do(e => CheckDecision(heroHand, "BB_VS_BTN_OPEN_13-16_BIGSTEAL", 0, PlMode.None));

                elements.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSizeMinRaise()
                    .EffectiveStackBetween(16, 20)
                    .Do(e => CheckDecision(heroHand, "BB_VS_BTN_OPEN_16-20_BIGSTEAL", 0, PlMode.None));

                elements.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSizeMinRaise()
                    .EffectiveStackBetween(20, 100)
                    .Do(e => CheckDecision(heroHand, "BB_VS_BTN_OPEN_20-100_BIGSTEAL", 0, PlMode.None));


            }
            if (openRaise > 50) {
                //VS LOOSE OPEN
                elements.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSize(2)
                    .EffectiveStackBetween(0, 3)
                    .Do(e => CheckDecision(heroHand, "BB_VS_BTN_OPEN_0-3_BIGSTEAL", 0, PlMode.None));

                elements.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSize(2)
                    .EffectiveStackBetween(3, 7)
                    .Do(e => CheckDecision(heroHand, "BB_VS_BTN_OPEN_3-7_BIGSTEAL", 0, PlMode.None));

                elements.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSize(2)
                    .EffectiveStackBetween(7, 10)
                    .Do(e => CheckDecision(heroHand, "BB_VS_BTN_OPEN_7-10_BIGSTEAL", openRaise, PlMode.More));

                elements.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSize(2)
                    .EffectiveStackBetween(10, 13)
                    .Do(e => CheckDecision(heroHand, "BB_VS_BTN_OPEN_10-13_BIGSTEAL", openRaise, PlMode.More));

                elements.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSizeMinRaise()
                    .EffectiveStackBetween(13, 16)
                    .Do(e => CheckDecision(heroHand, "BB_VS_BTN_OPEN_13-16_BIGSTEAL", openRaise, PlMode.More));

                elements.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSizeMinRaise()
                    .EffectiveStackBetween(16, 20)
                    .Do(e => CheckDecision(heroHand, "BB_VS_BTN_OPEN_16-20_BIGSTEAL", openRaise, PlMode.More));

                elements.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSizeMinRaise()
                    .EffectiveStackBetween(20, 100)
                    .Do(e => CheckDecision(heroHand, "BB_VS_BTN_OPEN_20-100_BIGSTEAL", 0, PlMode.None));
            }
            else {
                //VS TIGHT OPEN
                elements.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSize(2)
                    .EffectiveStackBetween(0, 3)
                    .Do(e => CheckDecision(heroHand, "BB_VS_BTN_OPEN_0-3_SMALLSTEAL", 0, PlMode.None));

                elements.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSize(2)
                    .EffectiveStackBetween(3, 7)
                    .Do(e => CheckDecision(heroHand, "BB_VS_BTN_OPEN_3-7_SMALLSTEAL", 0, PlMode.None));

                elements.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSize(2)
                    .EffectiveStackBetween(7, 10)
                    .Do(e => CheckDecision(heroHand, "BB_VS_BTN_OPEN_7-10_SMALLSTEAL", 0, PlMode.None));

                elements.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSize(2)
                    .EffectiveStackBetween(10, 13)
                    .Do(e => CheckDecision(heroHand, "BB_VS_BTN_OPEN_10-13_SMALLSTEAL", 0, PlMode.None));

                elements.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSizeMinRaise()
                    .EffectiveStackBetween(13, 16)
                    .Do(e => CheckDecision(heroHand, "BB_VS_BTN_OPEN_13-16_SMALLSTEAL", 0, PlMode.None));

                elements.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSizeMinRaise()
                    .EffectiveStackBetween(16, 20)
                    .Do(e => CheckDecision(heroHand, "BB_VS_BTN_OPEN_16-20_SMALLSTEAL", 0, PlMode.None));

                elements.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSizeMinRaise()
                    .EffectiveStackBetween(20, 100)
                    .Do(e => CheckDecision(heroHand, "BB_VS_BTN_OPEN_20-100_SMALLSTEAL", 0, PlMode.None));
                
            }






            //BB VS SB OPEN MINR 3MAX

            if (openRaise == null) openRaise = 50;


            elements.StartRule().HeroPosition(PlayerPosition.Bb)
               .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
               .HeroRelativePosition(HeroRelativePosition.InPosition)
               .OppBetSize(2)
               .BBEqOrMoreThen(60)
               .EffectiveStackBetween(5,8)
               .Do(e => CheckDecision(heroHand, "BB_VS_SB_OPEN_5-8bb_3max_BIGBLINDS", 0, PlMode.None));

         

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(0, 8)
                .Do(e => CheckDecision(heroHand, "COMMON_PushFoldVsOpen_NASH", elements.EffectiveStack, PlMode.Less));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSize(2)
                .BBEqOrMoreThen(60)
                .EffectiveStackBetween(8,10)
                .Do(e => CheckDecision(heroHand, "BB_VS_SB_OPEN_8-10bb_3max_BIGBLINDS", 0, PlMode.None));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(8, 10)
                .Do(e => CheckDecision(heroHand, "BB_VS_SB_OPEN_8-10bb_3max_SMALLBLINDS", openRaise, PlMode.More));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(10, 13)
                .Do(e => CheckDecision(heroHand, "BB_VS_SB_OPEN_10-13bb_3max", openRaise, PlMode.More));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(10, 13)
                .Do(e => CheckDecision(heroHand, "BB_VS_SB_OPEN_10-13bb_3max", openRaise, PlMode.More));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(13, 16)
                .Do(e => CheckDecision(heroHand, "BB_VS_SB_OPEN_13-16bb_3max", openRaise, PlMode.More));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(16, 20)
                .Do(e => CheckDecision(heroHand, "BB_VS_SB_OPEN_16-20bb_3max", openRaise, PlMode.More));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(20, 100)
                .Do(e => CheckDecision(heroHand, "BB_VS_SB_OPEN_20-100bb_3max", openRaise, PlMode.More));
           

             
            //BB FACING MINRAISE OOP


            elements.StartRule().HeroPosition(PlayerPosition.Bb)
               .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
               .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
               .OppBetSizeMinRaise()
               .EffectiveStackBetween(17, 20)
               .Do(e => CheckDecision(heroHand, "BB_FacingMinRaise_HU_OOP_17_20bb", 0, PlMode.None));
            

            if (openRaise == null) {
                //3max 
                elements.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU().Is3Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSize(2)
                    .EffectiveStackBetween(8, 9.5)
                    .Do(e => CheckDecision(heroHand, "BB_FacingMinRaise_HU_OOP_3max_ 8_9.5_UNK", 0, PlMode.None));

                elements.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU().Is3Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSize(2)
                    .EffectiveStackBetween(9.5, 10.5)
                    .Do(e => CheckDecision(heroHand, "BB_FacingMinRaise_HU_OOP_3max_ 9.5_10.5_UNK", 0, PlMode.None));

                elements.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU().Is3Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSize(2)
                    .EffectiveStackBetween(10.5, 12)
                    .Do(e => CheckDecision(heroHand, "BB_FacingMinRaise_HU_OOP_3max_10.5_12_UNK", 0, PlMode.None));
                //2max

                elements.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSize(2)
                    .EffectiveStackBetween(8, 12)
                    .Do(e => CheckDecision(heroHand, "BB_FacingMinRaise_HU_OOP_2max_8-12_UNK", elements.EffectiveStack, PlMode.Less));

            }
            else {
                
                elements.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSize(2)
                    .EffectiveStackBetween(8, 9.5)
                    .Do(e => CheckDecision(heroHand, "BB_FacingMinRaise_HU_OOP_8_9.5bb_EXPL", openRaise, PlMode.More));

                elements.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSize(2)
                    .EffectiveStackBetween(9.5, 10.5)
                    .Do(e => CheckDecision(heroHand, "BB_FacingMinRaise_HU_OOP_9.5_10.5_EXPL", openRaise, PlMode.More));

                elements.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSize(2)
                    .EffectiveStackBetween(10.5, 12)
                    .Do(e => CheckDecision(heroHand, "BB_FacingMinRaise_HU_OOP_ 10.5_12_EXPL", openRaise, PlMode.More));
                
            }


            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(12, 13)
                .VsSmallStack()
                .Do(e => CheckDecision(heroHand, "BB_FacingMinRaise_HU_OOP_VSMALL_8_13bb", 0, PlMode.None));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(12, 13)
                .VsBigStack()
                .Do(e => CheckDecision(heroHand, "BB_FacingMinRaise_HU_OOP_VBIG_11_13bb", 0, PlMode.None));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(13, 15)
                .Do(e => CheckDecision(heroHand, "BB_FacingMinRaise_HU_OOP_13_15bb", 0, PlMode.None));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(15, 17)
                .Do(e => CheckDecision(heroHand, "BB_FacingMinRaise_HU_OOP_15_17bb", 0, PlMode.None));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(17, 20)
                .Do(e => CheckDecision(heroHand, "BB_FacingMinRaise_HU_OOP_17_20bb", 0, PlMode.None));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(20, 100)
                .Do(e => CheckDecision(heroHand, "BB_FacingMinRaise_HU_OOP_20bb+", 0, PlMode.None));

            //BB vs BTN CALL OPEN PUSH

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingPush).IsHU()
                .OppPosition(PlayerPosition.Button)
                .EffectiveStackBetween(0, 8)
                .BBEqOrLessThen(40)
                .Do(e => CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb", e.HuOpp.Stack, PlMode.Less));


            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingPush).IsHU()
                .OppPosition(PlayerPosition.Button)
                .EffectiveStackBetween(4, 100)
                .Do(e => CheckDecision(heroHand, "BB_VS_BTN_OPENPUSH_4-100bb_UNK", e.HuOpp.Stack, PlMode.Less));
            

            //BB VS SB CALL OPEN PUSH
            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingPush).IsHU()
                .OppPosition(PlayerPosition.Sb)
                .EffectiveStackBetween(9, 100)
                .Do(e => CheckDecision(heroHand, "BB_VS_SB_CALL_OPENPUSH_9-100bb_UNK", e.EffectiveStack, PlMode.Less));



            #endregion

            #region COMMON
            //COMMON POST
            elements.StartRule()
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                .EffectiveStackBetween(0, 8)
                .Do(e => CheckDecision(heroHand, "COMMON_PushFoldVsOpen_NASH", e.EffectiveStack , PlMode.Less));
                


            elements.StartRule()
               .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingPush).IsHU()
               .EffectiveStackBetween(0, 8)
               .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
               .Do(e => CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb", e.EffectiveStack , PlMode.Less));
               

            elements.StartRule()
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingPush).IsHU()
                .EffectiveStackBetween(8, 100)
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .Do(e => CheckDecision(heroHand, "COMMON_FacingPush_HU_8_25bb", e.EffectiveStack , PlMode.Less));


            elements.StartRule()
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingPush).IsHU()
                .EffectiveStackBetween(0, 11)
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .Do(e => CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb", e.EffectiveStack , PlMode.Less));
               
            //TODO переделать со статами!!
            elements.StartRule()
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingPush).IsHU()
                .EffectiveStackBetween(11, 100)
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .Do(e => CheckDecision(heroHand, "COMMON_FacingPush_HU_8_25bb", e.EffectiveStack , PlMode.Less));
            #endregion


            if (_isNewDecision == false)
            {
                _hudInfo.HandRangeStat = 0.0;
                _hudInfo.DecisionPreflop = DecisionPreflop.None;
            }

            if (NewHudInfo != null)
            {
                NewHudInfo(_hudInfo);
            }

        }

        private Foldto3Bet ChooseFoldTo3betCategory(double? foldTo3bet, Elements elements) {
            if(foldTo3bet == null ) return Foldto3Bet.None;
            if (elements.EffectiveStack > 15) {
                if(foldTo3bet >=77) return Foldto3Bet.Big;
                if(foldTo3bet >= 65) return Foldto3Bet.Average;
                return Foldto3Bet.Small;
            }
            else {
                if (foldTo3bet >= 70) return Foldto3Bet.Big;
                if (foldTo3bet >= 60) return Foldto3Bet.Average;
                return Foldto3Bet.Small;
            }
        }
    

     

        private double FindPotOdds(Elements elements)
        {
            var heroBet = elements.HeroPlayer.Bet;
            var opponents = elements.ActivePlayers.Where(p => p.Name != elements.HeroPlayer.Name);
            if (!opponents.Any()) return 0.0;
            var oppBet = opponents.Select(o => o.Bet).Concat(new double[] { 0 }).Max();
            var pot = elements.TotalPot;
            if (heroBet >= oppBet || oppBet == 0) return 0.0;

            return (oppBet - heroBet) / pot * 100;
        }

        private void CheckDecision(Hand heroHand , string rangeName, double stat, PlMode plMode) {
            if (_isNewDecision || heroHand == null) return;
            Range range = _rangesList.FirstOrDefault(r => r.Name == rangeName);

            if (NewRangeChosen != null) {
                
                NewRangeChosen(rangeName);
            }

            if (range != null) {
                if (heroHand.Name == "" || heroHand.Name.Length != 4) {
                    Debug.WriteLine("wrong heroHand Name");
                    return;
                }

                DecisionPreflop decision1= (DecisionPreflop)range.Hands.First(n => n.Name == heroHand.Name).D1;
                DecisionPreflop decision2 = (DecisionPreflop)range.Hands.First(n => n.Name == heroHand.Name).D2;

                double statRange = range.Hands.First(n => n.Name == heroHand.Name).S1;

                switch (plMode) {
                        case PlMode.None:
                        _hudInfo.DecisionPreflop = decision1;
                        break;
                    case PlMode.More:
                        _hudInfo.DecisionPreflop = stat >= statRange ? decision1 : decision2;
                        break;
                    case PlMode.Less:
                        _hudInfo.DecisionPreflop = stat <= statRange ? decision1 : decision2;
                        break;
                }
                _isNewDecision = true;
            }
            else {
                Debug.WriteLine(String.Format("can't find range {0}", rangeName));
            }
        }

        private void CheckDecision(Hand heroHand, string rangeName, double? stat, PlMode plMode)
        {
            if (_isNewDecision || heroHand == null) return;
            Range range = _rangesList.FirstOrDefault(r => r.Name == rangeName);

            if (NewRangeChosen != null)
            {

                NewRangeChosen(rangeName);
            }

            if (range != null)
            {
                if (heroHand.Name == "" || heroHand.Name.Length != 4)
                {
                    Debug.WriteLine("wrong heroHand Name");
                    return;
                }

                DecisionPreflop decision1 = (DecisionPreflop)range.Hands.First(n => n.Name == heroHand.Name).D1;
                DecisionPreflop decision2 = (DecisionPreflop)range.Hands.First(n => n.Name == heroHand.Name).D2;

                double statRange = range.Hands.First(n => n.Name == heroHand.Name).S1;

                switch (plMode)
                {
                    case PlMode.None:
                        _hudInfo.DecisionPreflop = decision1;
                        break;
                    case PlMode.More:
                        _hudInfo.DecisionPreflop = stat >= statRange ? decision1 : decision2;
                        break;
                    case PlMode.Less:
                        _hudInfo.DecisionPreflop = stat <= statRange ? decision1 : decision2;
                        break;
                }
                _isNewDecision = true;
            }
            else
            {
                Debug.WriteLine(String.Format("can't find range {0}", rangeName));
            }
        }

    }

}
