﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace Pingvi
{
    public class LineInfo {
        public Elements Elements { get; set; }

        public HeroPreflopState HeroPreflopState { get; set; }

        public HeroFlopState HeroFlopState { get; set; }

        public HeroTurnState HeroTurnState { get; set; }

        public HeroRiverState HeroRiverState { get; set; }

        public PotType PotType { get; set; }

        public PotNType PotNType { get; set; }

        public string FinalCompositeLine { get; set; }



    }
}
