using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using PokerModel;

namespace Pingvi
{
    public enum PreflopDecision
    {
        None,
        Fold,
        Limp,
        OpenRaise,
        Call,
        _3Bet,
        Push
    }


    public struct PreviousLineInfo
    {
        public double LastEffectiveStack;
        public double LastPot;
        public double LastHeroPlayerStack;
        public PlayerPosition LastHeroPosition;
        public int? LastTourneyMultiplicator;
    }

    public class DecisionManager
    {
        private const string RangesPath = @"Data\Ranges";
        private const string GtoRangesList = @"Data\GtoRanges";
        private readonly Random _propapilityRandomizer = new Random((int)DateTime.Now.Ticks);
        private bool _isNewDecision;
        private PreflopDecision _preflopDecision;
        private string _preflopRangeName;
        private PreviousLineInfo _prevLineInfo;
        private double _probRand;
        private List<Range> _rangesList;
        private List<GtoRange> _gtoRangesList;

        private double[][] _probRanges = new[]
        {
            new[] {0.0, 0.0},
            new[] { 0.0, 0.0},
            new[] { 0.0, 0.0},
            new[] { 0.0, 0.0},
        };

        double? _raiseSize = 0;

        public DecisionManager()
        {
            LoadRanges(RangesPath);
            LoadGtoRanges(GtoRangesList);
        }

        public event Action<DecisionInfo> NewDecisionInfo;

        private bool IsPrevLineInfoEquals(LineInfo lineInfo, PreviousLineInfo prevLineInfo)
        {
            return prevLineInfo.LastEffectiveStack == lineInfo.Elements.EffectiveStack &&
                   prevLineInfo.LastHeroPlayerStack == lineInfo.Elements.HeroPlayer.Stack &&
                   prevLineInfo.LastHeroPosition == lineInfo.Elements.HeroPlayer.Position &&
                   prevLineInfo.LastPot == lineInfo.Elements.TotalPot &&
                   prevLineInfo.LastTourneyMultiplicator == lineInfo.Elements.TourneyMultiplier;
        }

        private void RefreshPrevLineInfo(LineInfo lineInfo)
        {
            _prevLineInfo.LastEffectiveStack = lineInfo.Elements.EffectiveStack;
            _prevLineInfo.LastHeroPlayerStack = lineInfo.Elements.HeroPlayer.Stack;
            _prevLineInfo.LastHeroPosition = lineInfo.Elements.HeroPlayer.Position;
            _prevLineInfo.LastPot = lineInfo.Elements.TotalPot;
            _prevLineInfo.LastTourneyMultiplicator = lineInfo.Elements.TourneyMultiplier;
        }

        public void OnNewLineInfo(LineInfo lineInfo)
        {
            if (!IsPrevLineInfoEquals(lineInfo, _prevLineInfo)) _probRand = GetRandomNumber(0.0,100.0);
            RefreshPrevLineInfo(lineInfo);

            _raiseSize = null;

            _isNewDecision = false;


            var heroHand = lineInfo.Elements.HeroPlayer.Hand;

            if (!lineInfo.Elements.HeroPlayer.IsHeroTurn)
            {
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
            var sb_3BetVsBtn = lineInfo.Elements.LeftPlayer.Stats.PfSb_3BetVsBtn;
            var bb_3BetVsBtn = lineInfo.Elements.RightPlayer.Stats.PfBb_3BetVsBtn;

            const double sb_3BetVsBtnDefault = 20;
            if (sb_3BetVsBtn == null) sb_3BetVsBtn = sb_3BetVsBtnDefault;
            const double bb_3BetVsBtnDefault = 20;
            if (bb_3BetVsBtn == null) bb_3BetVsBtn = bb_3BetVsBtnDefault;

            var merged_3BetVsBtn = sb_3BetVsBtn + bb_3BetVsBtn;

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
                .Do(l => CheckDecision(heroHand, "BTN_OPEN_8-13bb", merged_3BetVsBtn, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Button)
                .HeroPreflopState(HeroPreflopState.Open)
                .HeroStackBetween(13, 17)
                .Do(l => CheckDecision(heroHand, "BTN_OPEN_13-17bb", merged_3BetVsBtn, PlMode.More));

            //checking, not deploy!
          //  lineInfo.StartRule().HeroPosition(PlayerPosition.Button)
           //    .HeroPreflopState(HeroPreflopState.Open)
           //    .HeroStackBetween(17, 100)
           //    .Do(l => CheckGtoDecision(heroHand, "SB_VS_BB_OPEN_2max_18bb_GTO"));


            lineInfo.StartRule().HeroPosition(PlayerPosition.Button)
                .HeroPreflopState(HeroPreflopState.Open)
                .HeroStackBetween(17, 100)
                .Do(l => CheckDecision(heroHand, "BTN_OPEN_17-100bb", 0, PlMode.None));

            //BTN CALL PUSH AFTER OPEN
            //SMALL STAKES
            lineInfo.StartRule().HeroPosition(PlayerPosition.Button).IsHu()
                .HeroPreflopState(HeroPreflopState.FacingPushVsOpen)
                .EffectiveStackBetween(0, 8)
                .Do(
                    l =>
                        CheckDecision(heroHand, "BTN_CALLPUSH_VSOPEN_0-11bb_UNK", l.Elements.EffectiveStack, PlMode.Less));
            //BTN CALL PUSHVS SB 
            lineInfo.StartRule().HeroPosition(PlayerPosition.Button).IsHu()
                .HeroPreflopState(HeroPreflopState.FacingPushVsOpen)
                .EffectiveStackBetween(8, 100)
                .OppPosition(PlayerPosition.Sb)
                .Do(l => CheckDecision(heroHand, "btn_callpush_vs_SB", l.Elements.EffectiveStack, PlMode.Less));
            //BTN CALL PUSHVS BB 
            lineInfo.StartRule().HeroPosition(PlayerPosition.Button).IsHu()
                .HeroPreflopState(HeroPreflopState.FacingPushVsOpen)
                .EffectiveStackBetween(8, 100)
                .OppPosition(PlayerPosition.Bb)
                .Do(l => CheckDecision(heroHand, "btn_callpush_vs_BB", l.Elements.EffectiveStack, PlMode.Less));

            #endregion

            #region SB

            //SB
            //STATS
            double? bb_3BetVsSb = null;
            if (lineInfo.Elements.HuOpp != null) bb_3BetVsSb = lineInfo.Elements.HuOpp.Stats.PfBb_3BetVsSb;
            double? bbDefVsSbsteal = null;
            if (lineInfo.Elements.HuOpp != null) bbDefVsSbsteal = lineInfo.Elements.HuOpp.Stats.PfBbDefVsSbsteal;

            double? bbRaiseLimper = null;
            if (lineInfo.Elements.HuOpp != null) bbRaiseLimper = lineInfo.Elements.HuOpp.Stats.PfRaiseLimper;
            if (bbRaiseLimper == null) bbRaiseLimper = 40;

            //SB OPEN COMMON(SMALL STACKES)
            //VS SMALL STACK
            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(0, 6)
                .VsSmallStack()
                .Do(l => CheckDecision(heroHand, "SB_OPENPUSH_NASH_EXPANDED", l.Elements.EffectiveStack, PlMode.Less));

            //VS OTHERS
            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(0, 6)
                .Do(l => CheckDecision(heroHand, "SB_OPENPUSH_NASH", l.Elements.EffectiveStack, PlMode.Less));


            //SB OPEN VS BB 2MAX HU PROB MODE
            /*
            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb)
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(7, 8)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_2MAX_7-8BB", bbRaiseLimper, PlMode.More));
                */

            //gto
            lineInfo.StartRule()
            .HeroPosition(PlayerPosition.Sb)
            .HeroRelativePosition(HeroRelativePosition.InPosition)
            .HeroPreflopState(HeroPreflopState.Open)
            .EffectiveStackBetween(6, 8)
            .Do(l => CheckGtoDecision(heroHand, "SB_VS_BB_OPEN_2max_7bb_GTO"));

            lineInfo.StartRule()
             .HeroPosition(PlayerPosition.Sb)
             .HeroRelativePosition(HeroRelativePosition.InPosition)
             .HeroPreflopState(HeroPreflopState.Open)
             .EffectiveStackBetween(6, 8)
             .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_2MAX_7-8BB_GTO", _probRand, PlMode.Less));

            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb)
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(8, 9)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_2MAX_8-9BB", bbRaiseLimper, PlMode.More));

            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb)
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(9, 11)
                .StatEqOrLessThan(bbDefVsSbsteal, 40)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_2MAX_9-11BB_SMALLDEF", bbRaiseLimper, PlMode.More));

            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb)
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(9, 11)
                .StatEqOrMoreThan(bbDefVsSbsteal, 60)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_2MAX_9-11BB_BIGDEF", bbRaiseLimper, PlMode.More));

            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb)
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(9, 11)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_2MAX_9-11BB", bbRaiseLimper, PlMode.More));

            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb)
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(11, 13)
                .StatEqOrLessThan(bbDefVsSbsteal, 43)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_2MAX_11-13BB_SMALLDEF", _probRand, PlMode.Less));

            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb)
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(11, 13)
                .StatEqOrMoreThan(bbDefVsSbsteal, 68)
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

            //gto
              lineInfo.StartRule()
                   .HeroPosition(PlayerPosition.Sb)
                 .HeroRelativePosition(HeroRelativePosition.InPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                  .EffectiveStackBetween(17, 19)
                  .Do(l => CheckGtoDecision(heroHand, "SB_VS_BB_OPEN_2max_18bb_GTO"));



            //vs regs
            if (lineInfo.Elements.HuOpp != null && (lineInfo.Elements.HuOpp.Type == PlayerType.GoodReg ||
                                                    lineInfo.Elements.HuOpp.Type == PlayerType.Maniac ||
                                                    lineInfo.Elements.HuOpp.Type == PlayerType.WeakReg ||
                                                    lineInfo.Elements.HuOpp.Type == PlayerType.UberReg ||
                                                    lineInfo.Elements.HuOpp.Stats.PfBb_3BetVsSb > 30))
            {
                //lineInfo.StartRule()
                //    .HeroPosition(PlayerPosition.Sb)
                //    .HeroRelativePosition(HeroRelativePosition.InPosition)
                //    .HeroPreflopState(HeroPreflopState.Open)
                //    .EffectiveStackBetween(17, 19)
                //    .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_2MAX_17-19BB_REGS", _probRand, PlMode.Less));


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
          //  lineInfo.StartRule()
         //       .HeroPosition(PlayerPosition.Sb)
           //     .HeroRelativePosition(HeroRelativePosition.InPosition)
          //      .HeroPreflopState(HeroPreflopState.Open)
          //      .EffectiveStackBetween(17, 19)
          //      .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_2MAX_17-19BB_UNK", 0, PlMode.None));

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
                .HeroPosition(PlayerPosition.Sb).IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(6, 9)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_7-9BB_UNK", _probRand, PlMode.Less));

            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb).IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(9, 11)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_9-11BB_UNK", _probRand, PlMode.Less));

            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb).IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(11, 13)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_11-13BB_UNK", _probRand, PlMode.Less));

            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb).IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(13, 15)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_13-15BB_UNK", _probRand, PlMode.Less));

            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb).IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(15, 17)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_15-17BB_UNK", _probRand, PlMode.Less));

            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb).IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(17, 19)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_17-19BB_UNK", _probRand, PlMode.Less));

            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb).IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(19, 22)
                .Do(l => CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_19-22BB_UNK", _probRand, PlMode.Less));

            #region SB OPEN VS BB 22-100BB 3MAX

            //SB OPEN VS BB  22-100 3MAX
            //VS SMALL BB DEF && BIG 3BET
            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb).IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(22, 100)
                .StatEqOrLessThan(bbDefVsSbsteal, 50)
                .StatEqOrMoreThan(bb_3BetVsSb, 30)
                .Do(
                    l =>
                        CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_22-100BB_VS_SMALL_BBDEF_BIG3BET", _probRand,
                            PlMode.Less));
            //VS SMALL BB DEF && BIG 3BET
            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb).IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(22, 100)
                .StatEqOrLessThan(bbDefVsSbsteal, 50)
                .Do(
                    l =>
                        CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_22-100BB_VS_SMALL_BBDEF_BIG3BET", _probRand,
                            PlMode.Less));
            //VS BIG BB DEF && BIG 3BET
            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb).IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(22, 100)
                .StatEqOrMoreThan(bbDefVsSbsteal, 75)
                .StatEqOrMoreThan(bb_3BetVsSb, 30)
                .Do(
                    l =>
                        CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_22-100BB_VS_SMALL_BBDEF_BIG3BET", _probRand,
                            PlMode.Less));
            //VS BIG BB DEF && BIG 3BET
            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb).IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .HeroPreflopState(HeroPreflopState.Open)
                .EffectiveStackBetween(22, 100)
                .StatEqOrMoreThan(bbDefVsSbsteal, 75)
                .Do(
                    l =>
                        CheckDecision(heroHand, "SB_OPEN_VS_BB_3MAX_22-100BB_VS_BIGBBDEF_UNK3BET", _probRand,
                            PlMode.Less));
            //UNK
            lineInfo.StartRule()
                .HeroPosition(PlayerPosition.Sb).IsHu()
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
            double? btnSteal = null;
            var buttonOpener = lineInfo.Elements.ActivePlayers.FirstOrDefault(p => p.Position == PlayerPosition.Button);
            if (buttonOpener != null) btnSteal = buttonOpener.Stats.PfBtnSteal;
            const double defaultBtnSteal = 35;
            if (btnSteal == null) btnSteal = defaultBtnSteal;


           // lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
            //  .HeroPreflopState(HeroPreflopState.FacingOpen)
            //  .OppBetSizeMinRaise()
            //  .EffectiveStackSbVsBtnBetween(0, 15)
            //  .Do(l => CheckGtoDecision(heroHand, "RangeName"));



            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .OppBetSizeMinRaise()
                .EffectiveStackSbVsBtnBetween(0, 15)
                .Do(l => CheckDecision(heroHand, "SB_VS_BTN_OPEN_0-15", lineInfo.Elements.EffectiveStack, PlMode.Less));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .OppBetSizeMinRaise()
                .EffectiveStackSbVsBtnBetween(15, 17)
                .Do(l => CheckDecision(heroHand, "SB_VS_BTN_OPEN_15-17BB", btnSteal, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .OppBetSizeMinRaise()
                .EffectiveStackSbVsBtnBetween(17, 20)
                .Do(l => CheckDecision(heroHand, "SB_VS_BTN_OPEN_17-20BB", btnSteal, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .OppBetSizeMinRaise()
                .EffectiveStackSbVsBtnBetween(20, 100)
                .Do(l => CheckDecision(heroHand, "SB_VS_BTN_OPEN_20-100BB", btnSteal, PlMode.More));


            //SB LIMP DEF VS BB RAISE
            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.FacingIsOvsLimp)
                .OppBetSize(2)
                .EffectiveStackBetween(7, 9)
                .Do(l => CheckDecision(heroHand, "SB_DefVSMinRaise_AfterLIMP_IP_7-9bb_UNK", null, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.FacingIsOvsLimp)
                .OppBetSize(2)
                .EffectiveStackBetween(9, 12)
                .Do(l => CheckDecision(heroHand, "SB_DefVSMinRaise_AfterLIMP_IP_9-12bb_UNK", null, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                 .HeroPreflopState(HeroPreflopState.FacingIsOvsLimp)
                 .OppBetSizeBetween(2,3)
                 .EffectiveStackBetween(16, 19)
                 .Do(l => CheckGtoDecision(heroHand, "SB_VS_BB_ISODEF_2max_18bb_GTO"));

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

            //gto

            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroPreflopState(HeroPreflopState.FacingPushVsLimp)
                .EffectiveStackBetween(16, 19)
                .Do(l => CheckGtoDecision(heroHand, "SB_VS_BB_ISOPUSHDEF_2max_18bb_GTO"));

            //lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
            //    .HeroPreflopState(HeroPreflopState.FacingPushVsLimp)
            //    .EffectiveStackBetween(16, 20)
            //    .Do(l => CheckDecision(heroHand, "SB_CALLPUSH_VSLIMP_16-20bb_UNK", null, PlMode.None));


            //SB VS BB 3BET
            //gto

  
            //gto
            lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                   .HeroPreflopState(HeroPreflopState.FacingPushVsOpen)
                   .EffectiveStackBetween(16, 19)
                   .Do(l => CheckGtoDecision(heroHand, "SB_VS_BB_PUSHDEF_2max_18bb_GTO"));
            

            if (bb_3BetVsSb == null || bb_3BetVsSb > 30)
            {
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
            else
            {
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
                    .Do(l => CheckDecision(heroHand, "SB_CALLPuSH_VSOPEN_10-13bb_EXPL", bb_3BetVsSb, PlMode.More));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroPreflopState(HeroPreflopState.FacingPushVsOpen)
                    .EffectiveStackBetween(13, 20)
                    .Do(l => CheckDecision(heroHand, "SB_CALLPuSH_VSOPEN_13-20bb_EXPL", bb_3BetVsSb, PlMode.More));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroPreflopState(HeroPreflopState.FacingPushVsOpen)
                    .EffectiveStackBetween(20, 100)
                    .Do(l => CheckDecision(heroHand, "SB_CALLPuSH_VSOPEN_20bb+_EXPL", bb_3BetVsSb, PlMode.More));
            }

            #endregion

            #region BB

            //BB
            //STATS
            double? limpfold = null;
            if (lineInfo.Elements.HuOpp != null) limpfold = lineInfo.Elements.HuOpp.Stats.PfLimpFold;
            const double defaultLimpfold = 60;
            if (limpfold == null) limpfold = defaultLimpfold;

            //BB VS SB LIMP 2MAX(OOP)
            // const double bigLimpFold = 78;

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .Is2Max()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(6, 8)
                .VsBigStack()
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_2MAX_6-8BB_V_BIG_STAKE", limpfold, PlMode.More));


            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .Is2Max()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(6, 8)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_2MAX_6-8BB", limpfold, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .Is2Max()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(8, 10)
                .VsBigStack()
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_2MAX_8-10BB_V_BIG_STAKE", limpfold, PlMode.More));


            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .Is2Max()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(8, 10)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_2MAX_8-10BB", limpfold, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .Is2Max()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(10, 13)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_2MAX_10-13BB", limpfold, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .Is2Max()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(13, 16)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_2MAX_13-16BB", limpfold, PlMode.More));

            //gto
            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
               .HeroPreflopState(HeroPreflopState.FacingLimp)
               .Is2Max()
               .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
               .EffectiveStackBetween(16, 20)
               .Do(l => CheckGtoDecision(heroHand, "BB_VS_SB_LIMP_2max_18bb_GTO"));


            //lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
            //    .HeroPreflopState(HeroPreflopState.FacingLimp)
            //    .Is2Max()
            //    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
             //   .EffectiveStackBetween(16, 20)
             //   .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_2MAX_16-20BB", limpfold, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .Is2Max()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(20, 100)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_2MAX_20-100BB", limpfold, PlMode.More));


            //BB VS SB LIMP 3MAX(IP)

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingLimp).IsHu()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .EffectiveStackBetween(6, 8)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_3MAX_6-8BB", limpfold, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingLimp).IsHu()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .EffectiveStackBetween(8, 10)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_3MAX_8-10BB", limpfold, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingLimp).IsHu()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .EffectiveStackBetween(10, 13)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_3MAX_10-13BB", limpfold, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingLimp).IsHu()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .EffectiveStackBetween(13, 16)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_3MAX_13-16BB", limpfold, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingLimp).IsHu()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .EffectiveStackBetween(16, 100)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_LIMP_3MAX_16-100BB", limpfold, PlMode.More));

       


            //BB VS BTN LIMP 3MAX HU

            if (lineInfo.Elements.BbAmt < 80)
            {
                //small blinds
                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHu()
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is3Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(0, 3)
                    .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_0-3BB_SMALLBLINDS", 0, PlMode.None));
            }
            else
            {
                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHu()
                    .HeroPreflopState(HeroPreflopState.FacingLimp)
                    .Is3Max()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .EffectiveStackBetween(0, 3)
                    .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_0-3BB_BIGBLINDS", 0, PlMode.None));
            }

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHu()
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .Is3Max()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(3, 6)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_3-6bb", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHu()
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .Is3Max()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(6, 8)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_6-8bb", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHu()
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .Is3Max()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(8, 10)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_8-10bb", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHu()
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .Is3Max()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(10, 13)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_10-13bb", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHu()
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .Is3Max()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(13, 16)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_13-16bb", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHu()
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .Is3Max()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(16, 20)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_16-20bb", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).IsHu()
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .Is3Max()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(20, 100)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_LIMP_20-100bb", 0, PlMode.None));


            //BB FACING LIMP COMMON HU
            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .IsHu()
                .EffectiveStackBetween(0, 4)
                .Do(l => CheckDecision(heroHand, "BB_FacingLimp_HU_0_4bb", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .IsHu()
                .EffectiveStackBetween(4, 6)
                .Do(l => CheckDecision(heroHand, "BB_FacingLimp_HU_4_6bb", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingLimp)
                .IsHu()
                .EffectiveStackBetween(6, 8)
                .Do(l => CheckDecision(heroHand, "BB_FacingLimp_HU_6_8bb", 0, PlMode.None));

            //BB VS BTN OPEN MINR
            double? btnStealBb = null;
            if (lineInfo.Elements.HuOpp != null) btnStealBb = lineInfo.Elements.HuOpp.Stats.PfBtnSteal;
            const double defaultBtnStealBb = 35;
            if (btnStealBb == null) btnStealBb = defaultBtnStealBb;


            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(0, 3)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_0-3BB", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(3, 7)
                .BbEqOrMoreThen(80)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_3-7BB_BIGBLINDS", btnStealBb, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(3, 7)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_3-7BB_SMALLBLINDS", btnStealBb, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(7, 9)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_7-9BB", btnStealBb, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(9, 11)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_9-11BB", btnStealBb, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(11, 13)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_11-13BB", btnStealBb, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(13, 15)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_13-15BB", btnStealBb, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(15, 17)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_15-17BB", btnStealBb, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(17, 20)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_17-20BB", btnStealBb, PlMode.More));


            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(20, 100)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_20-100BB", btnStealBb, PlMode.More));


            //BB VS BTN OPEN BIG

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeBetween(2, 3)
                .EffectiveStackBetween(5, 9)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_5-9_BIG", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeBetween(2, 3)
                .EffectiveStackBetween(9, 11)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_9-11_BIG", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeBetween(2.5, 3)
                .EffectiveStackBetween(11, 15)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_11-15_BIG", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeBetween(2.5, 3)
                .EffectiveStackBetween(15, 17)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_15-17_BIG", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeBetween(2.5, 4)
                .EffectiveStackBetween(17, 20)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_17-20_BIG", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is3Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeBetween(2.5, 4)
                .EffectiveStackBetween(20, 100)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPEN_20-100_BIG", 0, PlMode.None));


            //BB VS SB OPEN MINR 3MAX

            double? sbSteal = null;
            if (lineInfo.Elements.HuOpp != null) sbSteal = lineInfo.Elements.HuOpp.Stats.PfSbSteal;
            if (sbSteal == null) sbSteal = 41;

            double? foldTo3Bet = null;
            if (lineInfo.Elements.HuOpp != null)
            {
                foldTo3Bet = lineInfo.Elements.HuOpp.Stats.PfFold_3Bet;
                const double defaultFoldTo3Bet = 65;
                if (foldTo3Bet == null) foldTo3Bet = defaultFoldTo3Bet;
                if (lineInfo.Elements.HuOpp.Stack >= 2*lineInfo.Elements.HeroPlayer.Stack) foldTo3Bet -= 5;
            }


            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHu()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSize(2)
                .BbEqOrMoreThen(60)
                .EffectiveStackBetween(5, 8)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_5-8bb_3max_BIGBLINDS", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHu()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(0, 8)
                .Do(l => CheckDecision(heroHand, "COMMON_PushFoldVsOpen_NASH", l.Elements.EffectiveStack, PlMode.Less));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
              .HeroPreflopState(HeroPreflopState.FacingOpen)
              .IsHu()
              .HeroRelativePosition(HeroRelativePosition.InPosition)
              .OppBetSize(2)
              .BbEqOrMoreThen(60)
              .EffectiveStackBetween(8, 10)
              .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_8-10bb_3max_BIGBLINDS", 0, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHu()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(8, 10)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_8-10bb_3max_SMALLBLINDS", sbSteal, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHu()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(10, 13)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_10-13bb_3max", sbSteal, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHu()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(10, 13)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_10-13bb_3max", sbSteal, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHu()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(13, 16)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_13-16bb_3max", sbSteal, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHu()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(16, 20)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_16-20bb_3max", sbSteal, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHu()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(20, 100)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_20-100bb_3max", sbSteal, PlMode.More));





            //BB VS SB OPEN BIG 3max
            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHu()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSizeBetween(2, 3)
                .EffectiveStackBetween(8, 10)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_BIG_8-10bb_3max", null, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHu()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSizeBetween(2, 3)
                .EffectiveStackBetween(10, 13)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_BIG_10-13bb_3max", null, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHu()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSizeBetween(2.5, 3)
                .EffectiveStackBetween(13, 16)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_BIG_13-16bb_3max", null, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHu()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSizeBetween(2.5, 4)
                .EffectiveStackBetween(16, 20)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_BIG_16-20bb_3max", null, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHu()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .OppBetSizeBetween(2.5, 4)
                .EffectiveStackBetween(20, 100)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_BIG_20-100bb_3max", null, PlMode.None));


            //BB VS SB OPEN MINR 2MAX HU
            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(0, 7)
                .Do(l => CheckDecision(heroHand, "COMMON_PushFoldVsOpen_NASH", l.Elements.EffectiveStack, PlMode.Less));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(7, 9)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_2MAX_7-9BB", sbSteal, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSize(2)
                .EffectiveStackBetween(9, 11)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_2MAX_9-11BB", sbSteal, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(11, 13)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_2MAX_11-13BB", sbSteal, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(13, 15)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_2MAX_13-15BB", sbSteal, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(15, 17)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_2MAX_15-17BB", sbSteal, PlMode.More));

            //gto
            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
              .HeroPreflopState(HeroPreflopState.FacingOpen).IsHu()
              .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
              .OppBetSizeMinRaise()
              .EffectiveStackBetween(17, 20)
              .Do(l => CheckGtoDecision(heroHand, "BB_VS_SB_OPEN_2max_18bb_GTO"));

           // lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
           //     .HeroPreflopState(HeroPreflopState.FacingOpen).IsHu()
           //     .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
           //     .OppBetSizeMinRaise()
           //     .EffectiveStackBetween(17, 20)
           //     .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_2MAX_17-20BB", sbSteal, PlMode.More));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen).IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(20, 100)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_2MAX_20-100BB", sbSteal, PlMode.More));

            //BB VS SB OPEN BIG 2max

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeBetween(2, 3)
                .EffectiveStackBetween(8, 10)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_BIG_8-10bb_2max", null, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeBetween(2, 3)
                .EffectiveStackBetween(10, 13)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_BIG_10-13bb_2max", null, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeBetween(2.5, 3)
                .EffectiveStackBetween(13, 16)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_BIG_13-16bb_2max", null, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeBetween(2.5, 4)
                .EffectiveStackBetween(16, 20)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_BIG_16-20bb_2max", null, PlMode.None));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max()
                .HeroPreflopState(HeroPreflopState.FacingOpen)
                .IsHu()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .OppBetSizeBetween(2.5, 4)
                .EffectiveStackBetween(20, 100)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_OPEN_BIG_20-100bb_2max", null, PlMode.None));


            double? openPush = null;
            if (lineInfo.Elements.HuOpp != null) openPush = lineInfo.Elements.HuOpp.Stats.PfOpenpush;

            //BB vs BTN CALL OPEN PUSH

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                .IsHu()
                .OppPosition(PlayerPosition.Button)
                .EffectiveStackBetween(0, 8)
                .BbEqOrLessThen(60)
                .Do(l => CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb", l.Elements.EffectiveStack, PlMode.Less));

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                .IsHu()
                .OppPosition(PlayerPosition.Button)
                .EffectiveStackBetween(0, 4)
                .BbEqOrMoreThen(80)
                .Do(
                    l =>
                        CheckDecision(heroHand, "COMMON_CALL_VS_OPENPUSH_SKLANSKY-CHEBUKOV", l.Elements.EffectiveStack,
                            PlMode.Less));

            //change from eff stack to hu opp stack
            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                .IsHu()
                .OppPosition(PlayerPosition.Button)
                .EffectiveStackBetween(4, 100)
                .Do(l => CheckDecision(heroHand, "BB_VS_BTN_OPENPUSH_4-100bb_UNK", l.Elements.HuOpp.Stack, PlMode.Less));


            //BB VS SB CALL OPEN PUSH 3max hu

            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroPreflopState(HeroPreflopState.FacingOpenPush).Is3Max().IsHu()
                .OppPosition(PlayerPosition.Sb)
                .EffectiveStackBetween(0, 100)
                .Do(l => CheckDecision(heroHand, "BB_VS_SB_3max_CALLOPENSHOVE_GRIZZ", l.Elements.EffectiveStack,
                    PlMode.Less));

            //BB VS SB CALL OPEN PUSH HU 2max
            lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHu()
                .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                .EffectiveStackBetween(8, 100)
                .Do(
                    l =>
                        CheckDecision(heroHand, "BB_VS_SB_2max_CALLOPENSHOVE_GRIZZ", l.Elements.EffectiveStack,
                            PlMode.Less));


            //bb  vs sb call openpush HU 0-8bb
            const double openPushSeparator08Bb = 30.0;
            const int multiplierSeparator = 9;
            if (lineInfo.Elements.TourneyMultiplier <= multiplierSeparator)
            {
                // small multiplier

                #region smallMultiplicator

                //big blinds

                #region bigblinds

                if (openPush >= openPushSeparator08Bb)
                {
                    // vs agr
                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHu()
                        .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                        .BbEqOrMoreThen(60)
                        .EffectiveStackBetween(5, 8)
                        .Do(l => CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb"
                            , l.Elements.EffectiveStack, PlMode.Less));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHu()
                        .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                        .BbEqOrMoreThen(80)
                        .EffectiveStackBetween(4, 5)
                        .Do(l => CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb"
                            , l.Elements.EffectiveStack, PlMode.Less));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHu()
                        .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                        .BbEqOrMoreThen(100)
                        .EffectiveStackBetween(0, 3)
                        .Do(l => CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb"
                            , l.Elements.EffectiveStack, PlMode.Less));
                }
                else if (openPush < openPushSeparator08Bb)
                {
                    // vs nit
                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHu()
                        .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                        .BbEqOrMoreThen(60)
                        .EffectiveStackBetween(5, 8)
                        .Do(l => CheckDecision(heroHand, "COMMON_CALL_VS_OPENPUSH_SKLANSKY-CHEBUKOV"
                            , l.Elements.EffectiveStack, PlMode.Less));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHu()
                        .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                        .BbEqOrMoreThen(80)
                        .EffectiveStackBetween(4, 5)
                        .Do(l => CheckDecision(heroHand, "COMMON_CALL_VS_OPENPUSH_SKLANSKY-CHEBUKOV"
                            , l.Elements.EffectiveStack, PlMode.Less));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHu()
                        .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                        .BbEqOrMoreThen(100)
                        .EffectiveStackBetween(0, 3)
                        .Do(l => CheckDecision(heroHand, "COMMON_CALL_VS_OPENPUSH_SKLANSKY-CHEBUKOV"
                            , l.Elements.EffectiveStack, PlMode.Less));
                }
                else
                {
                    //vs unk
                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHu()
                        .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                        .BbEqOrMoreThen(60)
                        .EffectiveStackBetween(5, 8)
                        .Do(l => CheckDecision(heroHand, "COMMON_CALL_VS_OPENPUSH_SKLANSKY-CHEBUKOV"
                            , l.Elements.EffectiveStack, PlMode.Less));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHu()
                        .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                        .BbEqOrMoreThen(80)
                        .EffectiveStackBetween(4, 5)
                        .Do(l => CheckDecision(heroHand, "COMMON_CALL_VS_OPENPUSH_SKLANSKY-CHEBUKOV"
                            , l.Elements.EffectiveStack, PlMode.Less));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHu()
                        .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                        .BbEqOrMoreThen(100)
                        .EffectiveStackBetween(0, 3)
                        .Do(l => CheckDecision(heroHand, "COMMON_CALL_VS_OPENPUSH_SKLANSKY-CHEBUKOV"
                            , l.Elements.EffectiveStack, PlMode.Less));
                }

                #endregion

                //small blinds
                if (openPush >= openPushSeparator08Bb)
                {
                    // vs agr

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHu()
                        .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                        .EffectiveStackBetween(0, 8)
                        .Do(l => CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb"
                            , l.Elements.EffectiveStack, PlMode.Less));


                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHu()
                        .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                        .EffectiveStackBetween(0, 8)
                        .Do(l => CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb"
                            , l.Elements.EffectiveStack, PlMode.Less));
                }
                else if (openPush < openPushSeparator08Bb)
                {
                    // vs nit
                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHu()
                        .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                        .EffectiveStackBetween(6, 8)
                        .Do(l => CheckDecision(heroHand, "COMMON_CALL_VS_OPENPUSH_SKLANSKY-CHEBUKOV"
                            , l.Elements.EffectiveStack, PlMode.Less));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHu()
                        .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                        .EffectiveStackBetween(6, 0)
                        .Do(l => CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb"
                            , l.Elements.EffectiveStack, PlMode.Less));
                }
                else
                {
                    //vs unk
                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHu()
                        .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                        .EffectiveStackBetween(0, 8)
                        .Do(l => CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb"
                            , l.Elements.EffectiveStack, PlMode.Less));
                }

                #endregion
            }
            else
            {
                // big multiplier

                #region bigMultiplicator

                #region bigblinds

                //big blinds
                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHu()
                    .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                    .BbEqOrMoreThen(60)
                    .EffectiveStackBetween(5, 8)
                    .Do(l => CheckDecision(heroHand, "COMMON_CALL_VS_OPENPUSH_SKLANSKY-CHEBUKOV"
                        , l.Elements.EffectiveStack, PlMode.Less));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHu()
                    .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                    .BbEqOrMoreThen(80)
                    .EffectiveStackBetween(4, 5)
                    .Do(l => CheckDecision(heroHand, "COMMON_CALL_VS_OPENPUSH_SKLANSKY-CHEBUKOV"
                        , l.Elements.EffectiveStack, PlMode.Less));

                lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHu()
                    .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                    .BbEqOrMoreThen(100)
                    .EffectiveStackBetween(0, 3)
                    .Do(l => CheckDecision(heroHand, "COMMON_CALL_VS_OPENPUSH_SKLANSKY-CHEBUKOV"
                        , l.Elements.EffectiveStack, PlMode.Less));

                #endregion

                //small blinds
                if (openPush >= openPushSeparator08Bb)
                {
                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHu()
                        .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                        .EffectiveStackBetween(0, 8)
                        .Do(l => CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb"
                            , l.Elements.EffectiveStack, PlMode.Less));
                }
                else if (openPush < openPushSeparator08Bb)
                {
                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHu()
                        .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                        .EffectiveStackBetween(6, 8)
                        .Do(l => CheckDecision(heroHand, "COMMON_CALL_VS_OPENPUSH_SKLANSKY-CHEBUKOV"
                            , l.Elements.EffectiveStack, PlMode.Less));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHu()
                        .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                        .EffectiveStackBetween(0, 6)
                        .Do(l => CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb"
                            , l.Elements.EffectiveStack, PlMode.Less));
                }
                else
                {
                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHu()
                        .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                        .EffectiveStackBetween(7, 8)
                        .Do(l => CheckDecision(heroHand, "COMMON_CALL_VS_OPENPUSH_SKLANSKY-CHEBUKOV"
                            , l.Elements.EffectiveStack, PlMode.Less));

                    lineInfo.StartRule().HeroPosition(PlayerPosition.Bb).Is2Max().IsHu()
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
                .IsHu()
                .EffectiveStackBetween(0, 8)
                .Do(l => CheckDecision(heroHand, "COMMON_PushFoldVsOpen_NASH", l.Elements.EffectiveStack, PlMode.Less));


            lineInfo.StartRule()
                .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                .IsHu()
                .EffectiveStackBetween(0, 8)
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .Do(l => CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb", l.Elements.EffectiveStack, PlMode.Less));


            lineInfo.StartRule()
                .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                .IsHu()
                .EffectiveStackBetween(8, 100)
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .Do(
                    l =>
                        CheckDecision(heroHand, "COMMON_FacingPush_HU_8_25bb", l.Elements.EffectiveStack,
                            PlMode.Less));


            lineInfo.StartRule()
                .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                .IsHu()
                .EffectiveStackBetween(0, 11)
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .Do(
                    l =>
                        CheckDecision(heroHand, "COMMON_FacingPush_HU_08bb", l.Elements.EffectiveStack, PlMode.Less));

            //TODO переделать со статами!!
            lineInfo.StartRule()
                .HeroPreflopState(HeroPreflopState.FacingOpenPush)
                .IsHu()
                .EffectiveStackBetween(11, 100)
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .Do(
                    l =>
                        CheckDecision(heroHand, "COMMON_FacingPush_HU_8_25bb", l.Elements.EffectiveStack,
                            PlMode.Less));

            #endregion

            if (_isNewDecision == false)
            {
                _preflopDecision = PreflopDecision.None;
                _preflopRangeName = "";
            }

            var decisionInfo = new DecisionInfo
            {
                LineInfo = lineInfo,
                PreflopRangeChosen = _preflopRangeName,
                PreflopDecision = _preflopDecision,
                RaiseSize = _raiseSize,
                PotOdds = FindPotOdds(lineInfo.Elements)
            };

            if (NewDecisionInfo != null)
            {
                NewDecisionInfo(decisionInfo);
            }
        }

        private Foldto3Bet ChooseFoldTo3BetCategory(double? foldTo3Bet, Elements elements)
        {
            if (foldTo3Bet == null) return Foldto3Bet.None;
            if (elements.EffectiveStack > 15)
            {
                if (foldTo3Bet >= 77) return Foldto3Bet.Big;
                if (foldTo3Bet >= 65) return Foldto3Bet.Average;
                return Foldto3Bet.Small;
            }
            if (foldTo3Bet >= 70) return Foldto3Bet.Big;
            if (foldTo3Bet >= 60) return Foldto3Bet.Average;
            return Foldto3Bet.Small;
        }

        private double FindPotOdds(Elements elements)
        {
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

        private void CheckDecision(Hand heroHand, string rangeName, double stat, PlMode plMode)
        {
            if (_isNewDecision || heroHand == null) return;
            var range = _rangesList.FirstOrDefault(r => r.Name == rangeName);
            _preflopRangeName = rangeName;

            if (range != null)
            {
                if (heroHand.Name == "" || heroHand.Name.Length != 4)
                {
                    Debug.WriteLine("wrong heroHand Name");
                    return;
                }

                var decision1 = (PreflopDecision) range.Hands.First(n => n.Name == heroHand.Name).D1;
                var decision2 = (PreflopDecision) range.Hands.First(n => n.Name == heroHand.Name).D2;

                var statRange = range.Hands.First(n => n.Name == heroHand.Name).S1;

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
                Debug.WriteLine(string.Format("can't find range {0}", rangeName));
            }
        }

        private void CheckDecision(Hand heroHand, string rangeName, double? stat, PlMode plMode)
        {
            if (_isNewDecision || heroHand == null) return;
            var range = _rangesList.FirstOrDefault(r => r.Name == rangeName);
            _preflopRangeName = rangeName;

            if (range != null)
            {
                if (heroHand.Name == "" || heroHand.Name.Length != 4)
                {
                    //Debug.WriteLine("wrong heroHand Name");
                    return;
                }

                var decision1 = (PreflopDecision) range.Hands.First(n => n.Name == heroHand.Name).D1;
                var decision2 = (PreflopDecision) range.Hands.First(n => n.Name == heroHand.Name).D2;

                var statRange = range.Hands.First(n => n.Name == heroHand.Name).S1;

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
                Debug.WriteLine(string.Format("can't find range {0}", rangeName));
            }
        }

        private void CheckGtoDecision(Hand heroHand, string rangeName) {
            if (_isNewDecision || heroHand == null) return;
            var range = _gtoRangesList.FirstOrDefault(r => r.Name == rangeName);
            _preflopRangeName = rangeName;
            if (range != null)
            {
                if (heroHand.Name == "" || heroHand.Name.Length != 4) return;
                var gtoHand = range.Hands.First(n => n.Name == heroHand.Name);

                CountProbRanges(gtoHand);

                if (_probRand > 0 && _probRand <= _probRanges[0][1])
                {
                    _preflopDecision = (PreflopDecision) gtoHand.Decisions[0].Value;
                    _raiseSize = gtoHand.Decisions[0].Size;

                } else if (_probRand > _probRanges[1][0] && _probRand <= _probRanges[1][1])
                {
                    _preflopDecision = (PreflopDecision)gtoHand.Decisions[1].Value;
                    _raiseSize = gtoHand.Decisions[1].Size;
                } else if (_probRand > _probRanges[2][0] && _probRand <= _probRanges[2][1])
                {
                    _preflopDecision = (PreflopDecision)gtoHand.Decisions[2].Value;
                    _raiseSize = gtoHand.Decisions[2].Size;
                } else if (_probRand > _probRanges[3][0] && _probRand <= _probRanges[3][1])
                {
                    _preflopDecision = (PreflopDecision)gtoHand.Decisions[3].Value;
                    _raiseSize = gtoHand.Decisions[3].Size;
                }
                _isNewDecision = true;
            }
            else
            {
                Debug.WriteLine(string.Format("can't find range {0}", rangeName));
            }
        }

        private  double GetRandomNumber(double minimum, double maximum)
        {
        
            return _propapilityRandomizer.NextDouble() * (maximum - minimum) + minimum;
        }
        private void CountProbRanges(GtoRangeHand hand)
        {
            _probRanges[0][0] = 0;
            _probRanges[0][1] = hand.Decisions[0].Probability;
            _probRanges[1][0] = _probRanges[0][1];
            _probRanges[1][1] = _probRanges[1][0] + hand.Decisions[1].Probability;
            _probRanges[2][0] = _probRanges[1][1];
            _probRanges[2][1] = _probRanges[2][0] + hand.Decisions[2].Probability;
            _probRanges[3][0] = _probRanges[2][1];
            _probRanges[3][1] = _probRanges[3][0] + hand.Decisions[3].Probability;
        }


        private void LoadRanges(string rangesPath)
        {
            _rangesList = new List<Range>();
            foreach (var file in Directory.GetFiles(rangesPath, "*.xml", SearchOption.AllDirectories))
            {
                _rangesList.Add(XmlRangeHelper.Load(file));
            }
        }

        private void LoadGtoRanges(string gtoRangesPath) {
            _gtoRangesList = new List<GtoRange>();
            foreach (var file in Directory.GetFiles(gtoRangesPath, "*.xml", SearchOption.AllDirectories))
            {
                _gtoRangesList.Add(XmlGtoRangeHelper.Load(file));
            }
        }

        private enum PlMode
        {
            None,
            More,
            Less
        };

        private enum Foldto3Bet
        {
            None,
            Small,
            Average,
            Big
        };
    }
}