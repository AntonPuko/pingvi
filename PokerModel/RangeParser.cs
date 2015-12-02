using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerModel
{
    public  class RangeParser : IDisposable {

        private const char Offsuited = 'o';
        private const char Suited = 's';

        private List<RangeHand> _parsedHands;

        

        public RangeParser() {
            _parsedHands = new List<RangeHand>();
        }
       
        public  List<RangeHand> Parse(string handNames) {
            var hNamesMass = handNames.Replace(" ", string.Empty).Split(',');

            foreach (var name in hNamesMass)
            {
                int nLength = name.Length;
                switch (nLength) {
                    case 2: ParseOffsuited(name); break;
                    case 3: ParseCase3(name); break;
                    case 4: ParseCase4(name); break;
                    case 5: ParsePaired3Or5(name); break;
                    case 7: ParseCase7(name); break;
                }
            }
            return _parsedHands;
        }

     

        private void ParseCase7(string name) {
            var c1Rang = (int)Card.SetRangByName(name.Substring(1, 1));
            var c2Rang = (int)Card.SetRangByName(name.Substring(5, 1));

            for (int i = c2Rang; i <= c1Rang; i++) {
                name = name.Insert(1, Card.SetRangName((Rang)i));
                name = name.Remove(2, 1);
                ParseCase3(name.Substring(0, 3));
            }
        }

        private void ParseCase4(string name ) {
            var c1Rang = (int)Card.SetRangByName(name.Substring(0, 1));
            var c2Rang = (int)Card.SetRangByName(name.Substring(1, 1));

            for (int i = c2Rang; i < c1Rang; i++) {
                name = name.Insert(1, Card.SetRangName((Rang) i));
                name = name.Remove(2, 1);
                ParseCase3(name.Substring(0,3));
            }
        }

        private  void ParseCase3(string name) {
            switch (name.Last()) {
                case Offsuited: ParseOffsuited(name.Substring(0,2)); break;
                case Suited: ParseSuited(name.Substring(0,2)); break;
                case '+': ParsePaired3Or5(name.Substring(0, 3)); break;
            }
        }

        private void ParseOffsuited(string name)
        {
            var c1Rang = Card.SetRangByName(name.First().ToString());
            var c2Rang = Card.SetRangByName(name.Last().ToString());
            if (c1Rang == c2Rang) {
                ParsePaired(name); return;
            }
            
            for (int i = 1; i <= 4; i++) {
                for (int j = 1; j <= 4; j++) {
                    if (i == j) continue;
                    var c1 = new Card(c1Rang, (Suit)i);
                    var c2 = new Card(c2Rang, (Suit)j);
                    _parsedHands.Add(new RangeHand(c1, c2));
                }
            }

        }

        private void ParseSuited(string name)
        {
            var c1Rang = Card.SetRangByName(name.First().ToString());
            var c2Rang = Card.SetRangByName(name.Last().ToString());
            

            for (int i = 1; i <= 4; i++)
            {
                for (int j = 1; j <= 4; j++)
                {
                    if (i != j) continue;
                    var c1 = new Card(c1Rang, (Suit)i);
                    var c2 = new Card(c2Rang, (Suit)j);
                    _parsedHands.Add(new RangeHand(c1, c2));
                }
            }

        }

        private void ParsePaired3Or5(string name) {
            const int cRangMax = (int) Rang.Ace;
            int cLimit = 0;
            int cRang = 0;
            if (name.Length == 3) {
                cRang = (int) Card.SetRangByName(name.Substring(0, 1));
                cLimit = cRangMax;
            }
            else if (name.Length == 5) {
                cLimit = (int) Card.SetRangByName(name.Substring(0, 1));
                cRang = (int) Card.SetRangByName(name.Substring(3, 1));
            }

                for (int i = cRang; i <= cLimit; i++) {
                name = name.Remove(0, 2);
                name = name.Insert(0, Card.SetRangName((Rang)i))
                    .Insert(0, Card.SetRangName((Rang)i));
                ParsePaired(name.Substring(0, 2));
            }
        }
        private void ParsePaired(string name) {
            var c1Rang = Card.SetRangByName(name.First().ToString());
            for (int i = 1; i <= 4; i++) {
                for (int j = i+1; j <= 4; j++) {
                    if (i == j) continue;
                    var c1 = new Card(c1Rang, (Suit)i);
                    var c2 = new Card(c1Rang, (Suit)j);
                    _parsedHands.Add(new RangeHand(c1, c2));
                }
            }
        }

        public void Dispose() {
           //TODO разобраться с этим методом
            
        }

       
    }
}
