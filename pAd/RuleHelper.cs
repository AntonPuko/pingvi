using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokerModel;

namespace Pingvi {
    public static class RuleHelper {
        public static RuleContext<T> StartRule<T>(this T value) {
            return new RuleContext<T>(value);
        }

        public static RuleContext<LineInfo> TourneyMultiplierEqOrMoreThan(this RuleContext<LineInfo> context, int value) {
            return context.If(l => l.Elements.TourneyMultiplier >= value);
        }

        public static RuleContext<LineInfo> TourneyMultiplierEqOrLessThan(this RuleContext<LineInfo> context, int value) {
            return context.If(l => l.Elements.TourneyMultiplier <= value);
        } 

        public static RuleContext<LineInfo> HeroPreflopState(this RuleContext<LineInfo> context,
            HeroPreflopState state) {
            return context.If(e => e.HeroPreflopState == state);
        }

        public static RuleContext<LineInfo> HeroFlopState(this RuleContext<LineInfo> context,
            HeroFlopState state) {
            return context.If(e => e.HeroFlopState == state);
        }

        public static RuleContext<LineInfo> LeftPlayerType(this RuleContext<LineInfo> context, PlayerType playerType) {
            return context.If(l => l.Elements.LeftPlayer.Type == playerType);
        }

        public static RuleContext<LineInfo> HeroPosition(this RuleContext<LineInfo> context, PlayerPosition position)
        {
            return context.If(l => l.Elements.HeroPlayer.Position == position);
        }

        public static RuleContext<LineInfo> EffectiveStackBetween(this RuleContext<LineInfo> context, double minAmtBB,
            double maxAmtBb) {
                return context.If(l => l.Elements.EffectiveStack > minAmtBB && l.Elements.EffectiveStack <= maxAmtBb);
        }

        public static RuleContext<LineInfo> EffectiveStackSbVsBtnBetween(this RuleContext<LineInfo> context, double minAmtBB,
         double maxAmtBb)
        {
            return context.If(l => l.Elements.SbBtnEffStack > minAmtBB && l.Elements.SbBtnEffStack <= maxAmtBb);
        }

        public static RuleContext<LineInfo> HeroStackBetween(this RuleContext<LineInfo> context, double minAmtBB,
      double maxAmtBb)
        {
            return context.If(l => l.Elements.HeroPlayer.Stack > minAmtBB && l.Elements.HeroPlayer.Stack <= maxAmtBb);
        }

        public static RuleContext<LineInfo> Street(this RuleContext<LineInfo> context, CurrentStreet street)
        {
            return context.If(l => l.Elements.CurrentStreet == street);
        }

        public static RuleContext<LineInfo> IsHU(this RuleContext<LineInfo> context)
        {
            return context.If(l => l.Elements.ActivePlayers.Count == 2);
        }

        public static RuleContext<LineInfo> IsMP(this RuleContext<LineInfo> context)
        {
            return context.If(l => l.Elements.ActivePlayers.Count == 3);
        }

        public static RuleContext<LineInfo> VsBigStack(this RuleContext<LineInfo> context)
        {
            return context.If(l => {
                var oppStack = l.Elements.ActivePlayers.First(p => p.Name != l.Elements.HeroPlayer.Name).Stack;
                return 2*oppStack >= l.Elements.HeroPlayer.Stack;
            });
        }

        public static RuleContext<LineInfo> VsSmallStack(this RuleContext<LineInfo> context)
        {
            return context.If(l =>
            {
                var oppStack = l.Elements.ActivePlayers.First(p => p.Name != l.Elements.HeroPlayer.Name).Stack;
                return 2*oppStack < l.Elements.HeroPlayer.Stack;
            });
        }

        public static RuleContext<LineInfo> SitOutOpp(this RuleContext<LineInfo> context)
        {
            return context.If(l =>l.Elements.ActivePlayers.Count == 1 && l.Elements.SitOutPlayers.Any() &&  l.Elements.SitOutPlayers.First().Status == PlayerStatus.SitOut);
        }

        public static RuleContext<LineInfo> HeroRole(this RuleContext<LineInfo> context, HeroRole role)
        {
            return context.If(l => l.Elements.HeroPlayer.Role == role);
        }
        public static RuleContext<LineInfo> HeroState(this RuleContext<LineInfo> context, HeroStatePreflop statePreflop)
        {
            return context.If(l => l.Elements.HeroPlayer.StatePreflop == statePreflop);
        }

        public static RuleContext<LineInfo> HeroRelativePosition(this RuleContext<LineInfo> context,
            HeroRelativePosition relativePosition) {
            return context.If(l => l.Elements.HeroPlayer.RelativePosition == relativePosition);
        }

        public static RuleContext<LineInfo> OppTypes(this RuleContext<LineInfo> context, PlayerType[] types)
        {
            return
                context.If(
                    l => {
                        bool pt = false;
                        foreach (var t in types) {
                            if (l.Elements.InGamePlayers.First(p => p.Name != l.Elements.HeroPlayer.Name).Type == t) {
                                pt = true;
                                break;
                            }
                        }
                        return pt;
                    });

        }

        public static RuleContext<LineInfo> OppBetSize(this RuleContext<LineInfo> context, double betSize)
        {
            return context.If(l => {
                var opponents = l.Elements.ActivePlayers.Where(p => p.Name != l.Elements.HeroPlayer.Name);
                if (!opponents.Any()) return false;
                double maxb = opponents.Select(o => o.Bet).Concat(new double[] {0}).Max();
                return maxb == betSize;
            });
        }

        public static RuleContext<LineInfo> OppBetSizeBetween(this RuleContext<LineInfo> context, double min, double max)
        {
            return context.If(l =>
            {
                var opponents = l.Elements.ActivePlayers.Where(p => p.Name != l.Elements.HeroPlayer.Name);
                if (!opponents.Any()) return false;
                double maxb = opponents.Select(o => o.Bet).Concat(new double[] { 0 }).Max();
                return maxb >= min && maxb <= max;
            });
        }

        public static RuleContext<LineInfo> OppBetSizeMinRaise(this RuleContext<LineInfo> context)
        {
            return context.If(l =>
            {
                var opponents = l.Elements.ActivePlayers.Where(p => p.Name != l.Elements.HeroPlayer.Name);
                if (!opponents.Any()) return false;

                double maxb = opponents.Select(o => o.Bet).Concat(new double[] { 0 }).Max();
                return maxb >= 2 && maxb <= 2.5;
            });
        }

        public static RuleContext<LineInfo> OppPosition(this RuleContext<LineInfo> context, PlayerPosition position)
        {
            return context.If(l => l.Elements.HuOpp.Position == position);
        }

        public static RuleContext<LineInfo> OppStackBetween(this RuleContext<LineInfo> context, double min, double max)
        {
            return context.If(l => l.Elements.HuOpp.Stack > min && l.Elements.HuOpp.Stack <= max);
        }

        public static RuleContext<LineInfo> BBEqOrMoreThen(this RuleContext<LineInfo> context, double BBSize)
        {
            return context.If(l => l.Elements.BbAmt >= BBSize);
        }


        public static RuleContext<LineInfo> BBEqOrLessThen(this RuleContext<LineInfo> context, double BBSize)
        {
            return context.If(l => l.Elements.BbAmt <= BBSize);
        }

        public static RuleContext<LineInfo> Is3Max(this RuleContext<LineInfo> context)
        {
            return context.If(l => l.Elements.InGamePlayers.Count == 3);
        }

        public static RuleContext<LineInfo> Is2Max(this RuleContext<LineInfo> context)
        {
            return context.If(l => l.Elements.InGamePlayers.Count == 2);
        }

    }
}
