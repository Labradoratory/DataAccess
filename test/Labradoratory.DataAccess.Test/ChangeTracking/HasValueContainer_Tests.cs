using System;
using Labradoratory.DataAccess.ChangeTracking;
using Xunit;

namespace Labradoratory.DataAccess.Test.ChangeTracking
{
    public class HasValueContainer_Tests
    {
        [Fact]
        public void HasValue_FalseAtInitialize()
        {
            var subject = new HasValueContainer<string>();
            Assert.False(subject.HasValue);
        }

        [Fact]
        public void HasValue_TrueWhenValueSet()
        {
            var subject = new HasValueContainer<string>();
            subject.Value = "test";
            Assert.True(subject.HasValue);
        }

        [Fact]
        public void Value_GetReturnsSet()
        {
            var subject = new HasValueContainer<string>();
            var expected = DateTime.Now.ToFileTimeUtc().ToString();
            subject.Value = expected;
            Assert.Equal(expected, subject.Value);
        }

        [Fact]
        public void Reset_HasValueIsFalse()
        {
            var subject = new HasValueContainer<string>();
            subject.Value = "test";
            subject.Reset();
            Assert.False(subject.HasValue);
        }

        [Fact]
        public void Reset_ValueIsDefault()
        {
            var subject = new HasValueContainer<int>();
            subject.Value = 100;
            subject.Reset();
            Assert.Equal(default(int), subject.Value);
        }
    }
}
