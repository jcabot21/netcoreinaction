using BizDayCalc;
using System;
using Xunit;

namespace BizDayCalcTests
{   
    // Fixures are used to share an object throughout all the tests
    // in a test class
    public class USRegionFixture
    {
        public Calculator Calc { get; }

        public USRegionFixture()
        {
            Calc = new Calculator();
            Calc.AddRule(new WeekendRule());
            Calc.AddRule(new HolidayRule());

            Console.WriteLine($"{nameof(USRegionFixture)} constructor called...");
        }
    }

    // Fixture collections can be used across multiple classes
    [CollectionDefinition(nameof(USRegionCollection))]
    public class USRegionCollection : ICollectionFixture<USRegionFixture>
    {
        // No implementation needed
    }
}