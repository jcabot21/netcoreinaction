using System;
using System.Linq;

namespace BizDayCalc
{
    public class HolidayRule : IRule
    {
        private static readonly int[,] USHolidays = {
            { 1, 1 },   // New Year's day
            { 7, 4 },   // Indepdence day
            { 12, 24 }, // Christmas eve
            { 12, 25 }, // Christmas day
        };

        public bool CheckIsBusinessDay(DateTime date)
        {            
            for (var day = 0; day <= USHolidays.GetUpperBound(0); day++)
            {
                if (date.Month == USHolidays[day, 0] && date.Day == USHolidays[day, 1])
                {
                    return false;
                }
            }

            return true;
        }
    }
}