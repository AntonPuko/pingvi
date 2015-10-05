using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralLineDetection
{
    public class LineLetter {
        public char Letter { get;  private set; }

        public int LetterNumber { get; private set; }

        public LineLetter(int letterNumber) {
            switch (letterNumber) {
                case 0: Letter = 'f'; break;
                case 1: Letter = 'l'; break;
                case 2: Letter = 'r'; break;
                case 3: Letter = 'c'; break;
                case 4: Letter = 'x'; break;
                case 5: Letter = 'b'; break;
                case 6: Letter = 's'; break;
                case 7: Letter = 'n'; break;
                default:Letter = 'n';break;
            }
        }

    }
}
