using System;
using System.Linq;
using System.Text;

using PokerModel;

namespace Pingvi {


    public enum HeroPreflopStatus {
        None,
        Agressor,
        Defender,
        Limper,
    }
    public enum HeroPreflopState {
        None,
        Open,
        FacingOpen,
        FacingLimp,
        FacingDoubleLimp,
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
        VsMissFCb,
        vsMissTCb,
        VsMissDonk2,
        BetAfterReraiseFlop,
        DelayCbet,
        Cbet2,
        FacingCbet2,
        FacingReraise,
        FacingCheckAfterFlopReraise,
        FacingCbetAfterFlopReraise,
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


        private PotNType DefinePotNType(Elements elements) {
            if(elements.ActivePlayers.Count == 3) return PotNType.Multipot;
            if(elements.ActivePlayers.Count == 2 && elements.InGamePlayers.Count == 3) return PotNType.Hu3Max;
            if(elements.InGamePlayers.Count == 2) return PotNType.Hu2Max;
            return PotNType.None;
        }

        private PotType DefinePotType(string preflopCompositiveLine) {
            int R = preflopCompositiveLine.Count(l => l == 'R' || l == 'r');
            int L = preflopCompositiveLine.Count(l => l == 'L' || l == 'l');
            if(L == 1 && R == 0) return  PotType.Limped;
            if(L == 1 && R == 1) return PotType.IsoLimped;
            if(R == 1) return PotType.Raised;
            if (R >= 2) return PotType.Reraised;
            return PotType.None;
            
        }

        private bool CheckFacingPush(Elements elements) {
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

        private bool CheckLineInLinesMass(string line, string[] linesMass) {
            var isLineInMass = false;
            foreach (var l in linesMass) {
                if (l == line) isLineInMass = true;
            }
            return isLineInMass;
        }
        private HeroPreflopStatus DefineHeroPreflopStatus(string preflopLine) {
            string[] agressorLines = {
                "Rcf|","Rfc|","Rcc|","rRfc|","rRcf|","rRcc|","fRc|","lRfc|","lRcc|","Rc|", 
                "llRfc|","llRcf|","llRcc|","lfRc|","flRc|","rfRc|","rcRcf|","rcRfc|","rcRcc|","lRc|","rRc|",  "frRc|",
            };
            if(CheckLineInLinesMass(preflopLine, agressorLines)) return HeroPreflopStatus.Agressor;
            string[] defenderLines = {
                "RrfC|", "RrcC|", "RfrC|", "RcrCf|", "RcrCc|",  "lLrcC|", "lLrfC|",
                "rCf|", "rCc|", "rCrcC|", "rCrfC|", "LrC|", "RrC|", "frC|", "flRrC|", "lrCc|", "lrCf|", "rcC|", "rfC|",
                "rrCf|","rrCc|", "rC|", "fRrC|"
            };
            if (CheckLineInLinesMass(preflopLine, defenderLines)) return HeroPreflopStatus.Defender;
            string[] limperLines = { "Llx|", "lLx|", "llX|", "lfX|", "lX|", "flX|", "fLx|", "Lx|" , "Lfx", };

            if (CheckLineInLinesMass(preflopLine, limperLines)) return HeroPreflopStatus.Limper;
            return  HeroPreflopStatus.None;
        }
        private HeroPreflopState DefineHeroPreflopState(Elements elements) {
            if (elements.CurrentStreet != CurrentStreet.Preflop) return HeroPreflopState.None;

            bool facingPush = CheckFacingPush(elements);

            //Open
            string[] openMass = {"", "f|"};
            bool open = CheckLineInLinesMass(_preflopCompositeLine, openMass);
            if(open) return HeroPreflopState.Open;
           
            //FacingOpen
            string[] facingOpenMass = {"r|", "fr|", "rf|",};
            bool facingOpen = CheckLineInLinesMass(_preflopCompositeLine, facingOpenMass);
            if (facingOpen && !facingPush) return HeroPreflopState.FacingOpen;
            //FacingLimp
            string[] facingLimpMass = {"l|", "fl|", "lf|"};
            bool facingLimp = CheckLineInLinesMass(_preflopCompositeLine, facingLimpMass);
            if (facingLimp) return HeroPreflopState.FacingLimp;
            //FacingDoubleLimp
            string[] facingDoubleLimpMass = { "ll|" };
            bool facingDoubleLimp = CheckLineInLinesMass(_preflopCompositeLine, facingDoubleLimpMass);
            if (facingDoubleLimp) return HeroPreflopState.FacingDoubleLimp;
            //FacingOpenPush
            string[] facingOpenPushMass = {"r|", "fr|", "rf|"};
            bool facingOpenPush = CheckLineInLinesMass(_preflopCompositeLine, facingOpenPushMass);
            if (facingOpenPush && facingPush) return HeroPreflopState.FacingOpenPush;
            //FacingPushVsOpen
            string[] facingPushVsOpenMass = { "Rrf|", "Rfr|", "fRr|", "Rr|"};
            bool facingPushVsOpen = CheckLineInLinesMass(_preflopCompositeLine, facingPushVsOpenMass);
            if (facingPushVsOpen && facingPush) return HeroPreflopState.FacingPushVsOpen;
            //FacingPushVsLimp
            string[] facingPushVsLimpMass = { "fLr|", "Lr|", };
            bool facingPushVsLimp = CheckLineInLinesMass(_preflopCompositeLine, facingPushVsLimpMass);
            if (facingPushVsLimp && facingPush) return HeroPreflopState.FacingPushVsLimp;
            //Facing3bet
            string[] facing3BetMass = {"RrF|", "Rfr|", "fRr|", "Rr|", "Rrf|"};
            bool facing3Bet = CheckLineInLinesMass(_preflopCompositeLine, facing3BetMass);
            if (facing3Bet && !facingPush) return HeroPreflopState.Facing3Bet;
            //Facing3betSqeeze
            string[] facing3BetSqueezeMass = {"Rcr|", "rCr|"};
            bool facing3BetSqueeze = CheckLineInLinesMass(_preflopCompositeLine, facing3BetSqueezeMass);
            if (facing3BetSqueeze && !facingPush) return HeroPreflopState.Facing3BetSqueeze;
            //FacingPushSqeeze
            string[] facingPushSqueezeMass = { "Rcr|", "rCr|" };
            bool facingPushSqueeze = CheckLineInLinesMass(_preflopCompositeLine, facingPushSqueezeMass);
            if (facingPushSqueeze && !facingPush) return HeroPreflopState.FacingPushSqueeze;
            //FacingISOvsLimp
            string[] facingISOvsLimpMass = {"Lrf|", "LfR|", "fLr|", "Lr|"};
            bool facingISOvsLimp = CheckLineInLinesMass(_preflopCompositeLine, facingISOvsLimpMass);
            if (facingISOvsLimp && !facingPush) return HeroPreflopState.FacingISOvsLimp;
            //FacingReraiseToISO
            string[]facingReraiseToISOMass = { "lfRr|", "flRr|", "lRr|"};
            bool facingReraiseToISO = CheckLineInLinesMass(_preflopCompositeLine, facingReraiseToISOMass);
            if (facingReraiseToISO && !facingPush) return HeroPreflopState.FacingReraiseIso;
            //FacingPushToISO
            string[] facingPushToISOMass = { "lfRr|", "flRr|", "lRr|" };
            bool facingPushToISO = CheckLineInLinesMass(_preflopCompositeLine, facingPushToISOMass);
            if (facingPushToISO && facingPush) return HeroPreflopState.FacingPushToIso;
            //FacingRaiseCall
            string[] facingRaiseCallMass = { "rc|" };
            bool facingRaiseCall = CheckLineInLinesMass(_preflopCompositeLine, facingRaiseCallMass);
            if (facingRaiseCall && facingPush) return HeroPreflopState.FacingRaiseCall;

            return HeroPreflopState.None;
        }

        private HeroFlopState DefineHeroFlopState(string compositeLine, Elements elements) {

            if(elements.CurrentStreet != CurrentStreet.Flop) return HeroFlopState.None;
            //Donk
            string[] donkMass = {"rC|","rfC|","fRrC|", "rCf|"};
            bool donk = CheckLineInLinesMass(compositeLine, donkMass);
            if (donk) return HeroFlopState.Donk;

            //Bet
            string[] betMass = {"lLx|x|", "Llx|xx|", "lfX|", "flX|x|", "lX|", "fLx|"};
            bool bet = CheckLineInLinesMass(compositeLine, betMass);
            if(bet) return HeroFlopState.Bet;
            //Cbet 
            string[] cbetMass = {
                "Rcc|xx|", "Rcf|x|", "Rfc|x|", "lRfc|", "rRfc|", "fRc|", "lfRc|", "rfRc|", "flRc|x|",
                "Lx|x|", "Rc|x|", "lRc|", "rRc|", "lRcf|", "frRc|x|"
            };
            bool cbet = CheckLineInLinesMass(compositeLine, cbetMass);
            if (cbet) return HeroFlopState.Cbet;
            //FacingBet
            string[] facingbetMass = { "lLx|Xbf|", "lLx|Xbc|", "lX|Xb|", "Lx|b|", "flX|b|" };
            bool facingbet = CheckLineInLinesMass(compositeLine, facingbetMass);
            if (facingbet) return HeroFlopState.FacingBet;
            //FacingCbet
            string[] facingCbetMass = {
                "rCc|Xxb|", "rcC|xXbf|", "rcC|xXbc|", "rC|Xb|", "RrC|b|", "frC|b|", "rfC|Xb|", "fRrC|Xb|", "rCf|Xb|",
                "RrfC|b|", "RfrC|b|", "LrC|b|", 
            };
            bool facingCbet = CheckLineInLinesMass(compositeLine, facingCbetMass);
            if (facingCbet) return HeroFlopState.FacingCbet;
            //FacingDonk
            string[] facingDonkMass = {"Rcc|bf|", "Rcc|bc|", "Rcc|xb|", "Rcf|b|", "Rfc|b|", "Rc|b|", "flRc|b|", "rcC|b|"};
            bool facingDonk = CheckLineInLinesMass(compositeLine, facingDonkMass);
            if(facingDonk) return HeroFlopState.FacingDonk;
            //FacingBet
            string[] facingBetMass = {
                "lX|Xb|", "lfX|Xb|", "Llx|b|", "Llx|xb|", "lLx|Xbf|", "lLx|Xbc|", "lLx|Xxb|",
                "flX|Xb|", "lX|b|"
            };
            bool facingBet = CheckLineInLinesMass(compositeLine, facingBetMass);
            if(facingBet) return HeroFlopState.FacingBet;
            //FacingMissCbet
            string[] facingMissCbetMass = {"RrfC|x|", "RfrC|x|", "frC|x|", "LrC|x", "RrC|x|",  "LrC|x|"};
            bool facingMissCbet = CheckLineInLinesMass(compositeLine, facingMissCbetMass);
            if(facingMissCbet) return HeroFlopState.FacingMissCbet;
            //FacingBetVsMissCbet
            string[] facingBetVsMissCbetMass = {"rfRc|Xb|", "lfRc|Xb|", "rRfC|Xb|", "fRc|Xb|", "lRc|Xb|", "rRc|Xb|"};
            bool facingBetVsMissCbet = CheckLineInLinesMass(compositeLine, facingBetVsMissCbetMass);
            if (facingBetVsMissCbet) return HeroFlopState.FacingBetVsMissCbet;
            //FacingReraise
            string[] facingReraiseMass = {
                "Rcf|xBr|", "lfX|Br|", "fRc|Br|", "lfRc|Br|", "rfRc|Br|", "flX|xBr|", "flRc|xBr|",
                "frC|xBr|", "Rc|xBr|", "lX|Br|", "rRc|Br|", "Rfc|xBr|", 
            };
            bool facingReraise = CheckLineInLinesMass(compositeLine, facingReraiseMass);
            if(facingReraise) return HeroFlopState.FacingReraise;

            return HeroFlopState.None;
        }

        private HeroTurnState DefineHeroTurnState(string compositeLine, Elements elements) {
            if (elements.CurrentStreet != CurrentStreet.Turn) return HeroTurnState.None;
            //Donk
            string[] donkMass = { "rC|XbC|",  "rfC|XbC|", "lX|XbC|"};
            bool donk = CheckLineInLinesMass(compositeLine, donkMass);
            if(donk) return HeroTurnState.Donk;
            //Bet2 
            string[] bet2Mass = { "lX|Bc|", "Lx|xBc|x|", "frC|xBc|x|", "lfX|Bc|", "flX|xBc|x|", "frC|xBc|x|", "fLx|Bc|"};
            bool bet2 = CheckLineInLinesMass(compositeLine, bet2Mass);
            if(bet2) return HeroTurnState.Bet2;
            //Cbet2
            string[] cbet2Mass = {
                "Rcf|xBc|x|", "Rfc|xBc|x|", "fRc|Bc|", "Rc|xBc|x|", "Lx|xBc|x|", "lRc|Bc|",
                "flRc|xBc|x|", "lfRc|Bc|"
            };
            bool cbet2 = CheckLineInLinesMass(compositeLine, cbet2Mass);
            if (cbet2) return HeroTurnState.Cbet2;

            //DelayBet
            string[] delayBetMass = {"lX|Xx|", "lfX|Xx|", "flX|xX|x|"};
            bool delayBet = CheckLineInLinesMass(compositeLine, delayBetMass);
            if(delayBet) return HeroTurnState.DelayBet;
            //DelayCbet
            string[] delayCbetMass = {"Rcf|xX|x|", "Rfc|xX|x|", "fRc|Xx|", "Rc|xX|x|", "Lx|xX|x|", "lRc|Xx|",
                "flRc|xBc|x|" , "flRc|xX|x|"};
            bool delayCbet = CheckLineInLinesMass(compositeLine, delayCbetMass);
            if(delayCbet) return HeroTurnState.DelayCbet;;
            //BetAfterReraiseFlop
            string[] betAfterReraiseFlopMass = {"rC|XbRc|", "rfC|XbRc|", "lX|XbRc|", "Lx|bRc|x|"};
            bool betAfterReraiseFlop = CheckLineInLinesMass(compositeLine, betAfterReraiseFlopMass);
            if(betAfterReraiseFlop) return HeroTurnState.BetAfterReraiseFlop;
           
            //FacingCbet2
            string[] facingCbet2Mass = {"RfrC|bC|b|", "RrfC|bC|b|", "rfC|XbC|Xb|", "frC|bC|b|", "LrC|bC|b|", "rC|XbC|Xb|", "RrC|bC|b|", "flX|bC|b|"};
            bool facingCbet2 = CheckLineInLinesMass(compositeLine, facingCbet2Mass);
            if(facingCbet2) return HeroTurnState.FacingCbet2;


            //facingDelayCbet2
            string[] facingDelayCbetMass = {"rC|Xx|Xb|", "LrC|xX|b|", "rfC|Xx|Xb|", "frC|Xx|b|"};
            bool facingDelayCbet = CheckLineInLinesMass(compositeLine, facingDelayCbetMass);
            if (facingDelayCbet) return HeroTurnState.FacingDelayCbet;
            //facingDonk
            string[] facingDonkMass = {"Rcf|xBc|b|", "Rc|xBc|b|", "flRc|xBc|b|"};
            bool facingDonk = CheckLineInLinesMass(compositeLine, facingDonkMass);
            if(facingDonk) return HeroTurnState.FacingDonk;

            //facingDonk2
            string[] facingDonk2Mass = { "Rcc|bfC|b|", "Rcc|bcC|bf|", "Rcc|bcC|bc|", "Rcc|xbCc|xb|", "Rcc|xbCf|b|", "Rcf|bC|b|", "Rfc|bC|b|", "Rc|bC|b|" };
            bool facingDonk2 = CheckLineInLinesMass(compositeLine, facingDonk2Mass);
            if(facingDonk2) return HeroTurnState.FacingDonk2;
            //
            //VsMissFLopCbet
            string[] vsMissFlopCbetMass = {"rfC|Xx|", "rC|Xx|", "frC|xX|x|"};
            bool vsMissFlopCbet = CheckLineInLinesMass(compositeLine, vsMissFlopCbetMass);
            if(vsMissFlopCbet) return HeroTurnState.VsMissFCb;
            //vsMissTurnCbet
            string[] vsMissTurnCbetMass = { "RfrC|bC|x|", "RrfC|bC|x|",  "frC|bC|x|", "LrC|bC|x|", "flX|bC|x|" };
            bool vsMissTurnCbet = CheckLineInLinesMass(compositeLine, vsMissTurnCbetMass);
            if(vsMissTurnCbet) return HeroTurnState.vsMissTCb;
            //facingDelayBet
            string[] facingDelayBetMass = {"lfX|Xx|Xb|"};
            bool facingDelayBet = CheckLineInLinesMass(compositeLine, facingDelayBetMass);
            if(facingDelayBet) return HeroTurnState.FacingDelayBet;
            //vsMissDonk2
            string[] vsMissDonk2Mass = {"flRc|bC|x|"};
            bool vsMissDonk2 = CheckLineInLinesMass(compositeLine, vsMissDonk2Mass);
            if (vsMissDonk2) return HeroTurnState.VsMissDonk2;
            //vsRFCbet
            string[] facingCbetkAfterFlopReraiseMass = {"Rc|xBrC|b|", "Lx|xBrC|b|"};
            bool facingCbetkAfterFlopReraise = CheckLineInLinesMass(compositeLine, facingCbetkAfterFlopReraiseMass);
            if( facingCbetkAfterFlopReraise) return HeroTurnState.FacingCbetAfterFlopReraise;
            //vs2CbetReraise
            string[] facingReraiseMass = {"Rc|xBc|xBr|"};
            bool facingReraise = CheckLineInLinesMass(compositeLine, facingReraiseMass);
            if(facingReraise) return HeroTurnState.FacingReraise;
     
            return HeroTurnState.None;
        }

        private HeroRiverState DefineHeroRiverState( string compositeLine, Elements elements) {
            if (elements.CurrentStreet != CurrentStreet.River) return HeroRiverState.None;
            //Cbet3
            string[] cbet3Mass = {"fRc|Bc|Bc|", "Rc|Bc|Bc|", "rRc|Bc|Bc|", "rRf|Bc|Bc|", "flRc|xBc|xBc|x|"};
            bool cbet3 = CheckLineInLinesMass(compositeLine, cbet3Mass);
            if(cbet3) return HeroRiverState.Cbet3;
            //facingCbet3
            string[] facingCbet3Mass = {"rC|XbC|XbC|Xb|", "rfC|XbC|XbC|Xb|", "frC|bC|bC|b|", "lX|XbC|XbC|Xb|"};
            bool facingCbet3 = CheckLineInLinesMass(compositeLine, facingCbet3Mass);
            if(facingCbet3) return HeroRiverState.FacingCbet3;
           
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

            switch (elements.HeroPlayer.Position) {
                    case PlayerPosition.Button:
                    btnLine = btnLine.ToUpper();
                    break;
                    case PlayerPosition.Sb:
                    sbLine = sbLine.ToUpper();
                    break;
                    case PlayerPosition.Bb:
                    bbLine = bbLine.ToUpper();
                    break;

            }
                



                StringBuilder finalLine = new StringBuilder();

                var linesMass = new[] {btnLine, sbLine, bbLine};
                var flopLines = CropLines(linesMass);

                switch (elements.InGamePlayers.Count) {
                    case 3:
                        flopLines = new[] {flopLines[1], flopLines[2], flopLines[0]};
                        break;
                    case 2:
                        flopLines = new[] {flopLines[2], flopLines[1]};
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
                var preflopState = DefineHeroPreflopState(elements);
            var heroPreflopStatus = DefineHeroPreflopStatus(_preflopCompositeLine);

                var flopState = DefineHeroFlopState(finalLine.ToString(), elements);
                var turnState = DefineHeroTurnState(finalLine.ToString(), elements);
                var riverState = DefineHeroRiverState(finalLine.ToString(), elements);


                var lineInfo = new LineInfo() {
                    HeroPreflopStatus =  heroPreflopStatus,
                    Elements = elements,
                    FinalCompositeLine = finalLine.ToString(),
                    PotNType = _potNtype,
                    PotType = potType,
                    HeroPreflopState = preflopState,
                    HeroFlopState = flopState,
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
