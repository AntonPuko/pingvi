using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using AForge.Imaging.ColorReduction;
using AForge.Imaging.Filters;
using Pingvi.Stuff;
using PokerModel;


namespace Pingvi {
    internal class ElementsManager {

        public Bitmap TableBitmap { get; private set; }

        public event Action<Elements> NewElements;

        private readonly ElementsConfig _elementsConfig;
        private Elements _elements;
        public ElementsConfig ElementConfig;

        
        public ElementsManager() {
            _elementsConfig = new ElementsConfig();
            _elements = new Elements();
            ElementConfig = _elementsConfig;
        }

        public void OnNewBitmap(Bitmap bmp) {
            TableBitmap = bmp;
            FindElements();
        }

        private void FindElements() {

            //TourneyMultiplier 
            _elements.TourneyMultiplier = CheckTourneyMultiplier();
             
            //CARDS
            #region Cards

            _elements.FlopCard1 = FindCard(_elementsConfig.Common.FlopCard1Rect);
            _elements.FlopCard2 = FindCard(_elementsConfig.Common.FlopCard2Rect);
            _elements.FlopCard3 = FindCard(_elementsConfig.Common.FlopCard3Rect);
            _elements.TurnCard = FindCard(_elementsConfig.Common.TurnCardRect);
            _elements.RiverCard = FindCard(_elementsConfig.Common.RiverCardRect);

            _elements.HeroPlayer.Hand  = new Hand(FindCard(_elementsConfig.Hero.Card1Rect), FindCard(_elementsConfig.Hero.Card2Rect));

            
            #endregion

            //CURRENT STREET (AFTER FINDING CARD)

            #region CurrentStreet

            _elements.CurrentStreet = CheckCurrentStreet(_elements.FlopCard1, _elements.TurnCard, _elements.RiverCard);

            #endregion

            //BLINDS
            FindBlinds();
            //BUTTON
            _elements.ButtonPosition = FindButton();


            //TOTAL POT
            _elements.TotalPot = FindNumber(_elementsConfig.Common.PotDigPosPoints,
               _elementsConfig.Common.PotDigitsRectMass, _elementsConfig.Common.PotDigitsList,
               _elementsConfig.Common.PotDigitsColor, true);

            //PLAYERs TYPE
            #region PlayerType
            _elements.LeftPlayer.Type = CheckPlayerType(_elementsConfig.LeftPlayer.PlayerTypePoint);
            _elements.RightPlayer.Type = CheckPlayerType(_elementsConfig.RightPlayer.PlayerTypePoint);
            #endregion

            //PLAYER STATISTICS
            #region player stats
            //LEFT
            //FIRST PANEL
            _elements.LeftPlayer.Stats.PF_BTN_STEAL =
                FindNumberN(_elementsConfig.LeftPlayer.PF_BTN_STEAL_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.PF_BTN_STEAL_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.PF_SB_STEAL =
                FindNumberN(_elementsConfig.LeftPlayer.PF_SB_STEAL_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.PF_SB_STEAL_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

             _elements.LeftPlayer.Stats.PF_OPENPUSH =
                FindNumberN(_elementsConfig.LeftPlayer.PF_OPENPUSH_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.PF_OPENPUSH_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.PF_LIMP_FOLD =
                FindNumberN(_elementsConfig.LeftPlayer.PF_LIMP_FOLD_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.PF_LIMP_FOLD_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.PF_LIMP_RERAISE =
                FindNumberN(_elementsConfig.LeftPlayer.PF_LIMP_RERAISE_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.PF_LIMP_RERAISE_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

             _elements.LeftPlayer.Stats.PF_FOLD_3BET =
                FindNumberN(_elementsConfig.LeftPlayer.PF_FOLD_3BET_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.PF_FOLD_3BET_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.PF_BB_DEF_VS_SBSTEAL =
                FindNumberN(_elementsConfig.LeftPlayer.PF_BB_DEF_VS_SBSTEAL_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.PF_BB_DEF_VS_SBSTEAL_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.PF_RAISE_LIMPER =
                FindNumberN(_elementsConfig.LeftPlayer.PF_RAISE_LIMPER_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.PF_RAISE_LIMPER_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.PF_SB_3BET_VS_BTN =
                FindNumberN(_elementsConfig.LeftPlayer.PF_SB_3BET_VS_BTN_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.PF_SB_3BET_VS_BTN_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);
          
             _elements.LeftPlayer.Stats.PF_BB_3BET_VS_BTN =
                FindNumberN(_elementsConfig.LeftPlayer.PF_BB_3BET_VS_BTN_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.PF_BB_3BET_VS_BTN_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.PF_BB_3BET_VS_SB =
                FindNumberN(_elementsConfig.LeftPlayer.PF_BB_3BET_VS_SB_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.PF_BB_3BET_VS_SB_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);
            
            //SECOND PANEL

            _elements.LeftPlayer.Stats.F_CBET =
                FindNumberN(_elementsConfig.LeftPlayer.F_CBET_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.F_CBET_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.T_CBET =
                FindNumberN(_elementsConfig.LeftPlayer.T_CBET_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.T_CBET_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.R_CBET =
                FindNumberN(_elementsConfig.LeftPlayer.R_CBET_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.R_CBET_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.F_FOLD_CBET =
                FindNumberN(_elementsConfig.LeftPlayer.F_FOLD_CBET_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.F_FOLD_CBET_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.T_FOLD_CBET =
                FindNumberN(_elementsConfig.LeftPlayer.T_FOLD_CBET_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.T_FOLD_CBET_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.R_FOLD_CBET =
                FindNumberN(_elementsConfig.LeftPlayer.R_FOLD_CBET_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.R_FOLD_CBET_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.F_CBET_FOLDRAISE =
                FindNumberN(_elementsConfig.LeftPlayer.F_CBET_FOLDRAISE_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.F_CBET_FOLDRAISE_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.T_CBET_FOLDRAISE =
                FindNumberN(_elementsConfig.LeftPlayer.T_CBET_FOLDRAISE_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.T_CBET_FOLDRAISE_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.F_RAISE_BET =
                FindNumberN(_elementsConfig.LeftPlayer.F_RAISE_BET_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.F_RAISE_BET_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.T_RAISE_BET =
                FindNumberN(_elementsConfig.LeftPlayer.T_RAISE_BET_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.T_RAISE_BET_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.F_LP_STEAL =
                FindNumberN(_elementsConfig.LeftPlayer.F_LP_STEAL_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.F_LP_STEAL_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);
            
            //THIRD PANEL
            _elements.LeftPlayer.Stats.F_LP_FOLD_VS_STEAL =
                FindNumberN(_elementsConfig.LeftPlayer.F_LP_FOLD_VS_STEAL_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.F_LP_FOLD_VS_STEAL_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.F_LP_FOLD_VS_XR =
                FindNumberN(_elementsConfig.LeftPlayer.F_LP_FOLD_VS_XR_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.F_LP_FOLD_VS_XR_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.F_CHECKFOLD_OOP =
                FindNumberN(_elementsConfig.LeftPlayer.F_CHECKFOLD_OOP_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.F_CHECKFOLD_OOP_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.T_SKIPF_FOLD_VS_T_PROBE =
                FindNumberN(_elementsConfig.LeftPlayer.T_SKIPF_FOLD_VS_T_PROBE_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.T_SKIPF_FOLD_VS_T_PROBE_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.R_SKIPT_FOLD_VS_R_PROBE =
                FindNumberN(_elementsConfig.LeftPlayer.R_SKIPT_FOLD_VS_R_PROBE_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.R_SKIPT_FOLD_VS_R_PROBE_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.F_DONK =
                FindNumberN(_elementsConfig.LeftPlayer.F_DONK_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.F_DONK_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

             _elements.LeftPlayer.Stats.T_DONK =
                FindNumberN(_elementsConfig.LeftPlayer.T_DONK_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.T_DONK_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.F_DONK_FOLDRAISE =
                FindNumberN(_elementsConfig.LeftPlayer.F_DONK_FOLDRAISE_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.F_DONK_FOLDRAISE_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);


            //RIGHT
            //FIRST PANEL
            _elements.RightPlayer.Stats.PF_BTN_STEAL =
                FindNumberN(_elementsConfig.RightPlayer.PF_BTN_STEAL_StatDigPosPoints,
                    _elementsConfig.RightPlayer.PF_BTN_STEAL_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.PF_SB_STEAL =
                FindNumberN(_elementsConfig.RightPlayer.PF_SB_STEAL_StatDigPosPoints,
                    _elementsConfig.RightPlayer.PF_SB_STEAL_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.PF_OPENPUSH =
               FindNumberN(_elementsConfig.RightPlayer.PF_OPENPUSH_StatDigPosPoints,
                   _elementsConfig.RightPlayer.PF_OPENPUSH_StatDigitsRectMass,
                   _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.PF_LIMP_FOLD =
                FindNumberN(_elementsConfig.RightPlayer.PF_LIMP_FOLD_StatDigPosPoints,
                    _elementsConfig.RightPlayer.PF_LIMP_FOLD_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.PF_LIMP_RERAISE =
                FindNumberN(_elementsConfig.RightPlayer.PF_LIMP_RERAISE_StatDigPosPoints,
                    _elementsConfig.RightPlayer.PF_LIMP_RERAISE_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.PF_FOLD_3BET =
               FindNumberN(_elementsConfig.RightPlayer.PF_FOLD_3BET_StatDigPosPoints,
                   _elementsConfig.RightPlayer.PF_FOLD_3BET_StatDigitsRectMass,
                   _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.PF_BB_DEF_VS_SBSTEAL =
                FindNumberN(_elementsConfig.RightPlayer.PF_BB_DEF_VS_SBSTEAL_StatDigPosPoints,
                    _elementsConfig.RightPlayer.PF_BB_DEF_VS_SBSTEAL_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.PF_RAISE_LIMPER =
                FindNumberN(_elementsConfig.RightPlayer.PF_RAISE_LIMPER_StatDigPosPoints,
                    _elementsConfig.RightPlayer.PF_RAISE_LIMPER_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.PF_SB_3BET_VS_BTN =
                FindNumberN(_elementsConfig.RightPlayer.PF_SB_3BET_VS_BTN_StatDigPosPoints,
                    _elementsConfig.RightPlayer.PF_SB_3BET_VS_BTN_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.PF_BB_3BET_VS_BTN =
               FindNumberN(_elementsConfig.RightPlayer.PF_BB_3BET_VS_BTN_StatDigPosPoints,
                   _elementsConfig.RightPlayer.PF_BB_3BET_VS_BTN_StatDigitsRectMass,
                   _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.PF_BB_3BET_VS_SB =
                FindNumberN(_elementsConfig.RightPlayer.PF_BB_3BET_VS_SB_StatDigPosPoints,
                    _elementsConfig.RightPlayer.PF_BB_3BET_VS_SB_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            //SECOND PANEL

            _elements.RightPlayer.Stats.F_CBET =
                FindNumberN(_elementsConfig.RightPlayer.F_CBET_StatDigPosPoints,
                    _elementsConfig.RightPlayer.F_CBET_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.T_CBET =
                FindNumberN(_elementsConfig.RightPlayer.T_CBET_StatDigPosPoints,
                    _elementsConfig.RightPlayer.T_CBET_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.R_CBET =
                FindNumberN(_elementsConfig.RightPlayer.R_CBET_StatDigPosPoints,
                    _elementsConfig.RightPlayer.R_CBET_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.F_FOLD_CBET =
                FindNumberN(_elementsConfig.RightPlayer.F_FOLD_CBET_StatDigPosPoints,
                    _elementsConfig.RightPlayer.F_FOLD_CBET_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.T_FOLD_CBET =
                FindNumberN(_elementsConfig.RightPlayer.T_FOLD_CBET_StatDigPosPoints,
                    _elementsConfig.RightPlayer.T_FOLD_CBET_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.R_FOLD_CBET =
                FindNumberN(_elementsConfig.RightPlayer.R_FOLD_CBET_StatDigPosPoints,
                    _elementsConfig.RightPlayer.R_FOLD_CBET_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.F_CBET_FOLDRAISE =
                FindNumberN(_elementsConfig.RightPlayer.F_CBET_FOLDRAISE_StatDigPosPoints,
                    _elementsConfig.RightPlayer.F_CBET_FOLDRAISE_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.T_CBET_FOLDRAISE =
                FindNumberN(_elementsConfig.RightPlayer.T_CBET_FOLDRAISE_StatDigPosPoints,
                    _elementsConfig.RightPlayer.T_CBET_FOLDRAISE_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.F_RAISE_BET =
                FindNumberN(_elementsConfig.RightPlayer.F_RAISE_BET_StatDigPosPoints,
                    _elementsConfig.RightPlayer.F_RAISE_BET_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.T_RAISE_BET =
                FindNumberN(_elementsConfig.RightPlayer.T_RAISE_BET_StatDigPosPoints,
                    _elementsConfig.RightPlayer.T_RAISE_BET_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.F_LP_STEAL =
                FindNumberN(_elementsConfig.RightPlayer.F_LP_STEAL_StatDigPosPoints,
                    _elementsConfig.RightPlayer.F_LP_STEAL_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            //THIRD PANEL
            _elements.RightPlayer.Stats.F_LP_FOLD_VS_STEAL =
                FindNumberN(_elementsConfig.RightPlayer.F_LP_FOLD_VS_STEAL_StatDigPosPoints,
                    _elementsConfig.RightPlayer.F_LP_FOLD_VS_STEAL_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.F_LP_FOLD_VS_XR =
                FindNumberN(_elementsConfig.RightPlayer.F_LP_FOLD_VS_XR_StatDigPosPoints,
                    _elementsConfig.RightPlayer.F_LP_FOLD_VS_XR_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.F_CHECKFOLD_OOP =
                FindNumberN(_elementsConfig.RightPlayer.F_CHECKFOLD_OOP_StatDigPosPoints,
                    _elementsConfig.RightPlayer.F_CHECKFOLD_OOP_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.T_SKIPF_FOLD_VS_T_PROBE =
                FindNumberN(_elementsConfig.RightPlayer.T_SKIPF_FOLD_VS_T_PROBE_StatDigPosPoints,
                    _elementsConfig.RightPlayer.T_SKIPF_FOLD_VS_T_PROBE_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.R_SKIPT_FOLD_VS_R_PROBE =
                FindNumberN(_elementsConfig.RightPlayer.R_SKIPT_FOLD_VS_R_PROBE_StatDigPosPoints,
                    _elementsConfig.RightPlayer.R_SKIPT_FOLD_VS_R_PROBE_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.F_DONK =
                FindNumberN(_elementsConfig.RightPlayer.F_DONK_StatDigPosPoints,
                    _elementsConfig.RightPlayer.F_DONK_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.T_DONK =
               FindNumberN(_elementsConfig.RightPlayer.T_DONK_StatDigPosPoints,
                   _elementsConfig.RightPlayer.T_DONK_StatDigitsRectMass,
                   _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.F_DONK_FOLDRAISE =
                FindNumberN(_elementsConfig.RightPlayer.F_DONK_FOLDRAISE_StatDigPosPoints,
                    _elementsConfig.RightPlayer.F_DONK_FOLDRAISE_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            #endregion

            //PLAYERs STATUS
            #region PlayerStatus

            _elements.HeroPlayer.Status = CheckPlayerStatus(_elementsConfig.Hero.PlayerStatusPointHand,
                _elementsConfig.Hero.PlayerStatusPointGame, _elementsConfig.Hero.PlayerStatusPointSitOut);
            _elements.LeftPlayer.Status = CheckPlayerStatus(_elementsConfig.LeftPlayer.PlayerStatusPointHand,
                _elementsConfig.LeftPlayer.PlayerStatusPointGame, _elementsConfig.LeftPlayer.PlayerStatusPointSitOut);
            _elements.RightPlayer.Status = CheckPlayerStatus(_elementsConfig.RightPlayer.PlayerStatusPointHand,
                _elementsConfig.RightPlayer.PlayerStatusPointGame, _elementsConfig.RightPlayer.PlayerStatusPointSitOut);

            #endregion

            //ACTIVE PLAYERS (AFTER CHECKING PLAYER STATUS)
            _elements.SitOutPlayers = CheckSitOutPlayers();
            _elements.ActivePlayers = CheckActivePlayers();
            _elements.InGamePlayers = CheckInGamePlayers();
            //HERO TURN
            _elements.HeroPlayer.IsHeroTurn = CheckIsHeroTurn();

            //PLAYERS POSITION

            CheckPlayersPosition(_elements.ButtonPosition);

            //CheckPlayerPosition(_elements.ButtonPosition);
            _elements.HeroPlayer.RelativePosition = CheckHeroRelativePosition();

            //PLAYERS LINE

          //  _elements.HeroPlayer.Line = ParseLine(_elementsConfig.Hero.LinePixelPositions);
         //   _elements.LeftPlayer.Line = ParseLine(_elementsConfig.LeftPlayer.LinePixelPositions);
          //  _elements.RightPlayer.Line = ParseLine(_elementsConfig.RightPlayer.LinePixelPositions);


            _elements.HeroPlayer.Line = ParseLineAlter(_elementsConfig.Hero.LineRectPosition);
            _elements.LeftPlayer.Line = ParseLineAlter(_elementsConfig.LeftPlayer.LineRectPosition);
            _elements.RightPlayer.Line = ParseLineAlter(_elementsConfig.RightPlayer.LineRectPosition);
            //CURRENT STACK
            #region CurrentPlayersStack

            _elements.HeroPlayer.CurrentStack = FindNumber(_elementsConfig.Hero.StackDigPosPoints,
                _elementsConfig.Hero.StackDigitsRectMass,
                _elementsConfig.Common.StackDigitsList, _elementsConfig.Common.StackDigitsColor, true);

            _elements.LeftPlayer.CurrentStack = FindNumber(_elementsConfig.LeftPlayer.StackDigPosPoints,
                _elementsConfig.LeftPlayer.StackDigitsRectMass,
                _elementsConfig.Common.StackDigitsList, _elementsConfig.Common.StackDigitsColor, true);

            _elements.RightPlayer.CurrentStack = FindNumber(_elementsConfig.RightPlayer.StackDigPosPoints,
                _elementsConfig.RightPlayer.StackDigitsRectMass,
                _elementsConfig.Common.StackDigitsList, _elementsConfig.Common.StackDigitsColor, true);

            #endregion

            //CURRENT BET

            #region CurrentPlayersBet

            _elements.HeroPlayer.Bet = FindNumber(_elementsConfig.Hero.BetDigPosPoints,
                _elementsConfig.Hero.BetDigitsRectMass,
                _elementsConfig.Common.BetDigitsList, _elementsConfig.Common.BetDigitsColor, true);

            _elements.LeftPlayer.Bet = FindNumber(_elementsConfig.LeftPlayer.BetDigPosPoints,
                _elementsConfig.LeftPlayer.BetDigitsRectMass,
                _elementsConfig.Common.BetDigitsList, _elementsConfig.Common.BetDigitsColor, true);

            _elements.RightPlayer.Bet = FindNumber(_elementsConfig.RightPlayer.BetDigPosPoints,
                _elementsConfig.RightPlayer.BetDigitsRectMass,
                _elementsConfig.Common.BetDigitsList, _elementsConfig.Common.BetDigitsColor, true);

            #endregion


            _elements.HeroPlayer.BetToPot = CountBetToPot(_elements.TotalPot, _elements.HeroPlayer.Bet);
            _elements.LeftPlayer.BetToPot = CountBetToPot(_elements.TotalPot, _elements.LeftPlayer.Bet);
            _elements.RightPlayer.BetToPot = CountBetToPot(_elements.TotalPot, _elements.RightPlayer.Bet);

            CheckIsHU();

            //COUNT EFFECTIVE STACK (AFTER CURRENT STACK AND CURRENT BET)
            _elements.EffectiveStack = CountEffStack();

            _elements.SbBtnEffStack = CountSbBtnEffStack();


            //FIRE EVENT
            if (NewElements != null) {
                NewElements(_elements);
            }
            
        }

        private void CheckIsHU() {
            if (_elements.ActivePlayers.Count == 2) {
                _elements.IsHU = true;
                _elements.HuOpp = _elements.ActivePlayers.FirstOrDefault(p => p.Name != _elements.HeroPlayer.Name);
            }
            else {
                _elements.IsHU = false;
                _elements.HuOpp = null;
            }
        }

        private Card FindCard(Rectangle cardPosition) {

            Crop filter = new Crop(cardPosition);
            var tbmp = filter.Apply(TableBitmap);

            int num = (from cBmp in _elementsConfig.Common.DeckList
                where BitmapHelper.BitmapsEquals(cBmp, tbmp)
                select _elementsConfig.Common.DeckList.IndexOf(cBmp) + 1).FirstOrDefault();
            return new Card(num);
        }

        private CurrentStreet CheckCurrentStreet(Card flopCard1, Card turnCard, Card riverCard) {
            if (flopCard1.Name == "") return CurrentStreet.Preflop;
            if (riverCard.Name != "") return CurrentStreet.River;
            if (turnCard.Name != "") return CurrentStreet.Turn;
            return CurrentStreet.Flop;
        }
        
        private void FindBlinds() {
          //  try {

                var digitColor = _elementsConfig.Common.Blinds.DigitsPointColor;
                int p = 0;
                foreach (var point in _elementsConfig.Common.Blinds.DigitsPosPoints) {
                    var pColor = TableBitmap.GetPixel(point.X, point.Y);
                    if (digitColor == pColor) break;
                    p++;
                }
                /*
                int p = _elementsConfig.Common.Blinds.DigitsPosPoints.TakeWhile(point =>
                    digitColor != TableBitmap.GetPixel(point.X, point.Y)).Count();
                */
                if (p >= _elementsConfig.Common.Blinds.DigitsRectMass.Length) return;
                var rect = _elementsConfig.Common.Blinds.DigitsRectMass[p];
                var digitsList = _elementsConfig.Common.Blinds.DigitsList;
                //find numbers
                for (int i = 0; i < digitsList.Count; i++) {
                    var digTablebmp = TableBitmap.Clone(rect, TableBitmap.PixelFormat);
                    if (BitmapHelper.BitmapsEquals(digTablebmp, digitsList[i])) {
                        _elements.BbAmt = _elementsConfig.Common.Blinds.BigBlindsDoubleMass[i];
                        _elements.SbAmt = _elements.BbAmt / 2;
                        break;
                    }
                }
       //     }
       //     catch (Exception ex) {
       //         Debug.WriteLine(ex.Message + "in Method FindBlinds");
        //    }

        }
        
        private ButtonPosition FindButton() {
            var heroButPixColor = TableBitmap.GetPixel(_elementsConfig.Hero.ButtonPoint.X,
                _elementsConfig.Hero.ButtonPoint.Y);
            var leftPButPixColor = TableBitmap.GetPixel(_elementsConfig.LeftPlayer.ButtonPoint.X,
                _elementsConfig.LeftPlayer.ButtonPoint.Y);
            var rightPButPixColor = TableBitmap.GetPixel(_elementsConfig.RightPlayer.ButtonPoint.X,
                _elementsConfig.RightPlayer.ButtonPoint.Y);
            var butColor = _elementsConfig.Common.ButtonColor;
            if (heroButPixColor == butColor) return ButtonPosition.Hero;
            if (leftPButPixColor == butColor) return ButtonPosition.Left;
            if (rightPButPixColor == butColor) return ButtonPosition.Right;
            return ButtonPosition.None;
        }

        private PlayerType CheckPlayerType(PixelPoint playerTypePoint) {
            var pixelColor = TableBitmap.GetPixel(playerTypePoint.X, playerTypePoint.Y);
            if (pixelColor == _elementsConfig.Common.RockColor) return PlayerType.Rock;
            if (pixelColor == _elementsConfig.Common.ManiacColor) return PlayerType.Maniac;
            if (pixelColor == _elementsConfig.Common.FishColor) return PlayerType.Fish;
            if (pixelColor == _elementsConfig.Common.WeakRegColor) return PlayerType.WeakReg;
            if (pixelColor == _elementsConfig.Common.GoodRegColor ||
                pixelColor == _elementsConfig.Common.GoodRegColor2 ||
                pixelColor == _elementsConfig.Common.GoodRegColor3 ||
                pixelColor == _elementsConfig.Common.GoodRegColor4) return PlayerType.GoodReg;
            if (pixelColor == _elementsConfig.Common.UberRegColor) return PlayerType.UberReg;
            return PlayerType.Unknown;
        }

        private PlayerStatus CheckPlayerStatus(PixelPoint playerStatusHand, PixelPoint playerStatusGame, PixelPoint playerStatusSitOut) {
       //     try {
                var statusPointColorSitOut = TableBitmap.GetPixel(playerStatusSitOut.X, playerStatusSitOut.Y);
                var statusPointColorHand = TableBitmap.GetPixel(playerStatusHand.X, playerStatusHand.Y);
                var statusPointColorGame = TableBitmap.GetPixel(playerStatusGame.X, playerStatusGame.Y);

                if (statusPointColorSitOut == _elementsConfig.Common.SitOutColor) return PlayerStatus.SitOut;
                if (statusPointColorHand == _elementsConfig.Common.InHandColor) return PlayerStatus.InHand;
                if (statusPointColorGame == _elementsConfig.Common.InGameColor) return PlayerStatus.OutOfHand;
                return PlayerStatus.OutOfGame;
     //       }
       //     catch (Exception ex) {
       //         Debug.WriteLine(ex.Message + "CheckPlayerStatus");
       //         return PlayerStatus.OutOfGame;
        //    }
        }

        private List<Player> CheckActivePlayers() {
            var activeOpponents = new List<Player>();
            if (_elements.HeroPlayer.Status == PlayerStatus.InHand) activeOpponents.Add(_elements.HeroPlayer);
            if (_elements.LeftPlayer.Status == PlayerStatus.InHand) activeOpponents.Add(_elements.LeftPlayer);
            if (_elements.RightPlayer.Status == PlayerStatus.InHand) activeOpponents.Add(_elements.RightPlayer);
            return activeOpponents;
        }

        private List<Player> CheckInGamePlayers()
        {
            var inGamePlayers = new List<Player>();
            if (_elements.HeroPlayer.Status != PlayerStatus.OutOfGame) inGamePlayers.Add(_elements.HeroPlayer);
            if (_elements.LeftPlayer.Status != PlayerStatus.OutOfGame) inGamePlayers.Add(_elements.LeftPlayer);
            if (_elements.RightPlayer.Status != PlayerStatus.OutOfGame) inGamePlayers.Add(_elements.RightPlayer);
            return inGamePlayers;
        }

        private List<Player> CheckSitOutPlayers()
        {
            var sitOutPlayers = new List<Player>();
            if (_elements.HeroPlayer.Status == PlayerStatus.SitOut) sitOutPlayers.Add(_elements.HeroPlayer);
            if (_elements.LeftPlayer.Status == PlayerStatus.SitOut) sitOutPlayers.Add(_elements.LeftPlayer);
            if (_elements.RightPlayer.Status == PlayerStatus.SitOut) sitOutPlayers.Add(_elements.RightPlayer);
            return sitOutPlayers;
        }

        private bool CheckIsHeroTurn() {
            //check does Hero have to make decision
            if (_elements.HeroPlayer.Status == PlayerStatus.OutOfGame) return false;
            return _elementsConfig.Hero.IsTurnColor ==
                   TableBitmap.GetPixel(_elementsConfig.Hero.IsTurnPoint.X,
                       _elementsConfig.Hero.IsTurnPoint.Y);
        }

        private void CheckPlayersPosition(ButtonPosition buttonPosition) {
            var InGamePlrsCnt = _elements.InGamePlayers.Count;
            switch (buttonPosition) {
                case ButtonPosition.Hero: {
                    if (InGamePlrsCnt == 2)
                    {
                        _elements.HeroPlayer.Position= PlayerPosition.Sb;
                        if (_elements.LeftPlayer.Status == PlayerStatus.OutOfGame) {
                            _elements.LeftPlayer.Position = PlayerPosition.None;
                            _elements.RightPlayer.Position = PlayerPosition.Bb;
                        }
                        else {
                            _elements.LeftPlayer.Position = PlayerPosition.Bb;
                            _elements.RightPlayer.Position = PlayerPosition.None;
                        }
                    }
                    else if (InGamePlrsCnt == 3)
                    {
                        _elements.HeroPlayer.Position = PlayerPosition.Button;
                        _elements.LeftPlayer.Position = PlayerPosition.Sb;
                        _elements.RightPlayer.Position = PlayerPosition.Bb;
                    }
                    break;
                }
                case ButtonPosition.Left: {
                    if (InGamePlrsCnt == 2)
                    {
                        _elements.LeftPlayer.Position = PlayerPosition.Sb;
                        if (_elements.RightPlayer.Status == PlayerStatus.OutOfGame)
                        {
                            _elements.RightPlayer.Position = PlayerPosition.None;
                            _elements.HeroPlayer.Position = PlayerPosition.Bb;
                        }
                        else
                        {
                            _elements.RightPlayer.Position = PlayerPosition.Bb;
                            _elements.HeroPlayer.Position = PlayerPosition.None;
                        }

                    }
                    else if (InGamePlrsCnt == 3)
                    {
                        _elements.HeroPlayer.Position = PlayerPosition.Bb;
                        _elements.LeftPlayer.Position = PlayerPosition.Button;
                        _elements.RightPlayer.Position = PlayerPosition.Sb;

                    }
                    break;
                }
                case ButtonPosition.Right: {
                    if (InGamePlrsCnt == 2)
                    {
                        _elements.RightPlayer.Position = PlayerPosition.Sb;
                        if (_elements.LeftPlayer.Status == PlayerStatus.OutOfGame)
                        {
                            _elements.LeftPlayer.Position = PlayerPosition.None;
                            _elements.HeroPlayer.Position = PlayerPosition.Bb;
                        }
                        else
                        {
                            _elements.LeftPlayer.Position = PlayerPosition.Bb;
                            _elements.HeroPlayer.Position = PlayerPosition.None;
                        }


                    }
                    else if (InGamePlrsCnt == 3)
                    {
                        _elements.HeroPlayer.Position = PlayerPosition.Sb;
                        _elements.LeftPlayer.Position = PlayerPosition.Bb;
                        _elements.RightPlayer.Position = PlayerPosition.Button;

                    }
                    break;
                }
                case ButtonPosition.None: {
                    _elements.HeroPlayer.Position = PlayerPosition.None;
                    _elements.LeftPlayer.Position = PlayerPosition.None;
                    _elements.RightPlayer.Position = PlayerPosition.None;
                    break;
                }
            }
        }

  


        private HeroRelativePosition CheckHeroRelativePosition() {
            //TODO переписать покрасивше
            if(_elements.HeroPlayer.Status == PlayerStatus.OutOfHand) return HeroRelativePosition.None;
            if (_elements.HeroPlayer.Position == PlayerPosition.Button) return HeroRelativePosition.InPosition;

            if (_elements.HeroPlayer.Position == PlayerPosition.Sb) {
                if (_elements.InGamePlayers.Count == 2)
                {
                    return HeroRelativePosition.InPosition;
                }
                if (_elements.InGamePlayers.Count == 3) {
                    return HeroRelativePosition.OutOfPosition;
                }
                
            }
            if (_elements.HeroPlayer.Position == PlayerPosition.Bb) {
                if (_elements.InGamePlayers.Count == 2) {
                    return HeroRelativePosition.OutOfPosition;
                }
                if (_elements.InGamePlayers.Count == 3) {
                    if (_elements.ActivePlayers.Count == 3) return HeroRelativePosition.OutOfPosition;
                    if (_elements.ActivePlayers.Count == 2) return _elements.ActivePlayers.First(p => p.Name != _elements.HeroPlayer.Name).Position
                        == PlayerPosition.Sb ? HeroRelativePosition.InPosition : HeroRelativePosition.OutOfPosition;}
                }
              
            return HeroRelativePosition.None;
        }

        private double FindNumber(PixelPoint[] numberCounterPoints, Rectangle[][] stackDigitsRactMass,
            List<Bitmap> digitsBitmapsList, Color digPosPointColor, bool inBb) {
            //find stack or bet size
        //    try {
                //how many digits in full number
                Color nubmerCounterColor = digPosPointColor;
                int numberCounts = 0;
                foreach (var nc in numberCounterPoints) {
                    if (TableBitmap.GetPixel(nc.X, nc.Y) == nubmerCounterColor) break;
                    numberCounts++;
                }
                if (numberCounts >= numberCounterPoints.Length) return 0.0;

                Rectangle[] digitRectMass = stackDigitsRactMass[numberCounts];
                //Debug.WriteLine("NUM: {0}", numberCounts);
                if (digitRectMass.Length == 0) return 0.0;
                //find numbers in stack
                double fullNumber = 0.0;
                int a = 1;

                for (int i = digitRectMass.Length - 1; i >= 0; i--) {


                    //var digitTableBmp = TableBitmap.Clone(digitRectMass[i], digitsBitmapsList[0].PixelFormat);

                    Crop filter = new Crop(digitRectMass[i]);
                    //var digitTableBmp = TableBitmap.Clone(digitRectMass[i], digitsBitmapsList[0].PixelFormat);
                    var digitTableBmp = filter.Apply(TableBitmap);

                    for (int j = 0; j <= 9; j++) {
                        if (!BitmapHelper.BitmapsEquals(digitTableBmp, digitsBitmapsList[j]))
                            continue;
                        //Debug.WriteLine("FIND NUMBER: {0}", j);
                        fullNumber += j*a;
                        break;
                    }
                    a = a*10;
                }
                if (inBb) {
                   return fullNumber/_elements.BbAmt;
                   // if (digitRectMass[digitRectMass.Length - 1].X - 11 == digitRectMass[digitRectMass.Length - 2].X &&
                   //     fullNumber.ToString().Length >1) {
                   //     return fullNumber / 10;
                   // }
                   // return fullNumber / 10;
                }
                else {
                    return fullNumber; 
                }
               
        //    }
        //    catch (Exception ex) {
         //       Debug.WriteLine(ex.Message + "In method FindNUmber");
        //        return 0.0;
         // }
        }

        private double? FindNumberN(PixelPoint[] numberCounterPoints, Rectangle[][] stackDigitsRactMass,
          List<Bitmap> digitsBitmapsList, Color digPosPointColor, bool inBb)
        {
            //find stack or bet size
       //     try
        //    {
                //how many digits in full number
                Color nubmerCounterColor = digPosPointColor;
                int numberCounts = 0;
                foreach (var nc in numberCounterPoints)
                {
                    if (TableBitmap.GetPixel(nc.X, nc.Y) == nubmerCounterColor) break;
                    numberCounts++;
                }
                if (numberCounts >= numberCounterPoints.Length) return null;

                Rectangle[] digitRectMass = stackDigitsRactMass[numberCounts];
                //Debug.WriteLine("NUM: {0}", numberCounts);
                if (digitRectMass.Length == 0) return 0.0;
                //find numbers in stack
                double fullNumber = 0.0;
                int a = 1;
            
                for (int i = digitRectMass.Length - 1; i >= 0; i--) {

                    Crop filter = new Crop(digitRectMass[i]);
                    //var digitTableBmp = TableBitmap.Clone(digitRectMass[i], digitsBitmapsList[0].PixelFormat);
                    var digitTableBmp = filter.Apply(TableBitmap);

                    for (int j = 0; j <= 9; j++)
                    {
                        if (!BitmapHelper.BitmapsEquals(digitTableBmp, digitsBitmapsList[j]))
                            continue;
                        //Debug.WriteLine("FIND NUMBER: {0}", j);
                        fullNumber += j * a;
                        break;
                    }
                    a = a * 10;
                }
                if (inBb)
                {
                    return fullNumber / _elements.BbAmt;
                }
                else
                {
                    return fullNumber;
                }

        //    }
        //    catch (Exception ex)
        //    {
       //         Debug.WriteLine(ex.Message + "In method FindNUmber");
       //         return null;
       //     }
        }

        private double CountEffStack() {
            if (!_elements.ActivePlayers.Any()) return 0.0f;
            double effStack =
                _elements.ActivePlayers.Select(player => player.Stack).Concat(new double[] {100}).Min();
            return effStack;
        }

        private double CountSbBtnEffStack() {
            if (_elements.ActivePlayers.FirstOrDefault(p => p.Position == PlayerPosition.Button) == null ||
                _elements.ActivePlayers.FirstOrDefault(p => p.Position == PlayerPosition.Sb) == null) return 0;
            var btnstack = _elements.ActivePlayers.First(p => p.Position == PlayerPosition.Button).Stack;
            var sbstack = _elements.ActivePlayers.First(p => p.Position == PlayerPosition.Sb).Stack;
            return Math.Min(btnstack, sbstack);

        }

  



        private string ParseLine(PixelPoint[] playerLinePixelPositions)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var pixel in playerLinePixelPositions)
            {
                foreach (var letter in _elementsConfig.Common.LineLettersColorDictionary)
                {
                    if (TableBitmap.GetPixel(pixel.X, pixel.Y) == letter.Key)
                        sb.Append(letter.Value);
                  
                }

            }

            return sb.ToString();
        }


  
        private string ParseLineAlter(Rectangle[] playerLineRectPositions){
            StringBuilder sb =  new StringBuilder();

            foreach (Rectangle t in playerLineRectPositions) {
                
                Crop filter = new Crop(t);
                var letterTableBitmap = filter.Apply(TableBitmap);
                letterTableBitmap = Grayscale.CommonAlgorithms.BT709.Apply(letterTableBitmap);
                Threshold tfilter = new Threshold();
                letterTableBitmap = tfilter.Apply(letterTableBitmap);

                var numericBitmap = CountBitmap(letterTableBitmap);
                var res = _elementsConfig.Common.LineNetwork.Compute(numericBitmap);


                int letterNumber = Array.IndexOf(res, res.Max());
                var letter = _elementsConfig.Common.LineLettersNumbersDictionary[letterNumber];
                if (letter == "") break;
                sb.Append(letter);


            }
  
            return sb.ToString();
        }

        private double[] CountBitmap(Bitmap letterBitmap) {
            UnsafeBitmap uBitmap = new UnsafeBitmap(letterBitmap);
            uBitmap.LockBitmap();
            var res = new double[letterBitmap.Size.Width * letterBitmap.Size.Height];
            int c = 0;
            for (int i = 0; i < letterBitmap.Width; i++)
            {
                for (int j = 0; j < letterBitmap.Height; j++)
                {
                    PixelData pixelColor = uBitmap.GetPixel(i, j);

                    if (pixelColor.red == 255 && pixelColor.green == 255
                        && pixelColor.blue == 255) res[c] = 0.5;
                    else res[c] = -0.5;
                    c++;
                }
            }
            uBitmap.Dispose();
            return res;
        }

        private double CountBetToPot(double pot, double playerBet) {
            if (pot == 0 || playerBet == 0) return 0.0;
            return playerBet/(pot-playerBet);
        }

        private int? CheckTourneyMultiplier() {
            var pixelColor = TableBitmap.GetPixel(_elementsConfig.Common.MultiplierPixelPoint.X,
                _elementsConfig.Common.MultiplierPixelPoint.Y);
            foreach (var c in _elementsConfig.Common.MultiplierColors) {
                if (pixelColor == c.Value) return c.Key;
            }
            return null;
        }
    
    }





}

