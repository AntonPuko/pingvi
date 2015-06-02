using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using PokerModel;

namespace Pingvi
{

    public enum PreflopDecision{
        None, Fold, Limp, OpenRaise, Call, _3Bet, Push
    }

    public class DecisionManager {

        private enum PlMode {
            None,More,Less
        };

        private enum Foldto3Bet {
            None, Small, Average, Big
        };
 

        private bool _isNewDecision;
        private const string RangesPath = @"Data\Ranges";
        private List<Range> _rangesList;
        private string _preflopRangeName;
        private PreflopDecision _preflopDecision;
        
        public event Action<DecisionInfo> NewDecisionInfo;

        public DecisionManager() {
            LoadRanges(RangesPath);
        }

        
     
        public void OnNewLineInfo(LineInfo lineInfo) {
            


                _isNewDecision = false;

                Hand heroHand = lineInfo.Elements.HeroPlayer.Hand;
            if (!lineInfo.Elements.HeroPlayer.IsHeroTurn) {
                _isNewDecision = true;
                _preflopDecision = PreflopDecision.None;
            }
                //COMMON
                lineInfo.StartRule().HeroPreflopState(HeroPreflopState.Open)
                    .SitOutOpp()
                    .Do(e => CheckDecision(heroHand, "COMMON_OPEN_100", 0, PlMode.None));

                #region BTN

                //BTN
                //BTN OPEN


                double? SB_3BET_VS_BTN = lineInfo.Elements.LeftPlayer.Stats.PF_SB_3BET_VS_BTN;
                double? BB_3BET_VS_BTN = lineInfo.Elements.RightPlayer.Stats.PF_BB_3BET_VS_BTN;

                const double SB_3BET_VS_BTN_default = 20;
                const double BB_3BET_VS_BTN_default = 20;
                if (SB_3BET_VS_BTN == null) SB_3BET_VS_BTN = SB_3BET_VS_BTN_default;
                if (BB_3BET_VS_BTN == null) BB_3BET_VS_BTN = BB_3BET_VS_BTN_default;

                double? merged3betVsBTN = SB_3BET_VS_BTN + BB_3BET_VS_BTN;

                lineInfo.StartRule().HeroPosition(PlayerPosition.Button)
                    .HeroPreflopState(HeroPreflopState.Open)
                    .HeroStackBetween(0, 4)
                    .Do(l => CheckDecision(heroHand, "BTN_OPEN_0-4bb", l.Elements.HeroPlayer.Stack, PlMode.Less));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Button)
                    .HeroPreflopState(HeroPreflopState.Open)
                    .HeroStackBetween(4, 6)
                    .Do(l => CheckDecision(heroHand, "BTN_OPEN_4-6bb", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Button)
                    .HeroPreflopState(HeroPreflopState.Open)
                    .HeroStackBetween(6, 8)
                    .Do(l => CheckDecision(heroHand, "BTN_OPEN_6-8bb", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Button)
                    .HeroPreflopState(HeroPreflopState.Open)
                    .HeroStackBetween(8, 13)
                    .Do(l => CheckDecision(heroHand, "BTN_OPEN_8-13bb", merged3betVsBTN, PlMode.More));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Button)
                    .HeroPreflopState(HeroPreflopState.Open)
                    .HeroStackBetween(13, 17)
                    .Do(l => CheckDecision(heroHand, "BTN_OPEN_13-17bb", merged3betVsBTN, PlMode.More));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Button)
                    .HeroPreflopState(HeroPreflopState.Open)
                    .HeroStackBetween(17, 100)
                    .Do(l => CheckDecision(heroHand, "BTN_OPEN_17-100bb", 0, PlMode.None));


                //BTN CALL PUSH AFTER OPEN
                //SMALL STAKES
                //TODO ДОДЕЛАТЬ ДИАПАЗОНЫ КОЛА В МЕЛКИХ СТЕКАХ, РАЗДЕЛИТЬ В ЗАВИСИМОСТИ ОТ РАЗМЕРА ББ
                lineInfo.StartRule().HeroPosition(PlayerPosition.Button).IsHU()
                    .HeroPreflopState(HeroPreflopState.FacingPushVsOpen)
                    .EffectiveStackBetween(0, 8)
                    .Do(
                        l =>
                            CheckDecision(heroHand, "BTN_CALLPUSH_VSOPEN_0-11bb_UNK", l.Elements.EffectiveStack,
                                PlMode.Less));
                //VS SB PUSH
                lineInfo.StartRule().HeroPosition(PlayerPosition.Button).IsHU()
                    .HeroPreflopState(HeroPreflopState.FacingPushVsOpen)
                    .EffectiveStackBetween(8, 100)
                    .OppPosition(PlayerPosition.Sb)
                    .Do(l => CheckDecision(heroHand, "btn_callpush_vs_SB", l.Elements.EffectiveStack, PlMode.Less));

                //VS BB PUSH
                lineInfo.StartRule().HeroPosition(PlayerPosition.Button).IsHU()
                    .HeroPreflopState(HeroPreflopState.FacingPushVsOpen)
                    .EffectiveStackBetween(8, 100)
                    .OppPosition(PlayerPosition.Bb)
                    .Do(l => CheckDecision(heroHand, "btn_callpush_vs_BB", l.Elements.EffectiveStack, PlMode.Less));








                #endregion

                #region SB

                //SB

                double? _3betStatBBvsSB = null;
                if (lineInfo.Elements.HuOpp != null) _3betStatBBvsSB = lineInfo.Elements.HuOpp.Stats.PF_BB_3BET_VS_SB;
                double? _DefStatBBvsSB = null;
                if (lineInfo.Elements.HuOpp != null)
                    _DefStatBBvsSB = lineInfo.Elements.HuOpp.Stats.PF_BB_DEF_VS_SBSTEAL;
                double? _FoldCBIP = null;
                if (lineInfo.Elements.HuOpp != null) _FoldCBIP = lineInfo.Elements.HuOpp.Stats.F_FOLD_CBET;

                //SB OPEN COMMON
                //vs BIG STACK
                lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroPreflopState(HeroPreflopState.Open)
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .EffectiveStackBetween(0, 7)
                    .VsBigStack()
                    .Do(l => CheckDecision(heroHand, "SB_OPENPUSH_CHEBUKOV", l.Elements.EffectiveStack, PlMode.Less));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroPreflopState(HeroPreflopState.Open)
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(0, 8)
                    .VsBigStack()
                    .Do(l => CheckDecision(heroHand, "SB_OPENPUSH_CHEBUKOV", l.Elements.EffectiveStack, PlMode.Less));

                //vs other
                lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroPreflopState(HeroPreflopState.Open)
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .EffectiveStackBetween(0, 7)
                    .Do(l => CheckDecision(heroHand, "SB_OPENPUSH_NASH", l.Elements.EffectiveStack, PlMode.Less));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroPreflopState(HeroPreflopState.Open)
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(0, 8)
                    .Do(l => CheckDecision(heroHand, "SB_OPENPUSH_NASH", l.Elements.EffectiveStack, PlMode.Less));

                //SB VS BB OPEN 2max HU

                lineInfo.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .HeroPreflopState(HeroPreflopState.Open)
                    .EffectiveStackBetween(7, 8)
                    .Do(l => CheckDecision(heroHand, "SB_VS_BB_OPEN_7-8bb_2max_UNK", 0, PlMode.None));

                lineInfo.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .HeroPreflopState(HeroPreflopState.Open)
                    .EffectiveStackBetween(8, 9)
                    .Do(l => CheckDecision(heroHand, "SB_VS_BB_OPEN_8-9bb_2max_UNK", 0, PlMode.None));


                if (_3betStatBBvsSB > 35 || (_DefStatBBvsSB > 65 && _FoldCBIP < 43)) {
                    //vs LAG
                    lineInfo.StartRule()
                        .HeroPosition(PlayerPosition.Sb)
                        .HeroRelativePosition(HeroRelativePosition.InPosition)
                        .HeroPreflopState(HeroPreflopState.Open)
                        .EffectiveStackBetween(9, 12)
                        .Do(l => CheckDecision(heroHand, "SB_VS_BB_OPEN_9-12bb_2max_AGR", 0, PlMode.None));

                    lineInfo.StartRule()
                        .HeroPosition(PlayerPosition.Sb)
                        .HeroRelativePosition(HeroRelativePosition.InPosition)
                        .HeroPreflopState(HeroPreflopState.Open)
                        .EffectiveStackBetween(12, 14)
                        .Do(l => CheckDecision(heroHand, "SB_VS_BB_OPEN_12-14bb_2max_AGR", 0, PlMode.None));

                    lineInfo.StartRule()
                        .HeroPosition(PlayerPosition.Sb)
                        .HeroRelativePosition(HeroRelativePosition.InPosition)
                        .HeroPreflopState(HeroPreflopState.Open)
                        .EffectiveStackBetween(14, 16)
                        .Do(l => CheckDecision(heroHand, "SB_VS_BB_OPEN_14-16bb_2max_AGR", 0, PlMode.None));

                    lineInfo.StartRule()
                        .HeroPosition(PlayerPosition.Sb)
                        .HeroRelativePosition(HeroRelativePosition.InPosition)
                        .HeroPreflopState(HeroPreflopState.Open)
                        .EffectiveStackBetween(16, 20)
                        .Do(l => CheckDecision(heroHand, "SB_VS_BB_OPEN_16-20bb_2max_AGR", 0, PlMode.None));

                    lineInfo.StartRule()
                        .HeroPosition(PlayerPosition.Sb)
                        .HeroRelativePosition(HeroRelativePosition.InPosition)
                        .HeroPreflopState(HeroPreflopState.Open)
                        .EffectiveStackBetween(20, 100)
                        .Do(l => CheckDecision(heroHand, "SB_VS_BB_OPEN_20-100bb_2max_AGR", 0, PlMode.None));
                }
                else {
                    //vs UNK
                    lineInfo.StartRule()
                        .HeroPosition(PlayerPosition.Sb)
                        .HeroRelativePosition(HeroRelativePosition.InPosition)
                        .HeroPreflopState(HeroPreflopState.Open)
                        .EffectiveStackBetween(9, 11)
                        .Do(l => CheckDecision(heroHand, "SB_VS_BB_OPEN_9-11bb_2max_UNK", 0, PlMode.None));

                    lineInfo.StartRule()
                        .HeroPosition(PlayerPosition.Sb)
                        .HeroRelativePosition(HeroRelativePosition.InPosition)
                        .HeroPreflopState(HeroPreflopState.Open)
                        .EffectiveStackBetween(11, 13)
                        .Do(l => CheckDecision(heroHand, "SB_VS_BB_OPEN_11-13bb_2max_UNK", 0, PlMode.None));

                    lineInfo.StartRule()
                        .HeroPosition(PlayerPosition.Sb)
                        .HeroRelativePosition(HeroRelativePosition.InPosition)
                        .HeroPreflopState(HeroPreflopState.Open)
                        .EffectiveStackBetween(13, 16)
                        .Do(l => CheckDecision(heroHand, "SB_VS_BB_OPEN_13-16bb_2max_UNK", 0, PlMode.None));

                    lineInfo.StartRule()
                        .HeroPosition(PlayerPosition.Sb)
                        .HeroRelativePosition(HeroRelativePosition.InPosition)
                        .HeroPreflopState(HeroPreflopState.Open)
                        .EffectiveStackBetween(16, 20)
                        .Do(l => CheckDecision(heroHand, "SB_VS_BB_OPEN_16-20bb_2max_UNK", 0, PlMode.None));

                    lineInfo.StartRule()
                        .HeroPosition(PlayerPosition.Sb)
                        .HeroRelativePosition(HeroRelativePosition.InPosition)
                        .HeroPreflopState(HeroPreflopState.Open)
                        .EffectiveStackBetween(20, 100)
                        .Do(l => CheckDecision(heroHand, "SB_VS_BB_OPEN_20-100bb_2max_UNK", 0, PlMode.None));

                }


                //SB OPEN VS BB 3MAX
                if (_3betStatBBvsSB == null || _3betStatBBvsSB < 35) {
                    //VS UNK
                    lineInfo.StartRule()
                        .HeroPosition(PlayerPosition.Sb)
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .HeroPreflopState(HeroPreflopState.Open)
                        .EffectiveStackBetween(8, 10)
                        .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_8-10bb_UNK", 0, PlMode.None));

                    lineInfo.StartRule()
                        .HeroPosition(PlayerPosition.Sb)
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .HeroPreflopState(HeroPreflopState.Open)
                        .EffectiveStackBetween(10, 13)
                        .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_10-13bb_UNK", 0, PlMode.None));

                    lineInfo.StartRule()
                        .HeroPosition(PlayerPosition.Sb)
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .HeroPreflopState(HeroPreflopState.Open)
                        .EffectiveStackBetween(13, 15)
                        .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_13-15bb_UNK", 0, PlMode.None));

                    lineInfo.StartRule()
                        .HeroPosition(PlayerPosition.Sb)
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .HeroPreflopState(HeroPreflopState.Open)
                        .EffectiveStackBetween(15, 20)
                        .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_15-20bb_UNK", 0, PlMode.None));

                    lineInfo.StartRule()
                        .HeroPosition(PlayerPosition.Sb)
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .HeroPreflopState(HeroPreflopState.Open)
                        .EffectiveStackBetween(20, 100)
                        .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_20-100bb_UNK", 0, PlMode.None));
                }
                else {
                    //VS LAG
                    lineInfo.StartRule()
                        .HeroPosition(PlayerPosition.Sb)
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .HeroPreflopState(HeroPreflopState.Open)
                        .EffectiveStackBetween(8, 10)
                        .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_8-10bb_AGR", 0, PlMode.None));

                    lineInfo.StartRule()
                        .HeroPosition(PlayerPosition.Sb)
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .HeroPreflopState(HeroPreflopState.Open)
                        .EffectiveStackBetween(10, 13)
                        .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_10-13bb_AGR", 0, PlMode.None));

                    lineInfo.StartRule()
                        .HeroPosition(PlayerPosition.Sb)
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .HeroPreflopState(HeroPreflopState.Open)
                        .EffectiveStackBetween(13, 15)
                        .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_13-15bb_AGR", 0, PlMode.None));

                    lineInfo.StartRule()
                        .HeroPosition(PlayerPosition.Sb)
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .HeroPreflopState(HeroPreflopState.Open)
                        .EffectiveStackBetween(15, 20)
                        .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_15-20bb_AGR", 0, PlMode.None));

                    lineInfo.StartRule()
                        .HeroPosition(PlayerPosition.Sb)
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .HeroPreflopState(HeroPreflopState.Open)
                        .EffectiveStackBetween(20, 100)
                        .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_20-100bb_AGR", 0, PlMode.None));
                }

                //SB CALL PUSH AFTER LIMP

                lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroPreflopState(HeroPreflopState.FacingPushVsLimp)
                    .EffectiveStackBetween(0, 9)
                    .Do(
                        l =>
                            CheckDecision(heroHand, "SB_CALLPUSH_VSLIMP_7_9bb_UNK", l.Elements.EffectiveStack,
                                PlMode.Less));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroPreflopState(HeroPreflopState.FacingPushVsLimp)
                    .EffectiveStackBetween(9, 12)
                    .Do(
                        l =>
                            CheckDecision(heroHand, "SB_CALLPUSH_VSLIMP_9-12bb_UNK", l.Elements.EffectiveStack,
                                PlMode.Less));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroPreflopState(HeroPreflopState.FacingPushVsLimp)
                    .EffectiveStackBetween(12, 14)
                    .Do(l => CheckDecision(heroHand, "SB_CALLPUSH_VSLIMP_12-14bb_UNK", null, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroPreflopState(HeroPreflopState.FacingPushVsLimp)
                    .EffectiveStackBetween(14, 16)
                    .Do(l => CheckDecision(heroHand, "SB_CALLPUSH_VSLIMP_14-16bb_UNK", null, PlMode.None));

                //SB DEF VS RAISE AFTER LIMP
                lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroPreflopState(HeroPreflopState.FacingISOvsLimp)
                    .OppBetSizeBetween(2, 3)
                    .EffectiveStackBetween(7, 9)
                    .Do(l => CheckDecision(heroHand, "SB_DefVSMinRaise_AfterLIMP_IP_7-9bb_UNK", null, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroPreflopState(HeroPreflopState.FacingISOvsLimp)
                    .OppBetSize(2)
                    .EffectiveStackBetween(9, 12)
                    .Do(l => CheckDecision(heroHand, "SB_DefVSMinRaise_AfterLIMP_IP_9-12bb_UNK", null, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroPreflopState(HeroPreflopState.FacingISOvsLimp)
                    .EffectiveStackBetween(12, 14)
                    .OppBetSizeMinRaise()
                    .Do(l => CheckDecision(heroHand, "SB_DefVSMinRaise_AfterLIMP_IP_12-14bb_UNK", null, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroPreflopState(HeroPreflopState.FacingISOvsLimp)
                    .EffectiveStackBetween(14, 16)
                    .OppBetSizeMinRaise()
                    .Do(l => CheckDecision(heroHand, "SB_DefVSMinRaise_AfterLIMP_IP_14-16bb_UNK", null, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroPreflopState(HeroPreflopState.FacingISOvsLimp)
                    .OppBetSizeBetween(2, 3)
                    .Do(l => CheckDecision(heroHand, "SB_DefVS3xR_AfterLIMP_IP_9-12bb_UNK", null, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroPreflopState(HeroPreflopState.FacingISOvsLimp)
                    .EffectiveStackBetween(12, 14)
                    .OppBetSizeBetween(2.5, 3)
                    .Do(l => CheckDecision(heroHand, "SB_DefVS3xR_AfterLIMP_IP_12-14bb_UNK", null, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroPreflopState(HeroPreflopState.FacingISOvsLimp)
                    .EffectiveStackBetween(14, 16)
                    .OppBetSizeBetween(2.5, 3)
                    .Do(l => CheckDecision(heroHand, "SB_DefVS3xR_AfterLIMP_IP_14-16bb_UNK", null, PlMode.None));

                //SB CALL PUSH AFTER OPEN
                //SB CALL PUSH AFTER VS UNK
                if (_3betStatBBvsSB == null) {
                    lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                        .HeroPreflopState(HeroPreflopState.FacingPushVsOpen)
                        .EffectiveStackBetween(0, 9)
                        .Do(
                            l =>
                                CheckDecision(heroHand, "SB_CALLPUSH_VSOPEN_0-11bb_UNK", l.Elements.EffectiveStack,
                                    PlMode.Less));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                        .HeroPreflopState(HeroPreflopState.FacingPushVsOpen)
                        .EffectiveStackBetween(9, 100)
                        .Do(
                            l =>
                                CheckDecision(heroHand, "SB_CALLPUSH_AFTEROPEN_9-100_UNK_NEW", l.Elements.EffectiveStack,
                                    PlMode.Less));

                }
                else {
                    lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                        .HeroPreflopState(HeroPreflopState.FacingPushVsOpen)
                        .EffectiveStackBetween(0, 9)
                        .Do(
                            l =>
                                CheckDecision(heroHand, "SB_CALLPUSH_VSOPEN_0-11bb_UNK", l.Elements.EffectiveStack,
                                    PlMode.Less));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                        .HeroPreflopState(HeroPreflopState.FacingPushVsOpen)
                        .EffectiveStackBetween(9, 13)
                        .Do(
                            l =>
                                CheckDecision(heroHand, "SB_CALLPuSH_VSOPEN_10-13bb_EXPL", _3betStatBBvsSB, PlMode.More));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                        .HeroPreflopState(HeroPreflopState.FacingPushVsOpen)
                        .EffectiveStackBetween(13, 20)
                        .Do(
                            l =>
                                CheckDecision(heroHand, "SB_CALLPuSH_VSOPEN_13-20bb_EXPL", _3betStatBBvsSB, PlMode.More));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                        .HeroPreflopState(HeroPreflopState.FacingPushVsOpen)
                        .EffectiveStackBetween(20, 100)
                        .Do(l => CheckDecision(heroHand, "SB_CALLPuSH_VSOPEN_20bb+_EXPL", _3betStatBBvsSB, PlMode.More));
                }

                //SB DEFEND VS BTN OPEN
                double? _btnOpenSteal = null;
                var buttonOpener =
                    lineInfo.Elements.ActivePlayers.FirstOrDefault(p => p.Position == PlayerPosition.Button);
                if (buttonOpener != null) _btnOpenSteal = buttonOpener.Stats.PF_BTN_STEAL;


                lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroPreflopState(HeroPreflopState.FacingOpen)
                    .OppBetSizeMinRaise()
                    .EffectiveStackBetween(8, 12)
                    .Do(l => CheckDecision(heroHand, "SB_FacingMinRaise_MP_8_12bb", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroPreflopState(HeroPreflopState.FacingOpen)
                    .OppBetSizeMinRaise()
                    .EffectiveStackBetween(12, 15)
                    .Do(l => CheckDecision(heroHand, "SB_FacingMinRaise_MP_12_15bb", 0, PlMode.None));

                if (_btnOpenSteal == null) {
                    lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                        .HeroPreflopState(HeroPreflopState.FacingOpen)
                        .OppBetSizeMinRaise()
                        .EffectiveStackBetween(15, 17)
                        .Do(l => CheckDecision(heroHand, "SB_FacingMinRaise_MP_15_17bb_UNK", 0, PlMode.None));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                        .HeroPreflopState(HeroPreflopState.FacingOpen)
                        .OppBetSizeMinRaise()
                        .EffectiveStackBetween(17, 19)
                        .Do(l => CheckDecision(heroHand, "SB_FacingMinRaise_MP_17_20bb_UNK", 0, PlMode.None));

                }
                else {
                    lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                        .HeroPreflopState(HeroPreflopState.FacingOpen)
                        .OppBetSizeMinRaise()
                        .EffectiveStackBetween(15, 17)
                        .Do(
                            l =>
                                CheckDecision(heroHand, "SB_FacingMinRaise_MP_15_17bb_EXPL", _btnOpenSteal, PlMode.More));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                        .HeroPreflopState(HeroPreflopState.FacingOpen)
                        .OppBetSizeMinRaise()
                        .EffectiveStackBetween(17, 19)
                        .Do(
                            l =>
                                CheckDecision(heroHand, "SB_FacingMinRaise_MP_17_20bb_EXPl", _btnOpenSteal, PlMode.More));

                }


                lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroPreflopState(HeroPreflopState.FacingOpen)
                    .OppBetSizeMinRaise()
                    .EffectiveStackBetween(19, 100)
                    .Do(l => CheckDecision(heroHand, "SB_VS_BTN_OPEN_MP_19-100bb", 0, PlMode.None));

                //SB ISO VS BTN LIMP
                lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .EffectiveStackSbVsBtnBetween(20, 100)
                    .Do(l => CheckDecision(heroHand, "SB_ISO_VS_BTN_LIMP_20-100bb_UNK", 0, PlMode.None));


                lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .EffectiveStackSbVsBtnBetween(16, 20)
                    .Do(l => CheckDecision(heroHand, "SB_ISO_VS_BTN_LIMP_16-20bb_UNK", 0, PlMode.None));



                #endregion

                #region BB

                //BB
                //BB FACING LIMP 2MAX HU
                double? limpfold = null;
                if (lineInfo.Elements.HuOpp != null) limpfold = lineInfo.Elements.HuOpp.Stats.PF_LIMP_FOLD;

                if (limpfold > 78) {
                    //VS BIG LIMP-FOLD
                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                        .HeroPreflopState(HeroPreflopState.FacingLimp)
                        .Is2Max()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .EffectiveStackBetween(6, 8)
                        .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_6-8bb_2max_BIGFOLD", 0, PlMode.None));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                        .HeroPreflopState(HeroPreflopState.FacingLimp)
                        .Is2Max()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .EffectiveStackBetween(8, 10)
                        .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_8-10bb_2max_BIGFOLD", 0, PlMode.None));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                        .HeroPreflopState(HeroPreflopState.FacingLimp)
                        .Is2Max()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .EffectiveStackBetween(10, 13)
                        .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_10-13bb_2max_BIGFOLD", 0, PlMode.None));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                        .HeroPreflopState(HeroPreflopState.FacingLimp)
                        .Is2Max()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .EffectiveStackBetween(13, 16)
                        .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_13-16bb_2max_BIGFOLD", 0, PlMode.None));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                        .HeroPreflopState(HeroPreflopState.FacingLimp)
                        .Is2Max()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .EffectiveStackBetween(16, 20)
                        .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_16-20bb_2max_BIGFOLD", 0, PlMode.None));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                        .HeroPreflopState(HeroPreflopState.FacingLimp)
                        .Is2Max()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .EffectiveStackBetween(20, 100)
                        .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_16-20bb_2max_BIGFOLD", 0, PlMode.None));

                }
                else if (limpfold < 50) {
                    //vs small limp fold
                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                        .HeroPreflopState(HeroPreflopState.FacingLimp)
                        .Is2Max()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .EffectiveStackBetween(6, 8)
                        .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_6-8bb_2max_SMALLFOLD", 0, PlMode.None));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                        .HeroPreflopState(HeroPreflopState.FacingLimp)
                        .Is2Max()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .EffectiveStackBetween(8, 10)
                        .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_8-10bb_2max_SMALLFOLD", 0, PlMode.None));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                        .HeroPreflopState(HeroPreflopState.FacingLimp)
                        .Is2Max()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .EffectiveStackBetween(10, 13)
                        .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_10-13bb_2max_SMALLFOLD", 0, PlMode.None));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                        .HeroPreflopState(HeroPreflopState.FacingLimp)
                        .Is2Max()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .EffectiveStackBetween(13, 16)
                        .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_13-16bb_2max_UNK", 0, PlMode.None));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                        .HeroPreflopState(HeroPreflopState.FacingLimp)
                        .Is2Max()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .EffectiveStackBetween(16, 20)
                        .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_16-20bb_2max_UNK", 0, PlMode.None));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                        .HeroPreflopState(HeroPreflopState.FacingLimp)
                        .Is2Max()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .EffectiveStackBetween(20, 100)
                        .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_16-20bb_2max_UNK", 0, PlMode.None));

                }
                else {
                    //vs UNK
                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                        .HeroPreflopState(HeroPreflopState.FacingLimp)
                        .Is2Max()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .EffectiveStackBetween(6, 8)
                        .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_6-8bb_2max_UNK", 0, PlMode.None));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                        .HeroPreflopState(HeroPreflopState.FacingLimp)
                        .Is2Max()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .EffectiveStackBetween(8, 10)
                        .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_8-10bb_2max_UNK", 0, PlMode.None));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                        .HeroPreflopState(HeroPreflopState.FacingLimp)
                        .Is2Max()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .EffectiveStackBetween(10, 13)
                        .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_10-13bb_2max_UNK", 0, PlMode.None));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                        .HeroPreflopState(HeroPreflopState.FacingLimp)
                        .Is2Max()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .EffectiveStackBetween(13, 16)
                        .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_13-16bb_2max_UNK", 0, PlMode.None));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                        .HeroPreflopState(HeroPreflopState.FacingLimp)
                        .Is2Max()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .EffectiveStackBetween(16, 20)
                        .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_16-20bb_2max_UNK", 0, PlMode.None));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                        .HeroPreflopState(HeroPreflopState.FacingLimp)
                        .Is2Max()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .EffectiveStackBetween(20, 100)
                        .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_16-20bb_2max_UNK", 0, PlMode.None));
                }

                //BB FACING LIMP COMMON HU

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .IsHU()
                    .EffectiveStackBetween(0, 4)
                    .Do(l => CheckDecision(heroHand, "BB_FacingLimp_HU_0_4bb", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .IsHU()
                    .EffectiveStackBetween(4, 6)
                    .Do(l => CheckDecision(heroHand, "BB_FacingLimp_HU_4_6bb", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .IsHU()
                    .EffectiveStackBetween(6, 8)
                    .Do(l => CheckDecision(heroHand, "BB_FacingLimp_HU_6_8bb", 0, PlMode.None));


                //BB FACING LIMP IP
                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .IsHU()
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .EffectiveStackBetween(8, 10)
                    .Do(l => CheckDecision(heroHand, "BB_FacingLimp_HU_IP_8_10bb", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .IsHU()
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .EffectiveStackBetween(10, 13)
                    .Do(l => CheckDecision(heroHand, "BB_FacingLimp_HU_IP_10_13bb", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .IsHU()
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .EffectiveStackBetween(13, 15)
                    .Do(l => CheckDecision(heroHand, "BB_FacingLimp_HU_IP_13_15bb", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .IsHU()
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .EffectiveStackBetween(15, 100)
                    .Do(l => CheckDecision(heroHand, "BB_FacingLimp_HU_IP_15bb+", 0, PlMode.None));

                double? openRaise = null;
                double? foldTo3bet = null;

                if (lineInfo.Elements.HuOpp != null) {
                    if (lineInfo.Elements.HuOpp.Position == PlayerPosition.Button)
                        openRaise = lineInfo.Elements.HuOpp.Stats.PF_BTN_STEAL;
                    else if (lineInfo.Elements.HuOpp.Position == PlayerPosition.Sb)
                        openRaise = lineInfo.Elements.HuOpp.Stats.PF_SB_OPENMINRAISE;
                    foldTo3bet = lineInfo.Elements.HuOpp.Stats.PF_FOLD_3BET;


                    const double defaultFoldTo3Bet = 65;
                    if (foldTo3bet == null) foldTo3bet = defaultFoldTo3Bet;
                    if (lineInfo.Elements.HuOpp.Stack >= 2*lineInfo.Elements.HeroPlayer.Stack) foldTo3bet -= 5;

                }




                //BB VS BTN OPEN 3MAX
                if (openRaise == null) {
                    //VS UNK
                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                        .HeroPreflopState(HeroPreflopState.FacingOpen)
                        .IsHU()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .OppBetSize(2)
                        .EffectiveStackBetween(0, 3)
                        .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_0-3_SMALLSTEAL", 0, PlMode.None));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                        .HeroPreflopState(HeroPreflopState.FacingOpen)
                        .IsHU()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .OppBetSize(2)
                        .EffectiveStackBetween(3, 7)
                        .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_3-7_SMALLSTEAL", 0, PlMode.None));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                        .HeroPreflopState(HeroPreflopState.FacingOpen)
                        .IsHU()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .OppBetSize(2)
                        .EffectiveStackBetween(7, 10)
                        .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_7-10_SMALLSTEAL", 0, PlMode.None));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                        .HeroPreflopState(HeroPreflopState.FacingOpen)
                        .IsHU()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .OppBetSize(2)
                        .EffectiveStackBetween(10, 13)
                        .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_10-13_BIGSTEAL", 0, PlMode.None));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                        .HeroPreflopState(HeroPreflopState.FacingOpen)
                        .IsHU()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .OppBetSizeMinRaise()
                        .EffectiveStackBetween(13, 16)
                        .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_13-16_BIGSTEAL", 0, PlMode.None));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                        .HeroPreflopState(HeroPreflopState.FacingOpen)
                        .IsHU()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .OppBetSizeMinRaise()
                        .EffectiveStackBetween(16, 20)
                        .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_16-20_BIGSTEAL", 0, PlMode.None));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                        .HeroPreflopState(HeroPreflopState.FacingOpen)
                        .IsHU()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .OppBetSizeMinRaise()
                        .EffectiveStackBetween(20, 100)
                        .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_20-100_BIGSTEAL", 0, PlMode.None));


                }
                if (openRaise > 50) {
                    //VS LOOSE OPEN
                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                        .HeroPreflopState(HeroPreflopState.FacingOpen)
                        .IsHU()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .OppBetSize(2)
                        .EffectiveStackBetween(0, 3)
                        .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_0-3_BIGSTEAL", 0, PlMode.None));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                        .HeroPreflopState(HeroPreflopState.FacingOpen)
                        .IsHU()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .OppBetSize(2)
                        .EffectiveStackBetween(3, 7)
                        .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_3-7_BIGSTEAL", 0, PlMode.None));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                        .HeroPreflopState(HeroPreflopState.FacingOpen)
                        .IsHU()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .OppBetSize(2)
                        .EffectiveStackBetween(7, 10)
                        .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_7-10_BIGSTEAL", openRaise, PlMode.More));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                        .HeroPreflopState(HeroPreflopState.FacingOpen)
                        .IsHU()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .OppBetSize(2)
                        .EffectiveStackBetween(10, 13)
                        .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_10-13_BIGSTEAL", openRaise, PlMode.More));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                        .HeroPreflopState(HeroPreflopState.FacingOpen)
                        .IsHU()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .OppBetSizeMinRaise()
                        .EffectiveStackBetween(13, 16)
                        .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_13-16_BIGSTEAL", openRaise, PlMode.More));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                        .HeroPreflopState(HeroPreflopState.FacingOpen)
                        .IsHU()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .OppBetSizeMinRaise()
                        .EffectiveStackBetween(16, 20)
                        .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_16-20_BIGSTEAL", openRaise, PlMode.More));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                        .HeroPreflopState(HeroPreflopState.FacingOpen)
                        .IsHU()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .OppBetSizeMinRaise()
                        .EffectiveStackBetween(20, 100)
                        .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_20-100_BIGSTEAL", 0, PlMode.None));
                }
                else {
                    //VS TIGHT OPEN
                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                        .HeroPreflopState(HeroPreflopState.FacingOpen)
                        .IsHU()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .OppBetSize(2)
                        .EffectiveStackBetween(0, 3)
                        .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_0-3_SMALLSTEAL", 0, PlMode.None));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                        .HeroPreflopState(HeroPreflopState.FacingOpen)
                        .IsHU()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .OppBetSize(2)
                        .EffectiveStackBetween(3, 7)
                        .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_3-7_SMALLSTEAL", 0, PlMode.None));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                        .HeroPreflopState(HeroPreflopState.FacingOpen)
                        .IsHU()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .OppBetSize(2)
                        .EffectiveStackBetween(7, 10)
                        .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_7-10_SMALLSTEAL", 0, PlMode.None));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                        .HeroPreflopState(HeroPreflopState.FacingOpen)
                        .IsHU()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .OppBetSize(2)
                        .EffectiveStackBetween(10, 13)
                        .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_10-13_SMALLSTEAL", 0, PlMode.None));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                        .HeroPreflopState(HeroPreflopState.FacingOpen)
                        .IsHU()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .OppBetSizeMinRaise()
                        .EffectiveStackBetween(13, 16)
                        .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_13-16_SMALLSTEAL", 0, PlMode.None));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                        .HeroPreflopState(HeroPreflopState.FacingOpen)
                        .IsHU()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .OppBetSizeMinRaise()
                        .EffectiveStackBetween(16, 20)
                        .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_16-20_SMALLSTEAL", 0, PlMode.None));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                        .HeroPreflopState(HeroPreflopState.FacingOpen)
                        .IsHU()
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .OppBetSizeMinRaise()
                        .EffectiveStackBetween(20, 100)
                        .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_20-100_SMALLSTEAL", 0, PlMode.None));

                }






                //BB VS SB OPEN MINR 3MAX

                if (openRaise == null) openRaise = 50;


                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingOpen)
                    .IsHU()
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .OppBetSize(2)
                    .BBEqOrMoreThen(60)
                    .EffectiveStackBetween(5, 8)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_5-8bb_3max_BIGBLINDS", 0, PlMode.None));



                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingOpen)
                    .IsHU()
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .OppBetSize(2)
                    .EffectiveStackBetween(0, 8)
                    .Do(
                        l =>
                            CheckDecision(heroHand, "COMMON_PushFoldVsOpen_NASH", l.Elements.EffectiveStack, PlMode.Less));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingOpen)
                    .IsHU()
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .OppBetSize(2)
                    .BBEqOrMoreThen(60)
                    .EffectiveStackBetween(8, 10)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_8-10bb_3max_BIGBLINDS", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingOpen)
                    .IsHU()
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .OppBetSize(2)
                    .EffectiveStackBetween(8, 10)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_8-10bb_3max_SMALLBLINDS", openRaise, PlMode.More));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingOpen)
                    .IsHU()
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .OppBetSize(2)
                    .EffectiveStackBetween(10, 13)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_10-13bb_3max", openRaise, PlMode.More));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingOpen)
                    .IsHU()
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .OppBetSize(2)
                    .EffectiveStackBetween(10, 13)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_10-13bb_3max", openRaise, PlMode.More));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingOpen)
                    .IsHU()
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .OppBetSizeMinRaise()
                    .EffectiveStackBetween(13, 16)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_13-16bb_3max", openRaise, PlMode.More));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingOpen)
                    .IsHU()
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .OppBetSizeMinRaise()
                    .EffectiveStackBetween(16, 20)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_16-20bb_3max", openRaise, PlMode.More));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingOpen)
                    .IsHU()
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .OppBetSizeMinRaise()
                    .EffectiveStackBetween(20, 100)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_20-100bb_3max", openRaise, PlMode.More));

                //BB VS SB OPEN BIG 3max

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingOpen)
                    .IsHU()
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .OppBetSizeBetween(2, 3)
                    .EffectiveStackBetween(8, 10)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_BIG_8-10bb_3max", null, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingOpen)
                    .IsHU()
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .OppBetSizeBetween(2, 3)
                    .EffectiveStackBetween(10, 13)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_BIG_10-13bb_3max", null, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingOpen)
                    .IsHU()
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .OppBetSizeBetween(2.5, 3)
                    .EffectiveStackBetween(13, 16)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_BIG_13-16bb_3max", null, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingOpen)
                    .IsHU()
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .OppBetSizeBetween(2.5, 4)
                    .EffectiveStackBetween(16, 20)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_BIG_16-20bb_3max", null, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingOpen)
                    .IsHU()
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .OppBetSizeBetween(2.5, 4)
                    .EffectiveStackBetween(20, 100)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_BIG_20-100bb_3max", null, PlMode.None));




                //BB VS SB OPEN MINR 2max

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
                    .HeroPreflopState(HeroPreflopState.FacingOpen)
                    .IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSize(2)
                    .EffectiveStackBetween(0, 8)
                    .Do(
                        l =>
                            CheckDecision(heroHand, "COMMON_PushFoldVsOpen_NASH", l.Elements.EffectiveStack, PlMode.Less));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
                    .HeroPreflopState(HeroPreflopState.FacingOpen)
                    .IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSize(2)
                    .EffectiveStackBetween(8, 10)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_8-10bb_2max", foldTo3bet, PlMode.More));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
                    .HeroPreflopState(HeroPreflopState.FacingOpen)
                    .IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSize(2)
                    .EffectiveStackBetween(10, 13)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_10-13bb_2max", foldTo3bet, PlMode.More));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
                    .HeroPreflopState(HeroPreflopState.FacingOpen)
                    .IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSizeMinRaise()
                    .EffectiveStackBetween(13, 16)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_13-16bb_2max", foldTo3bet, PlMode.More));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
                    .HeroPreflopState(HeroPreflopState.FacingOpen)
                    .IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSizeMinRaise()
                    .EffectiveStackBetween(16, 20)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_16-20bb_2max", openRaise, PlMode.More));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
                    .HeroPreflopState(HeroPreflopState.FacingOpen)
                    .IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSizeMinRaise()
                    .EffectiveStackBetween(20, 100)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_20-100bb_2max", 0, PlMode.None));

                //BB VS SB OPEN BIG 2max

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
                    .HeroPreflopState(HeroPreflopState.FacingOpen)
                    .IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSizeBetween(2, 3)
                    .EffectiveStackBetween(8, 10)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_BIG_8-10bb_2max", null, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
                    .HeroPreflopState(HeroPreflopState.FacingOpen)
                    .IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSizeBetween(2, 3)
                    .EffectiveStackBetween(10, 13)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_BIG_10-13bb_2max", null, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
                    .HeroPreflopState(HeroPreflopState.FacingOpen)
                    .IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSizeBetween(2.5, 3)
                    .EffectiveStackBetween(13, 16)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_BIG_13-16bb_2max", null, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
                    .HeroPreflopState(HeroPreflopState.FacingOpen)
                    .IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSizeBetween(2.5, 4)
                    .EffectiveStackBetween(16, 20)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_BIG_16-20bb_2max", null, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
                    .HeroPreflopState(HeroPreflopState.FacingOpen)
                    .IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSizeBetween(2.5, 4)
                    .EffectiveStackBetween(20, 100)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_BIG_20-100bb_2max", null, PlMode.None));




                //BB vs BTN CALL OPEN PUSH

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                    .IsHU()
                    .OppPosition(PlayerPosition.Button)
                    .EffectiveStackBetween(0, 8)
                    .BBEqOrLessThen(40)
                    .Do(l => CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb", l.Elements.HuOpp.Stack, PlMode.Less));


                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                    .IsHU()
                    .OppPosition(PlayerPosition.Button)
                    .EffectiveStackBetween(4, 100)
                    .Do(
                        l =>
                            CheckDecision(heroHand, "BB_VS_BTN_OPENPUSH_4-100bb_UNK", l.Elements.HuOpp.Stack,
                                PlMode.Less));


                //BB VS SB CALL OPEN PUSH
                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                    .IsHU()
                    .OppPosition(PlayerPosition.Sb)
                    .EffectiveStackBetween(9, 100)
                    .Do(
                        l =>
                            CheckDecision(heroHand, "BB_VS_SB_CALL_OPENPUSH_9-100bb_UNK", l.Elements.EffectiveStack,
                                PlMode.Less));



                #endregion

                #region COMMON

                //COMMON POST
                lineInfo.StartRule()
                    .HeroPreflopState(HeroPreflopState.FacingOpen)
                    .IsHU()
                    .EffectiveStackBetween(0, 8)
                    .Do(
                        l =>
                            CheckDecision(heroHand, "COMMON_PushFoldVsOpen_NASH", l.Elements.EffectiveStack, PlMode.Less));



                lineInfo.StartRule()
                    .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                    .IsHU()
                    .EffectiveStackBetween(0, 8)
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .Do(
                        l =>
                            CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb", l.Elements.EffectiveStack, PlMode.Less));


                lineInfo.StartRule()
                    .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                    .IsHU()
                    .EffectiveStackBetween(8, 100)
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .Do(
                        l =>
                            CheckDecision(heroHand, "COMMON_FacingPush_HU_8_25bb", l.Elements.EffectiveStack,
                                PlMode.Less));


                lineInfo.StartRule()
                    .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                    .IsHU()
                    .EffectiveStackBetween(0, 11)
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .Do(
                        l =>
                            CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb", l.Elements.EffectiveStack, PlMode.Less));

                //TODO переделать со статами!!
                lineInfo.StartRule()
                    .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                    .IsHU()
                    .EffectiveStackBetween(11, 100)
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .Do(
                        l =>
                            CheckDecision(heroHand, "COMMON_FacingPush_HU_8_25bb", l.Elements.EffectiveStack,
                                PlMode.Less));

                #endregion


                if (_isNewDecision == false) {
                    _preflopDecision = PreflopDecision.None;
                    _preflopRangeName = "";
                }

                DecisionInfo decisionInfo = new DecisionInfo() {
                    LineInfo = lineInfo,
                    PreflopRangeChosen = _preflopRangeName,
                    PreflopDecision = _preflopDecision,
                    PotOdds = FindPotOdds(lineInfo.Elements)
                };

                if (NewDecisionInfo != null) {
                    NewDecisionInfo(decisionInfo);
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
            //TODO возможно нерпавильно считается в мультипотах, разобраться
            var heroBet = elements.HeroPlayer.Bet;
            var opponents = elements.ActivePlayers.Where(p => p.Name != elements.HeroPlayer.Name);
            if (!opponents.Any()) return 0.0;
            var oppBet = opponents.Select(o => o.Bet).Concat(new double[] { 0 }).Max();
            var pot = elements.TotalPot + heroBet + oppBet;
            if (pot == 0) return 0.0;
            if (heroBet >= oppBet || oppBet == 0) return 0.0;

            return (oppBet - heroBet) / pot*100;
        }

        private void CheckDecision(Hand heroHand , string rangeName, double stat, PlMode plMode) {
            if (_isNewDecision || heroHand == null) return;
            Range range = _rangesList.FirstOrDefault(r => r.Name == rangeName);
            _preflopRangeName = rangeName;

            if (range != null) {
                if (heroHand.Name == "" || heroHand.Name.Length != 4) {
                    Debug.WriteLine("wrong heroHand Name");
                    return;
                }

                PreflopDecision decision1= (PreflopDecision)range.Hands.First(n => n.Name == heroHand.Name).D1;
                PreflopDecision decision2 = (PreflopDecision)range.Hands.First(n => n.Name == heroHand.Name).D2;

                double statRange = range.Hands.First(n => n.Name == heroHand.Name).S1;

                switch (plMode) {
                        case PlMode.None:
                        _preflopDecision = decision1;
                        break;
                    case PlMode.More:
                        _preflopDecision = stat >= statRange ? decision1 : decision2;
                        break;
                    case PlMode.Less:
                        _preflopDecision = stat <= statRange ? decision1 : decision2;
                        break;
                }
                _isNewDecision = true;
            }
            else {
                Debug.WriteLine(String.Format("can't find range {0}", rangeName));
            }
        }

        private void CheckDecision(Hand heroHand, string rangeName, double? stat, PlMode plMode) {
            if (_isNewDecision || heroHand == null) return;
            Range range = _rangesList.FirstOrDefault(r => r.Name == rangeName);
            _preflopRangeName = rangeName;

            if (range != null)
            {
                if (heroHand.Name == "" || heroHand.Name.Length != 4)
                {
                    Debug.WriteLine("wrong heroHand Name");
                    return;
                }

                PreflopDecision decision1 = (PreflopDecision)range.Hands.First(n => n.Name == heroHand.Name).D1;
                PreflopDecision decision2 = (PreflopDecision)range.Hands.First(n => n.Name == heroHand.Name).D2;

                double statRange = range.Hands.First(n => n.Name == heroHand.Name).S1;

                switch (plMode)
                {
                    case PlMode.None:
                        _preflopDecision = decision1;
                        break;
                    case PlMode.More:
                        _preflopDecision = stat >= statRange ? decision1 : decision2;
                        break;
                    case PlMode.Less:
                        _preflopDecision = stat <= statRange ? decision1 : decision2;
                        break;
                }
                _isNewDecision = true;
            }
            else
            {
                Debug.WriteLine(String.Format("can't find range {0}", rangeName));
            }
        }

        private void LoadRanges(string rangesPath) {
            _rangesList = new List<Range>();
            foreach (var file in Directory.GetFiles(rangesPath, "*.xml", SearchOption.AllDirectories))
            {
                _rangesList.Add(XmlRangeHelper.Load(file));
            }
        }

    }



}
