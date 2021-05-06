using System;
using System.Collections.Generic;
using System.Linq;
using Labradoratory.Fetch.Processors.Stages;
using Moq;
using Xunit;

namespace Labradoratory.Fetch.Test.Processors.Stages
{
    public class NumericPriorityStage_Tests
    {
        [Fact]
        public void CompareTo_Correct_Equal()
        {
            var subject = new NumericPriorityStage(107);
            var compareTo = new NumericPriorityStage(107);

            var result = subject.CompareTo(compareTo);
            Assert.Equal(0, result);
        }

        [Fact]
        public void CompareTo_Correct_LessThan()
        {
            var subject = new NumericPriorityStage(107);
            var compareTo = new NumericPriorityStage(65);

            var result = subject.CompareTo(compareTo);
            Assert.Equal(-1, result);
        }

        [Fact]
        public void CompareTo_Correct_GreaterThan()
        {
            var subject = new NumericPriorityStage(107);
            var compareTo = new NumericPriorityStage(143);

            var result = subject.CompareTo(compareTo);
            Assert.Equal(1, result);
        }

        [Fact]
        public void CompareTo_Correct_null()
        {
            var subject = new NumericPriorityStage(107);
            var compareTo = (IStage)null;

            var result = subject.CompareTo(compareTo);
            Assert.Equal(1, result);
        }

        [Fact]
        public void CompareTo_Throws()
        {
            var subject = new NumericPriorityStage(107);
            var compareTo = Mock.Of<IStage>();

            Assert.Throws<InvalidOperationException>(() => subject.CompareTo(compareTo));
        }

        [Fact]
        public void CompareTo_CorrectOrder()
        {
            var item1 = new NumericPriorityStage(3654);
            var item2 = new NumericPriorityStage(345);
            var item3 = new NumericPriorityStage(6276);
            var item4 = new NumericPriorityStage(2);
            var item5 = new NumericPriorityStage(479);

            var items = new List<IStage> { item1, item2, item3, item4, item5 };
            var inOrder = items.OrderBy(i => i);

            Assert.Collection(inOrder.OfType<NumericPriorityStage>(),
                item => Assert.Same(item3, item),
                item => Assert.Same(item1, item),
                item => Assert.Same(item5, item),
                item => Assert.Same(item2, item),
                item => Assert.Same(item4, item));
        }
    }
}
