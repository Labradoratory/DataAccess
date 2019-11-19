using System.Collections.Generic;
using System.Linq;
using Labradoratory.Fetch.ChangeTracking;
using Xunit;

namespace Labradoratory.Fetch.Test.ChangeTracking
{
    public class ChangeTrackingCollection_Tests
    {
        [Fact]
        public void Ctor_NoParams_Empty()
        {
            var subject = new ChangeTrackingCollection<TestItem>();
            Assert.Empty(subject);
        }

        [Fact]
        public void Ctor_Collection_ContainsItems()
        {
            var expectedItems = new List<TestItem>
            {
                new TestItem(),
                new TestItem(),
                new TestItem()
            };
            var subject = new ChangeTrackingCollection<TestItem>(expectedItems);
            Assert.Equal(expectedItems.Count, subject.Count);
            Assert.Same(expectedItems[0], subject.ElementAt(0));
            Assert.Same(expectedItems[1], subject.ElementAt(1));
            Assert.Same(expectedItems[2], subject.ElementAt(2));
        }

        [Fact]
        public void HasChanges_FalseAtStart()
        {
            var expectedItems = new List<TestItem>
            {
                ChangeTrackingObject.CreateTrackable<TestItem>()
            };
            var subject = new ChangeTrackingCollection<TestItem>(expectedItems);
            Assert.False(subject.HasChanges);
        }

        [Fact]
        public void HasChanges_TrueWhenAdd()
        {
            var subject = new ChangeTrackingCollection<TestItem>();
            subject.Add(ChangeTrackingObject.CreateTrackable<TestItem>());
            Assert.True(subject.HasChanges);
        }

        [Fact]
        public void HasChanges_TrueWhenRemove()
        {
            var expectedItems = new List<TestItem>
            {
                new TestItem(),
                new TestItem(),
                new TestItem()
            };
            var subject = new ChangeTrackingCollection<TestItem>(expectedItems);
            subject.Remove(expectedItems[0]);
            Assert.True(subject.HasChanges);
        }

        [Fact]
        public void HasChanges_TrueWhenItemChange()
        {
            var item = ChangeTrackingObject.CreateTrackable<TestItem>();
            var expectedItems = new List<TestItem>
            {
                item
            };
            var subject = new ChangeTrackingCollection<TestItem>(expectedItems);
            item.StringValue = "New value";
            Assert.True(subject.HasChanges);
        }

        [Fact]
        public void IsReadOnly_False()
        {
            var subject = new ChangeTrackingCollection<TestItem>();
            Assert.False(subject.IsReadOnly);
        }

        [Fact]
        public void Clear_RemovesAllItems()
        {
            var expectedItems = new List<TestItem>
            {
                new TestItem(),
                new TestItem(),
                new TestItem()
            };
            var subject = new ChangeTrackingCollection<TestItem>(expectedItems);
            subject.Clear();
            Assert.True(subject.HasChanges);
            Assert.Empty(subject);
        }

        [Fact]
        public void Contains_ReturnsTrueWhenFound()
        {
            var findMe = new TestItem();
            var expectedItems = new List<TestItem>
            {
                new TestItem(),
                findMe,
                new TestItem()
            };
            var subject = new ChangeTrackingCollection<TestItem>(expectedItems);
            Assert.Contains(findMe, subject);
        }

        [Fact]
        public void Contains_ReturnsFalseWhenNotFound()
        {
            var findMe = new TestItem();
            var expectedItems = new List<TestItem>
            {
                new TestItem(),
                new TestItem(),
                new TestItem()
            };
            var subject = new ChangeTrackingCollection<TestItem>(expectedItems);
            Assert.DoesNotContain(findMe, subject);
        }

        [Fact]
        public void CopyTo_CopiesItemsToArray()
        {
            var expectedItems = new List<TestItem>
            {
                new TestItem(),
                new TestItem(),
                new TestItem()
            };
            var subject = new ChangeTrackingCollection<TestItem>(expectedItems);
            var populateMe = new TestItem[5];
            subject.CopyTo(populateMe, 1);
            Assert.Null(populateMe[0]);
            Assert.Same(expectedItems[0], populateMe[1]);
            Assert.Same(expectedItems[1], populateMe[2]);
            Assert.Same(expectedItems[2], populateMe[3]);
            Assert.Null(populateMe[4]);
        }

        [Fact]
        public void Remove_FalseWhenNotFound()
        {
            var expectedItems = new List<TestItem>
            {
                new TestItem(),
                new TestItem(),
                new TestItem()
            };
            var subject = new ChangeTrackingCollection<TestItem>(expectedItems);
            Assert.False(subject.Remove(new TestItem()));
        }

        [Fact]
        public void GetChangeSet_ContainsAdds()
        {
            var subject = new ChangeTrackingCollection<TestItem>();
            var expectedItem = new TestItem();
            subject.Add(expectedItem);
            var path = "path";
            var expectedKey = $"{path}.1.add";
            var result = subject.GetChangeSet(path);
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Contains(expectedKey, result as IDictionary<string, ChangeValue>);
            var change = result[expectedKey];
            Assert.Equal(ChangeAction.Add, change.Action);
            Assert.Equal(ChangeTarget.Collection, change.Target);
            Assert.Null(change.OldValue);
            Assert.Same(expectedItem, change.NewValue);
        }

        [Fact]
        public void GetChangeSet_ContainsRemoves()
        {
            var expectedRemove = new TestItem();
            var expectedItems = new List<TestItem>
            {
                new TestItem(),
                expectedRemove,
                new TestItem()
            };
            var subject = new ChangeTrackingCollection<TestItem>(expectedItems);
            var path = "path";
            var expectedKey = $"{path}.1.remove";
            Assert.True(subject.Remove(expectedRemove));
            var result = subject.GetChangeSet(path);
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Contains(expectedKey, result as IDictionary<string, ChangeValue>);
            var change = result[expectedKey];
            Assert.Equal(ChangeAction.Remove, change.Action);
            Assert.Equal(ChangeTarget.Collection, change.Target);
            Assert.Null(change.NewValue);
            Assert.Same(expectedRemove, change.OldValue);
        }

        [Fact]
        public void GetChangeSet_ContainsUpdates()
        {
            var expectedOldValue = "My old value";
            var expectedNewValue = "The new value";
            var expectedUpdate = new TestItem
            {
                StringValue = expectedOldValue
            };
            var expectedItems = new List<TestItem>
            {
                new TestItem(),
                expectedUpdate,
                new TestItem()
            };
            var subject = new ChangeTrackingCollection<TestItem>(expectedItems);
            var path = "path";
            var expectedKey = $"{path}.1.{nameof(TestItem.StringValue)}";
            expectedUpdate.StringValue = expectedNewValue;
            var result = subject.GetChangeSet(path);
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Contains(expectedKey, result as IDictionary<string, ChangeValue>);
            var change = result[expectedKey];
            Assert.Equal(ChangeAction.Update, change.Action);
            Assert.Equal(ChangeTarget.Object, change.Target);
            Assert.Equal(expectedNewValue, change.NewValue);
            Assert.Equal(expectedOldValue, change.OldValue);
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
