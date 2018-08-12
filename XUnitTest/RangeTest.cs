using GameLogic.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace XUnitTest
{
    public class RangeTest
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(1.5)]
        public void ClosedInterval_InRange_Test(float num)
        {
            Range range = new Range(1, 2);
            range.lowOpen = range.highOpen = false;
            Assert.True(range.InRange(num));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(0.5)]
        [InlineData(2.5)]
        public void OpenInterval_OutRange_Test(float num)
        {
            Range range = new Range(1, 2);
            range.lowOpen = range.highOpen = true;
            Assert.True(range.OutOfRange(num));
        }

    }
}
