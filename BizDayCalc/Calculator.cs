using System;
using System.Collections.Generic;
using System.Linq;

namespace BizDayCalc
{
    public class Calculator
    {
        private IList<IRule> rules = new List<IRule>();

        public void AddRule(IRule rule) => rules.Add(rule);

        public bool IsBusinessDay(DateTime date) => rules.All(r => r.CheckIsBusinessDay(date));
    }
}
