using System.Collections.Generic;
using Labradoratory.Fetch.ChangeTracking;
using Xunit;

namespace Labradoratory.Fetch.Test.ChangeTracking
{
    public class ChangePath_Tests
    {
        [Fact]
        public void Create_Property_Success()
        {
            var expectedValue = "test";
            var path = ChangePath.Create(expectedValue);
            Assert.Single(path.Parts);
            Assert.True(path.Parts[0] is ChangePathProperty);
            var cpp = path.Parts[0] as ChangePathProperty;
            Assert.Equal(expectedValue, cpp.Property);
        }

        [Fact]
        public void Create_Part_Success()
        {
            var expectedPart = new ChangePathProperty("value");
            var path = ChangePath.Create(expectedPart);
            Assert.Single(path.Parts);
            Assert.Same(expectedPart, path.Parts[0]);
        }

        [Fact]
        public void Append_Success()
        {
            var expectedPart = new ChangePathProperty("value");
            var path = ChangePath.Empty.Append(expectedPart);
            Assert.Single(path.Parts);
            Assert.Same(expectedPart, path.Parts[0]);
        }

        [Fact]
        public void AppendProperty_Success()
        {
            var expectedValue = "test";
            var path = ChangePath.Empty.AppendProperty(expectedValue);
            Assert.Single(path.Parts);
            Assert.True(path.Parts[0] is ChangePathProperty);
            var cpp = path.Parts[0] as ChangePathProperty;
            Assert.Equal(expectedValue, cpp.Property);
        }

        [Fact]
        public void AppendKey_Success()
        {
            var expectedValue = "test";
            var path = ChangePath.Empty.AppendKey(expectedValue);
            Assert.Single(path.Parts);
            Assert.True(path.Parts[0] is ChangePathKey);
            var cpk = path.Parts[0] as ChangePathKey;
            Assert.Equal(expectedValue, cpk.Key);
        }

        [Fact]
        public void AppendIndex_Success()
        {
            var expectedValue = 123;
            var path = ChangePath.Empty.AppendIndex(expectedValue);
            Assert.Single(path.Parts);
            Assert.True(path.Parts[0] is ChangePathIndex);
            var cpi = path.Parts[0] as ChangePathIndex;
            Assert.Equal(expectedValue, cpi.Index);
        }

        [Fact]
        public void AppendAction_Success()
        {
            var expectedValue = ChangeAction.Remove;
            var path = ChangePath.Empty.AppendAction(expectedValue);
            Assert.Single(path.Parts);
            Assert.True(path.Parts[0] is ChangePathAction);
            var cpa = path.Parts[0] as ChangePathAction;
            Assert.Equal(expectedValue, cpa.Action);
        }

        [Fact]
        public void Equals_Success()
        {
            var one = ChangePath.Create("one");
            var two = ChangePath.Create("one");
            Assert.Equal(one, two);
        }

        [Fact]
        public void Equals_Fails()
        {
            var one = ChangePath.Create("one");
            var two = ChangePath.Create("two");
            Assert.NotEqual(one, two);
        }

        [Fact]
        public void Equals_NotSameType_Fails()
        {
            var one = ChangePath.Create("one");
            var two = new ChangeSet();
            Assert.False(Equals(one, two));
        }

        [Fact]
        public void GetHashCode_EqualsTrue()
        {
            var one = ChangePath.Create("one").AppendProperty("two");
            var two = ChangePath.Create("one").AppendProperty("two");
            var hashOne = one.GetHashCode();
            var hashTwo = two.GetHashCode();

            Assert.Equal(hashOne, hashTwo);
        }
    }
}
