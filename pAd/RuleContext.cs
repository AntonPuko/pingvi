using System;
using System.Collections.Generic;

namespace Pingvi
{
    public class RuleContext<T>
    {
        private readonly T _value;
        private LinkedList<Func<T, bool>> _rules;

        public RuleContext(T v) {
            _value = v;
        }

        public RuleContext<T> If(Func<T, bool> rule) {
            if (_rules == null)
                _rules = new LinkedList<Func<T, bool>>();
            _rules.AddLast(rule);
            return this;
        }

        public void Do(Action<T> work) {
            if (_rules != null)
            {
                var result = true;
                foreach (var func in _rules)
                {
                    result &= func(_value);
                    if (result == false) return;
                }
            }
            work(_value);
        }
    }
}