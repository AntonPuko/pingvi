using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using PokerModel;

namespace Pingvi
{
    public enum PlMode {
        None, More, Less
    }
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

            //COMMON PRE

            elements.StartRule().HeroRole(HeroRole.Opener)
               .SitOutOpp()
               .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.HeroPlayer.Stack, PlMode.Less, "COMMON_OPEN_100"));


            #region BTN
            //OPEN 
            elements.StartRule().HeroPosition(PlayerPosition.Button)
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                .HeroStackBetween(8, 100)
                .EffectiveStackBetween(0, 8)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "BTN_OPEN_0_8bb_EFFSTACK"));

            elements.StartRule().HeroPosition(PlayerPosition.Button)
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                .HeroStackBetween(0, 4)
                .EffectiveStackBetween(0, 100)
                .BBEqOrMoreThen(60)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.More, "BTN_OPEN_0_4bb_HEROSTACK_BIGBB"));

            elements.StartRule().HeroPosition(PlayerPosition.Button)
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                .HeroStackBetween(4, 100)
                .EffectiveStackBetween(0, 6)
                .BBEqOrMoreThen(60)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.More, "BTN_OPEN_0_6bb_EFFSTACK_BIGBB"));

            elements.StartRule().HeroPosition(PlayerPosition.Button)
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                .HeroStackBetween(4, 100)
                .EffectiveStackBetween(6, 8)
                .BBEqOrMoreThen(60)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.More, "BTN_OPEN_6_8bb_EFFSTACK_BIGBB"));

           

            elements.StartRule().HeroPosition(PlayerPosition.Button)
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                .HeroStackBetween(0, 8)
                .EffectiveStackBetween(0, 8)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.More, "BTN_OPEN_0_8bb_HEROSTACK"));

            elements.StartRule().HeroPosition(PlayerPosition.Button)
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                .HeroStackBetween(8,11)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "BTN_OPEN_8_11bb_HEROSTACK"));

            elements.StartRule().HeroPosition(PlayerPosition.Button)
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                .HeroStackBetween(11,13)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "BTN_OPEN_11_13bb_HEROSTACK"));

            elements.StartRule().HeroPosition(PlayerPosition.Button)
               .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
               .HeroStackBetween(13, 100)
               .EffectiveStackBetween(8, 11)
               .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "BTN_OPEN_8_11bb_EFFSTACK"));

            elements.StartRule().HeroPosition(PlayerPosition.Button)
               .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
               .HeroStackBetween(13, 100)
               .EffectiveStackBetween(11, 13)
               .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "BTN_OPEN_11_13bb_EFFSTACK"));


            elements.StartRule().HeroPosition(PlayerPosition.Button)
               .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
               .HeroStackBetween(13,100)
               .EffectiveStackBetween(11, 13)
               .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "BTN_OPEN_13bb+"));



            elements.StartRule().HeroPosition(PlayerPosition.Button)
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                .EffectiveStackBetween(13, 100)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "BTN_OPEN_13bb+"));

            //CALL PUSH VS OPEN HU
            double _3betStatBtn = 0;
            if (elements.HuOpp != null && elements.HuOpp.Position == PlayerPosition.Sb) _3betStatBtn = elements.HuOpp.Stats.PF_SB_3BET_VS_BTN;
            if (elements.HuOpp != null && elements.HuOpp.Position == PlayerPosition.Bb) _3betStatBtn = elements.HuOpp.Stats.PF_BB_3BET_VS_BTN;

            if (_3betStatBtn == 0)
            {
                elements.StartRule().HeroPosition(PlayerPosition.Button).IsHU()
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.FacingPush)
                    .EffectiveStackBetween(0, 11)
                    .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingPush, elements.EffectiveStack,
                        PlMode.More, "BTN_CALLPUSH_VSOPEN_0-11bb_UNK"));

                elements.StartRule().HeroPosition(PlayerPosition.Button).IsHU()
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.FacingPush)
                    .EffectiveStackBetween(11, 100)
                    .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingPush, elements.EffectiveStack,
                        PlMode.More, "BTN_CALLPUSH_VSOPEN_11bb+_UNK"));

            }
            else
            {
                elements.StartRule().HeroPosition(PlayerPosition.Button).IsHU()
                  .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.FacingPush)
                  .EffectiveStackBetween(0, 10)
                  .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingPush, elements.EffectiveStack,
                      PlMode.More, "BTN_CALLPUSH_VSOPEN_0-11bb_UNK"));

                elements.StartRule().HeroPosition(PlayerPosition.Button).IsHU()
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.FacingPush)
                    .EffectiveStackBetween(10, 13)
                    .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingPush, _3betStatBtn,
                        PlMode.Less, "BTN_CALLPUSH_VSOPEN_10-13bb_EXPL"));

                elements.StartRule().HeroPosition(PlayerPosition.Button).IsHU()
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.FacingPush)
                    .EffectiveStackBetween(13, 20)
                    .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingPush, _3betStatBtn,
                        PlMode.Less, "BTN_CALLPUSH_VSOPEN_13-20bb_EXPL"));

                elements.StartRule().HeroPosition(PlayerPosition.Button).IsHU()
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.FacingPush)
                    .EffectiveStackBetween(20, 100)
                    .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingPush, _3betStatBtn,
                        PlMode.Less, "BTN_CALLPuSH_VSOPEN_20bb+_EXPL"));
            }





            #endregion

          

            #region SB

            double _3betStatSB = 0;
            if (elements.HuOpp != null) _3betStatSB = elements.HuOpp.Stats.PF_BB_3BET_VS_SB;
            double _DefStatSB = 0;
            if (elements.HuOpp != null) _DefStatSB = elements.HuOpp.Stats.PF_BB_DEF_VS_SBSTEAL;
            double _FoldCBIP = 0;
            if (elements.HuOpp != null) _FoldCBIP = elements.HuOpp.Stats.F_FOLD_CBET;

            //OPEN COMMON
            elements.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open).HeroRelativePosition(HeroRelativePosition.InPosition)
                .EffectiveStackBetween(0, 7)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.More, "SB_OPEN_0_8bb"));

            elements.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open).HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(0, 8)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.More, "SB_OPEN_0_8bb"));

             //OPEN IP
              //OPEN IP VS LAG


            //Open Unex IP
            elements.StartRule().HeroPosition(PlayerPosition.Sb).HeroRelativePosition(HeroRelativePosition.InPosition)
                   .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                   .EffectiveStackBetween(7, 8)
                   .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "SB_OPEN_IP_7-8bb_UNEX"));

            elements.StartRule().HeroPosition(PlayerPosition.Sb).HeroRelativePosition(HeroRelativePosition.InPosition)
                   .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                   .EffectiveStackBetween(8, 9)
                   .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "SB_OPEN_IP_8-9bb_UNEX"));

            elements.StartRule().HeroPosition(PlayerPosition.Sb).HeroRelativePosition(HeroRelativePosition.InPosition)
                 .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                 .EffectiveStackBetween(9, 12)
                 .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "SB_OPEN_IP_9-12bb_UNEX"));


            if (_3betStatSB > 35)
            {
                elements.StartRule().HeroPosition(PlayerPosition.Sb).HeroRelativePosition(HeroRelativePosition.InPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(8, 11)
                    .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "SB_OPEN_IP_8-11bb_LAG"));

                elements.StartRule().HeroPosition(PlayerPosition.Sb).HeroRelativePosition(HeroRelativePosition.InPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(11, 13)
                    .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "SB_OPEN_IP_11-13bb_LAG"));

                elements.StartRule().HeroPosition(PlayerPosition.Sb).HeroRelativePosition(HeroRelativePosition.InPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(13, 15)
                    .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "SB_OPEN_IP_13-15bb_LAG"));

                elements.StartRule().HeroPosition(PlayerPosition.Sb).HeroRelativePosition(HeroRelativePosition.InPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(15, 20)
                    .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "SB_OPEN_IP_15-20bb_LAG"));

                elements.StartRule().HeroPosition(PlayerPosition.Sb).HeroRelativePosition(HeroRelativePosition.InPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(20, 100)
                    .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "SB_OPEN_IP_20bb+_LAG"));
            }
              //OPEN IP UNK
            elements.StartRule().HeroPosition(PlayerPosition.Sb).HeroRelativePosition(HeroRelativePosition.InPosition)
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                .EffectiveStackBetween(8, 11)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "SB_OPEN_IP_8_11bb_UNK"));

            elements.StartRule().HeroPosition(PlayerPosition.Sb).HeroRelativePosition(HeroRelativePosition.InPosition)
               .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
               .EffectiveStackBetween(11, 13)
               .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "SB_OPEN_IP_11_13bb_UNK"));

            elements.StartRule().HeroPosition(PlayerPosition.Sb).HeroRelativePosition(HeroRelativePosition.InPosition)
               .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
               .EffectiveStackBetween(13, 15)
               .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "SB_OPEN_IP_13_15bb_UNK"));

            elements.StartRule().HeroPosition(PlayerPosition.Sb).HeroRelativePosition(HeroRelativePosition.InPosition)
               .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
               .EffectiveStackBetween(15, 17)
               .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "SB_OPEN_IP_15_17bb_UNK"));

            elements.StartRule().HeroPosition(PlayerPosition.Sb).HeroRelativePosition(HeroRelativePosition.InPosition)
               .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
               .EffectiveStackBetween(17, 100)
               .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "SB_OPEN_IP_17bb+_UNK"));


            //OPEN OOP

            //OPEN VS NIT OR LOOSEPASSIVE ( DEF < 50% || (3bet < 25 && Fold to cbet > 60)  )

            if (_DefStatSB != 0 && _DefStatSB < 50 || (_DefStatSB >= 50 && _FoldCBIP >60 && _3betStatSB < 25)) {

                elements.StartRule().HeroPosition(PlayerPosition.Sb).HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                   .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                   .EffectiveStackBetween(8, 11)
                   .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "SB_OPEN_OOP_8-11bb_ROCKorLP"));

                elements.StartRule().HeroPosition(PlayerPosition.Sb).HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(11, 13)
                    .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "SB_OPEN_OOP_11-13bb_ROCKorLP"));

                elements.StartRule().HeroPosition(PlayerPosition.Sb).HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(13, 15)
                    .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "SB_OPEN_OOP_13-15bb_ROCKorLP"));

                elements.StartRule().HeroPosition(PlayerPosition.Sb).HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(15, 20)
                    .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "SB_OPEN_OOP_15-20bb_ROCKorLP"));

                elements.StartRule().HeroPosition(PlayerPosition.Sb).HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(20, 100)
                    .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "SB_OPEN_OOP_20bb+_ROCKorLP"));
                
            }

            //OPEN VS MANIAC( 3bet> 42)

            if (_3betStatSB > 40) {
                elements.StartRule().HeroPosition(PlayerPosition.Sb).HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(8, 11)
                    .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "SB_OPEN_OOP_8-11bb_LAG"));

                elements.StartRule().HeroPosition(PlayerPosition.Sb).HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(11, 13)
                    .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "SB_OPEN_OOP_11-13bb_LAG"));

                elements.StartRule().HeroPosition(PlayerPosition.Sb).HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(13, 15)
                    .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "SB_OPEN_OOP_13-15bb_LAG"));

                elements.StartRule().HeroPosition(PlayerPosition.Sb).HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(15, 20)
                    .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "SB_OPEN_OOP_15-20bb_LAG"));

                elements.StartRule().HeroPosition(PlayerPosition.Sb).HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
                    .EffectiveStackBetween(20, 100)
                    .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "SB_OPEN_OOP_20bb+_LAG"));
            }

            //OPEN OOP UNK
            elements.StartRule().HeroPosition(PlayerPosition.Sb).HeroRelativePosition(HeroRelativePosition.OutOfPosition)
              .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
              .EffectiveStackBetween(8, 11)
              .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "SB_OPEN_OOP_8-11bb_UNK"));

            elements.StartRule().HeroPosition(PlayerPosition.Sb).HeroRelativePosition(HeroRelativePosition.OutOfPosition)
              .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
              .EffectiveStackBetween(11, 13)
              .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "SB_OPEN_OOP_11-13bb_UNK"));

            elements.StartRule().HeroPosition(PlayerPosition.Sb).HeroRelativePosition(HeroRelativePosition.OutOfPosition)
              .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
              .EffectiveStackBetween(13, 15)
              .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "SB_OPEN_OOP_13-15bb_UNK"));

            elements.StartRule().HeroPosition(PlayerPosition.Sb).HeroRelativePosition(HeroRelativePosition.OutOfPosition)
              .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
              .EffectiveStackBetween(15, 20)
              .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "SB_OPEN_OOP_15-20bb_UNK"));

            elements.StartRule().HeroPosition(PlayerPosition.Sb).HeroRelativePosition(HeroRelativePosition.OutOfPosition)
              .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.Open)
              .EffectiveStackBetween(20, 100)
              .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.EffectiveStack, PlMode.Less, "SB_OPEN_OOP_20bb+_UNK"));

            //CALL PUSH AFTER LIMP
            elements.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.FacingPushVsLimp)
                .EffectiveStackBetween(0, 9)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingPush, elements.EffectiveStack,
                        PlMode.More, "SB_CALLPUSH_VSLIMP_7_9bb_UNK"));

            elements.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.FacingPushVsLimp)
                .EffectiveStackBetween(9, 12)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingPush, elements.EffectiveStack,
                        PlMode.More, "SB_CALLPUSH_VSLIMP_9-12bb_UNK"));


            //CALL PUSH AFTER OPEN
            
             

            if (_3betStatSB == 0)
            {
                elements.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.FacingPush)
                    .EffectiveStackBetween(0, 11)
                    .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingPush, elements.EffectiveStack,
                        PlMode.More, "SB_CALLPUSH_VSOPEN_0-11bb_UNK"));

                elements.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.FacingPush)
                    .EffectiveStackBetween(11, 100)
                    .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingPush, elements.EffectiveStack,
                        PlMode.More, "SB_CALLPUSH_VSOPEN_11bb+_UNK"));

            }
            else
            {
                elements.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.FacingPush)
                    .EffectiveStackBetween(0, 10)
                    .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingPush, elements.EffectiveStack,
                        PlMode.More, "SB_CALLPUSH_VSOPEN_0-11bb_UNK"));

                elements.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.FacingPush)
                    .EffectiveStackBetween(10, 13)
                    .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingPush, _3betStatSB,
                        PlMode.Less, "SB_CALLPuSH_VSOPEN_10-13bb_EXPL"));

                elements.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.FacingPush)
                    .EffectiveStackBetween(13, 20)
                    .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingPush, _3betStatSB,
                        PlMode.Less, "SB_CALLPuSH_VSOPEN_13-20bb_EXPL"));

                elements.StartRule().HeroPosition(PlayerPosition.Sb)
                    .HeroRole(HeroRole.Opener).HeroState(HeroStatePreflop.FacingPush)
                    .EffectiveStackBetween(20, 100)
                    .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingPush, _3betStatSB,
                        PlMode.Less, "SB_CALLPuSH_VSOPEN_20bb+_EXPL"));
            }


            //DEFEND VS OPENRAISE
            elements.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(8,12)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingOpen, 0, PlMode.None, "SB_FacingMinRaise_MP_8_12bb"));

            elements.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(12, 15)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingOpen, 0, PlMode.None, "SB_FacingMinRaise_MP_12_15bb"));

            elements.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(15, 17)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingOpen, 0, PlMode.None, "SB_FacingMinRaise_MP_15_17bb"));

            elements.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(17, 20)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingOpen, 0, PlMode.None, "SB_FacingMinRaise_MP_17_20bb"));

            elements.StartRule().HeroPosition(PlayerPosition.Sb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen)
                .OppBetSizeMinRaise()
                .EffectiveStackBetween(20, 100)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingOpen, 0, PlMode.None, "SB_FacingMinRaise_MP_20bb+"));


            

            #endregion

            #region BB
            //BB FACING LIMP
          
            //BB FACING LIMP IP
            elements.StartRule().HeroPosition(PlayerPosition.Bb)
               .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingLimp).IsHU()
               .HeroRelativePosition(HeroRelativePosition.InPosition)
               .EffectiveStackBetween(0, 4)
               .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingLimp, 0, PlMode.None, "BB_FacingLimp_HU_OOP_0_4bb"));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingLimp).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .EffectiveStackBetween(4, 6)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingLimp, 0, PlMode.None, "BB_FacingLimp_HU_OOP_4_6bb"));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
             .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingLimp).IsHU()
             .HeroRelativePosition(HeroRelativePosition.InPosition)
             .EffectiveStackBetween(6, 8)
             .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingLimp, 0, PlMode.None, "BB_FacingLimp_HU_OOP_6_8bb"));


            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingLimp).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .EffectiveStackBetween(8, 10)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingLimp, 0, PlMode.None, "BB_FacingLimp_HU_IP_8_10bb"));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingLimp).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .EffectiveStackBetween(10, 13)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingLimp, 0, PlMode.None, "BB_FacingLimp_HU_IP_10_13bb"));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingLimp).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .EffectiveStackBetween(10, 13)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingLimp, 0, PlMode.None, "BB_FacingLimp_HU_IP_10_13bb"));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingLimp).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .EffectiveStackBetween(13, 15)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingLimp, 0, PlMode.None, "BB_FacingLimp_HU_IP_13_15bb"));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingLimp).IsHU()
                .HeroRelativePosition(HeroRelativePosition.InPosition)
                .EffectiveStackBetween(15, 100)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingLimp, 0, PlMode.None, "BB_FacingLimp_HU_IP_15bb+"));
            //BB FACING LIMP OOP
            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingLimp).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(0, 4)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingLimp, 0, PlMode.None, "BB_FacingLimp_HU_OOP_0_4bb"));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingLimp).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(4, 6)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingLimp, 0, PlMode.None, "BB_FacingLimp_HU_OOP_4_6bb"));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingLimp).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(6, 8)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingLimp, 0, PlMode.None, "BB_FacingLimp_HU_OOP_6_8bb"));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingLimp).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(8, 10)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingLimp, 0, PlMode.None, "BB_FacingLimp_HU_OOP_8_10bb"));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingLimp).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(10, 13)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingLimp, 0, PlMode.None, "BB_FacingLimp_HU_OOP_10_13bb"));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingLimp).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(13, 16)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingLimp, 0, PlMode.None, "BB_FacingLimp_HU_OOP_13_16bb"));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingLimp).IsHU()
                .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                .EffectiveStackBetween(16, 100)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingLimp, 0, PlMode.None, "BB_FacingLimp_HU_OOP_16bb+"));

            
            ////BB FACING MINRAISE IP
            elements.StartRule().HeroPosition(PlayerPosition.Bb)
               .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
               .HeroRelativePosition(HeroRelativePosition.InPosition)
               .OppBetSize(2)
               .EffectiveStackBetween(8, 12)
               .VsBigStack()
               .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingOpen, 0, PlMode.None, "BB_FacingMinRaise_HU_IP_VBIG_8_12bb"));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
               .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
               .HeroRelativePosition(HeroRelativePosition.InPosition)
               .OppBetSize(2)
               .EffectiveStackBetween(8, 12)
               .VsSmallStack()
               .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingOpen, 0, PlMode.None, "BB_FacingMinRaise_HU_IP_VSMALL_8_12bb"));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
              .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
              .HeroRelativePosition(HeroRelativePosition.InPosition)
              .OppBetSizeMinRaise()
              .EffectiveStackBetween(12, 16)
              .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingOpen, 0, PlMode.None, "BB_FacingMinRaise_HU_IP_12_16bb"));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
              .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
              .HeroRelativePosition(HeroRelativePosition.InPosition)
              .OppBetSizeMinRaise()
              .EffectiveStackBetween(16, 20)
              .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingOpen, 0, PlMode.None, "BB_FacingMinRaise_HU_IP_16_20bb"));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
              .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
              .HeroRelativePosition(HeroRelativePosition.InPosition)
              .OppBetSizeMinRaise()
              .EffectiveStackBetween(20, 100)
              .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingOpen, 0, PlMode.None, "BB_FacingMinRaise_HU_IP_20bb+"));

            ////BB FACING MINRAISE OOP

            double _OpenRaise = 0;
            if (elements.HuOpp != null) {
                if (elements.HuOpp.Position == PlayerPosition.Button) _OpenRaise = elements.HuOpp.Stats.PF_BTN_STEAL;
                else if (elements.HuOpp.Position == PlayerPosition.Sb) _OpenRaise = elements.HuOpp.Stats.PF_SB_OPENMINRAISE;
            }


            if (_OpenRaise == 0) {

                elements.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSize(2)
                    .EffectiveStackBetween(8, 9.5)
                    .Do(
                        e =>
                            CheckHandInRange(e, HeroStatePreflop.FacingOpen, _hudInfo.EffectiveStack, PlMode.More,
                                "BB_FacingMinRaise_HU_OOP_8_9.5bb_UNK"));

            }
            else {
                elements.StartRule().HeroPosition(PlayerPosition.Bb)
                    .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                    .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
                    .OppBetSize(2)
                    .EffectiveStackBetween(8, 9.5)
                    .Do(
                        e =>
                            CheckHandInRange(e, HeroStatePreflop.FacingOpen, _OpenRaise, PlMode.More,
                                "BB_FacingMinRaise_HU_OOP_8_9.5bb_EXPL"));
            }

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
               .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
               .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
               .OppBetSize(2)
               .EffectiveStackBetween(9.5, 13)
               .VsSmallStack()
               .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingOpen, 0, PlMode.None, "BB_FacingMinRaise_HU_OOP_VSMALL_8_13bb"));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
               .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
               .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
               .OppBetSize(2)
               .EffectiveStackBetween(9.5, 11)
               .VsBigStack()
               .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingOpen, 0, PlMode.None, "BB_FacingMinRaise_HU_OOP_VBIG_8_11bb"));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
               .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
               .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
               .OppBetSize(2)
               .EffectiveStackBetween(11, 13)
               .VsBigStack()
               .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingOpen, 0, PlMode.None, "BB_FacingMinRaise_HU_OOP_VBIG_11_13bb"));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
               .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
               .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
               .OppBetSize(2)
               .EffectiveStackBetween(13, 15)
               .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingOpen, 0, PlMode.None, "BB_FacingMinRaise_HU_OOP_13_15bb"));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
               .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
               .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
               .OppBetSizeMinRaise()
               .EffectiveStackBetween(15, 17)
               .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingOpen, 0, PlMode.None, "BB_FacingMinRaise_HU_OOP_15_17bb"));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
               .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
               .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
               .OppBetSizeMinRaise()
               .EffectiveStackBetween(17, 20)
               .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingOpen, 0, PlMode.None, "BB_FacingMinRaise_HU_OOP_17_20bb"));

            elements.StartRule().HeroPosition(PlayerPosition.Bb)
               .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
               .HeroRelativePosition(HeroRelativePosition.OutOfPosition)
               .OppBetSizeMinRaise()
               .EffectiveStackBetween(20, 100)
               .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingOpen, 0, PlMode.None, "BB_FacingMinRaise_HU_OOP_20bb+"));

            #endregion

            #region COMMON

            elements.StartRule().HeroRole(HeroRole.Opener)
              .SitOutOpp()
              .Do(e => CheckHandInRange(e, HeroStatePreflop.Open, elements.HeroPlayer.Stack, PlMode.More, "COMMON_OPEN_100"));

            elements.StartRule()
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingOpen).IsHU()
                .EffectiveStackBetween(0,8)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingOpen, elements.EffectiveStack, PlMode.More, "COMMON_PushFoldVsOpen_NASH"));

            elements.StartRule()
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingPush).IsHU()
                .EffectiveStackBetween(0,11)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingPush, elements.EffectiveStack, PlMode.More, "COMMON_FacingPush_HU_08bb"));
            //TODO переделать со статами!!
            elements.StartRule()
                .HeroRole(HeroRole.Defender).HeroState(HeroStatePreflop.FacingPush).IsHU()
                .EffectiveStackBetween(11, 100)
                .Do(e => CheckHandInRange(e, HeroStatePreflop.FacingPush, elements.EffectiveStack, PlMode.More, "COMMON_FacingPush_HU_8_25bb"));

            #endregion

        
         


    

            if (_isNewDecision == false)
            {
                _hudInfo.HandPlayability = 0.0;
                _hudInfo.Decision = Decision.None;
            }

            if (NewHudInfo != null)
            {
                NewHudInfo(_hudInfo);
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

        private void CheckHandInRange(Elements elements, HeroStatePreflop statePreflop, double plStat, PlMode plMode, string rangeName) {

            if (_isNewDecision) return;
            Hand heroHand = elements.HeroPlayer.Hand;
            Range range = _rangesList.FirstOrDefault(r => r.Name == rangeName);

            if (NewRangeChosen != null)
            {
                NewRangeChosen(rangeName);
            }
          

            if (range != null) {

               

                if (heroHand.Name == "") return;
                double hPlayability = range.Hands.First(h => h.Name == heroHand.Name).Playability;
                _hudInfo.HandPlayability = hPlayability;

                Decision[] decisions = SelectDecisionArray(elements, statePreflop, hPlayability);

                if (plMode == PlMode.None) {
                    _hudInfo.Decision = decisions[(int) hPlayability];
                }
                else {
                    if (plMode == PlMode.More) {
                        _hudInfo.Decision = hPlayability >= plStat ? decisions[1] : decisions[0];
                    } else if (plMode == PlMode.Less) {
                        _hudInfo.Decision = hPlayability <= plStat ? decisions[1] : decisions[0];
                    }
                }

                

                _isNewDecision = true;
            }
            else {
                Debug.WriteLine(String.Format("can't find range {0}",rangeName ));
            }
        }


        private Decision[] SelectDecisionArray(Elements elements, HeroStatePreflop statePreflop, double hPlayability)
        {
            switch (statePreflop)
            {
                case HeroStatePreflop.Open: {
                    var nDecision = Decision.Fold;
                    var pDecision = (elements.EffectiveStack <= 8 && elements.IsHU) ? Decision.OpenPush : Decision.OpenRaise;
                    if ((elements.HeroPlayer.Stack <= 8 && elements.IsHU) || (elements.HeroPlayer.Stack <= 8 && 
                        !elements.IsHU && elements.BbAmt < 60)) pDecision = Decision.OpenPush;
                    if(hPlayability == -1) pDecision = Decision.OpenPush;
                    if(hPlayability == -2) pDecision = Decision.Limp;
                    Decision[] d  = { nDecision, pDecision};
                    return d;
                }
                case HeroStatePreflop.FacingLimp: {
                    Decision[] d = { Decision.CallToLimp, Decision.OpenRaise, Decision.PushToLimp,  };
                    return d;
                }
                case HeroStatePreflop.FacingOpen: {
                    var callDec = elements.HeroPlayer.Stack <= 9.5 ? Decision.PushToOpen : Decision.CallToOpen;
                    if (elements.EffectiveStack <= 9.5 && elements.IsHU) callDec = Decision.PushToOpen;
                    Decision[] d = { Decision.Fold, callDec, Decision._3Bet4, Decision.PushToOpen, };
                    return d;
                }
                case HeroStatePreflop.FacingPush: {
                    Decision[] d = { Decision.Fold, Decision.CallToPush };
                    return d;
                }
                default: {
                    Decision[] d = { Decision.None, };
                    return d;
                }

            }     
        }


        private void CheckOpen(Elements elements, double hPlayability) {
            var positiveDecision = elements.EffectiveStack <= 8 ? Decision.OpenPush : Decision.OpenRaise;
            var negativeDecision = Decision.Fold;

            switch (positiveDecision) {
                case Decision.OpenPush:
                    _hudInfo.Decision = elements.EffectiveStack <= hPlayability  ? positiveDecision : negativeDecision;
                    break;
                case Decision.OpenRaise:
                    _hudInfo.Decision = elements.EffectiveStack >= hPlayability   ? positiveDecision : negativeDecision;
                    break;
            }
        }



        private void CheckFacingLimp(Elements elements, double hPlayability)
        {
            switch ((int)hPlayability) {
                case 0: _hudInfo.Decision = Decision.CallToLimp; break;
                case 1: _hudInfo.Decision = Decision.RaiseToLimp; break;
                case 2: _hudInfo.Decision = Decision.PushToLimp; break;
            }
            
        }

        private void CheckFacingOpen(Elements elements, double hPlayability)
        {
            switch ((int)hPlayability)
            {
                case 0: _hudInfo.Decision = Decision.Fold; break;
                case 1: _hudInfo.Decision = Decision.CallToOpen; break;
                case 2: {
                    _hudInfo.Decision = elements.EffectiveStack < 18 ? Decision._3Bet4 : Decision._3Bet45;
                    break;
                }
                case 3: _hudInfo.Decision = Decision.PushToOpen; break;
            }

        }

        private void CheckFacingPush(Elements elements, double hPlayability, double plStat, PlMode plMode)
        {
            var positiveDecision = Decision.CallToPush;
            var negativeDecision = Decision.Fold;

            if (plMode == PlMode.More) {
                _hudInfo.Decision = plStat >= hPlayability ? positiveDecision : negativeDecision;
            }
            if (plMode == PlMode.Less)
            {
                _hudInfo.Decision = plStat <= hPlayability ? positiveDecision : negativeDecision;
            }
            
        }
        

        


    }

}
