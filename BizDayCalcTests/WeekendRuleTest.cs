using System;
using Xunit;
using BizDayCalc;

namespace BizDayCalcTests
{
    public class UnitTest1
    {
        [Fact]
        public void TestCheckIsBusinessDay()
        {
            var rule = new WeekendRule();

            Assert.True(rule.CheckIsBusinessDay(new DateTime(2016, 6, 27)));
            Assert.False(rule.CheckIsBusinessDay(new DateTime(2016, 6, 26)));
        }

        [Theory]
        [InlineData("2016-06-27")] // Monday
        [InlineData("2016-03-28")] // Tuesday
        [InlineData("2016-06-29")] // Wednesday
        [InlineData("2016-06-30")] // Thursday
        [InlineData("2016-07-01")] // Friday
        public void IsBusinessDay(string date)
        {
            var rule = new WeekendRule();
            Assert.True(rule.CheckIsBusinessDay(DateTime.Parse(date)));
        }

        [Theory]
        [InlineData("2016-06-25")] // Saturday
        [InlineData("2016-03-26")] // Sunday
        public void IsNotBusinessDay(string date)
        {
            var rule = new WeekendRule();
            Assert.False(rule.CheckIsBusinessDay(DateTime.Parse(date)));
        }
    }
}
