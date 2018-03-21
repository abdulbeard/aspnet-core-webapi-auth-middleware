using System;
using MisturTee.Extensions;
using Xunit;

namespace MisturTee.TestMeFool.Extensions
{
    public class DateTimeExtensionsTests
    {
        [Fact]
        public void DateTimeTest()
        {
            var dateTime = new DateTime(2000, 12, 10, 23, 58, 59);
            var secondsSinceEpoch = dateTime.TotalSecondsSinceEpoch();
            Assert.Equal(976492739, secondsSinceEpoch);
        }
    }
}
