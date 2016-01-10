using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using AForge.Imaging;
using AForge.Imaging.Filters;
using PokerModel;

namespace Pingvi
{
    internal class ElementsManager
    {
        private readonly Elements _elements;
        private readonly ElementsConfig _elementsConfig;
        public ElementsConfig ElementConfig;


        //private Stopwatch sw = new Stopwatch();


        public ElementsManager() {
            _elementsConfig = new ElementsConfig();
            _elements = new Elements();
            ElementConfig = _elementsConfig;
        }

        public UnmanagedImage TableUnmanagedImage { get; private set; }


        public Bitmap TableBitmap { get; private set; }

        public event Action<Elements> NewElements;

        public void OnNewTableImage(UnmanagedImage tableImage) {
            TableUnmanagedImage = tableImage;
            FindElements();
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

            _elements.HeroPlayer.Hand = new Hand(FindCard(_elementsConfig.Hero.Card1Rect),
                FindCard(_elementsConfig.Hero.Card2Rect));

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
                _elementsConfig.Common.PotDigitsRectMass, _elementsConfig.Common.PotDigitsListUnmanaged,
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
            _elements.LeftPlayer.Stats.PfBtnSteal =
                FindNumberN(_elementsConfig.LeftPlayer.PfBtnStealStatDigPosPoints,
                    _elementsConfig.LeftPlayer.PfBtnStealStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.PfSbSteal =
                FindNumberN(_elementsConfig.LeftPlayer.PfSbStealStatDigPosPoints,
                    _elementsConfig.LeftPlayer.PfSbStealStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.PfOpenpush =
                FindNumberN(_elementsConfig.LeftPlayer.PfOpenpushStatDigPosPoints,
                    _elementsConfig.LeftPlayer.PfOpenpushStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.PfLimpFold =
                FindNumberN(_elementsConfig.LeftPlayer.PfLimpFoldStatDigPosPoints,
                    _elementsConfig.LeftPlayer.PfLimpFoldStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.PfLimpReraise =
                FindNumberN(_elementsConfig.LeftPlayer.PfLimpReraiseStatDigPosPoints,
                    _elementsConfig.LeftPlayer.PfLimpReraiseStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.PfFold_3Bet =
                FindNumberN(_elementsConfig.LeftPlayer.PfFold_3BetStatDigPosPoints,
                    _elementsConfig.LeftPlayer.PfFold_3BetStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.PfBbDefVsSbsteal =
                FindNumberN(_elementsConfig.LeftPlayer.PfBbDefVsSbstealStatDigPosPoints,
                    _elementsConfig.LeftPlayer.PfBbDefVsSbstealStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.PfRaiseLimper =
                FindNumberN(_elementsConfig.LeftPlayer.PfRaiseLimperStatDigPosPoints,
                    _elementsConfig.LeftPlayer.PfRaiseLimperStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.PfSb_3BetVsBtn =
                FindNumberN(_elementsConfig.LeftPlayer.PfSb_3BetVsBtnStatDigPosPoints,
                    _elementsConfig.LeftPlayer.PfSb_3BetVsBtnStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.PfBb_3BetVsBtn =
                FindNumberN(_elementsConfig.LeftPlayer.PfBb_3BetVsBtnStatDigPosPoints,
                    _elementsConfig.LeftPlayer.PfBb_3BetVsBtnStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.PfBb_3BetVsSb =
                FindNumberN(_elementsConfig.LeftPlayer.PfBb_3BetVsSbStatDigPosPoints,
                    _elementsConfig.LeftPlayer.PfBb_3BetVsSbStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            //SECOND PANEL

            _elements.LeftPlayer.Stats.FCbet =
                FindNumberN(_elementsConfig.LeftPlayer.FCbetStatDigPosPoints,
                    _elementsConfig.LeftPlayer.FCbetStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.Cbet =
                FindNumberN(_elementsConfig.LeftPlayer.CbetStatDigPosPoints,
                    _elementsConfig.LeftPlayer.CbetStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.RCbet =
                FindNumberN(_elementsConfig.LeftPlayer.RCbetStatDigPosPoints,
                    _elementsConfig.LeftPlayer.RCbetStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.FFoldCbet =
                FindNumberN(_elementsConfig.LeftPlayer.FFoldCbetStatDigPosPoints,
                    _elementsConfig.LeftPlayer.FFoldCbetStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.FoldCbet =
                FindNumberN(_elementsConfig.LeftPlayer.FoldCbetStatDigPosPoints,
                    _elementsConfig.LeftPlayer.FoldCbetStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.RFoldCbet =
                FindNumberN(_elementsConfig.LeftPlayer.RFoldCbetStatDigPosPoints,
                    _elementsConfig.LeftPlayer.RFoldCbetStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.FCbetFoldraise =
                FindNumberN(_elementsConfig.LeftPlayer.FCbetFoldraiseStatDigPosPoints,
                    _elementsConfig.LeftPlayer.FCbetFoldraiseStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.CbetFoldraise =
                FindNumberN(_elementsConfig.LeftPlayer.CbetFoldraiseStatDigPosPoints,
                    _elementsConfig.LeftPlayer.CbetFoldraiseStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.FRaiseBet =
                FindNumberN(_elementsConfig.LeftPlayer.FRaiseBetStatDigPosPoints,
                    _elementsConfig.LeftPlayer.FRaiseBetStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.RaiseBet =
                FindNumberN(_elementsConfig.LeftPlayer.RaiseBetStatDigPosPoints,
                    _elementsConfig.LeftPlayer.RaiseBetStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.FLpSteal =
                FindNumberN(_elementsConfig.LeftPlayer.FLpStealStatDigPosPoints,
                    _elementsConfig.LeftPlayer.FLpStealStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            //THIRD PANEL
            _elements.LeftPlayer.Stats.FLpFoldVsSteal =
                FindNumberN(_elementsConfig.LeftPlayer.FLpFoldVsStealStatDigPosPoints,
                    _elementsConfig.LeftPlayer.FLpFoldVsStealStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.FLpFoldVsXr =
                FindNumberN(_elementsConfig.LeftPlayer.FLpFoldVsXrStatDigPosPoints,
                    _elementsConfig.LeftPlayer.FLpFoldVsXrStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.FCheckfoldOop =
                FindNumberN(_elementsConfig.LeftPlayer.FCheckfoldOopStatDigPosPoints,
                    _elementsConfig.LeftPlayer.FCheckfoldOopStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.SkipfFoldVsTProbe =
                FindNumberN(_elementsConfig.LeftPlayer.SkipfFoldVsTProbeStatDigPosPoints,
                    _elementsConfig.LeftPlayer.SkipfFoldVsTProbeStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.RSkiptFoldVsRProbe =
                FindNumberN(_elementsConfig.LeftPlayer.RSkiptFoldVsRProbeStatDigPosPoints,
                    _elementsConfig.LeftPlayer.RSkiptFoldVsRProbeStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.FDonk =
                FindNumberN(_elementsConfig.LeftPlayer.FDonkStatDigPosPoints,
                    _elementsConfig.LeftPlayer.FDonkStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.Donk =
                FindNumberN(_elementsConfig.LeftPlayer.DonkStatDigPosPoints,
                    _elementsConfig.LeftPlayer.DonkStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.FDonkFoldraise =
                FindNumberN(_elementsConfig.LeftPlayer.FDonkFoldraiseStatDigPosPoints,
                    _elementsConfig.LeftPlayer.FDonkFoldraiseStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);


            //RIGHT
            //FIRST PANEL
            _elements.RightPlayer.Stats.PfBtnSteal =
                FindNumberN(_elementsConfig.RightPlayer.PfBtnStealStatDigPosPoints,
                    _elementsConfig.RightPlayer.PfBtnStealStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.PfSbSteal =
                FindNumberN(_elementsConfig.RightPlayer.PfSbStealStatDigPosPoints,
                    _elementsConfig.RightPlayer.PfSbStealStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.PfOpenpush =
                FindNumberN(_elementsConfig.RightPlayer.PfOpenpushStatDigPosPoints,
                    _elementsConfig.RightPlayer.PfOpenpushStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.PfLimpFold =
                FindNumberN(_elementsConfig.RightPlayer.PfLimpFoldStatDigPosPoints,
                    _elementsConfig.RightPlayer.PfLimpFoldStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.PfLimpReraise =
                FindNumberN(_elementsConfig.RightPlayer.PfLimpReraiseStatDigPosPoints,
                    _elementsConfig.RightPlayer.PfLimpReraiseStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.PfFold_3Bet =
                FindNumberN(_elementsConfig.RightPlayer.PfFold_3BetStatDigPosPoints,
                    _elementsConfig.RightPlayer.PfFold_3BetStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.PfBbDefVsSbsteal =
                FindNumberN(_elementsConfig.RightPlayer.PfBbDefVsSbstealStatDigPosPoints,
                    _elementsConfig.RightPlayer.PfBbDefVsSbstealStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.PfRaiseLimper =
                FindNumberN(_elementsConfig.RightPlayer.PfRaiseLimperStatDigPosPoints,
                    _elementsConfig.RightPlayer.PfRaiseLimperStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.PfSb_3BetVsBtn =
                FindNumberN(_elementsConfig.RightPlayer.PfSb_3BetVsBtnStatDigPosPoints,
                    _elementsConfig.RightPlayer.PfSb_3BetVsBtnStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.PfBb_3BetVsBtn =
                FindNumberN(_elementsConfig.RightPlayer.PfBb_3BetVsBtnStatDigPosPoints,
                    _elementsConfig.RightPlayer.PfBb_3BetVsBtnStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.PfBb_3BetVsSb =
                FindNumberN(_elementsConfig.RightPlayer.PfBb_3BetVsSbStatDigPosPoints,
                    _elementsConfig.RightPlayer.PfBb_3BetVsSbStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            //SECOND PANEL

            _elements.RightPlayer.Stats.FCbet =
                FindNumberN(_elementsConfig.RightPlayer.FCbetStatDigPosPoints,
                    _elementsConfig.RightPlayer.FCbetStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.Cbet =
                FindNumberN(_elementsConfig.RightPlayer.CbetStatDigPosPoints,
                    _elementsConfig.RightPlayer.CbetStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.RCbet =
                FindNumberN(_elementsConfig.RightPlayer.RCbetStatDigPosPoints,
                    _elementsConfig.RightPlayer.RCbetStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.FFoldCbet =
                FindNumberN(_elementsConfig.RightPlayer.FFoldCbetStatDigPosPoints,
                    _elementsConfig.RightPlayer.FFoldCbetStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.FoldCbet =
                FindNumberN(_elementsConfig.RightPlayer.FoldCbetStatDigPosPoints,
                    _elementsConfig.RightPlayer.FoldCbetStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.RFoldCbet =
                FindNumberN(_elementsConfig.RightPlayer.RFoldCbetStatDigPosPoints,
                    _elementsConfig.RightPlayer.RFoldCbetStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.FCbetFoldraise =
                FindNumberN(_elementsConfig.RightPlayer.FCbetFoldraiseStatDigPosPoints,
                    _elementsConfig.RightPlayer.FCbetFoldraiseStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.CbetFoldraise =
                FindNumberN(_elementsConfig.RightPlayer.CbetFoldraiseStatDigPosPoints,
                    _elementsConfig.RightPlayer.CbetFoldraiseStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.FRaiseBet =
                FindNumberN(_elementsConfig.RightPlayer.FRaiseBetStatDigPosPoints,
                    _elementsConfig.RightPlayer.FRaiseBetStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.RaiseBet =
                FindNumberN(_elementsConfig.RightPlayer.RaiseBetStatDigPosPoints,
                    _elementsConfig.RightPlayer.RaiseBetStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.FLpSteal =
                FindNumberN(_elementsConfig.RightPlayer.FLpStealStatDigPosPoints,
                    _elementsConfig.RightPlayer.FLpStealStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            //THIRD PANEL
            _elements.RightPlayer.Stats.FLpFoldVsSteal =
                FindNumberN(_elementsConfig.RightPlayer.FLpFoldVsStealStatDigPosPoints,
                    _elementsConfig.RightPlayer.FLpFoldVsStealStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.FLpFoldVsXr =
                FindNumberN(_elementsConfig.RightPlayer.FLpFoldVsXrStatDigPosPoints,
                    _elementsConfig.RightPlayer.FLpFoldVsXrStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.FCheckfoldOop =
                FindNumberN(_elementsConfig.RightPlayer.FCheckfoldOopStatDigPosPoints,
                    _elementsConfig.RightPlayer.FCheckfoldOopStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.SkipfFoldVsTProbe =
                FindNumberN(_elementsConfig.RightPlayer.SkipfFoldVsTProbeStatDigPosPoints,
                    _elementsConfig.RightPlayer.SkipfFoldVsTProbeStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.RSkiptFoldVsRProbe =
                FindNumberN(_elementsConfig.RightPlayer.RSkiptFoldVsRProbeStatDigPosPoints,
                    _elementsConfig.RightPlayer.RSkiptFoldVsRProbeStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.FDonk =
                FindNumberN(_elementsConfig.RightPlayer.FDonkStatDigPosPoints,
                    _elementsConfig.RightPlayer.FDonkStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.Donk =
                FindNumberN(_elementsConfig.RightPlayer.DonkStatDigPosPoints,
                    _elementsConfig.RightPlayer.DonkStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.FDonkFoldraise =
                FindNumberN(_elementsConfig.RightPlayer.FDonkFoldraiseStatDigPosPoints,
                    _elementsConfig.RightPlayer.FDonkFoldraiseStatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsListUnmanaged, _elementsConfig.Common.StatsDigitsColor, false);

            #endregion

            //PLAYERs STATUS

            #region PlayerStatus

            _elements.HeroPlayer.Status = PlayerStatus.InHand;
            //CheckPlayerStatus(_elementsConfig.Hero.PlayerStatusPointHand,
            //_elementsConfig.Hero.PlayerStatusPointGame, _elementsConfig.Hero.PlayerStatusPointSitOut);  // always in hand
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

            //    _elements.HeroPlayer.Line = ParseLine(_elementsConfig.Hero.LinePixelPositions);
            //     _elements.LeftPlayer.Line = ParseLine(_elementsConfig.LeftPlayer.LinePixelPositions);
            //     _elements.RightPlayer.Line = ParseLine(_elementsConfig.RightPlayer.LinePixelPositions);


            _elements.HeroPlayer.Line = ParseLineAlter(_elementsConfig.Hero.LineRectPosition);
            _elements.LeftPlayer.Line = ParseLineAlter(_elementsConfig.LeftPlayer.LineRectPosition);
            _elements.RightPlayer.Line = ParseLineAlter(_elementsConfig.RightPlayer.LineRectPosition);
            //CURRENT STACK

            #region CurrentPlayersStack

            _elements.HeroPlayer.CurrentStack = FindNumber(_elementsConfig.Hero.StackDigPosPoints,
                _elementsConfig.Hero.StackDigitsRectMass,
                _elementsConfig.Common.StackDigitsListUnmanaged, _elementsConfig.Common.StackDigitsColor, true);

            _elements.LeftPlayer.CurrentStack = FindNumber(_elementsConfig.LeftPlayer.StackDigPosPoints,
                _elementsConfig.LeftPlayer.StackDigitsRectMass,
                _elementsConfig.Common.StackDigitsListUnmanaged, _elementsConfig.Common.StackDigitsColor, true);

            _elements.RightPlayer.CurrentStack = FindNumber(_elementsConfig.RightPlayer.StackDigPosPoints,
                _elementsConfig.RightPlayer.StackDigitsRectMass,
                _elementsConfig.Common.StackDigitsListUnmanaged, _elementsConfig.Common.StackDigitsColor, true);

            #endregion

            //CURRENT BET

            #region CurrentPlayersBet

            _elements.HeroPlayer.Bet = FindNumber(_elementsConfig.Hero.BetDigPosPoints,
                _elementsConfig.Hero.BetDigitsRectMass,
                _elementsConfig.Common.BetDigitsListUnmanaged, _elementsConfig.Common.BetDigitsColor, true);

            _elements.LeftPlayer.Bet = FindNumber(_elementsConfig.LeftPlayer.BetDigPosPoints,
                _elementsConfig.LeftPlayer.BetDigitsRectMass,
                _elementsConfig.Common.BetDigitsListUnmanaged, _elementsConfig.Common.BetDigitsColor, true);

            _elements.RightPlayer.Bet = FindNumber(_elementsConfig.RightPlayer.BetDigPosPoints,
                _elementsConfig.RightPlayer.BetDigitsRectMass,
                _elementsConfig.Common.BetDigitsListUnmanaged, _elementsConfig.Common.BetDigitsColor, true);

            #endregion

            _elements.HeroPlayer.BetToPot = CountBetToPot(_elements.TotalPot, _elements.HeroPlayer.Bet);
            _elements.LeftPlayer.BetToPot = CountBetToPot(_elements.TotalPot, _elements.LeftPlayer.Bet);
            _elements.RightPlayer.BetToPot = CountBetToPot(_elements.TotalPot, _elements.RightPlayer.Bet);

            CheckIsHu();

            //COUNT EFFECTIVE STACK (AFTER CURRENT STACK AND CURRENT BET)
            _elements.EffectiveStack = CountEffStack();
            _elements.SbBtnEffStack = CountSbBtnEffStack();


            if (NewElements != null)
            {
                NewElements(_elements);
            }
        }

        private void CheckIsHu() {
            if (_elements.ActivePlayers.Count == 2)
            {
                _elements.IsHu = true;
                _elements.HuOpp = _elements.ActivePlayers.FirstOrDefault(p => p.Name != _elements.HeroPlayer.Name);
            }
            else
            {
                _elements.IsHu = false;
                _elements.HuOpp = null;
            }
        }

        private Card FindCard(Rectangle cardPosition) {
            //    sw.Start();
            var filter = new Crop(cardPosition);
            var cTImage = filter.Apply(TableUnmanagedImage);

            var num = (from cBmp in _elementsConfig.Common.DeckListUnmanaged
                where BitmapHelper.BitmapsEqualsUnmanaged(cBmp, cTImage)
                select _elementsConfig.Common.DeckListUnmanaged.IndexOf(cBmp) + 1).FirstOrDefault();

            cTImage.Dispose();
            //   sw.Stop();
            //   Debug.WriteLine(sw.ElapsedTicks);
            // sw.Reset();
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
            //     sw.Start();
            var digitColor = _elementsConfig.Common.Blinds.DigitsPointColor;
            var p = 0;
            foreach (var point in _elementsConfig.Common.Blinds.DigitsPosPoints)
            {
                var pColor = TableUnmanagedImage.GetPixel(point.X, point.Y);
                if (digitColor == pColor) break;
                p++;
            }
            /*
                int p = _elementsConfig.Common.Blinds.DigitsPosPoints.TakeWhile(point =>
                    digitColor != TableBitmap.GetPixel(point.X, point.Y)).Count();
                */
            if (p >= _elementsConfig.Common.Blinds.DigitsRectMass.Length) return;

            var rect = _elementsConfig.Common.Blinds.DigitsRectMass[p];
            var digitsListUnmanged = _elementsConfig.Common.Blinds.DigitsListUnmanaged;
            var cFilter = new Crop(rect);
            var digTbmp = cFilter.Apply(TableUnmanagedImage);

            //find numbers
            for (var i = 0; i < digitsListUnmanged.Count; i++)
            {
                //   var digTablebmp = TableBitmap.Clone(rect, TableBitmap.PixelFormat);
                if (BitmapHelper.BitmapsEqualsUnmanaged(digTbmp, digitsListUnmanged[i]))
                {
                    _elements.BbAmt = _elementsConfig.Common.Blinds.BigBlindsDoubleMass[i];
                    _elements.SbAmt = _elements.BbAmt/2;
                    break;
                }
            }
            digTbmp.Dispose();
            //     }
            //     catch (Exception ex) {
            //         Debug.WriteLine(ex.Message + "in Method FindBlinds");
            //    }
            //      sw.Stop();
            //     Debug.WriteLine(sw.ElapsedTicks);
            //     sw.Reset();
        }

        private ButtonPosition FindButton() {
            var buttonColor = _elementsConfig.Common.ButtonColor;

            if (TableUnmanagedImage.GetPixel(_elementsConfig.Hero.ButtonPoint.X,
                _elementsConfig.Hero.ButtonPoint.Y) == buttonColor) return ButtonPosition.Hero;
            if (TableUnmanagedImage.GetPixel(_elementsConfig.LeftPlayer.ButtonPoint.X,
                _elementsConfig.LeftPlayer.ButtonPoint.Y) == buttonColor) return ButtonPosition.Left;
            if (TableUnmanagedImage.GetPixel(_elementsConfig.RightPlayer.ButtonPoint.X,
                _elementsConfig.RightPlayer.ButtonPoint.Y) == buttonColor) return ButtonPosition.Right;

            return ButtonPosition.None;
        }

        private PlayerType CheckPlayerType(PixelPoint playerTypePoint) {
            var pixelColor = TableUnmanagedImage.GetPixel(playerTypePoint.X, playerTypePoint.Y);

       

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

        private PlayerStatus CheckPlayerStatus(PixelPoint playerStatusHand, PixelPoint playerStatusGame,
            PixelPoint playerStatusSitOut) {
            if (TableUnmanagedImage.GetPixel(playerStatusSitOut.X, playerStatusSitOut.Y) ==
                _elementsConfig.Common.SitOutColor) return PlayerStatus.SitOut;
            if (TableUnmanagedImage.GetPixel(playerStatusHand.X, playerStatusHand.Y) ==
                _elementsConfig.Common.InHandColor) return PlayerStatus.InHand;
            if (TableUnmanagedImage.GetPixel(playerStatusGame.X, playerStatusGame.Y) ==
                _elementsConfig.Common.InGameColor) return PlayerStatus.OutOfHand;
            return PlayerStatus.OutOfGame;
        }

        private List<Player> CheckActivePlayers() {
            var activeOpponents = new List<Player>();
            if (_elements.HeroPlayer.Status == PlayerStatus.InHand) activeOpponents.Add(_elements.HeroPlayer);
            if (_elements.LeftPlayer.Status == PlayerStatus.InHand) activeOpponents.Add(_elements.LeftPlayer);
            if (_elements.RightPlayer.Status == PlayerStatus.InHand) activeOpponents.Add(_elements.RightPlayer);
            return activeOpponents;
        }

        private List<Player> CheckInGamePlayers() {
            var inGamePlayers = new List<Player>();
            if (_elements.HeroPlayer.Status != PlayerStatus.OutOfGame) inGamePlayers.Add(_elements.HeroPlayer);
            if (_elements.LeftPlayer.Status != PlayerStatus.OutOfGame) inGamePlayers.Add(_elements.LeftPlayer);
            if (_elements.RightPlayer.Status != PlayerStatus.OutOfGame) inGamePlayers.Add(_elements.RightPlayer);
            return inGamePlayers;
        }

        private List<Player> CheckSitOutPlayers() {
            var sitOutPlayers = new List<Player>();
            if (_elements.HeroPlayer.Status == PlayerStatus.SitOut) sitOutPlayers.Add(_elements.HeroPlayer);
            if (_elements.LeftPlayer.Status == PlayerStatus.SitOut) sitOutPlayers.Add(_elements.LeftPlayer);
            if (_elements.RightPlayer.Status == PlayerStatus.SitOut) sitOutPlayers.Add(_elements.RightPlayer);
            return sitOutPlayers;
        }

        private bool CheckIsHeroTurn() {
            if (_elements.HeroPlayer.Status == PlayerStatus.OutOfGame) return false;
            return _elementsConfig.Hero.IsTurnColor ==
                   TableUnmanagedImage.GetPixel(_elementsConfig.Hero.IsTurnPoint.X,
                       _elementsConfig.Hero.IsTurnPoint.Y);
        }

        private void CheckPlayersPosition(ButtonPosition buttonPosition) {
            var inGamePlrsCnt = _elements.InGamePlayers.Count;
            switch (buttonPosition)
            {
                case ButtonPosition.Hero:
                {
                    if (inGamePlrsCnt == 2)
                    {
                        _elements.HeroPlayer.Position = PlayerPosition.Sb;
                        if (_elements.LeftPlayer.Status == PlayerStatus.OutOfGame)
                        {
                            _elements.LeftPlayer.Position = PlayerPosition.None;
                            _elements.RightPlayer.Position = PlayerPosition.Bb;
                        }
                        else
                        {
                            _elements.LeftPlayer.Position = PlayerPosition.Bb;
                            _elements.RightPlayer.Position = PlayerPosition.None;
                        }
                    }
                    else if (inGamePlrsCnt == 3)
                    {
                        _elements.HeroPlayer.Position = PlayerPosition.Button;
                        _elements.LeftPlayer.Position = PlayerPosition.Sb;
                        _elements.RightPlayer.Position = PlayerPosition.Bb;
                    }
                    break;
                }
                case ButtonPosition.Left:
                {
                    if (inGamePlrsCnt == 2)
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
                    else if (inGamePlrsCnt == 3)
                    {
                        _elements.HeroPlayer.Position = PlayerPosition.Bb;
                        _elements.LeftPlayer.Position = PlayerPosition.Button;
                        _elements.RightPlayer.Position = PlayerPosition.Sb;
                    }
                    break;
                }
                case ButtonPosition.Right:
                {
                    if (inGamePlrsCnt == 2)
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
                    else if (inGamePlrsCnt == 3)
                    {
                        _elements.HeroPlayer.Position = PlayerPosition.Sb;
                        _elements.LeftPlayer.Position = PlayerPosition.Bb;
                        _elements.RightPlayer.Position = PlayerPosition.Button;
                    }
                    break;
                }
                case ButtonPosition.None:
                {
                    _elements.HeroPlayer.Position = PlayerPosition.None;
                    _elements.LeftPlayer.Position = PlayerPosition.None;
                    _elements.RightPlayer.Position = PlayerPosition.None;
                    break;
                }
            }
        }

        private HeroRelativePosition CheckHeroRelativePosition() {
            //TODO переписать покрасивше
            if (_elements.HeroPlayer.Status == PlayerStatus.OutOfHand) return HeroRelativePosition.None;
            if (_elements.HeroPlayer.Position == PlayerPosition.Button) return HeroRelativePosition.InPosition;

            if (_elements.HeroPlayer.Position == PlayerPosition.Sb)
            {
                if (_elements.InGamePlayers.Count == 2)
                {
                    return HeroRelativePosition.InPosition;
                }
                if (_elements.InGamePlayers.Count == 3)
                {
                    return HeroRelativePosition.OutOfPosition;
                }
            }
            if (_elements.HeroPlayer.Position == PlayerPosition.Bb)
            {
                if (_elements.InGamePlayers.Count == 2)
                {
                    return HeroRelativePosition.OutOfPosition;
                }
                if (_elements.InGamePlayers.Count == 3)
                {
                    if (_elements.ActivePlayers.Count == 3) return HeroRelativePosition.OutOfPosition;
                    if (_elements.ActivePlayers.Count == 2)
                        return _elements.ActivePlayers.First(p => p.Name != _elements.HeroPlayer.Name).Position
                               == PlayerPosition.Sb
                            ? HeroRelativePosition.InPosition
                            : HeroRelativePosition.OutOfPosition;
                }
            }

            return HeroRelativePosition.None;
        }

        private double FindNumber(PixelPoint[] numberCounterPoints, Rectangle[][] stackDigitsRactMass,
            List<UnmanagedImage> digitsBitmapsList, Color digPosPointColor, bool inBb) {
            //find stack or bet size
            try
            {
                //how many digits in full number
                var nubmerCounterColor = digPosPointColor;
                var numberCounts = 0;
                foreach (var nc in numberCounterPoints)
                {
                    if (TableUnmanagedImage.GetPixel(nc.X, nc.Y) == nubmerCounterColor) break;
                    numberCounts++;
                }
                if (numberCounts >= numberCounterPoints.Length) return 0.0;

                var digitRectMass = stackDigitsRactMass[numberCounts];
                if (digitRectMass.Length == 0) return 0.0;

                //find numbers in stack
                var fullNumber = 0.0;
                var a = 1;

                for (var i = digitRectMass.Length - 1; i >= 0; i--)
                {
                    var filter = new Crop(digitRectMass[i]);
                    var dTImage = filter.Apply(TableUnmanagedImage);

                    for (var j = 0; j <= 9; j++)
                    {
                        if (!BitmapHelper.BitmapsEqualsUnmanaged(dTImage, digitsBitmapsList[j]))
                            continue;
                        //Debug.WriteLine("FIND NUMBER: {0}", j);
                        fullNumber += j*a;
                        break;
                    }
                    a = a*10;

                    dTImage.Dispose();
                }

                if (inBb)
                {
                    //  return fullNumber/_elements.BbAmt;
                    if (digitRectMass[digitRectMass.Length - 1].X - 11 == digitRectMass[digitRectMass.Length - 2].X &&
                        fullNumber.ToString(CultureInfo.CurrentCulture).Length > 1)
                    {
                        return fullNumber/10;
                    }
                    return fullNumber/10;
                }
                return fullNumber;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "In method FindNUmber1");
                return 0.0;
            }
        }

        private double? FindNumberN(PixelPoint[] numberCounterPoints, Rectangle[][] stackDigitsRactMass,
            List<UnmanagedImage> digitsBitmapsList, Color digPosPointColor, bool inBb) {
            //find stack or bet size
            try
            {
                //how many digits in full number
                var nubmerCounterColor = digPosPointColor;
                var numberCounts = 0;
                foreach (var nc in numberCounterPoints)
                {
                    if (TableUnmanagedImage.GetPixel(nc.X, nc.Y) == nubmerCounterColor) break;
                    numberCounts++;
                }
                if (numberCounts >= numberCounterPoints.Length) return null;

                var digitRectMass = stackDigitsRactMass[numberCounts];
                //Debug.WriteLine("NUM: {0}", numberCounts);
                if (digitRectMass.Length == 0) return 0.0;
                //find numbers in stack
                var fullNumber = 0.0;
                var a = 1;

                for (var i = digitRectMass.Length - 1; i >= 0; i--)
                {
                    var filter = new Crop(digitRectMass[i]);
                    var dTImage = filter.Apply(TableUnmanagedImage);

                    for (var j = 0; j <= 9; j++)
                    {
                        if (!BitmapHelper.BitmapsEqualsUnmanaged(dTImage, digitsBitmapsList[j]))
                            continue;
                        //Debug.WriteLine("FIND NUMBER: {0}", j);
                        fullNumber += j*a;
                        break;
                    }
                    a = a*10;
                    dTImage.Dispose();
                }
                if (inBb)
                {
                    return fullNumber/_elements.BbAmt;
                }
                return fullNumber;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "In method FindNUmber2");
                return null;
            }
        }

        private double CountEffStack() {
            if (!_elements.ActivePlayers.Any()) return 0.0f;
            var effStack =
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


        /*
        private string ParseLine(PixelPoint[] playerLinePixelPositions)
        {
            //       Stopwatch sw = new Stopwatch();
            //      sw.Start();
            var sb = new StringBuilder();

            foreach (var pixel in playerLinePixelPositions)
            {
                foreach (var letter in _elementsConfig.Common.LineLettersColorDictionary)
                {
                    if (TableBitmap.GetPixel(pixel.X, pixel.Y) == letter.Key)
                        sb.Append(letter.Value);
                }
            }
            //    MessageBox.Show(sw.ElapsedTicks.ToString());
            return sb.ToString();
        }
        */
        //  Stopwatch sw = new Stopwatch();

        private string ParseLineAlter(Rectangle[] playerLineRectPositions) {
            // sw.Start();
            var sb = new StringBuilder();

            foreach (var t in playerLineRectPositions)
            {
                try
                {
                    //filtering
                    var filter = new Crop(t);
                    var lTImage = filter.Apply(TableUnmanagedImage);
                    lTImage = Grayscale.CommonAlgorithms.BT709.Apply(lTImage);
                    var tfilter = new Threshold();
                    lTImage = tfilter.Apply(lTImage);

                    //to numerical array
                    var numericBitmap = CountUnmanagedImage(lTImage);
                    //neuralnetwork computing
                    var res = _elementsConfig.Common.LineNetwork.Compute(numericBitmap);

                    var letterNumber = Array.IndexOf(res, res.Max());
                    var letter = _elementsConfig.Common.LineLettersNumbersDictionary[letterNumber];
                    if (letter == "") break;

                    lTImage.Dispose();
                    sb.Append(letter);
                }
                catch (Exception ex)
                {
                    Debug.Write(ex.Message + " in ParseAlterLine()");
                }
            }

            //Debug.WriteLine(sw.ElapsedTicks.ToString());
            // sw.Stop();
            // sw.Reset();
            return sb.ToString();
        }

        private double[] CountUnmanagedImage(UnmanagedImage image) {
            var res = new double[image.Height*image.Width];
            var c = 0;

            for (var i = 0; i < image.Width; i++)
            {
                for (var j = 0; j < image.Height; j++)
                {
                    if (image.GetPixel(i, j) == Color.FromArgb(255, 255, 255, 255)) res[c] = 0.5;
                    else res[c] = -0.5;
                    c++;
                }
            }
            image.Dispose();
            return res;
        }


        private double CountBetToPot(double pot, double playerBet) {
            if (pot == 0 || playerBet == 0) return 0.0;
            return playerBet/(pot - playerBet);
        }

        private int? CheckTourneyMultiplier() {
            var pixelColor = TableUnmanagedImage.GetPixel(_elementsConfig.Common.MultiplierPixelPoint.X,
                _elementsConfig.Common.MultiplierPixelPoint.Y);
            foreach (var c in _elementsConfig.Common.MultiplierColors.Where(c => pixelColor == c.Value))
            {
                return c.Key;
            }
            return null;
        }
    }
}