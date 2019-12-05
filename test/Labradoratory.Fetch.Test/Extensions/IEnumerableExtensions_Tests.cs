using System.Collections.Generic;
using Labradoratory.Fetch.Extensions;
using Xunit;

namespace Labradoratory.Fetch.Test.Extensions
{
    public class IEnumerableExtensions_Tests
    {
        [Fact]
        public void EmptyIfNull_ReturnsTargetWhenNotNull()
        {
            var target = new List<string>
            {
                "My test value 1",
                "My test value 2"
            };

            var result = target.EmptyIfNull();
            Assert.Same(target, result);
        }

        [Fact]
        public void EmptyIfNull_ReturnsEmptyWhenTargetNull()
        {
            IEnumerable<string> target = null;
            var result = target.EmptyIfNull();
            Assert.Empty(result);
        }
    }
}
