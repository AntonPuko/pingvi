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
            
            //sitout
            lineInfo.StartRule().HeroPreflopState(HeroPreflopState.Open)
                    .SitOutOpp()
                    .Do(e => CheckDecision(heroHand, "COMMON_OPEN_100", 0, PlMode.None));
            
            #region BTN
            //BTN
            //BTN STATS
            double? SB_3BET_VS_BTN = lineInfo.Elements.LeftPlayer.Stats.PF_SB_3BET_VS_BTN;
            double? BB_3BET_VS_BTN = lineInfo.Elements.RightPlayer.Stats.PF_BB_3BET_VS_BTN;

            const double SB_3BET_VS_BTN_default = 20; if (SB_3BET_VS_BTN == null) SB_3BET_VS_BTN = SB_3BET_VS_BTN_default;
            const double BB_3BET_VS_BTN_default = 20; if (BB_3BET_VS_BTN == null) BB_3BET_VS_BTN = BB_3BET_VS_BTN_default;

            double? MERGED_3BET_VS_BTN = SB_3BET_VS_BTN + BB_3BET_VS_BTN;

            //BTN OPEN
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
                .Do(l => CheckDecision(heroHand, "BTN_OPEN_8-13bb", MERGED_3BET_VS_BTN, PlMode.More));
            
            lineInfo.StartRule().HeroPosition(PlayerPosition.Button)
                .HeroPreflopState(HeroPreflopState.Open)
                .HeroStackBetween(13, 17)
                .Do(l => CheckDecision(heroHand, "BTN_OPEN_13-17bb", MERGED_3BET_VS_BTN, PlMode.More));
            
            lineInfo.StartRule().HeroPosition(PlayerPosition.Button)
                .HeroPreflopState(HeroPreflopState.Open)
                .HeroStackBetween(17, 100)
                .Do(l => CheckDecision(heroHand, "BTN_OPEN_17-100bb", 0, PlMode.None));
            
            //BTN CALL PUSH AFTER OPEN
            //SMALL STAKES
            lineInfo.StartRule().HeroPosition(PlayerPosition.Button).IsHU()
                .HeroPreflopState(HeroPreflopState.FacingPushVsOpen)
                .EffectiveStackBetween(0, 8)
                .Do(l => CheckDecision(heroHand, "BTN_CALLPUSH_VSOPEN_0-11bb_UNK", l.Elements.EffectiveStack, PlMode.Less));
            //BTN CALL PUSHVS SB 
            lineInfo.StartRule().HeroPosition(PlayerPosition.Button).IsHU()
                .HeroPreflopState(HeroPreflopState.FacingPushVsOpen)
                .EffectiveStackBetween(8, 100)
                .OppPosition(PlayerPosition.Sb)
                .Do(l => CheckDecision(heroHand, "btn_callpush_vs_SB", l.Elements.EffectiveStack, PlMode.Less));
            //BTN CALL PUSHVS BB 
            lineInfo.StartRule().HeroPosition(PlayerPosition.Button).IsHU()
                .HeroPreflopState(HeroPreflopState.FacingPushVsOpen)
                .EffectiveStackBetween(8, 100)
                .OppPosition(PlayerPosition.Bb)
                .Do(l => CheckDecision(heroHand, "btn_callpush_vs_BB", l.Elements.EffectiveStack, PlMode.Less));
            
            #endregion
            
            #region SB
            //SB
            //STATS
            double? BB_3BET_VS_SB = null;
            if (lineInfo.Elements.HuOpp != null) BB_3BET_VS_SB = lineInfo.Elements.HuOpp.Stats.PF_BB_3BET_VS_SB;
            double? BB_DEF_VS_SBSTEAL = null;
            if (lineInfo.Elements.HuOpp != null) BB_DEF_VS_SBSTEAL = lineInfo.Elements.HuOpp.Stats.PF_BB_DEF_VS_SBSTEAL;
            double? F_FOLD_CBET = null;
            if (lineInfo.Elements.HuOpp != null) F_FOLD_CBET = lineInfo.Elements.HuOpp.Stats.F_FOLD_CBET;
            
            //SB OPEN COMMON(SMALL STACKES)
            //VS BIG STACK
            //IP
            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.Open)
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .EffectiveStackBetween(0, 7)
                .VsBigStack()
                .Do(l => CheckDecision(heroHand, "SB_OPENPUSH_CHEBUKOV", l.Elements.EffectiveStack, PlMode.Less));
            //OOP
            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.Open)
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(0, 9)
                .VsBigStack()
                .Do(l => CheckDecision(heroHand, "SB_OPENPUSH_CHEBUKOV", l.Elements.EffectiveStack, PlMode.Less));

            //VS OTHERS
            //IP
            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.Open)
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .EffectiveStackBetween(0, 7)
                .Do(l => CheckDecision(heroHand, "SB_OPENPUSH_NASH", l.Elements.EffectiveStack, PlMode.Less));
            //OOP
            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.Open)
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(0, 9)
                .Do(l => CheckDecision(heroHand, "SB_OPENPUSH_NASH", l.Elements.EffectiveStack, PlMode.Less));
            
            
            //SB OPEN VS BB 2MAX HU
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

            
            bool IsOppLAG = false || (BB_3BET_VS_SB > 30 || (BB_DEF_VS_SBSTEAL > 65 && F_FOLD_CBET < 41)); //lag definition
            bool IsOppROck = false || (BB_DEF_VS_SBSTEAL < 45);
            bool IsOppGoodReg = false;
            if (lineInfo.Elements.HuOpp != null &&
                (lineInfo.Elements.HuOpp.Type == PlayerType.UberReg
                 || lineInfo.Elements.HuOpp.Type == PlayerType.GoodReg)) IsOppGoodReg = true;

            if (IsOppLAG) {
                //VS LAG
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
                } else {
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
            if (IsOppLAG) {
                    lineInfo.StartRule()
                        .HeroPosition(PlayerPosition.Sb)
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .HeroPreflopState(HeroPreflopState.Open)
                        .EffectiveStackBetween(9, 11)
                        .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_9-11BB__AGR", 0, PlMode.None));

                    lineInfo.StartRule()
                        .HeroPosition(PlayerPosition.Sb)
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .HeroPreflopState(HeroPreflopState.Open)
                        .EffectiveStackBetween(11, 13)
                        .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_11-13BB__AGR", 0, PlMode.None));

                    lineInfo.StartRule()
                        .HeroPosition(PlayerPosition.Sb)
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .HeroPreflopState(HeroPreflopState.Open)
                        .EffectiveStackBetween(13, 15)
                        .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_13-15BB_AGR", 0, PlMode.None));

                    lineInfo.StartRule()
                        .HeroPosition(PlayerPosition.Sb)
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .HeroPreflopState(HeroPreflopState.Open)
                        .EffectiveStackBetween(15, 20)
                        .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_15-20BB_AGR", 0, PlMode.None));

                    lineInfo.StartRule()
                        .HeroPosition(PlayerPosition.Sb)
                        .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                        .HeroPreflopState(HeroPreflopState.Open)
                        .EffectiveStackBetween(20, 100)
                        .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_20-100BB_AGR", 0, PlMode.None));
            }
            else if (IsOppROck) {
                lineInfo.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .HeroPreflopState(HeroPreflopState.Open)
                    .EffectiveStackBetween(9, 11)
                    .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_9-11BB_NIT", 0, PlMode.None));

                lineInfo.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .HeroPreflopState(HeroPreflopState.Open)
                    .EffectiveStackBetween(11, 13)
                    .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_11-13BB_NIT", 0, PlMode.None));

                lineInfo.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .HeroPreflopState(HeroPreflopState.Open)
                    .EffectiveStackBetween(13, 15)
                    .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_13-15BB_NIT", 0, PlMode.None));

                lineInfo.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .HeroPreflopState(HeroPreflopState.Open)
                    .EffectiveStackBetween(15, 20)
                    .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_15-20BB_NIT", 0, PlMode.None));

                lineInfo.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .HeroPreflopState(HeroPreflopState.Open)
                    .EffectiveStackBetween(20, 100)
                    .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_20-100BB_NIT", 0, PlMode.None));
            }
            else {
                lineInfo.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .HeroPreflopState(HeroPreflopState.Open)
                    .EffectiveStackBetween(9, 11)
                    .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_9-11BB_UNK", 0, PlMode.None));

                lineInfo.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .HeroPreflopState(HeroPreflopState.Open)
                    .EffectiveStackBetween(11, 13)
                    .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_11-13BB_UNK", 0, PlMode.None));

                lineInfo.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .HeroPreflopState(HeroPreflopState.Open)
                    .EffectiveStackBetween(13, 15)
                    .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_13-15BB_UNK", 0, PlMode.None));

                lineInfo.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .HeroPreflopState(HeroPreflopState.Open)
                    .EffectiveStackBetween(15, 20)
                    .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_15-20BB_UNK", 0, PlMode.None));

                lineInfo.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .HeroPreflopState(HeroPreflopState.Open)
                    .EffectiveStackBetween(20, 100)
                    .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_20-100BB_UNK", 0, PlMode.None));
                
            }
        


            //SB VS BTN LIMP 3MAX
            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .EffectiveStackSbVsBtnBetween(10, 13)
                .Do(l => CheckDecision(heroHand, "SB_VS_BTN_LIMP_10-13BB", 0, PlMode.None));


            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .EffectiveStackSbVsBtnBetween(13, 16)
                .Do(l => CheckDecision(heroHand, "SB_VS_BTN_LIMP_13-16BB", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                   .HeroPreflopState(HeroPreflopState.FacingLimp)
                   .EffectiveStackSbVsBtnBetween(16, 20)
                   .Do(l => CheckDecision(heroHand, "SB_VS_BTN_LIMP_16-20BB", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                   .HeroPreflopState(HeroPreflopState.FacingLimp)
                   .EffectiveStackSbVsBtnBetween(20, 100)
                   .Do(l => CheckDecision(heroHand, "SB_VS_BTN_LIMP_20-100BB", 0, PlMode.None));



            //SB VS BTN OPEN 3MAX
            double? BTN_STEAL = null;
            var buttonOpener = lineInfo.Elements.ActivePlayers.FirstOrDefault(p => p.Position == PlayerPosition.Button);
            if (buttonOpener != null) BTN_STEAL = buttonOpener.Stats.PF_BTN_STEAL;
            const double defaultBTN_STEAL = 45;
            if (BTN_STEAL == null) BTN_STEAL = defaultBTN_STEAL;

            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .OppBetSizeMinRaise()
                .EffectiveStackSbVsBtnBetween(8, 10)
                .Do(l => CheckDecision(heroHand, "SB_VS_BTN_OPEN_8-10", BTN_STEAL, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .OppBetSizeMinRaise()
                .EffectiveStackSbVsBtnBetween(10, 13)
                .Do(l => CheckDecision(heroHand, "SB_VS_BTN_OPEN_10-13", BTN_STEAL, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .OppBetSizeMinRaise()
                .EffectiveStackSbVsBtnBetween(13, 16)
                .Do(l => CheckDecision(heroHand, "SB_VS_BTN_OPEN_13-16", BTN_STEAL, PlMode.More));
            //Is good reg on BB
            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .LeftPlayerType(PlayerType.GoodReg)
                .OppBetSizeMinRaise()
                .EffectiveStackSbVsBtnBetween(16, 20)
                .Do(l => CheckDecision(heroHand, "SB_VS_BTN_OPEN_16-20bb_GREG", BTN_STEAL, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .LeftPlayerType(PlayerType.GoodReg)
                .OppBetSizeMinRaise()
                .EffectiveStackSbVsBtnBetween(20, 100)
                .Do(l => CheckDecision(heroHand, "SB_VS_BTN_OPEN_20-100BB_GREGBB", BTN_STEAL, PlMode.More));
            //unk bb player
            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
              .HeroPreflopState(HeroPreflopState.FacingOpen)
              .OppBetSizeMinRaise()
              .EffectiveStackSbVsBtnBetween(16, 20)
              .Do(l => CheckDecision(heroHand, "SB_VS_BTN_OPEN_16-20bb_UNK", BTN_STEAL, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .OppBetSizeMinRaise()
                .EffectiveStackSbVsBtnBetween(20, 100)
                .Do(l => CheckDecision(heroHand, "SB_VS_BTN_OPEN_20-100BB_UNKBB", BTN_STEAL, PlMode.More));



            //SB LIMP DEF VS BB RAISE
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



            //SB LIMPDEF VS BB PUSH
            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.FacingPushVsLimp)
                .EffectiveStackBetween(0, 9)
                .Do(l => CheckDecision(heroHand, "SB_CALLPUSH_VSLIMP_7_9bb_UNK", l.Elements.EffectiveStack, PlMode.Less));
            
            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.FacingPushVsLimp)
                .EffectiveStackBetween(9, 12)
                .Do(l => CheckDecision(heroHand, "SB_CALLPUSH_VSLIMP_9-12bb_UNK", l.Elements.EffectiveStack, PlMode.Less));
            
            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.FacingPushVsLimp)
                .EffectiveStackBetween(12, 14)
                .Do(l => CheckDecision(heroHand, "SB_CALLPUSH_VSLIMP_12-14bb_UNK", null, PlMode.None));
            
            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.FacingPushVsLimp)
                .EffectiveStackBetween(14, 16)
                .Do(l => CheckDecision(heroHand, "SB_CALLPUSH_VSLIMP_14-16bb_UNK", null, PlMode.None));
            
            
            //SB CALL PUSH AFTER OPEN
            if (BB_3BET_VS_SB == null || BB_3BET_VS_SB > 15) {
                lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroPreflopState(HeroPreflopState.FacingPushVsOpen)
                    .EffectiveStackBetween(0, 11)
                    .Do(l => CheckDecision(heroHand, "SB_CALLPUSH_VSOPEN_0-11bb_UNK", l.Elements.EffectiveStack, PlMode.Less));
                
                lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroPreflopState(HeroPreflopState.FacingPushVsOpen)
                    .EffectiveStackBetween(11, 100)
                    .Do(l => CheckDecision(heroHand, "SB_CALLPUSH_AFTEROPEN_9-100_UNK_NEW", l.Elements.EffectiveStack, PlMode.Less));
            } else {
                lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroPreflopState(HeroPreflopState.FacingPushVsOpen)
                    .EffectiveStackBetween(0, 9)
                    .Do(l => CheckDecision(heroHand, "SB_CALLPUSH_VSOPEN_0-11bb_UNK", l.Elements.EffectiveStack, PlMode.Less));
                
                lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroPreflopState(HeroPreflopState.FacingPushVsOpen)
                    .EffectiveStackBetween(9, 13)
                    .Do(l => CheckDecision(heroHand, "SB_CALLPuSH_VSOPEN_10-13bb_EXPL", BB_3BET_VS_SB, PlMode.More));
                
                lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroPreflopState(HeroPreflopState.FacingPushVsOpen)
                    .EffectiveStackBetween(13, 20)
                    .Do(l => CheckDecision(heroHand, "SB_CALLPuSH_VSOPEN_13-20bb_EXPL", BB_3BET_VS_SB, PlMode.More));
                
                lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroPreflopState(HeroPreflopState.FacingPushVsOpen)
                    .EffectiveStackBetween(20, 100)
                    .Do(l => CheckDecision(heroHand, "SB_CALLPuSH_VSOPEN_20bb+_EXPL", BB_3BET_VS_SB, PlMode.More));
            }
            
            #endregion

            #region BB
            //BB
            //STATS
            double? LIMPFOLD = null;
            if (lineInfo.Elements.HuOpp != null) LIMPFOLD = lineInfo.Elements.HuOpp.Stats.PF_LIMP_FOLD;


            //BB VS SB LIMP 2MAX(OOP)
            const double bigLimpFold = 78;
            if (LIMPFOLD == null || LIMPFOLD < bigLimpFold) {
                //vs unk or small limpfold
                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is2Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(6, 8)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_2MAX_6-8BB_UNK", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is2Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(8, 10)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_2MAX_8-10BB_UNK", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is2Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(10, 13)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_2MAX_10-13BB_UNK", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is2Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(13, 16)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_2MAX_13-16BB_UNK", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is2Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(16, 20)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_2MAX_16-20BB_UNK", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is2Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(20, 100)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_2MAX_20-100BB_UNK", 0, PlMode.None));
            } else {
                //vs biglimpfold
                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is2Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(6, 8)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_2MAX_8-10BB_BIGLF", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is2Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(8, 10)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_2MAX_8-10BB_BIGLF", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is2Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(10, 13)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_2MAX_10-13BB_BIGLF", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is2Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(13, 16)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_2MAX_13-16BB_BIGLF", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is2Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(16, 20)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_2MAX_16-20BB_BIGLF", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is2Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(20, 100)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_2MAX_20-100BB_BIGLF", 0, PlMode.None));
                
            }


            //BB VS SB LIMP 3MAX(IP)
            if (LIMPFOLD == null || LIMPFOLD < bigLimpFold) {
                //vs unk or small limpfold
                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingLimp).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .EffectiveStackBetween(6, 8)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_3MAX_6-8BB_UNK", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingLimp).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .EffectiveStackBetween(8, 10)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_3MAX_8-10BB_UNK", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingLimp).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .EffectiveStackBetween(10, 13)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_3MAX_10-13BB_UNK", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingLimp).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .EffectiveStackBetween(13, 16)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_3MAX_13-16BB_UNK", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                   .HeroPreflopState(HeroPreflopState.FacingLimp).IsHU()
                   .HeroRelativePosition(HeroRelativePosition.InPosition)
                   .EffectiveStackBetween(16, 20)
                   .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_3MAX_16-20BB_UNK", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                   .HeroPreflopState(HeroPreflopState.FacingLimp).IsHU()
                   .HeroRelativePosition(HeroRelativePosition.InPosition)
                   .EffectiveStackBetween(20, 100)
                   .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_3MAX_20-100BB_UNK", 0, PlMode.None));
             
            } else {
                //vs biglimpfold
                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                      .HeroPreflopState(HeroPreflopState.FacingLimp).IsHU()
                      .HeroRelativePosition(HeroRelativePosition.InPosition)
                      .EffectiveStackBetween(6, 8)
                      .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_3MAX_6-8BB_BIGLF", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingLimp).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .EffectiveStackBetween(8, 10)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_3MAX_8-10BB_BIGLF", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingLimp).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .EffectiveStackBetween(10, 13)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_3MAX_10-13BB_BIGLF", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingLimp).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .EffectiveStackBetween(13, 16)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_3MAX_13-16BB_BIGLF", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                   .HeroPreflopState(HeroPreflopState.FacingLimp).IsHU()
                   .HeroRelativePosition(HeroRelativePosition.InPosition)
                   .EffectiveStackBetween(16, 20)
                   .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_3MAX_16-20BB_BIGLF", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                   .HeroPreflopState(HeroPreflopState.FacingLimp).IsHU()
                   .HeroRelativePosition(HeroRelativePosition.InPosition)
                   .EffectiveStackBetween(20, 100)
                   .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_3MAX_20-100BB_BIGLF", 0, PlMode.None));

            }

            //BB VS BTN LIMP 3MAX HU

            if (lineInfo.Elements.BbAmt < 80) {
                //small blinds
                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHU()
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is3Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(0, 3)
                    .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_0-3BB_SMALLBLINDS", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHU()
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is3Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(3, 6)
                    .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_3-6BB_SMALLBLINDS", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHU()
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is3Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(6, 8)
                    .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_6-8BB_SMALLBLINDS", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHU()
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is3Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(8, 10)
                    .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_8-10BB_SMALLBLINDS", 0, PlMode.None));

            } else {

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHU()
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is3Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(0, 3)
                    .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_0-3BB_BIGBLINDS", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHU()
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is3Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(3, 6)
                    .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_3-6BB_BIGBLINDS", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHU()
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is3Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(6, 8)
                    .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_6-8BB_BIGBLINDSS", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHU()
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is3Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(8, 10)
                    .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_8-10BB_BIGBLINDS", 0, PlMode.None));
            }

            if (LIMPFOLD == null || LIMPFOLD < bigLimpFold) {

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHU()
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is3Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(10, 13)
                    .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_10-13BB", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHU()
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is3Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(13, 16)
                    .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_13-16BB", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHU()
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is3Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(16, 20)
                    .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_16-20BB", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHU()
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is3Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(20, 100)
                    .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_20-100BB", 0, PlMode.None));
            }
            else {
                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHU()
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is3Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(10, 13)
                    .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_10-13BB_BIGLF", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHU()
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is3Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(13, 16)
                    .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_13-16BB_BIGLF", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHU()
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is3Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(16, 20)
                    .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_16-20BB_BIGLF", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHU()
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is3Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(20, 100)
                    .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_20-100BB_BIGLF", 0, PlMode.None));
                
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

            //BB VS BTN OPEN MINR
            double? BTN_STEAL_BB = null;
            if (lineInfo.Elements.HuOpp != null) BTN_STEAL_BB = lineInfo.Elements.HuOpp.Stats.PF_BTN_STEAL;
            const double defaultBTN_STEAL_BB = 35;
            if (BTN_STEAL_BB == null) BTN_STEAL_BB = defaultBTN_STEAL_BB;



            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(0, 3)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_0-3BB", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(3, 7)
                .BBEqOrMoreThen(80)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_3-7BB_BIGBLINDS", BTN_STEAL_BB, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(3, 7)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_3-7BB_SMALLBLINDS", BTN_STEAL_BB, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(7, 9)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_7-9BB", BTN_STEAL_BB, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(9, 11)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_9-11BB", BTN_STEAL_BB, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(11, 13)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_11-13BB", BTN_STEAL_BB, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(13, 15)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_13-15BB", BTN_STEAL_BB, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(15, 17)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_15-17BB", BTN_STEAL_BB, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(17, 20)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_17-20BB", BTN_STEAL_BB, PlMode.More));


            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(20, 100)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_20-100BB", BTN_STEAL_BB, PlMode.More));



            //BB VS BTN OPEN BIG
            
            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeBetween(2, 3)
                .EffectiveStackBetween(5, 9)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_5-9_BIG", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeBetween(2, 3)
                .EffectiveStackBetween(9, 11)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_9-11_BIG", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeBetween(2.5, 3)
                .EffectiveStackBetween(11, 15)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_11-15_BIG", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeBetween(2.5, 3)
                .EffectiveStackBetween(15, 17)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_15-17_BIG", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeBetween(2.5, 4)
                .EffectiveStackBetween(17, 20)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_17-20_BIG", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeBetween(2.5, 4)
                .EffectiveStackBetween(20, 100)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_20-100_BIG", 0, PlMode.None));



              

            

           

            //BB VS SB OPEN MINR 3MAX

            double? SB_STEAL = null;
            if (lineInfo.Elements.HuOpp != null) SB_STEAL = lineInfo.Elements.HuOpp.Stats.PF_SB_STEAL;
            if (SB_STEAL == null) SB_STEAL = 40;

            if (!IsOppGoodReg || lineInfo.Elements.TourneyMultiplier >= 9) {
                
                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingOpen).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .OppBetSize(2)
                    .EffectiveStackBetween(5, 7)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_3MAX_5-7BB_BIGMULTI", 0, PlMode.None));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingOpen).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .OppBetSize(2)
                    .EffectiveStackBetween(7, 9)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_3MAX_7-9BB_BIGMULTI", 0, PlMode.None));
            }
            
            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(0, 7)
                .Do(l => CheckDecision(heroHand, "COMMON_PushFoldVsOpen_NASH", l.Elements.EffectiveStack, PlMode.Less));
            
            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(7, 9)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_3MAX_7-9BB", SB_STEAL, PlMode.More));
            
            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(9, 11)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_3MAX_9-11BB", SB_STEAL, PlMode.More));
            
            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(11, 13)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_3MAX_11-13BB", SB_STEAL, PlMode.More));
            
            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(13, 15)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_3MAX_13-15BB", SB_STEAL, PlMode.More));
            
            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(15, 17)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_3MAX_15-17BB", SB_STEAL, PlMode.More));
            
            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(17, 20)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_3MAX_17-20BB", SB_STEAL, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(20, 100)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_3MAX_20-100BB", SB_STEAL, PlMode.More));
            
            
            //BB VS SB OPEN BIG 3max
            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSizeBetween(2, 3)
                .EffectiveStackBetween(8, 10)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_BIG_8-10bb_3max", null, PlMode.None));
            
            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSizeBetween(2, 3)
                .EffectiveStackBetween(10, 13)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_BIG_10-13bb_3max", null, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHU()
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
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSizeBetween(2.5, 4)
                .EffectiveStackBetween(20, 100)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_BIG_20-100bb_3max", null, PlMode.None));
            
            
            //BB VS SB OPEN MINR 2MAX HU
            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(0, 7)
                .Do(l => CheckDecision(heroHand, "COMMON_PushFoldVsOpen_NASH", l.Elements.EffectiveStack, PlMode.Less));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(7, 9)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_2MAX_7-9BB", SB_STEAL, PlMode.More));
            
            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(9, 11)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_2MAX_9-11BB", SB_STEAL, PlMode.More));
            
            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(11, 13)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_2MAX_11-13BB", SB_STEAL, PlMode.More));
            
            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(13, 15)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_2MAX_13-15BB", SB_STEAL, PlMode.More));
            
            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(15, 17)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_2MAX_15-17BB", SB_STEAL, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(17, 20)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_2MAX_17-20BB", SB_STEAL, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(20, 100)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_2MAX_20-100BB", SB_STEAL, PlMode.More));
            
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


                double? openPush = null;
                if (lineInfo.Elements.HuOpp != null) openPush = lineInfo.Elements.HuOpp.Stats.PF_OPENPUSH;

                //BB vs BTN CALL OPEN PUSH

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                    .IsHU()
                    .OppPosition(PlayerPosition.Button)
                    .EffectiveStackBetween(0, 8)
                    .BBEqOrLessThen(60)
                    .Do(l => CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb", l.Elements.EffectiveStack, PlMode.Less));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                    .IsHU()
                    .OppPosition(PlayerPosition.Button)
                    .EffectiveStackBetween(0, 4)
                    .BBEqOrMoreThen(80)
                    .Do(
                        l =>
                            CheckDecision(heroHand, "COMMON_CALL_VS_OPENPUSH_SKLANSKY-CHEBUKOV", l.Elements.EffectiveStack,
                                PlMode.Less));

            //change from hu oop stack to eff stack 02.07< check results later
                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                    .IsHU()
                    .OppPosition(PlayerPosition.Button)
                    .EffectiveStackBetween(4, 100)
                    .Do(
                        l =>
                            CheckDecision(heroHand, "BB_VS_BTN_OPENPUSH_4-100bb_UNK", l.Elements.EffectiveStack,
                                PlMode.Less));

            PlayerType oopType =  PlayerType.Unknown;
            if (lineInfo.Elements.HuOpp != null) oopType = lineInfo.Elements.HuOpp.Type;


            //BB VS SB CALL OPEN PUSH 3max hu

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                  .HeroPreflopState(HeroPreflopState.FacingOpenPush).Is3Max()
                  .IsHU()
                  .OppPosition(PlayerPosition.Sb)
                  .EffectiveStackBetween(8, 100)
                  .Do(l => CheckDecision(heroHand, "BB_VS_SB_CALL_OPENPUSH_9-100bb_UNK",
                              l.Elements.EffectiveStack, PlMode.Less));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                 .HeroPreflopState(HeroPreflopState.FacingOpenPush).Is3Max()
                 .IsHU()
                 .OppPosition(PlayerPosition.Sb)
                 .EffectiveStackBetween(0, 8)
                 .Do(l => CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb",
                             l.Elements.EffectiveStack, PlMode.Less));

   
            
                //BB VS SB CALL OPEN PUSH HU 2max
            if ((openPush > 20 && lineInfo.Elements.TourneyMultiplier >= 6) || oopType == PlayerType.GoodReg ||
                lineInfo.Elements.TourneyMultiplier < 6)
            {
                //vs agr or good reg or small multi
                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroPreflopState(HeroPreflopState.FacingOpenPush).Is2Max()
                    .IsHU()
                    .OppPosition(PlayerPosition.Sb)
                    .EffectiveStackBetween(8, 100)
                    .Do(l => CheckDecision(heroHand, "BB_VS_SB_CALL_OPENPUSH_9-100bb_UNK",
                                l.Elements.EffectiveStack, PlMode.Less));
            }
            else {
                //vs unk and big multi
                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                   .HeroPreflopState(HeroPreflopState.FacingOpenPush).Is2Max()
                   .IsHU()
                   .OppPosition(PlayerPosition.Sb)
                   .EffectiveStackBetween(8, 100)
                   .Do(l => CheckDecision(heroHand, "COMMON_CALL_VS_OPENPUSH_SKLANSKY-CHEBUKOV",
                       l.Elements.EffectiveStack, PlMode.Less));
            }


               


            //bb  vs sb call openpush HU 0-8bb
            const double openPushSeparator08Bb = 40.0;
            const int multiplierSeparator = 6;
            if (lineInfo.Elements.TourneyMultiplier <= multiplierSeparator)
            { // small multiplier
                #region smallMultiplicator
                //big blinds
                #region bigblinds
                if (openPush >= openPushSeparator08Bb) { // vs agr
                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHU()
                        .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                        .BBEqOrMoreThen(60)
                        .EffectiveStackBetween(5, 8)
                        .Do(l => CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb"
                            , l.Elements.EffectiveStack, PlMode.Less));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHU()
                        .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                        .BBEqOrMoreThen(80)
                        .EffectiveStackBetween(4, 5)
                        .Do(l => CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb"
                            , l.Elements.EffectiveStack, PlMode.Less));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHU()
                        .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                        .BBEqOrMoreThen(100)
                        .EffectiveStackBetween(0, 3)
                        .Do(l => CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb"
                            , l.Elements.EffectiveStack, PlMode.Less));
                }
                else if (openPush < openPushSeparator08Bb)
                { // vs nit
                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHU()
                        .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                        .BBEqOrMoreThen(60)
                        .EffectiveStackBetween(5, 8)
                        .Do(l => CheckDecision(heroHand, "COMMON_CALL_VS_OPENPUSH_SKLANSKY-CHEBUKOV"
                            , l.Elements.EffectiveStack, PlMode.Less));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHU()
                        .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                        .BBEqOrMoreThen(80)
                        .EffectiveStackBetween(4, 5)
                        .Do(l => CheckDecision(heroHand, "COMMON_CALL_VS_OPENPUSH_SKLANSKY-CHEBUKOV"
                            , l.Elements.EffectiveStack, PlMode.Less));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHU()
                        .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                        .BBEqOrMoreThen(100)
                        .EffectiveStackBetween(0, 3)
                        .Do(l => CheckDecision(heroHand, "COMMON_CALL_VS_OPENPUSH_SKLANSKY-CHEBUKOV"
                            , l.Elements.EffectiveStack, PlMode.Less));

                }
                else { //vs unk
                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHU()
                     .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                     .BBEqOrMoreThen(60)
                     .EffectiveStackBetween(5, 8)
                     .Do(l => CheckDecision(heroHand, "COMMON_CALL_VS_OPENPUSH_SKLANSKY-CHEBUKOV"
                         , l.Elements.EffectiveStack, PlMode.Less));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHU()
                        .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                        .BBEqOrMoreThen(80)
                        .EffectiveStackBetween(4, 5)
                        .Do(l => CheckDecision(heroHand, "COMMON_CALL_VS_OPENPUSH_SKLANSKY-CHEBUKOV"
                            , l.Elements.EffectiveStack, PlMode.Less));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHU()
                        .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                        .BBEqOrMoreThen(100)
                        .EffectiveStackBetween(0, 3)
                        .Do(l => CheckDecision(heroHand, "COMMON_CALL_VS_OPENPUSH_SKLANSKY-CHEBUKOV"
                            , l.Elements.EffectiveStack, PlMode.Less));
                }
                #endregion
                //small blinds
                if (openPush >= openPushSeparator08Bb)
                { // vs agr

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHU()
                      .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                      .EffectiveStackBetween(0, 8)
                      .Do(l => CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb"
                          , l.Elements.EffectiveStack, PlMode.Less));


                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHU()
                      .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                      .EffectiveStackBetween(0, 8)
                      .Do(l => CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb"
                          , l.Elements.EffectiveStack, PlMode.Less));
                }
                else if (openPush < openPushSeparator08Bb)
                { // vs nit
                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHU()
                     .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                     .EffectiveStackBetween(6, 8)
                     .Do(l => CheckDecision(heroHand, "COMMON_CALL_VS_OPENPUSH_SKLANSKY-CHEBUKOV"
                         , l.Elements.EffectiveStack, PlMode.Less));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHU()
                     .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                     .EffectiveStackBetween(6, 0)
                     .Do(l => CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb"
                         , l.Elements.EffectiveStack, PlMode.Less));
                }
                else { //vs unk
                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHU()
                     .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                     .EffectiveStackBetween(0, 8)
                     .Do(l => CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb"
                         , l.Elements.EffectiveStack, PlMode.Less));
                }
                #endregion
            }
            else { // big multiplier
                #region bigMultiplicator

                #region bigblinds
                //big blinds
                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHU()
                    .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                    .BBEqOrMoreThen(60)
                    .EffectiveStackBetween(5,8)
                    .Do(l => CheckDecision(heroHand, "COMMON_CALL_VS_OPENPUSH_SKLANSKY-CHEBUKOV"
                        , l.Elements.EffectiveStack, PlMode.Less));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHU()
                    .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                    .BBEqOrMoreThen(80)
                    .EffectiveStackBetween(4, 5)
                    .Do(l => CheckDecision(heroHand, "COMMON_CALL_VS_OPENPUSH_SKLANSKY-CHEBUKOV"
                        , l.Elements.EffectiveStack, PlMode.Less));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHU()
                    .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                    .BBEqOrMoreThen(100)
                    .EffectiveStackBetween(0, 3)
                    .Do(l => CheckDecision(heroHand, "COMMON_CALL_VS_OPENPUSH_SKLANSKY-CHEBUKOV"
                        , l.Elements.EffectiveStack, PlMode.Less));
                #endregion
                //small blinds
                if (openPush >= openPushSeparator08Bb) {

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHU()
                        .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                        .EffectiveStackBetween(0, 8)
                        .Do(l => CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb"
                            , l.Elements.EffectiveStack, PlMode.Less));
                }
                else if (openPush < openPushSeparator08Bb)
                {
                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHU()
                        .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                        .EffectiveStackBetween(6, 8)
                        .Do(l => CheckDecision(heroHand, "COMMON_CALL_VS_OPENPUSH_SKLANSKY-CHEBUKOV"
                            , l.Elements.EffectiveStack, PlMode.Less));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHU()
                        .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                        .EffectiveStackBetween(0, 6)
                        .Do(l => CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb"
                            , l.Elements.EffectiveStack, PlMode.Less));
                }
                else {
                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHU()
                      .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                      .EffectiveStackBetween(7, 8)
                      .Do(l => CheckDecision(heroHand, "COMMON_CALL_VS_OPENPUSH_SKLANSKY-CHEBUKOV"
                          , l.Elements.EffectiveStack, PlMode.Less));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHU()
                        .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                        .EffectiveStackBetween(0, 7)
                        .Do(l => CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb"
                            , l.Elements.EffectiveStack, PlMode.Less));
                }

                #endregion
            }




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
