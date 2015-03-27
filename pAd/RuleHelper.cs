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

        public static RuleContext<Elements> HeroPosition(this RuleContext<Elements> context, PlayerPosition position) {
            return context.If(e => e.HeroPlayer.Position == position);
        }

        public static RuleContext<Elements> EffectiveStackBetween(this RuleContext<Elements> context, double minAmtBB,
            double maxAmtBb) {
                return context.If(e => e.EffectiveStack > minAmtBB && e.EffectiveStack <= maxAmtBb);
        }

        public static RuleContext<Elements> HeroStackBetween(this RuleContext<Elements> context, double minAmtBB,
      double maxAmtBb)
        {
            return context.If(e => e.HeroPlayer.Stack > minAmtBB && e.HeroPlayer.Stack <= maxAmtBb);
        }

        public static RuleContext<Elements> Street(this RuleContext<Elements> context, CurrentStreet street) {
            return context.If(e => e.CurrentStreet == street);
        }

        public static RuleContext<Elements> IsHU(this RuleContext<Elements> context) {
            return context.If(e => e.ActivePlayers.Count == 2);
        }

        public static RuleContext<Elements> IsMP(this RuleContext<Elements> context) {
            return context.If(e => e.ActivePlayers.Count == 3);
        }

        public static RuleContext<Elements> VsBigStack(this RuleContext<Elements> context) {
            return context.If(e => {
                var oppStack = e.ActivePlayers.First(p => p.Name != e.HeroPlayer.Name).Stack;
                return 2*oppStack >= e.HeroPlayer.Stack;
            });
        }

        public static RuleContext<Elements> VsSmallStack(this RuleContext<Elements> context)
        {
            return context.If(e =>
            {
                var oppStack = e.ActivePlayers.First(p => p.Name != e.HeroPlayer.Name).Stack;
                return 2*oppStack < e.HeroPlayer.Stack;
            });
        }

        public static RuleContext<Elements> SitOutOpp(this RuleContext<Elements> context)
        {
            return context.If(e =>e.ActivePlayers.Count == 1 && e.SitOutPlayers.Any() &&  e.SitOutPlayers.First().Status == PlayerStatus.SitOut);
        } 

        public static RuleContext<Elements> HeroRole(this RuleContext<Elements> context, HeroRole role) {
            return context.If(e => e.HeroPlayer.Role == role);
        } 
        public static RuleContext<Elements> HeroState(this RuleContext<Elements> context, HeroStatePreflop statePreflop) {
            return context.If(e => e.HeroPlayer.StatePreflop == statePreflop);
        }

        public static RuleContext<Elements> HeroRelativePosition(this RuleContext<Elements> context,
            HeroRelativePosition relativePosition) {
            return context.If(e => e.HeroPlayer.RelativePosition == relativePosition);
        }

        public static RuleContext<Elements> OppTypes(this RuleContext<Elements> context, PlayerType[] types) {
            return
                context.If(
                    e => {
                        bool pt = false;
                        foreach (var t in types) {
                            if (e.InGamePlayers.First(p => p.Name != e.HeroPlayer.Name).Type == t) {
                                pt = true;
                                break;
                            }
                        }
                        return pt;
                    });

        } 

        public static RuleContext<Elements> OppBetSize(this RuleContext<Elements> context, double betSize) {
            return context.If(e => {
                var opponents = e.ActivePlayers.Where(p => p.Name != e.HeroPlayer.Name);
                if (!opponents.Any()) return false;
                double maxb = opponents.Select(o => o.Bet).Concat(new double[] {0}).Max();
                return maxb == betSize;
            });
        }

        public static RuleContext<Elements> OppBetSizeMinRaise(this RuleContext<Elements> context)
        {
            return context.If(e =>
            {
                var opponents = e.ActivePlayers.Where(p => p.Name != e.HeroPlayer.Name);
                if (!opponents.Any()) return false;

                double maxb = opponents.Select(o => o.Bet).Concat(new double[] { 0 }).Max();
                return maxb >= 2 && maxb < 2.5;
            });
        }

        public static RuleContext<Elements> BBEqOrMoreThen(this RuleContext<Elements> context, double BBSize) {
            return context.If(e => e.BbAmt >= BBSize);
        }

        public static RuleContext<Elements> Is3Max(this RuleContext<Elements> context)
        {
            return context.If(e => e.InGamePlayers.Count == 3);
        }

        public static RuleContext<Elements> Is2Max(this RuleContext<Elements> context)
        {
            return context.If(e => e.InGamePlayers.Count == 2);
        }

    }
}
