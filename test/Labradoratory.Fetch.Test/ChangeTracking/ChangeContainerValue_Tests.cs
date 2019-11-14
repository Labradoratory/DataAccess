using Labradoratory.Fetch.ChangeTracking;
using Xunit;

namespace Labradoratory.Fetch.Test.ChangeTracking
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

        [Fact]
        public void Reset_HasChangesFalseAfterReset()
        {
            var subject = new ChangeContainerValue("Initial Value");
            subject.CurrentValue = "FirstValue";
            Assert.True(subject.HasChanges);
            subject.Reset();
            Assert.False(subject.HasChanges);
        }

        [Fact]
        public void Reset_CurrentValueBackToOldAfterReset()
        {
            var expectedValue = "Initial Value";
            var subject = new ChangeContainerValue(expectedValue);            
            subject.CurrentValue = "FirstValue";
            subject.Reset();
            Assert.Equal(expectedValue, subject.CurrentValue);
        }

        [Fact]
        public void GetChangeSet_GeneratesCorrectChangeSet()
        {
            var expectedOldValue = "Initial Value";
            var expectedNewValue = "New Value";
            var expectedKey = "Key";
            var subject = new ChangeContainerValue(expectedOldValue);
            subject.CurrentValue = expectedNewValue;
            var changes = subject.GetChangeSet(expectedKey);

            Assert.Single(changes);
            Assert.True(changes.ContainsKey(expectedKey));
            Assert.Equal(ChangeAction.Update, changes[expectedKey].Action);
            Assert.Equal(expectedOldValue, changes[expectedKey].OldValue);
            Assert.Equal(expectedNewValue, changes[expectedKey].NewValue);
        }

        [Fact]
        public void GetChangeSet_CommitFalseKeepsChanges()
        {
            var subject = new ChangeContainerValue("Initial Value");
            subject.CurrentValue = "New Value";
            Assert.True(subject.HasChanges);
            var changes = subject.GetChangeSet();
            Assert.True(subject.HasChanges);
        }

        [Fact]
        public void GetChangeSet_CommitTrueCommitsChanges()
        {
            var expectedNewValue = "New Value";
            var subject = new ChangeContainerValue("Initial Value");
            subject.CurrentValue = expectedNewValue;
            Assert.True(subject.HasChanges);
            var changes = subject.GetChangeSet(commit: true);
            Assert.False(subject.HasChanges);
            Assert.Equal(expectedNewValue, subject.CurrentValue);
        }

        [Fact]
        public void GetChangeSet_NullWhenNoChanges()
        {
            var subject = new ChangeContainerValue("Initial Value");
            Assert.False(subject.HasChanges);
            var changes = subject.GetChangeSet(commit: true);
            Assert.Null(changes);
        }
    }
}
