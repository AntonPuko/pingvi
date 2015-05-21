using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerModel.Combinations
{
    class Combination {

        public string Name { get; set; }

        private List<Card> _cardsList;

        private void DefineStraight() {
            var prevrange = 0;

            _cardsList.Sort();//tidi
            foreach (var card in _cardsList) {
                
            }
        }



    }
}
