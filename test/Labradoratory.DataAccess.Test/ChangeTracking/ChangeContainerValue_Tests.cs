using Labradoratory.DataAccess.ChangeTracking;
using Xunit;

namespace Labradoratory.DataAccess.Test.ChangeTracking
{
    public class ChangeContainerValue_Tests
    {
        [Fact]
        public void CurrentValue_EqualsInitialValueAtCreation()
        {
            var expectedValue = "Initial Value";
            var subject = new ChangeContainerValue(expectedValue);
            Assert.Equal(expectedValue, subject.CurrentValue);
        }

        [Fact]
        public void HasChanges_FalseOnInitialize()
        {
            var subject = new ChangeContainerValue("Initial Value");
            Assert.False(subject.HasChanges);
        }

        [Fact]
        public void HasChanges_TrueAfterCurrentValueSet()
        {
            var subject = new ChangeContainerValue("Initial Value");
            subject.CurrentValue = "FirstValue";
            Assert.True(subject.HasChanges);
        }
    }
}
