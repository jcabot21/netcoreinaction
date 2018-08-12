using System;

namespace BizDayCalc
{
    public class WeekendRule : IRule
    {
        public bool CheckIsBusinessDay(DateTime date) =>
            date.DayOfWeek != DayOfWeek.Saturday 
            && date.DayOfWeek != DayOfWeek.Sunday;
    }
}