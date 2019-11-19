using System;
using System.Collections.Generic;
using System.Text;
using Labradoratory.Fetch.ChangeTracking;
using Xunit;

namespace Labradoratory.Fetch.Test.ChangeTracking
{
    public class ChangeSet_Tests
    {
        [Fact]
        public void CombinePaths_ValueValue()
        {
            var part1 = "Part1";
            var part2 = "Part2";
            var expectedResult = $"{part1}.{part2}";
            var result = ChangeSet.CombinePaths(part1, part2);
            Assert.Equal(expectedResult, result);
        }

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
            var expectedKeyOne = "one";
            var expectedKeyTwo = "two";
            var subject = new ChangeSet();
            subject.Add(expectedKeyOne, new ChangeValue());
            var mergeMe = new ChangeSet();
            mergeMe.Add(expectedKeyTwo, new ChangeValue());

            subject.Merge(mergeMe);

            Assert.Equal(2, subject.Count);
            Assert.Single(mergeMe);
            Assert.Contains(expectedKeyOne, subject as IDictionary<string, ChangeValue>);
            Assert.Contains(expectedKeyTwo, subject as IDictionary<string, ChangeValue>);
        }
    }
}
