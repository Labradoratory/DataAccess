using System;
using System.Collections.Generic;
using Labradoratory.Fetch.ChangeTracking;
using Moq;
using Xunit;

namespace Labradoratory.Fetch.Test.ChangeTracking
{
    public class ChangeItemContainer_Tests
    {
        [Fact]
        public void Ctor_ValuesCorrect()
        {
            var expectedItem = new TestItem();
            var expectedTarget = ChangeTarget.Collection;
            var expectedAction = ChangeAction.Add;
            var subject = new ChangeContainerItem<TestItem>(expectedItem, ChangeTarget.Collection, ChangeAction.Add);
            Assert.Same(expectedItem, subject.Item);
            Assert.Equal(expectedTarget, subject.Target);
            Assert.Equal(expectedAction, subject.Action);
        }

        [Fact]
        public void Ctor_DefaultAction()
        {
            var expectedItem = new TestItem();
            var subject = new ChangeContainerItem<TestItem>(expectedItem, ChangeTarget.Collection);
            Assert.Equal(ChangeAction.None, subject.Action);
        }

        [Fact]
        public void Action_ItemHasChanges_ReturnsUpdate()
        {
            var expectedItem = ChangeTrackingObject.CreateTrackable<TestItem>();
            expectedItem.StringValue = "Updated value";
            Assert.True(expectedItem.HasChanges);

            var subject = new ChangeContainerItem<TestItem>(expectedItem, ChangeTarget.Object);
            Assert.Equal(ChangeAction.Update, subject.Action);
        }

        [Fact]
        public void Action_SetWorks()
        {
            var subject = new ChangeContainerItem<TestItem>(new TestItem(), ChangeTarget.Object);
            Assert.Equal(ChangeAction.None, subject.Action);

            var expectedAction = ChangeAction.Add;
            subject.Action = expectedAction;
            Assert.Equal(expectedAction, subject.Action);
        }

        [Fact]
        public void Item_SetWorksAndChangesAction()
        {
            var subject = new ChangeContainerItem<TestItem>(new TestItem(), ChangeTarget.Object);
            var expectedItem = new TestItem();
            subject.Item = expectedItem;
            Assert.Same(expectedItem, subject.Item);
            Assert.Equal(ChangeAction.Update, subject.Action);
        }

        [Fact]
        public void HasChanges_FalseWhenNoChanges()
        {
            var subject = new ChangeContainerItem<TestItem>(new TestItem(), ChangeTarget.Object);
            Assert.False(subject.HasChanges);
        }

        [Fact]
        public void HasChanges_TrueWhenDirectChanges()
        {
            var subject = new ChangeContainerItem<TestItem>(new TestItem(), ChangeTarget.Object);
            subject.Item = new TestItem();
            Assert.True(subject.HasChanges);
        }

        [Fact]
        public void HasChanges_TrueWhenChangesToItem()
        {
            var item = ChangeTrackingObject.CreateTrackable<TestItem>();
            var subject = new ChangeContainerItem<TestItem>(item, ChangeTarget.Object);
            item.StringValue = "Updated value";
            Assert.True(subject.HasChanges);
        }

        [Fact]
        public void Reset_DoesNothingIfNoChanges()
        {
            var mockItem = new Mock<ChangeTrackingObject>(MockBehavior.Strict);
            var expectedTarget = ChangeTarget.Collection;
            var subject = new ChangeContainerItem<ChangeTrackingObject>(mockItem.Object, ChangeTarget.Collection);
            subject.Reset();
            Assert.Same(mockItem.Object, subject.Item);
            Assert.Equal(expectedTarget, subject.Target);
            Assert.Equal(ChangeAction.None, subject.Action);
        }

        [Fact]
        public void Reset_RevertsDirectChanges()
        {
            var expectedItem = new TestItem { StringValue = "Orginal" };
            var subject = new ChangeContainerItem<TestItem>(expectedItem, ChangeTarget.Collection);
            subject.Item = new TestItem();
            subject.Reset();

            Assert.False(subject.HasChanges);
            Assert.Same(expectedItem, subject.Item);
            Assert.Equal(ChangeAction.None, subject.Action);
        }

        [Fact]
        public void Reset_RevertsItemChanges()
        {
            var expectedItem = ChangeTrackingObject.CreateTrackable<TestItem>();
            var subject = new ChangeContainerItem<TestItem>(expectedItem, ChangeTarget.Collection);
            subject.Item.StringValue = "My new value";
            Assert.True(subject.HasChanges);
            Assert.Equal(ChangeAction.Update, subject.Action);

            subject.Reset();
            Assert.Same(expectedItem, subject.Item);
            Assert.False(subject.HasChanges);
            Assert.Equal(ChangeAction.None, subject.Action);
        }

        [Fact]
        public void GetChangeSet_CorrectWhenAdd()
        {
            var expectedItem = ChangeTrackingObject.CreateTrackable<TestItem>();
            var expectedChangeAction = ChangeAction.Add;
            var subject = new ChangeContainerItem<TestItem>(expectedItem, ChangeTarget.Collection, expectedChangeAction);
            var path = ChangePath.Create("Path");
            var result = subject.GetChangeSet(path);

            var expectedPath = path.WithAction(ChangeAction.Add);

            Assert.Single(result);
            Assert.Contains(expectedPath, result as IDictionary<ChangePath, ChangeValue>);
            var change = result[expectedPath];
            Assert.Equal(expectedChangeAction, change.Action);
            Assert.Null(change.OldValue);
            Assert.Same(expectedItem, change.NewValue);
        }

        [Fact]
        public void GetChangeSet_CorrectWhenDirectUpdate()
        {
            var originalItem = ChangeTrackingObject.CreateTrackable<TestItem>();
            originalItem.StringValue = "New value";
            var subject = new ChangeContainerItem<TestItem>(originalItem, ChangeTarget.Collection);

            var expectedItem = ChangeTrackingObject.CreateTrackable<TestItem>();
            subject.Item = expectedItem;

            var expectedPath = ChangePath.Create("Path");
            var result = subject.GetChangeSet(expectedPath);

            Assert.Single(result);
            Assert.Contains(expectedPath, result as IDictionary<ChangePath, ChangeValue>);
            var change = result[expectedPath];
            Assert.Equal(ChangeAction.Update, change.Action);
            Assert.Same(originalItem, change.OldValue);
            Assert.Same(expectedItem, change.NewValue);
        }

        [Fact]
        public void GetChangeSet_CorrectWhenItemUpdate()
        {
            var expectedOldValue = "My old value";
            var expectedItem = new TestItem
            {
                StringValue = expectedOldValue
            };
            var subject = new ChangeContainerItem<TestItem>(expectedItem, ChangeTarget.Collection);

            var expectedNewValue = "New value";
            expectedItem.StringValue = expectedNewValue;

            var path = ChangePath.Create("Path");
            var result = subject.GetChangeSet(path);

            var expectedPath = path.AppendProperty(nameof(TestItem.StringValue));

            Assert.Single(result);
            Assert.Contains(expectedPath, result as IDictionary<ChangePath, ChangeValue>);
            var change = result[expectedPath];
            Assert.Equal(ChangeAction.Update, change.Action);
            Assert.Same(expectedOldValue, change.OldValue);
            Assert.Same(expectedNewValue, change.NewValue);
        }

        [Fact]
        public void GetChangeSet_ThrowsIfUpdateNonTrackChanges()
        {
            var expectedOldValue = "My old value";
            var expectedItem = new TestItem
            {
                StringValue = expectedOldValue
            };
            var subject = new ChangeContainerItem<string>("Value", ChangeTarget.Collection, ChangeAction.Update);

            Assert.Throws<InvalidOperationException>(() => subject.GetChangeSet(ChangePath.Empty));
        }

        [Fact]
        public void GetChangeSet_CorrectWhenRemove()
        {
            var expectedItem = ChangeTrackingObject.CreateTrackable<TestItem>();
            var expectedChangeAction = ChangeAction.Remove;
            var subject = new ChangeContainerItem<TestItem>(expectedItem, ChangeTarget.Collection, expectedChangeAction);
            var path = ChangePath.Create("Path");
            var result = subject.GetChangeSet(path);

            var expectedPath = path.WithAction(ChangeAction.Remove);

            Assert.Single(result);
            Assert.Contains(expectedPath, result as IDictionary<ChangePath, ChangeValue>);
            var change = result[expectedPath];
            Assert.Equal(expectedChangeAction, change.Action);
            Assert.Null(change.NewValue);
            Assert.Same(expectedItem, change.OldValue);
        }

        [Fact]
        public void GetChangeSet_HasChangeFalseWhenCommit()
        {
            var expectedItem = ChangeTrackingObject.CreateTrackable<TestItem>();
            var subject = new ChangeContainerItem<TestItem>(expectedItem, ChangeTarget.Collection);
            expectedItem.StringValue = "New value";

            subject.GetChangeSet(ChangePath.Create("path"), true);

            Assert.False(subject.HasChanges);
            Assert.False(expectedItem.HasChanges);
        }

        [Fact]
        public void GetChangeSet_NullWhenNoChanges()
        {
            var expectedItem = ChangeTrackingObject.CreateTrackable<TestItem>();
            var subject = new ChangeContainerItem<TestItem>(expectedItem, ChangeTarget.Collection);
            var result = subject.GetChangeSet(ChangePath.Create("path"));
            Assert.Null(result);
        }

        private class TestItem : ChangeTrackingObject
        {
            public string StringValue
            {
                get => GetValue<string>();
                set => SetValue(value);
            }
        }
    }
}
