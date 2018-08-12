using System;
using BizDayCalc;
using Xunit;
using Xunit.Abstractions; // source for ITestOutputHelper

namespace BizDayCalcTests
{
    public class USRegionTest : IClassFixture<USRegionFixture>
    {
        private readonly USRegionFixture _fixture;
        private readonly ITestOutputHelper _output; // Xunit logger

        // constructor will we called for each test
        // but fixture will only be created once
        public USRegionTest(USRegionFixture fixture, ITestOutputHelper output) 
        {
            _fixture = fixture;
            _output = output;  // Injected automagically by Xunit
        }

        [Theory]
        [InlineData("2016-01-01")]
        [InlineData("2016-12-25")]
        [Trait("Holiday", "true")] // Used from cmd 'dotnet test --filter Holiday=true'
        public void TestHolidays(string date) 
        {
            // Will log when test fails
            _output.WriteLine($@"{nameof(TestHolidays)}(""{date}"")");
            Assert.False(_fixture.Calc.IsBusinessDay(DateTime.Parse(date)));
        }

        [Theory]
        [InlineData("2016-02-29")]
        [InlineData("2016-01-04")]
        [Trait("Holiday", "true")] // Used from cmd 'dotnet test --filter Holiday=true'
        public void TestNonHolidays(string date) => 
            Assert.True(_fixture.Calc.IsBusinessDay(DateTime.Parse(date)));
    }

    // Demo of a Fixture collection
    [Collection(nameof(USRegionCollection))]
    public class USRegionCollectionHolidayTest
    {
        private readonly USRegionFixture _fixture;

        // Constructor will be called for each tests
        // fixture will only be called once for set of classes
        public USRegionCollectionHolidayTest(USRegionFixture fixture)
        {
            _fixture = fixture;
        }

        // Repeating test here
        [Theory]
        [InlineData("2016-01-01")]
        [InlineData("2016-12-25")]
        public void TestHolidays(string date) => 
            Assert.False(_fixture.Calc.IsBusinessDay(DateTime.Parse(date)));
    }

    [Collection(nameof(USRegionCollection))]
    public class USRegionCollectionNotHolidayTest
    {
        private readonly USRegionFixture _fixture;

        // Constructor will be called for each tests
        // fixture will only be called once for set of classes
        public USRegionCollectionNotHolidayTest(USRegionFixture fixture)
        {
            _fixture = fixture;
        }

        // Repeating test here
        [Theory]
        [InlineData("2016-02-29")]
        [InlineData("2016-01-04")]
        public void TestNonHolidays(string date) => 
            Assert.True(_fixture.Calc.IsBusinessDay(DateTime.Parse(date)));
    }
}