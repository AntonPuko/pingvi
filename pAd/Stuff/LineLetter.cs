using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pingvi.Stuff
{
    class LineLetter {
        public string Letter { get; private set; }

        public int LetterNumber { get; private set; }

        public LineLetter(int letterNumber)
        {
            switch (letterNumber)
            {
                case 0: Letter = "f"; break;
                case 1: Letter = "l"; break;
                case 2: Letter = "r"; break;
                case 3: Letter = "c"; break;
                case 4: Letter = "x"; break;
                case 5: Letter = "b"; break;
                case 6: Letter = "|"; break;
                case 7: Letter = ""; break;
                default: Letter = ""; break;
            }
        }
    }
}
