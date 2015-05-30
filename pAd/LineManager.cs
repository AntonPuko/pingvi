using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using PokerModel;

namespace Pingvi {
    public enum HeroPreflopState {
        None,
        Open,
        FacingOpen,
        FacingLimp,
        FacingISOvsLimp,
        FacingLimpIsoShove,
        FacingReraiseIso,
        FacingPushToIso,
        Facing3Bet,
        Facing3BetSqueeze,
        FacingPushSqueeze,
        FacingRaiseCall,
        FacingPush,
        FacingPushVsOpen,
        FacingOpenPush,
        FacingPushVsLimp
    }

    public enum HeroFlopState {
        None,
        Cbet,
        Bet,
        Donk,
        FacingDonk,
        FacingBet,
        FacingCbet,
        FacingMissCbet,
        FacingBetVsMissCbet,
        FacingReraise
    }

    public enum HeroTurnState {
        None,
        Donk,
        Donk2,
        FacingDonk2,
        FacingDonk,
        Bet2,
        DelayBet,
        FacingBet2,
        BetVsMissFCb,
        BetVsMissTCb,
        BetAfterFReraise,
        DelayCbet,
        Cbet2,
        FacingCbet2,
        FacingReraise,
        FacingCheckAfterFlopReraise,
        FacingDelayBet,
        FacingDelayCbet,
    }

    public enum HeroRiverState {
        None,
        Donk,
        Donk3,
        FacingDonk,
        FacingDonk3,
        Bet3,
        DCbet2,
        Cbet3,
        FacingBet3,
        FacingCbet3,
        FacingCbXX,
        FacingdCbX,
    }


    public enum PotType {
        None, Limped,IsoLimped,Raised, Reraised,
    }

    public enum PotNType {
        None, Multipot, Hu3Max, Hu2Max
    }


    public class LineManager {
        private string _preflopCompositeLine;
        private string _flopCompositeLine;
        private string _turnCompositeLine;
        private string _riverCompositeLine;
        private string _resultCompositeLine;
        private PotNType _potNtype;

        
        public Action<LineInfo> NewLineInfo;


        public PotNType DefinePotNType(Elements elements) {
            if(elements.ActivePlayers.Count == 3) return PotNType.Multipot;
            if(elements.ActivePlayers.Count == 2 && elements.InGamePlayers.Count == 3) return PotNType.Hu3Max;
            if(elements.InGamePlayers.Count == 2) return PotNType.Hu2Max;
            return PotNType.None;
        }

        public PotType DefinePotType(string preflopCompositiveLine) {
            int R = preflopCompositiveLine.Count(l => l == 'R');
            int L = preflopCompositiveLine.Count(l => l == 'L');
            if(L == 1 && R == 0) return  PotType.Limped;
            if(L == 1 && R == 1) return PotType.IsoLimped;
            if(R == 1) return PotType.Raised;
            if (R >= 2) return PotType.Reraised;
            return PotType.None;
            
        }

        public bool CheckFacingPush(Elements elements) {
            double maxOppBet;
            double maxBetPStack;
            if (elements.InGamePlayers == null || elements.InGamePlayers.Count < 2)
            {
                maxOppBet = 0;
                maxBetPStack = 0;
            }
            else
            {
                maxOppBet = elements.InGamePlayers.Where(p => p.Name != elements.HeroPlayer.Name).Select(p => p.Bet).Max();
                maxBetPStack = elements.InGamePlayers.FirstOrDefault(p => p.Bet == maxOppBet).Stack;
            }

            if ((maxOppBet > elements.HeroPlayer.Bet && maxOppBet >= maxBetPStack) || maxOppBet > elements.HeroPlayer.Stack / 3) return  true;
            return false;
        }

        public HeroPreflopState DefineHeroPreflopState(Elements elements) {
            if (elements.CurrentStreet != CurrentStreet.Preflop) return HeroPreflopState.None;

            bool facingPush = CheckFacingPush(elements);

            switch (elements.HeroPlayer.Position) {
                case PlayerPosition.Button: {
                    if (_preflopCompositeLine == "") return HeroPreflopState.Open;
                    if ((_preflopCompositeLine == "LRF|" || _preflopCompositeLine == "LFR|") && !facingPush) return HeroPreflopState.FacingISOvsLimp;
                    if ((_preflopCompositeLine == "RRF|" || _preflopCompositeLine == "RFR|") && !facingPush) return HeroPreflopState.Facing3Bet;
                    if (_preflopCompositeLine == "RCR|" && !facingPush) return HeroPreflopState.Facing3BetSqueeze;
                    if (_preflopCompositeLine == "RCR|" && !facingPush) return HeroPreflopState.FacingPushSqueeze;
                    if ((_preflopCompositeLine == "RRF|" || _preflopCompositeLine == "RFR|") && facingPush) return HeroPreflopState.FacingPushVsOpen;
                    return HeroPreflopState.None;
                }
                case PlayerPosition.Sb: {
                    switch (_potNtype) {
                        case PotNType.Multipot: {
                            if (_preflopCompositeLine == "L|") return HeroPreflopState.FacingLimp;
                            if (_preflopCompositeLine == "R|" && !facingPush) return HeroPreflopState.FacingOpen;
                            if (_preflopCompositeLine == "R|" && facingPush) return HeroPreflopState.FacingOpenPush;
                            if (_preflopCompositeLine == "RCR|" && !facingPush) return HeroPreflopState.Facing3BetSqueeze;
                            if (_preflopCompositeLine == "RCR|" && facingPush) return HeroPreflopState.FacingPushSqueeze;
                            return HeroPreflopState.None;
                        }
                        case PotNType.Hu3Max: {
                            if (_preflopCompositeLine == "F|") return HeroPreflopState.Open;
                            if (_preflopCompositeLine == "FLR|" && !facingPush) return HeroPreflopState.FacingISOvsLimp;
                            if (_preflopCompositeLine == "FLR|" && facingPush) return HeroPreflopState.FacingPushVsLimp;
                            if (_preflopCompositeLine == "FRR|" && !facingPush) return HeroPreflopState.Facing3Bet;
                            if (_preflopCompositeLine == "FRR|") return HeroPreflopState.FacingPush;
                            return HeroPreflopState.None;
                        }
                        case PotNType.Hu2Max: {
                            if(_preflopCompositeLine == "") return HeroPreflopState.Open;
                            if (_preflopCompositeLine == "LR|" && !facingPush) return HeroPreflopState.FacingISOvsLimp;
                            if (_preflopCompositeLine == "LR|" && facingPush) return HeroPreflopState.FacingPushVsLimp;
                            if (_preflopCompositeLine == "RR|" && !facingPush) return HeroPreflopState.Facing3Bet;
                            if (_preflopCompositeLine == "RR|" && facingPush) return HeroPreflopState.FacingPushVsOpen;
                            return  HeroPreflopState.None;
                        }
                    }
                    return HeroPreflopState.None;
                }
                case PlayerPosition.Bb: {
                    switch (_potNtype) {
                        case PotNType.Multipot: {
                            if (_preflopCompositeLine == "RC|" && !facingPush) return HeroPreflopState.FacingRaiseCall;
                            if (_preflopCompositeLine == "LR|" && !facingPush) return HeroPreflopState.FacingISOvsLimp;
                            if (_preflopCompositeLine == "LR|" && facingPush) return HeroPreflopState.FacingLimpIsoShove;
                            return HeroPreflopState.None;
                        }
                        case PotNType.Hu3Max: {
                            if (_preflopCompositeLine == "FL|") return HeroPreflopState.FacingLimp;
                            if ((_preflopCompositeLine == "FR|" || _preflopCompositeLine == "RF|") && !facingPush) return HeroPreflopState.FacingOpen;
                            if ((_preflopCompositeLine == "FR|" || _preflopCompositeLine == "RF|") && facingPush) return HeroPreflopState.FacingOpenPush;
                           
                            return HeroPreflopState.None;
                        }
                        case PotNType.Hu2Max: {
                            if (_preflopCompositeLine == "L|") return HeroPreflopState.FacingLimp;
                            if (_preflopCompositeLine == "R|" && !facingPush) return HeroPreflopState.FacingOpen;
                            if (_preflopCompositeLine == "R|" && facingPush) return HeroPreflopState.FacingOpenPush;
                            if (_preflopCompositeLine == "LRR|" && !facingPush) return HeroPreflopState.FacingReraiseIso;
                            if (_preflopCompositeLine == "LRR|" && facingPush) return HeroPreflopState.FacingPushToIso;
                            return HeroPreflopState.None;
                        }
                    }

                    }
                    return HeroPreflopState.None;
            }

            return HeroPreflopState.None;
        }

        public HeroFlopState DefineHeroFlopState(PotType potType, string compositeLine, Elements elements) {
            if(elements.CurrentStreet != CurrentStreet.Flop) return HeroFlopState.None;

            bool facingPush = CheckFacingPush(elements);
            switch (_potNtype) {
                case PotNType.Multipot: {
                    switch (elements.HeroPlayer.Position) {
                        case PlayerPosition.Button: {
                            if (compositeLine == "RCC|XX|") return HeroFlopState.Cbet;
                            if (compositeLine == "RCC|XB|" || compositeLine == "RCC|BF|") return HeroFlopState.FacingDonk;
                            
                            return HeroFlopState.None;
                        }
                        case PlayerPosition.Sb: {
                            return HeroFlopState.None;
                        }
                        case PlayerPosition.Bb: {
                            return  HeroFlopState.None;
                        }
                    }
                    return HeroFlopState.None;
                }
                case PotNType.Hu3Max: {
                    switch (elements.HeroPlayer.Position) {
                        //TODO не все ситуации разобраны
                        case PlayerPosition.Button: {
                            if (compositeLine == "RCF|X|" || compositeLine == "RFC|X|") return HeroFlopState.Cbet;
                            if (compositeLine == "RCF|B|" || compositeLine == "RFC|B|") return HeroFlopState.FacingDonk;
                            if (compositeLine == "RCF|XBR|" || compositeLine == "RFC|XBR|") return HeroFlopState.FacingReraise;

                            if (compositeLine == "RRFC|X" || compositeLine == "RFRC|X|") return HeroFlopState.FacingMissCbet;
                            if (compositeLine == "RRFC|B" || compositeLine == "RFRC|B|") return HeroFlopState.FacingCbet;
                            
                            return HeroFlopState.None;
                        }
                        case PlayerPosition.Sb: {
                            //SB VS BTN
                            if(compositeLine == "LRFC|") return HeroFlopState.Cbet;

                            if(compositeLine == "RCF|") return HeroFlopState.Donk;
                            if (compositeLine == "RCF|XB|") return HeroFlopState.FacingCbet;

                            if (compositeLine == "RRFC|") return HeroFlopState.Cbet;
                            if (compositeLine == "RRFC|XB|") return HeroFlopState.FacingBetVsMissCbet;

                            
                            //SB VS BB
                            if (compositeLine == "FLC|") return HeroFlopState.Cbet; //возможно стоит подругому статус сделать
                            if (compositeLine == "FLC|XB|") return HeroFlopState.FacingBet;
                            if (compositeLine == "FLRC|") return HeroFlopState.Donk;

                            if (compositeLine == "FRC|") return HeroFlopState.Cbet;
                            if (compositeLine == "FRC|XB|") return HeroFlopState.FacingBetVsMissCbet;
                            if (compositeLine == "FRC|BR|") return HeroFlopState.FacingReraise;

                            if (compositeLine == "FRRC|") return HeroFlopState.Donk;
                            if (compositeLine == "FRRC|XB|") return HeroFlopState.FacingCbet;

                            
                            return HeroFlopState.None;
                        }
                        case PlayerPosition.Bb: {
                            //TODO
                            //BB VS BTN
                            if(compositeLine == "LFX|") return HeroFlopState.Bet;
                            if(compositeLine == "LFX|BR|") return  HeroFlopState.FacingReraise;
                            if(compositeLine == "LFX|XB|") return  HeroFlopState.FacingBet;

                            if(compositeLine == "LFRC|") return HeroFlopState.Cbet;
                            if(compositeLine == "LFRC|BR|") return  HeroFlopState.FacingReraise;
                            if(compositeLine == "LFRC|XB|") return HeroFlopState.FacingBetVsMissCbet;

                            if(compositeLine == "RFC|") return  HeroFlopState.Donk;
                            if(compositeLine == "RFC|BR") return  HeroFlopState.FacingReraise;
                            if(compositeLine == "RFC|XB|") return HeroFlopState.FacingCbet;

                            if(compositeLine == "RFRC|") return HeroFlopState.Cbet;
                            if(compositeLine == "RFRC|BR|") return HeroFlopState.FacingReraise;
                            if(compositeLine == "RFRC|XB|") return HeroFlopState.FacingBetVsMissCbet;

                            //BB VS SB
                            if(compositeLine == "FLX|B|") return HeroFlopState.FacingBet;
                            if(compositeLine == "FLX|X|") return HeroFlopState.Bet;
                            if(compositeLine == "FLX|XBR|") return  HeroFlopState.FacingReraise;

                            if(compositeLine == "FLRC|B|") return  HeroFlopState.Donk;
                            if(compositeLine == "FLRC|X|") return  HeroFlopState.Cbet;
                            if(compositeLine == "FLRC|XBR|") return HeroFlopState.FacingReraise;
                            
                            
                            return HeroFlopState.None;
                        }
                    }
                    return HeroFlopState.None;
                }
                case PotNType.Hu2Max: {
                    switch (elements.HeroPlayer.Position) {
                        case PlayerPosition.Sb: {
                            //TODO не все ситуации разобраны
                            if (compositeLine == "LX|X|") return HeroFlopState.Cbet; //возможно стоит подругому статус сделать
                            if (compositeLine == "LX|B|") return HeroFlopState.FacingBet;

                            if (compositeLine == "LRC|B|") return HeroFlopState.FacingCbet;
                            if (compositeLine == "LRC|X|") return HeroFlopState.FacingMissCbet;

                            if (compositeLine == "RC|X|") return HeroFlopState.Cbet;
                            if (compositeLine == "RC|B|") return HeroFlopState.FacingDonk;
                            if (compositeLine == "RC|XBR|") return HeroFlopState.FacingReraise;

                            if (compositeLine == "RRC|X|") return HeroFlopState.FacingMissCbet;
                            if (compositeLine == "RRC|B|") return HeroFlopState.FacingCbet;
                            return  HeroFlopState.None;
                        }
                        case PlayerPosition.Bb: {
                            //TODO не все ситуации разобраны
                            if (compositeLine == "LX|") return HeroFlopState.Bet;
                            if (compositeLine == "LX|BR|") return  HeroFlopState.FacingReraise;
                            if (compositeLine == "LX|XB|") return HeroFlopState.FacingBet;

                            if (compositeLine == "RC|") return HeroFlopState.Donk;
                            if (compositeLine == "RC|XB|") return HeroFlopState.FacingCbet;

                            if(compositeLine == "RRC|") return HeroFlopState.Cbet;
                            if (compositeLine == "RRC|XB|") return HeroFlopState.FacingBetVsMissCbet;
                            if (compositeLine == "RRC|BR|") return HeroFlopState.FacingReraise;

                            return  HeroFlopState.None;
                        }
                    }
                    return HeroFlopState.None;
                }
            }
            return HeroFlopState.None;
        }

        public HeroTurnState DefineHeroTurnState(PotType potType, string compositeLine, Elements elements) {
            if (elements.CurrentStreet != CurrentStreet.Turn) return HeroTurnState.None;

            bool facingPush = CheckFacingPush(elements);
            switch (_potNtype) {
                case PotNType.Multipot: {
                        switch (elements.HeroPlayer.Position) {
                            case PlayerPosition.Button: {
                                
                                return HeroTurnState.None;
                                }
                            case PlayerPosition.Sb: {

                                    return HeroTurnState.None;
                                }
                            case PlayerPosition.Bb: {

                                    return HeroTurnState.None;
                                }
                        }
                        return HeroTurnState.None;
                    }
                case PotNType.Hu3Max: {
                        switch (elements.HeroPlayer.Position) {
                            case PlayerPosition.Button: {
                                if (compositeLine == "RCF|XBC|X|" || compositeLine == "RFC|XBC|X|") return HeroTurnState.Cbet2;
                                if (compositeLine == "RFRC|BC|B|" || compositeLine == "RRFC|BC|B|") return HeroTurnState.FacingCbet2;
                                    return HeroTurnState.None;
                                }
                            case PlayerPosition.Sb: {
                                if(compositeLine == "FRC|BC|") return HeroTurnState.Cbet2;
                                if(compositeLine == "FRC|XX|") return HeroTurnState.DelayCbet;
                                if(compositeLine == "FRC|XX|XB|") return HeroTurnState.BetVsMissTCb;
                                return HeroTurnState.None;
                                }
                            case PlayerPosition.Bb: {
                                if(compositeLine == "FLRC|XX|B|") return  HeroTurnState.BetVsMissFCb;
                                if(compositeLine == "RFC|XBC|XB|" || compositeLine == "FRC|BC|B|") return HeroTurnState.FacingCbet2;
                                if(compositeLine == "RFC|XX|XB|" || compositeLine == "FRC|XX|B") return HeroTurnState.FacingDelayCbet;
                                return HeroTurnState.None;
                                }
                        }
                        return HeroTurnState.None;
                    }
                case PotNType.Hu2Max: {
                        switch (elements.HeroPlayer.Position) {
                            case PlayerPosition.Sb: {
                                if(compositeLine == "LX|XX|B|") return  HeroTurnState.FacingDelayBet;
                                if(compositeLine == "RC|BC|B|") return  HeroTurnState.FacingDonk2;
                                if (compositeLine == "RC|XBC|B|") return HeroTurnState.FacingDonk;

                                if (compositeLine == "RC|XBC|X|" || compositeLine == "LX|XBC|X|") return HeroTurnState.Cbet2;
                                if(compositeLine == "RC|XBC|XBR|" || compositeLine == "LC|XBC|XBR|") return  HeroTurnState.FacingReraise;
                                if(compositeLine == "RC|XX|B|" || compositeLine == "LC|XX|B|") return  HeroTurnState.DelayCbet;

                                if(compositeLine == "LRC|BC|B|") return HeroTurnState.FacingCbet2;
                                if(compositeLine == "LRC|XX|B|") return HeroTurnState.FacingDelayCbet;

                                return HeroTurnState.None;
                            }
                            case PlayerPosition.Bb: {
                                if(compositeLine == "LX|XX|") return HeroTurnState.DelayBet;
                                if(compositeLine == "LX|XX|XB|") return HeroTurnState.FacingDelayBet;
                                if(compositeLine == "LX|BC|B|") return HeroTurnState.FacingBet2;

                                if(compositeLine == "LRC|XBC|") return HeroTurnState.Cbet2;
                                if(compositeLine == "LRC|XBC|B|") return HeroTurnState.FacingDonk;
                                if(compositeLine == "LRC|BC|B|") return HeroTurnState.FacingDonk2;

                                if(compositeLine == "RC|BC|") return HeroTurnState.Donk;
                                if (compositeLine == "RC|BC|XB|") return HeroTurnState.FacingCbet2;
                                if(compositeLine == "RC|XX|XB") return HeroTurnState.FacingDelayCbet;
                                return HeroTurnState.None;
                            }
                        }
                        return HeroTurnState.None;
                    }
            }
            return HeroTurnState.None;
        }

        public HeroRiverState DefineHeroRiverState(PotType potType, string compositeLine, Elements elements) {
            if (elements.CurrentStreet != CurrentStreet.Turn) return HeroRiverState.None;

            bool facingPush = CheckFacingPush(elements);
            switch (_potNtype)
            {
                case PotNType.Multipot:
                    {
                        switch (elements.HeroPlayer.Position)
                        {
                            case PlayerPosition.Button:
                                {

                                    return HeroRiverState.None;
                                }
                            case PlayerPosition.Sb:
                                {

                                    return HeroRiverState.None;
                                }
                            case PlayerPosition.Bb:
                                {

                                    return HeroRiverState.None;
                                }
                        }
                        return HeroRiverState.None;
                    }
                case PotNType.Hu3Max:
                    {
                        switch (elements.HeroPlayer.Position)
                        {
                            case PlayerPosition.Button:
                                {
                                    return HeroRiverState.None;
                                }
                            case PlayerPosition.Sb:
                                {
                                    if(compositeLine == "FRC|BC|BC|") return HeroRiverState.Cbet3;
                                    
                                    return HeroRiverState.None;
                                }
                            case PlayerPosition.Bb:
                                {
                                    if(compositeLine == "RFC|XBC|XBC|XB|" || compositeLine == "FRC|BC|BC|B|") return HeroRiverState.Cbet3;
                                    return HeroRiverState.None;
                                }
                        }
                        return HeroRiverState.None;
                    }
                case PotNType.Hu2Max:
                    {
                        switch (elements.HeroPlayer.Position)
                        {
                            case PlayerPosition.Sb:
                                {
                                    if(compositeLine == "RC|XBC|XBC|X|" || compositeLine == "LX|XBC|XBC|X|") return HeroRiverState.Cbet3;
                                    if(compositeLine == "RC|XBC|XBC|B" || compositeLine == "LX|XBC|XBC|B|") return HeroRiverState.FacingDonk;
                                    if(compositeLine == "RC|BC|BC|B|") return HeroRiverState.FacingDonk3;
                                    return HeroRiverState.None;
                                }
                            case PlayerPosition.Bb:
                                {
                                    if(compositeLine == "RC|XBC|XBC|") return HeroRiverState.Donk;
                                    if(compositeLine == "RC|XBC|XBC|XB|") return HeroRiverState.FacingCbet3;
                                    return HeroRiverState.None;
                                }
                        }
                        return HeroRiverState.None;
                    }
            }
            return HeroRiverState.None;
        }




        public void OnNewElements(Elements elements) {
            

            string btnLine = elements.InGamePlayers.Any(p => p.Position == PlayerPosition.Button)
                ? elements.InGamePlayers.First(p => p.Position == PlayerPosition.Button).Line
                : "";
            string sbLine = elements.InGamePlayers.Any(p => p.Position == PlayerPosition.Sb)
                ? elements.InGamePlayers.First(p => p.Position == PlayerPosition.Sb).Line
                : "";
            string bbLine = elements.InGamePlayers.Any(p => p.Position == PlayerPosition.Bb)
                ? elements.InGamePlayers.First(p => p.Position == PlayerPosition.Bb).Line
                : "";

            StringBuilder finalLine = new StringBuilder();

            var linesMass = new[] {btnLine, sbLine, bbLine};
            var flopLines = CropLines(linesMass);

            switch (elements.InGamePlayers.Count) {
                case 3: flopLines = new[] { flopLines[1], flopLines[2], flopLines[0] };
                    break;
                case 2: flopLines = new[] { flopLines[2], flopLines[1] };
                    break;
            }

            var turnLines = CropLines(flopLines);
            var riverLines = CropLines(turnLines);

            _preflopCompositeLine = CompositeLine(linesMass);
            _flopCompositeLine = CompositeLine(flopLines);
            _turnCompositeLine = CompositeLine(turnLines);
            _riverCompositeLine = CompositeLine(riverLines);


            finalLine.Append(CompositeLine(linesMass));
            finalLine.Append(CompositeLine(flopLines));
            finalLine.Append(CompositeLine(turnLines));
            finalLine.Append(CompositeLine(riverLines));

            _potNtype = DefinePotNType(elements);

            var potType = DefinePotType(_preflopCompositeLine);
            var preflopState =  DefineHeroPreflopState(elements);

            var flopState = DefineHeroFlopState(potType, finalLine.ToString(), elements);
            var turnState = DefineHeroTurnState(potType, finalLine.ToString(), elements);
            var riverState = DefineHeroRiverState(potType, finalLine.ToString(), elements);


            var lineInfo = new LineInfo() {
                Elements =  elements,
                FinalCompositeLine = finalLine.ToString(),
                PotNType =  _potNtype,
                PotType =  potType,
                HeroPreflopState =  preflopState,
                HeroFlopState =  flopState,
                HeroTurnState = turnState,
                HeroRiverState = riverState,

            };

            if (NewLineInfo != null) {
                NewLineInfo(lineInfo);
            }


        }

        private string[] CropLines(string[] linesMass)
        {
            string[] resultMass = new string[linesMass.Length];
            for (int i = 0; i < linesMass.Length; i++)
            {
                resultMass[i] = linesMass[i].Any(c => c == '|') ? linesMass[i].Substring(linesMass[i].IndexOf('|')) : "";
                if (resultMass[i].Length > 0) resultMass[i] = resultMass[i].Remove(0, 1);
            }
            return resultMass;
        }
        private string CompositeLine(string[] lineMassive) {
            StringBuilder result = new StringBuilder();

           var maxLength = lineMassive.Select(mass => mass.Length).Max();

            for (int i = 0; i < maxLength; i++) {
                foreach (var line in lineMassive) {
                    if (line.Length > i) {
                        result.Append(line[i]);
                        //if (line[i] == '|') break;
                    }
                }
                if (result[result.Length -1 ] == '|') break;
            }
            result = result.Replace("|", "");
            if(result.Length > 0) result.Append('|');
            return  result.ToString();
        }

        
    }

    
}
