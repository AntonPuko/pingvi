using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Threading;
using System.Xml.Linq;
using PokerModel;


namespace Pingvi {
    internal class ElementsManager {

        public Bitmap TableBitmap { get; private set; }

        public event Action<Elements> NewElements;

        private readonly ElementsConfig _elementsConfig;
        private Elements _elements;

        
        public ElementsManager() {
            _elementsConfig = new ElementsConfig();
            _elements = new Elements();
        }

        public void OnNewBitmap(Bitmap bmp) {
            TableBitmap = bmp;
            FindElements();
        }

        public void  ProcessTable() {

            if (!_elements.TablesDictionary.ContainsKey(_elements.TableNumber))
                _elements.TablesDictionary.Add(_elements.TableNumber, _elements.HeroPlayer.StatePreflop);
            else {
                if (_elements.CurrentStreet != CurrentStreet.Preflop) return;
                _elements.TablesDictionary.Remove(_elements.TablesDictionary.FirstOrDefault(k => k.Key == _elements.TableNumber).Key);
                _elements.TablesDictionary.Add(_elements.TableNumber, _elements.HeroPlayer.StatePreflop);
            }
     
            //TODO
            
        }

        private void FindElements() {

            //TableNumber
            _elements.TableNumber = (int)(FindNumber(_elementsConfig.Common.TableNumberDigPosPoints,
              _elementsConfig.Common.TableNumberDigitsRectMass,
              _elementsConfig.Common.TableNumberDigitsList, _elementsConfig.Common.TableNumberDigitsColor, false));
            
         
             
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

            //LEFT
            //PREFLOP
            _elements.LeftPlayer.Stats.PF_BTN_STEAL =
                FindNumberN(_elementsConfig.LeftPlayer.PF_BTN_STEAL_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.PF_BTN_STEAL_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.PF_SB_STEAL =
                FindNumberN(_elementsConfig.LeftPlayer.PF_SB_STEAL_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.PF_SB_STEAL_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.PF_FOLD_3BET_IP =
                FindNumberN(_elementsConfig.LeftPlayer.PF_FOLD_3BET_IP_StatDigPosPoints,
                    _elementsConfig.LeftPlayer.PF_FOLD_3BET_IP_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.PF_FOLD_3BET_OOP =
               FindNumberN(_elementsConfig.LeftPlayer.PF_FOLD_3BET_OOP_StatDigPosPoints,
                   _elementsConfig.LeftPlayer.PF_FOLD_3BET_OOP_StatDigitsRectMass,
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

            _elements.LeftPlayer.Stats.PF_BB_DEF_VS_SBSTEAL =
               FindNumberN(_elementsConfig.LeftPlayer.PF_BB_DEF_VS_SBSTEAL_StatDigPosPoints,
                   _elementsConfig.LeftPlayer.PF_BB_DEF_VS_SBSTEAL_StatDigitsRectMass,
                   _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);
            
            _elements.LeftPlayer.Stats.PF_SB_OPENMINRAISE =
               FindNumberN(_elementsConfig.LeftPlayer.PF_SB_OPENMINRAISE_StatDigPosPoints,
                   _elementsConfig.LeftPlayer.PF_SB_OPENMINRAISE_StatDigitsRectMass,
                   _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);
            
            //LEFT FLOP
            _elements.LeftPlayer.Stats.F_CBET =
               FindNumberN(_elementsConfig.LeftPlayer.F_CBET_StatDigPosPoints,
                   _elementsConfig.LeftPlayer.F_CBET_StatDigitsRectMass,
                   _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.F_BET_LPOT =
              FindNumberN(_elementsConfig.LeftPlayer.F_BET_LPOT_StatDigPosPoints,
                  _elementsConfig.LeftPlayer.F_BET_LPOT_StatDigitsRectMass,
                  _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.F_CBET_FOLDRAISE =
               FindNumberN(_elementsConfig.LeftPlayer.F_CBET_FOLDRAISE_StatDigPosPoints,
                   _elementsConfig.LeftPlayer.F_CBET_FOLDRAISE_StatDigitsRectMass,
                   _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.F_FOLD_CBET =
              FindNumberN(_elementsConfig.LeftPlayer.F_FOLD_CBET_StatDigPosPoints,
                  _elementsConfig.LeftPlayer.F_FOLD_CBET_StatDigitsRectMass,
                  _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.F_RAISE_CBET =
               FindNumberN(_elementsConfig.LeftPlayer.F_RAISE_CBET_StatDigPosPoints,
                   _elementsConfig.LeftPlayer.F_RAISE_CBET_StatDigitsRectMass,
                   _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.F_DONK =
               FindNumberN(_elementsConfig.LeftPlayer.F_DONK_StatDigPosPoints,
                   _elementsConfig.LeftPlayer.F_DONK_StatDigitsRectMass,
                   _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.LeftPlayer.Stats.F_DONK_FOLDRAISE =
               FindNumberN(_elementsConfig.LeftPlayer.F_DONK_FOLDRAISE_StatDigPosPoints,
                   _elementsConfig.LeftPlayer.F_DONK_FOLDRAISE_StatDigitsRectMass,
                   _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            //RIGHT
            //PREFLOP
            _elements.RightPlayer.Stats.PF_BTN_STEAL =
                FindNumberN(_elementsConfig.RightPlayer.PF_BTN_STEAL_StatDigPosPoints,
                    _elementsConfig.RightPlayer.PF_BTN_STEAL_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.PF_SB_STEAL =
                FindNumberN(_elementsConfig.RightPlayer.PF_SB_STEAL_StatDigPosPoints,
                    _elementsConfig.RightPlayer.PF_SB_STEAL_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.PF_FOLD_3BET_IP =
                FindNumberN(_elementsConfig.RightPlayer.PF_FOLD_3BET_IP_StatDigPosPoints,
                    _elementsConfig.RightPlayer.PF_FOLD_3BET_IP_StatDigitsRectMass,
                    _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.PF_FOLD_3BET_OOP =
               FindNumberN(_elementsConfig.RightPlayer.PF_FOLD_3BET_OOP_StatDigPosPoints,
                   _elementsConfig.RightPlayer.PF_FOLD_3BET_OOP_StatDigitsRectMass,
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

            _elements.RightPlayer.Stats.PF_BB_DEF_VS_SBSTEAL =
               FindNumberN(_elementsConfig.RightPlayer.PF_BB_DEF_VS_SBSTEAL_StatDigPosPoints,
                   _elementsConfig.RightPlayer.PF_BB_DEF_VS_SBSTEAL_StatDigitsRectMass,
                   _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            
            _elements.RightPlayer.Stats.PF_SB_OPENMINRAISE =
              FindNumberN(_elementsConfig.RightPlayer.PF_SB_OPENMINRAISE_StatDigPosPoints,
                  _elementsConfig.RightPlayer.PF_SB_OPENMINRAISE_StatDigitsRectMass,
                  _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);
            
            //RIGHT FLOP
            _elements.RightPlayer.Stats.F_CBET =
               FindNumberN(_elementsConfig.RightPlayer.F_CBET_StatDigPosPoints,
                   _elementsConfig.RightPlayer.F_CBET_StatDigitsRectMass,
                   _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.F_BET_LPOT =
              FindNumberN(_elementsConfig.RightPlayer.F_BET_LPOT_StatDigPosPoints,
                  _elementsConfig.RightPlayer.F_BET_LPOT_StatDigitsRectMass,
                  _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.F_CBET_FOLDRAISE =
               FindNumberN(_elementsConfig.RightPlayer.F_CBET_FOLDRAISE_StatDigPosPoints,
                   _elementsConfig.RightPlayer.F_CBET_FOLDRAISE_StatDigitsRectMass,
                   _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.F_FOLD_CBET =
              FindNumberN(_elementsConfig.RightPlayer.F_FOLD_CBET_StatDigPosPoints,
                  _elementsConfig.RightPlayer.F_FOLD_CBET_StatDigitsRectMass,
                  _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.F_RAISE_CBET =
               FindNumberN(_elementsConfig.RightPlayer.F_RAISE_CBET_StatDigPosPoints,
                   _elementsConfig.RightPlayer.F_RAISE_CBET_StatDigitsRectMass,
                   _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.F_DONK =
               FindNumberN(_elementsConfig.RightPlayer.F_DONK_StatDigPosPoints,
                   _elementsConfig.RightPlayer.F_DONK_StatDigitsRectMass,
                   _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);

            _elements.RightPlayer.Stats.F_DONK_FOLDRAISE =
               FindNumberN(_elementsConfig.RightPlayer.F_DONK_FOLDRAISE_StatDigPosPoints,
                   _elementsConfig.RightPlayer.F_DONK_FOLDRAISE_StatDigitsRectMass,
                   _elementsConfig.Common.StatsDigitsList, _elementsConfig.Common.StatsDigitsColor, false);



           

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


            CheckIsHU();

            //COUNT EFFECTIVE STACK (AFTER CURRENT STACK AND CURRENT BET)
            _elements.EffectiveStack = CountEffStack();

            //HERO ROLE
            _elements.HeroPlayer.Role = CheckHeroRole();

            //HERO STATE
            _elements.HeroPlayer.StatePreflop = CheckHeroStatePreflop();

            ProcessTable();

            _elements.HeroPlayer.StatePostflop = CheckHeroStatePostflop();

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

        private Card FindCard(RectangleF cardPosition) {
            var tbmp = TableBitmap.Clone(cardPosition, TableBitmap.PixelFormat);
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
            try {

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
                        _elements.SbAmt = _elementsConfig.Common.Blinds.SmallBlindsDoubleMass[i];
                        _elements.BbAmt = _elements.SbAmt*2;
                        break;
                    }
                }
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message + "in Method FindBlinds");
            }

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
            if (pixelColor == _elementsConfig.Common.GoodRegColor) return PlayerType.GoodReg;
            if (pixelColor == _elementsConfig.Common.UberRegColor) return PlayerType.UberReg;
            return PlayerType.Unknown;
        }

        private PlayerStatus CheckPlayerStatus(PixelPoint playerStatusHand, PixelPoint playerStatusGame, PixelPoint playerStatusSitOut) {
            try {
                var statusPointColorSitOut = TableBitmap.GetPixel(playerStatusSitOut.X, playerStatusSitOut.Y);
                var statusPointColorHand = TableBitmap.GetPixel(playerStatusHand.X, playerStatusHand.Y);
                var statusPointColorGame = TableBitmap.GetPixel(playerStatusGame.X, playerStatusGame.Y);

                if (statusPointColorSitOut == _elementsConfig.Common.SitOutColor) return PlayerStatus.SitOut;
                if (statusPointColorHand == _elementsConfig.Common.InHandColor) return PlayerStatus.InHand;
                if (statusPointColorGame == _elementsConfig.Common.InGameColor) return PlayerStatus.OutOfHand;
                return PlayerStatus.OutOfGame;
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message + "CheckPlayerStatus");
                return PlayerStatus.OutOfGame;
            }
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

        private double FindNumber(PixelPoint[] numberCounterPoints, RectangleF[][] stackDigitsRactMass,
            List<Bitmap> digitsBitmapsList, Color digPosPointColor, bool inBb) {
            //find stack or bet size
            try {
                //how many digits in full number
                Color nubmerCounterColor = digPosPointColor;
                int numberCounts = 0;
                foreach (var nc in numberCounterPoints) {
                    if (TableBitmap.GetPixel(nc.X, nc.Y) == nubmerCounterColor) break;
                    numberCounts++;
                }
                if (numberCounts >= numberCounterPoints.Length) return 0.0;

                RectangleF[] digitRectMass = stackDigitsRactMass[numberCounts];
                //Debug.WriteLine("NUM: {0}", numberCounts);
                if (digitRectMass.Length == 0) return 0.0;
                //find numbers in stack
                double fullNumber = 0.0;
                int a = 1;

                for (int i = digitRectMass.Length - 1; i >= 0; i--) {
                    var digitTableBmp = TableBitmap.Clone(digitRectMass[i], digitsBitmapsList[0].PixelFormat);
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
                }
                else {
                    return fullNumber; 
                }
               
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message + "In method FindNUmber");
                return 0.0;
          }
        }

        private double? FindNumberN(PixelPoint[] numberCounterPoints, RectangleF[][] stackDigitsRactMass,
          List<Bitmap> digitsBitmapsList, Color digPosPointColor, bool inBb)
        {
            //find stack or bet size
            try
            {
                //how many digits in full number
                Color nubmerCounterColor = digPosPointColor;
                int numberCounts = 0;
                foreach (var nc in numberCounterPoints)
                {
                    if (TableBitmap.GetPixel(nc.X, nc.Y) == nubmerCounterColor) break;
                    numberCounts++;
                }
                if (numberCounts >= numberCounterPoints.Length) return null;

                RectangleF[] digitRectMass = stackDigitsRactMass[numberCounts];
                //Debug.WriteLine("NUM: {0}", numberCounts);
                if (digitRectMass.Length == 0) return 0.0;
                //find numbers in stack
                double fullNumber = 0.0;
                int a = 1;

                for (int i = digitRectMass.Length - 1; i >= 0; i--)
                {
                    var digitTableBmp = TableBitmap.Clone(digitRectMass[i], digitsBitmapsList[0].PixelFormat);
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

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "In method FindNUmber");
                return null;
            }
        }

        private double CountEffStack() {
            if (!_elements.ActivePlayers.Any()) return 0.0f;
            double effStack =
                _elements.ActivePlayers.Select(player => player.Stack).Concat(new double[] {100}).Min();
            return effStack;
        }

        private HeroRole CheckHeroRole() {
            if (!_elements.HeroPlayer.IsHeroTurn) return HeroRole.None;
            if (_elements.CurrentStreet == CurrentStreet.Preflop) {
                if (_elements.HeroPlayer.Position == PlayerPosition.Button) {
                    return HeroRole.Opener;
                }
                else if (_elements.HeroPlayer.Position == PlayerPosition.Sb) {
                    //HU
                    if(_elements.InGamePlayers.Count == 2) return HeroRole.Opener;
                    //MP
                    if (_elements.InGamePlayers.Count == 3) {
                        if(_elements.ActivePlayers.Count <= 2) return HeroRole.Opener;
                        return HeroRole.Defender;
                    }
                }
                else if (_elements.HeroPlayer.Position == PlayerPosition.Bb) {
                    return HeroRole.Defender;
                }
            }
            else {
                return HeroRole.None;
            }
            return HeroRole.None;
        }


        private HeroStatePreflop CheckHeroStatePreflop() {
            
            double maxOppBet;
            double maxBetPStack;
            if (_elements.InGamePlayers == null || _elements.InGamePlayers.Count < 2) {
                maxOppBet = 0;
                maxBetPStack = 0;
            }
            else {
                maxOppBet = _elements.InGamePlayers.Where(p => p.Name != _elements.HeroPlayer.Name).Select(p => p.Bet).Max();
                maxBetPStack = _elements.InGamePlayers.FirstOrDefault(p => p.Bet == maxOppBet).Stack;
            }


            if (_elements.CurrentStreet == CurrentStreet.Preflop) {
                switch (_elements.HeroPlayer.Role) {
                    // case HeroRole.None:
                    //    return HeroStatePreflop.None;
                    case HeroRole.Opener: {
                        if (maxOppBet <= 1) return HeroStatePreflop.Open;
                        if (maxOppBet >= _elements.HeroPlayer.Bet &&
                            (maxOppBet < maxBetPStack/3 && maxOppBet < _elements.HeroPlayer.Stack/3))
                            return HeroStatePreflop.Facing3Bet;

                        if (maxOppBet >= _elements.HeroPlayer.Bet &&
                            (maxOppBet >= maxBetPStack || maxOppBet >= _elements.HeroPlayer.Stack / 3) && _elements.HeroPlayer.Bet == 1)
                            return HeroStatePreflop.FacingPushVsLimp;
                       

                        if (maxOppBet >= _elements.HeroPlayer.Bet &&
                            (maxOppBet >= maxBetPStack || maxOppBet >= _elements.HeroPlayer.Stack/3))
                            return HeroStatePreflop.FacingPush;
                        return HeroStatePreflop.None;
                    }
                    case HeroRole.Defender: {
                        if (_elements.HeroPlayer.Bet == 1 && maxOppBet == 1) return HeroStatePreflop.FacingLimp;
                        if (_elements.HeroPlayer.Bet <= 1 && maxOppBet > _elements.HeroPlayer.Bet &&
                            maxOppBet < maxBetPStack &&
                            (maxOppBet > maxBetPStack/2 || maxOppBet >= _elements.HeroPlayer.Stack/2))
                            return HeroStatePreflop.FacingPush;
                        if (_elements.HeroPlayer.Bet <= 1 && maxOppBet > _elements.HeroPlayer.Bet &&
                            maxOppBet < maxBetPStack) return HeroStatePreflop.FacingOpen;
                        if (_elements.HeroPlayer.Bet <= 1 && maxOppBet > _elements.HeroPlayer.Bet &&
                            maxOppBet >= maxBetPStack) return HeroStatePreflop.FacingPush;
                        return HeroStatePreflop.None;
                    }
                }
            }
            var tState = _elements.TablesDictionary.FirstOrDefault(t => t.Key == _elements.TableNumber).Value;
            if(tState == null) return HeroStatePreflop.None;
            return tState;
        }


        private HeroStatePostflop CheckHeroStatePostflop() {
            if(_elements.ActivePlayers.Count >2) return HeroStatePostflop.None;
            double maxOppBet;
            double maxBetPStack;
            if (_elements.InGamePlayers == null || _elements.InGamePlayers.Count < 2)
            {
                maxOppBet = 0;
            }
            else
            {
                maxOppBet = _elements.InGamePlayers.Where(p => p.Name != _elements.HeroPlayer.Name).Select(p => p.Bet).Max();
            }

            if (_elements.CurrentStreet == CurrentStreet.Flop) {
                 if(_elements.HeroPlayer.StatePreflop == HeroStatePreflop.None) return HeroStatePostflop.None;

                if (_elements.HeroPlayer.StatePreflop == HeroStatePreflop.Open) {
                    if (_elements.TotalPot - maxOppBet == 2) {
                        if (_elements.HeroPlayer.Bet == 0 && maxOppBet == 0) return HeroStatePostflop.LimpBet;
                        if (_elements.HeroPlayer.Bet == 0 && maxOppBet > 0 && _elements.HeroPlayer.RelativePosition == HeroRelativePosition.InPosition) return HeroStatePostflop.FacingDONKVsOpenLimp;
                    }
                }
                 if (_elements.HeroPlayer.StatePreflop == HeroStatePreflop.Open || _elements.HeroPlayer.StatePreflop == HeroStatePreflop.FacingLimp) {
                     if(_elements.HeroPlayer.Bet == 0 && maxOppBet == 0) return HeroStatePostflop.Cbet;
                     if(_elements.HeroPlayer.Bet == 0 && maxOppBet > 0 && _elements.HeroPlayer.RelativePosition ==  HeroRelativePosition.InPosition ) return HeroStatePostflop.FacingDonk;
                     if(_elements.HeroPlayer.Bet == 0 && maxOppBet > 0 && _elements.HeroPlayer.RelativePosition == HeroRelativePosition.OutOfPosition) return HeroStatePostflop.FacingBetToCheck;
                     if(_elements.HeroPlayer.Bet > 0 && maxOppBet > _elements.HeroPlayer.Bet) return HeroStatePostflop.FacingRaiseToCbet;
                     return HeroStatePostflop.None;
                 }
                 if (_elements.HeroPlayer.StatePreflop == HeroStatePreflop.FacingOpen) {
                     if(maxOppBet ==0 && _elements.HeroPlayer.RelativePosition == HeroRelativePosition.InPosition) return HeroStatePostflop.MissedCbet;
                     if(_elements.HeroPlayer.Bet == 0 && maxOppBet > 0) return HeroStatePostflop.FacingCbet;
                     if(_elements.HeroPlayer.Bet > 0 && maxOppBet > _elements.HeroPlayer.Bet && _elements.HeroPlayer.RelativePosition == HeroRelativePosition.InPosition) return HeroStatePostflop.FacingCheckRaise;
                     return HeroStatePostflop.None;
                 }
                if (_elements.HeroPlayer.StatePreflop == HeroStatePreflop.Facing3Bet) {
                    if(_elements.HeroPlayer.Bet == 0 && maxOppBet > 0) return HeroStatePostflop.FacingCbet;
                    if(_elements.HeroPlayer.RelativePosition ==  HeroRelativePosition.InPosition && maxOppBet == 0 ) return HeroStatePostflop.MissedCbet;
                }
            }
            else {
                return HeroStatePostflop.None;
            }
            return HeroStatePostflop.None;
           
        }
    }





}

