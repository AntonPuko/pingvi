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

        
        public LineManager() {
            SLineHero = "O.B";
            SLineLeftPlayer = "F";
            SlineRightPlayer = "C.XC";
        }



    }
}
