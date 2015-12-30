namespace Pingvi
{
    public class DecisionInfo
    {
        public LineInfo LineInfo { get; set; }
        public string PreflopRangeChosen { get; set; }
        public PreflopDecision PreflopDecision { get; set; }
        public double? RaiseSize { get; set; }
        public double PotOdds { get; set; }
    }
}