using System;
using Labradoratory.Fetch.ChangeTracking;
using Xunit;
using System.Linq;

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
        public void HasChanges_String_FalseWhenSameValueSet()
        {
            var expected = "Initial Value";
            var subject = new ChangeContainerValue(expected);
            subject.CurrentValue = expected;
            Assert.False(subject.HasChanges);
        }
        
        // NOTE: This test was added because we were using == to check
        // equality and because the ChangeContainerValue treats the
        // values as objects, it was doing a reference comparison which
        // was failing for primitives (string worked fine).  This test 
        // covers making sure equality is being tested correctly.
        [Fact]
        public void HasChanges_Int_FalseWhenSameValueSet()
        {
            var expected = 123;
            var subject = new ChangeContainerValue(expected);
            subject.CurrentValue = expected;
            Assert.False(subject.HasChanges);
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
            var expectedPath = ChangePath.Create("Key");
            var subject = new ChangeContainerValue(expectedOldValue);
            subject.CurrentValue = expectedNewValue;
            var changes = subject.GetChangeSet(expectedPath);

            Assert.Single(changes);
            Assert.True(changes.ContainsKey(expectedPath));
            Assert.Single(changes[expectedPath]);
            Assert.Equal(ChangeAction.Update, changes[expectedPath][0].Action);
            Assert.Equal(expectedOldValue, changes[expectedPath][0].OldValue);
            Assert.Equal(expectedNewValue, changes[expectedPath][0].NewValue);
        }

        [Fact]
        public void GetChangeSet_GeneratesCorrectChangeSetForITracksChanges()
        {
            var path = ChangePath.Create("Key");
            var expectedPath = path.AppendProperty(nameof(NestedObject.StringValue));
            var nested = ChangeTrackingObject.CreateTrackable<NestedObject>();
            var subject = new ChangeContainerValue(nested);

            nested.StringValue = "My new value;";

            var changes = subject.GetChangeSet(path);
            Assert.Single(changes);
            Assert.True(changes.ContainsKey(expectedPath));
        }

        [Fact]
        public void GetChangeSet_CommitFalseKeepsChanges()
        {
            var subject = new ChangeContainerValue("Initial Value");
            subject.CurrentValue = "New Value";
            Assert.True(subject.HasChanges);
            var changes = subject.GetChangeSet(ChangePath.Empty);
            Assert.True(subject.HasChanges);
        }

        [Fact]
        public void GetChangeSet_CommitTrueCommitsChanges()
        {
            var expectedNewValue = "New Value";
            var subject = new ChangeContainerValue("Initial Value");
            subject.CurrentValue = expectedNewValue;
            Assert.True(subject.HasChanges);
            var changes = subject.GetChangeSet(ChangePath.Empty, commit: true);
            Assert.False(subject.HasChanges);
            Assert.Equal(expectedNewValue, subject.CurrentValue);
        }

        [Fact]
        public void GetChangeSet_EmptyWhenNoChanges()
        {
            var subject = new ChangeContainerValue("Initial Value");
            Assert.False(subject.HasChanges);
            var changes = subject.GetChangeSet(ChangePath.Empty, commit: true);
            Assert.Empty(changes);
        }

        [Fact]
        public void GetChangeSet_ThrowWhenPathNull()
        {
            var subject = new ChangeContainerValue("Initial Value");
            subject.CurrentValue = "new value";
            Assert.Throws<ArgumentNullException>(() => subject.GetChangeSet(null));
        }

        private class NestedObject : ChangeTrackingObject
        {
            public string StringValue
            {
                get => GetValue<string>();
                set => SetValue(value);
            }
        }
    }
}
