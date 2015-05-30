using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pingvi
{
    public class DecisionInfo {
        public LineInfo LineInfo { get; set; }
        public string PreflopRangeChosen { get; set; }
        public PreflopDecision PreflopDecision { get; set; }
        public double PotOdds { get; set; }
    }
}
