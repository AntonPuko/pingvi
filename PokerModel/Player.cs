using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerModel {
    public enum PlayerType {
        Unknown, Fish, WeakReg, GoodReg, UberReg, Rock, Maniac
    }

    public enum PlayerStatus {
        InHand, OutOfHand, OutOfGame, SitOut
    }

    public enum PlayerPosition {
        None,Button, Sb, Bb, 
    }
    public class Player {
        public string Name { get; set; }
        public PlayerType Type { get; set; }
        public PlayerPosition Position { get; set; }
        public Hand Hand { get; set; }
        public PlayerStatus Status { get; set; }

        public Stats Stats;

        public double CurrentStack { get; set; }

        public double Bet { get; set; }

        public double Stack { get { return CurrentStack + Bet; }
            set { _stack = value; }
        }

        private double _stack;

        public Player(string name) {
            Name = name;
            Hand = new Hand(new Card(), new Card());
        }
    }
}
