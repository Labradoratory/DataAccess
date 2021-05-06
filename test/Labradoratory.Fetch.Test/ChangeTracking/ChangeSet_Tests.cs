using System.Collections.Generic;
using Labradoratory.Fetch.ChangeTracking;
using Xunit;

namespace Labradoratory.Fetch.Test.ChangeTracking
{
    public class ChangeSet_Tests
    {
        [Fact]
        public void Merge_NullDoesNothing()
        {
            var subject = new ChangeSet();
            var mergeMe = default(ChangeSet);
            subject.Merge(mergeMe);
            Assert.Empty(subject);
        }

        [Fact]
        public void Merge_MergesEntries()
        {
            var expectetPathOne = ChangePath.Create("one");
            var expectedPathTwo = ChangePath.Create("two");
            var subject = new ChangeSet();
            subject.Append(expectetPathOne, new ChangeValue());
            var mergeMe = new ChangeSet();
            mergeMe.Append(expectedPathTwo, new ChangeValue());

            subject.Merge(mergeMe);

            Assert.Equal(2, subject.Count);
            Assert.Single(mergeMe);
            Assert.Contains(expectetPathOne, subject as IDictionary<ChangePath, List<ChangeValue>>);
            Assert.Contains(expectedPathTwo, subject as IDictionary<ChangePath, List<ChangeValue>>);
        }
    }
}
