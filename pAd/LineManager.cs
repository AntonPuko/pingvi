using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pingvi
{
    enum HeroPreflopStatus {
        OpenLimp, OpenRaise, FacingOpen, FacingLimp, Facing3Bet, FacingPush
    }

    enum HeroFlopStatus
    {
       
    }

    class LineManager
    {
        string SLineHero { get; set; }
        string SLineLeftPlayer { get; set; }
        string SlineRightPlayer { get; set; }

        string ResultLine { get; set; }

        string BTNLine { get; set; }
        string SBLine { get; set; }
        string BBLine { get; set; }

        public LineManager() {
            BTNLine = "O";
            SBLine = "F";
            BBLine = "C";
        }




    }
}
