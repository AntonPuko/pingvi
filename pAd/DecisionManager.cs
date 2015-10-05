using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using PokerModel;

namespace Pingvi {
    public enum PreflopDecision {
        None,
        Fold,
        Limp,
        OpenRaise,
        Call,
        _3Bet,
        Push
    }


    public struct PreviousLineInfo {
        public double LastEffectiveStack;
        public double LastPot;
        public double LastHeroPlayerStack;
        public PlayerPosition LastHeroPosition;
        public int? LastTourneyMultiplicator;
    }

    public class DecisionManager {
        private const string RangesPath = @"Data\Ranges";
        private readonly Random _propapilityRandomizer = new Random((int) DateTime.Now.Ticks);
        private bool _isNewDecision;
        private PreflopDecision _preflopDecision;
        private string _preflopRangeName;
        private int _probRand;
        private PreviousLineInfo _prevLineInfo = new PreviousLineInfo();
        private List<Range> _rangesList;

        public DecisionManager() {
            LoadRanges(RangesPath);
        }

        public event Action<DecisionInfo> NewDecisionInfo;

        private bool IsPrevLineInfoEquals(LineInfo lineInfo, PreviousLineInfo prevLineInfo) {
            return prevLineInfo.LastEffectiveStack == lineInfo.Elements.EffectiveStack &&
                   prevLineInfo.LastHeroPlayerStack == lineInfo.Elements.HeroPlayer.Stack &&
                   prevLineInfo.LastHeroPosition == lineInfo.Elements.HeroPlayer.Position &&
                   prevLineInfo.LastPot == lineInfo.Elements.TotalPot &&
                   prevLineInfo.LastTourneyMultiplicator == lineInfo.Elements.TourneyMultiplier;
        }

        private void RefreshPrevLineInfo(LineInfo lineInfo) {
            _prevLineInfo.LastEffectiveStack = lineInfo.Elements.EffectiveStack;
            _prevLineInfo.LastHeroPlayerStack = lineInfo.Elements.HeroPlayer.Stack;
            _prevLineInfo.LastHeroPosition = lineInfo.Elements.HeroPlayer.Position;
            _prevLineInfo.LastPot = lineInfo.Elements.TotalPot;
            _prevLineInfo.LastTourneyMultiplicator = lineInfo.Elements.TourneyMultiplier;
        }

        public void OnNewLineInfo(LineInfo lineInfo) {

            if(!IsPrevLineInfoEquals(lineInfo, _prevLineInfo)) _probRand = _propapilityRandomizer.Next(1, 100);
            RefreshPrevLineInfo(lineInfo);
            
         
          
            
            _isNewDecision = false;


            var heroHand = lineInfo.Elements.HeroPlayer.Hand;

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
            var SB_3BET_VS_BTN = lineInfo.Elements.LeftPlayer.Stats.PF_SB_3BET_VS_BTN;
            var BB_3BET_VS_BTN = lineInfo.Elements.RightPlayer.Stats.PF_BB_3BET_VS_BTN;

            const double SB_3BET_VS_BTN_default = 20;
            if (SB_3BET_VS_BTN == null) SB_3BET_VS_BTN = SB_3BET_VS_BTN_default;
            const double BB_3BET_VS_BTN_default = 20;
            if (BB_3BET_VS_BTN == null) BB_3BET_VS_BTN = BB_3BET_VS_BTN_default;

            var MERGED_3BET_VS_BTN = SB_3BET_VS_BTN + BB_3BET_VS_BTN;

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
                .Do(
                    l =>
                        CheckDecision(heroHand, "BTN_CALLPUSH_VSOPEN_0-11bb_UNK", l.Elements.EffectiveStack, PlMode.Less));
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

            double? BB_RAISE_LIMPER = null;
            if (lineInfo.Elements.HuOpp != null) BB_RAISE_LIMPER = lineInfo.Elements.HuOpp.Stats.PF_RAISE_LIMPER;
            if (BB_RAISE_LIMPER == null) BB_RAISE_LIMPER = 40;

            //SB OPEN COMMON(SMALL STACKES)
            //VS SMALL STACK
            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(0, 7)
                .VsSmallStack()
                .Do(l => CheckDecision(heroHand, "SB_OPENPUSH_NASH_EXPANDED", l.Elements.EffectiveStack, PlMode.Less));

            //VS OTHERS
            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(0, 7)
                .Do(l => CheckDecision(heroHand, "SB_OPENPUSH_NASH", l.Elements.EffectiveStack, PlMode.Less));


            //SB OPEN VS BB 2MAX HU PROB MODE
            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb)
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(7, 8)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_2MAX_7-8BB", BB_RAISE_LIMPER, PlMode.More));

            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb)
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(8, 9)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_2MAX_8-9BB", BB_RAISE_LIMPER, PlMode.More));

            lineInfo.StartRule()
               .HeroPosition(PlayerPosition.Sb)
               .HeroRelativePosition(HeroRelativePosition.InPosition)
               .HeroPreflopState(HeroPreflopState.Open)
               .EffectiveStackBetween(9, 11)
               .StatEqOrLessThan(BB_DEF_VS_SBSTEAL, 40)
               .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_2MAX_9-11BB_SMALLDEF", BB_RAISE_LIMPER, PlMode.More));

            lineInfo.StartRule()
               .HeroPosition(PlayerPosition.Sb)
               .HeroRelativePosition(HeroRelativePosition.InPosition)
               .HeroPreflopState(HeroPreflopState.Open)
               .EffectiveStackBetween(9, 11)
               .StatEqOrMoreThan(BB_DEF_VS_SBSTEAL, 60)
               .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_2MAX_9-11BB_BIGDEF", BB_RAISE_LIMPER, PlMode.More));

            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb)
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(9, 11)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_2MAX_9-11BB", BB_RAISE_LIMPER, PlMode.More));

            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb)
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(11, 13)
                .StatEqOrLessThan(BB_DEF_VS_SBSTEAL, 43)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_2MAX_11-13BB_SMALLDEF", _probRand, PlMode.Less));

            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb)
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(11, 13)
                .StatEqOrMoreThan(BB_DEF_VS_SBSTEAL, 68)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_2MAX_11-13BB_BIGDEF", _probRand, PlMode.Less));

            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb)
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(11, 13)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_2MAX_11-13BB_UNK", _probRand, PlMode.Less));

            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb)
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(13, 15)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_2MAX_13-15BB_UNK", _probRand, PlMode.Less));

            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb)
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(15, 17)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_2MAX_15-17BB_UNK", _probRand, PlMode.Less));


            //vs regs
            if (lineInfo.Elements.HuOpp != null && (lineInfo.Elements.HuOpp.Type == PlayerType.GoodReg ||
                                                    lineInfo.Elements.HuOpp.Type == PlayerType.Maniac ||
                                                    lineInfo.Elements.HuOpp.Type == PlayerType.WeakReg ||
                                                    lineInfo.Elements.HuOpp.Type == PlayerType.UberReg ||
                                                    lineInfo.Elements.HuOpp.Stats.PF_BB_3BET_VS_SB > 30)) {
                lineInfo.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .HeroPreflopState(HeroPreflopState.Open)
                    .EffectiveStackBetween(17, 19)
                    .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_2MAX_17-19BB_REGS", _probRand, PlMode.Less));
                
                
                lineInfo.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .HeroPreflopState(HeroPreflopState.Open)
                    .EffectiveStackBetween(19, 22)
                    .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_2MAX_19-22BB_REGS", _probRand, PlMode.Less));
                lineInfo.StartRule()
                    .HeroPosition(PlayerPosition.Sb)
                    .HeroRelativePosition(HeroRelativePosition.InPosition)
                    .HeroPreflopState(HeroPreflopState.Open)
                    .EffectiveStackBetween(22, 100)
                    .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_2MAX_22-100BB_REGS", _probRand, PlMode.Less));

            }

            //vs unk
            lineInfo.StartRule()
                  .HeroPosition(PlayerPosition.Sb)
                  .HeroRelativePosition(HeroRelativePosition.InPosition)
                  .HeroPreflopState(HeroPreflopState.Open)
                  .EffectiveStackBetween(17, 19)
                  .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_2MAX_17-19BB_UNK", 0, PlMode.None));

            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb)
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(19, 22)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_2MAX_19-22BB_UNK", 0, PlMode.None));
            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb)
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(22, 100)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_2MAX_22-100BB_UNK", 0, PlMode.None));



            #region SB OPEN VS BB 3MAX
            //SB VS BB OPEN 3MAX
            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb)
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(7, 9)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_7-9BB_UNK", _probRand, PlMode.Less));

            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb)
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(9, 11)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_9-11BB_UNK", _probRand, PlMode.Less));

            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb)
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(11, 13)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_11-13BB_UNK", _probRand, PlMode.Less));

            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb)
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(13, 15)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_13-15BB_UNK", _probRand, PlMode.Less));

            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb)
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(15, 17)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_15-17BB_UNK", _probRand, PlMode.Less));

            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb)
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(17, 19)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_17-19BB_UNK", _probRand, PlMode.Less));

            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb)
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(19, 22)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_19-22BB_UNK", _probRand, PlMode.Less));

            #region SB OPEN VS BB 22-100BB 3MAX
            //SB OPEN VS BB  22-100 3MAX
                //VS SMALL BB DEF && BIG 3BET
            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb)
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(22, 100)
                .StatEqOrLessThan(BB_DEF_VS_SBSTEAL, 50)
                .StatEqOrMoreThan(BB_3BET_VS_SB, 30)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_22-100BB_VS_SMALL_BBDEF_BIG3BET", _probRand, PlMode.Less));
                //VS SMALL BB DEF && BIG 3BET
            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb)
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(22, 100)
                .StatEqOrLessThan(BB_DEF_VS_SBSTEAL, 50)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_22-100BB_VS_SMALL_BBDEF_BIG3BET", _probRand, PlMode.Less));
                 //VS BIG BB DEF && BIG 3BET
            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb)
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(22, 100)
                .StatEqOrMoreThan(BB_DEF_VS_SBSTEAL, 75)
                .StatEqOrMoreThan(BB_3BET_VS_SB, 30)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_22-100BB_VS_SMALL_BBDEF_BIG3BET", _probRand, PlMode.Less));
                 //VS BIG BB DEF && BIG 3BET
            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb)
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(22, 100)
                .StatEqOrMoreThan(BB_DEF_VS_SBSTEAL, 75)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_22-100BB_VS_BIGBBDEF_UNK3BET", _probRand, PlMode.Less));
                 //UNK
            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb)
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(22, 100)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_22-100BB_UNK", _probRand, PlMode.Less));
            #endregion  
            #endregion

            #region SB VS BTN LIMP 3MAX
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
            #endregion



            //SB VS BTN OPEN 3MAX
            double? BTN_STEAL = null;
            var buttonOpener = lineInfo.Elements.ActivePlayers.FirstOrDefault(p => p.Position == PlayerPosition.Button);
            if (buttonOpener != null) BTN_STEAL = buttonOpener.Stats.PF_BTN_STEAL;
            const double defaultBTN_STEAL = 45;
            if (BTN_STEAL == null) BTN_STEAL = defaultBTN_STEAL;


            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(3, 20)
                .Do(
                    l =>
                        CheckDecision(heroHand, "SB_VS_BTN_OPEN_3max_0-20bb", lineInfo.Elements.SbBtnEffStack,
                            PlMode.Less));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(20, 100)
                .Do(
                    l =>
                        CheckDecision(heroHand, "SB_VS_BTN_OPEN_3max_20-100bb", lineInfo.Elements.SbBtnEffStack,
                            PlMode.Less));


            //SB LIMP DEF VS BB RAISE
            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.FacingISOvsLimp)
                .OppBetSize(2)
                .EffectiveStackBetween(7, 9)
                .Do(l => CheckDecision(heroHand, "SB_DefVSMinRaise_AfterLIMP_IP_7-9bb_UNK", null, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.FacingISOvsLimp)
                .OppBetSize(2)
                .EffectiveStackBetween(9, 12)
                .Do(l => CheckDecision(heroHand, "SB_DefVSMinRaise_AfterLIMP_IP_9-12bb_UNK", null, PlMode.None));


            //SB LIMPDEF VS BB PUSH
            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.FacingPushVsLimp)
                .EffectiveStackBetween(0, 9)
                .Do(l => CheckDecision(heroHand, "SB_CALLPUSH_VSLIMP_7_9bb_UNK", l.Elements.EffectiveStack, PlMode.Less));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.FacingPushVsLimp)
                .EffectiveStackBetween(9, 12)
                .Do(
                    l =>
                        CheckDecision(heroHand, "SB_CALLPUSH_VSLIMP_9-12bb_UNK", l.Elements.EffectiveStack, PlMode.Less));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.FacingPushVsLimp)
                .EffectiveStackBetween(12, 14)
                .Do(l => CheckDecision(heroHand, "SB_CALLPUSH_VSLIMP_12-14bb_UNK", null, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.FacingPushVsLimp)
                .EffectiveStackBetween(14, 16)
                .Do(l => CheckDecision(heroHand, "SB_CALLPUSH_VSLIMP_14-16bb_UNK", null, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.FacingPushVsLimp)
                .EffectiveStackBetween(16, 20)
                .Do(l => CheckDecision(heroHand, "SB_CALLPUSH_VSLIMP_16-20bb_UNK", null, PlMode.None));


            //SB CALL PUSH AFTER OPEN
            if (BB_3BET_VS_SB == null || BB_3BET_VS_SB > 30) {
                lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroPreflopState(HeroPreflopState.FacingPushVsOpen)
                    .EffectiveStackBetween(0, 9)
                    .Do(l => CheckDecision(heroHand, "SB_CALLPUSH_VSOPEN_0-11bb_UNK", l.Elements.EffectiveStack,
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
                    .Do(l => CheckDecision(heroHand, "SB_CALLPUSH_VSOPEN_0-11bb_UNK", l.Elements.EffectiveStack,PlMode.Less));

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
            const double defaultLIMPFOLD = 60;
            if (LIMPFOLD == null) LIMPFOLD = defaultLIMPFOLD;

            //BB VS SB LIMP 2MAX(OOP)
            const double bigLimpFold = 78;

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .Is2Max()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(6, 8)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_2MAX_6-8BB", LIMPFOLD, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .Is2Max()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(8, 10)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_2MAX_8-10BB", LIMPFOLD, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .Is2Max()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(10, 13)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_2MAX_10-13BB", LIMPFOLD, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .Is2Max()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(13, 16)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_2MAX_13-16BB", LIMPFOLD, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .Is2Max()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(16, 20)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_2MAX_16-20BB", LIMPFOLD, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .Is2Max()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(20, 100)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_2MAX_20-100BB", LIMPFOLD, PlMode.More));


            //BB VS SB LIMP 3MAX(IP)

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingLimp).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .EffectiveStackBetween(6, 8)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_3MAX_6-8BB", LIMPFOLD, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingLimp).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .EffectiveStackBetween(8, 10)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_3MAX_8-10BB", LIMPFOLD, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingLimp).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .EffectiveStackBetween(10, 13)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_3MAX_10-13BB", LIMPFOLD, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingLimp).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .EffectiveStackBetween(13, 16)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_3MAX_13-16BB", LIMPFOLD, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingLimp).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .EffectiveStackBetween(16, 20)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_3MAX_16-20BB", LIMPFOLD, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingLimp).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .EffectiveStackBetween(20, 100)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_3MAX_20-100BB", LIMPFOLD, PlMode.More));


            //BB VS BTN LIMP 3MAX HU

            if (lineInfo.Elements.BbAmt < 80) {
                //small blinds
                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHU()
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is3Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(0, 3)
                    .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_0-3BB_SMALLBLINDS", 0, PlMode.None));
            }
            else {
                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHU()
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is3Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(0, 3)
                    .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_0-3BB_BIGBLINDS", 0, PlMode.None));
            }

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHU()
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .Is3Max()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(3, 6)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_3-6bb", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHU()
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .Is3Max()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(6, 8)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_6-8bb", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHU()
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .Is3Max()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(8, 10)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_8-10bb", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHU()
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .Is3Max()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(10, 13)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_10-13bb", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHU()
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .Is3Max()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(13, 16)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_13-16bb", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHU()
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .Is3Max()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(16, 20)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_16-20bb", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHU()
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .Is3Max()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(20, 100)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_20-100bb", 0, PlMode.None));


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
            if (SB_STEAL == null) SB_STEAL = 41;

            double? foldTo3bet = null;
            if (lineInfo.Elements.HuOpp != null) {
                foldTo3bet = lineInfo.Elements.HuOpp.Stats.PF_FOLD_3BET;
                const double defaultFoldTo3Bet = 65;
                if (foldTo3bet == null) foldTo3bet = defaultFoldTo3Bet;
                if (lineInfo.Elements.HuOpp.Stack >= 2*lineInfo.Elements.HeroPlayer.Stack) foldTo3bet -= 5;
            }


            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSize(2)
                .BBEqOrMoreThen(60)
                .EffectiveStackBetween(5, 8)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_5-8bb_3max_BIGBLINDS", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(0, 8)
                .Do(l => CheckDecision(heroHand, "COMMON_PushFoldVsOpen_NASH", l.Elements.EffectiveStack, PlMode.Less));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSize(2)
                .BBEqOrMoreThen(60)
                .EffectiveStackBetween(8, 10)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_8-10bb_3max_BIGBLINDS", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(8, 10)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_8-10bb_3max_SMALLBLINDS", foldTo3bet, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(10, 12)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_10-13bb_3max", SB_STEAL, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(10, 12)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_10-13bb_3max", SB_STEAL, PlMode.More));


            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(12, 100)
                .Do(
                    l =>
                        CheckDecision(heroHand, "BB_VS_SB_OPEN_3max_12-100bb", lineInfo.Elements.EffectiveStack,
                            PlMode.Less));


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

            //change from eff stack to hu opp stack
            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                .IsHU()
                .OppPosition(PlayerPosition.Button)
                .EffectiveStackBetween(4, 100)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPENPUSH_4-100bb_UNK", l.Elements.HuOpp.Stack, PlMode.Less));

    


            //BB VS SB CALL OPEN PUSH 3max hu

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpenPush).Is3Max().IsHU()
                .OppPosition(PlayerPosition.Sb)
                .EffectiveStackBetween(0, 100)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_3max_CALLOPENSHOVE_GRIZZ", l.Elements.EffectiveStack,
                            PlMode.Less));

            //BB VS SB CALL OPEN PUSH HU 2max
            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHU()
                .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                .EffectiveStackBetween(8, 100)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_2max_CALLOPENSHOVE_GRIZZ", l.Elements.EffectiveStack, PlMode.Less));


            //bb  vs sb call openpush HU 0-8bb
            const double openPushSeparator08Bb = 30.0;
            const int multiplierSeparator = 9;
            if (lineInfo.Elements.TourneyMultiplier <= multiplierSeparator)
            { // small multiplier
                #region smallMultiplicator
                //big blinds
                #region bigblinds
                if (openPush >= openPushSeparator08Bb)
                { // vs agr
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
                else
                { //vs unk
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
                else
                { //vs unk
                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHU()
                     .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                     .EffectiveStackBetween(0, 8)
                     .Do(l => CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb"
                         , l.Elements.EffectiveStack, PlMode.Less));
                }
                #endregion
            }
            else
            { // big multiplier
                #region bigMultiplicator

                #region bigblinds
                //big blinds
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
                #endregion
                //small blinds
                if (openPush >= openPushSeparator08Bb)
                {

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
                else
                {
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
                .Do(l => CheckDecision(heroHand, "COMMON_PushFoldVsOpen_NASH", l.Elements.EffectiveStack, PlMode.Less));


            lineInfo.StartRule()
                .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                .IsHU()
                .EffectiveStackBetween(0, 8)
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .Do(l => CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb", l.Elements.EffectiveStack, PlMode.Less));


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

            var decisionInfo = new DecisionInfo {
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
            if (foldTo3bet == null) return Foldto3Bet.None;
            if (elements.EffectiveStack > 15) {
                if (foldTo3bet >= 77) return Foldto3Bet.Big;
                if (foldTo3bet >= 65) return Foldto3Bet.Average;
                return Foldto3Bet.Small;
            }
            if (foldTo3bet >= 70) return Foldto3Bet.Big;
            if (foldTo3bet >= 60) return Foldto3Bet.Average;
            return Foldto3Bet.Small;
        }

        private double FindPotOdds(Elements elements) {
            //TODO возможно нерпавильно считается в мультипотах, разобраться
            var heroBet = elements.HeroPlayer.Bet;
            var opponents = elements.ActivePlayers.Where(p => p.Name != elements.HeroPlayer.Name);
            if (!opponents.Any()) return 0.0;
            var oppBet = opponents.Select(o => o.Bet).Concat(new double[] {0}).Max();
            var pot = elements.TotalPot + heroBet + oppBet;
            if (pot == 0) return 0.0;
            if (heroBet >= oppBet || oppBet == 0) return 0.0;

            return (oppBet - heroBet)/pot*100;
        }

        private void CheckDecision(Hand heroHand, string rangeName, double stat, PlMode plMode) {
            if (_isNewDecision || heroHand == null) return;
            var range = _rangesList.FirstOrDefault(r => r.Name == rangeName);
            _preflopRangeName = rangeName;

            if (range != null) {
                if (heroHand.Name == "" || heroHand.Name.Length != 4) {
                    Debug.WriteLine("wrong heroHand Name");
                    return;
                }

                var decision1 = (PreflopDecision) range.Hands.First(n => n.Name == heroHand.Name).D1;
                var decision2 = (PreflopDecision) range.Hands.First(n => n.Name == heroHand.Name).D2;

                var statRange = range.Hands.First(n => n.Name == heroHand.Name).S1;

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
                Debug.WriteLine(string.Format("can't find range {0}", rangeName));
            }
        }

        private void CheckDecision(Hand heroHand, string rangeName, double? stat, PlMode plMode) {
            if (_isNewDecision || heroHand == null) return;
            var range = _rangesList.FirstOrDefault(r => r.Name == rangeName);
            _preflopRangeName = rangeName;

            if (range != null) {
                if (heroHand.Name == "" || heroHand.Name.Length != 4) {
                    Debug.WriteLine("wrong heroHand Name");
                    return;
                }

                var decision1 = (PreflopDecision) range.Hands.First(n => n.Name == heroHand.Name).D1;
                var decision2 = (PreflopDecision) range.Hands.First(n => n.Name == heroHand.Name).D2;

                var statRange = range.Hands.First(n => n.Name == heroHand.Name).S1;

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
                Debug.WriteLine(string.Format("can't find range {0}", rangeName));
            }
        }

        private void LoadRanges(string rangesPath) {
            _rangesList = new List<Range>();
            foreach (var file in Directory.GetFiles(rangesPath, "*.xml", SearchOption.AllDirectories)) {
                _rangesList.Add(XmlRangeHelper.Load(file));
            }
        }

        private enum PlMode {
            None,
            More,
            Less
        };

        private enum Foldto3Bet {
            None,
            Small,
            Average,
            Big
        };
    }
}