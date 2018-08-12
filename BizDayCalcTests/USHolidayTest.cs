using System;
using System.Collections.Generic;
using Xunit;
using BizDayCalc;

namespace BizDayCalcTests
{
    public class USHolidayTest
    {
        public static IEnumerable<object[]> Holidays 
        {
            get
            {
                yield return new object[] { new DateTime(2016, 1, 1) };
                yield return new object[] { new DateTime(2016, 7, 4) };
                yield return new object[] { new DateTime(2016, 12, 24) };
                yield return new object[] { new DateTime(2016, 12, 25) };
            }
        }

        private readonly Calculator _calculator;

        public USHolidayTest()
        {
            _calculator = new Calculator();
            _calculator.AddRule(new HolidayRule());
        }

        [Theory]
        [MemberData(nameof(Holidays))]
        public void TestHolidays(DateTime date) => Assert.False(_calculator.IsBusinessDay(date));
    }
}