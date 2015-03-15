using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pingvi {
    public class RuleContext<T> {
        private T value;
        private LinkedList<Func<T, bool>> rules;

        public RuleContext(T v) {
            value = v;
        }

        public RuleContext<T> If(Func<T, bool> rule) {
            if(rules == null) 
                rules = new LinkedList<Func<T, bool>>();
            rules.AddLast(rule);
            return this;
        }

        public void Do(Action<T> work) {
            if (rules != null) {
                var result = true;
                foreach (var func in rules) {
                    result &= func(value);
                    if (result == false) return;
                }
            }
            work(value);
        }

    }
}

