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
        public void GetChangeSet_CommitFalse_ContainsAdds()
        {
            var subject = new ChangeTrackingCollection<TestItem>();
            var expectedItem = new TestItem();
            subject.Add(expectedItem);
            var basePath = ChangePath.Create("path");
            var result = subject.GetChangeSet(basePath);
            var expectedPath = basePath.AppendIndex(0);
            Assert.True(subject.HasChanges);
            Assert.NotNull(result);
            var kvp = Assert.Single(result);
            Assert.Equal(ChangeTarget.Collection, kvp.Key.Target);
            Assert.Contains(expectedPath, result as IDictionary<ChangePath, List<ChangeValue>>);
            var values = result[expectedPath];
            var value = Assert.Single(values);
            Assert.Equal(ChangeAction.Add, value.Action);
            Assert.Null(value.OldValue);
            Assert.Same(expectedItem, value.NewValue);
        }

        [Fact]
        public void GetChangeSet_CommitFalse_ContainsRemoves()
        {
            var expectedRemove = new TestItem();
            var expectedItems = new List<TestItem>
            {
                new TestItem(),
                expectedRemove,
                new TestItem()
            };
            var subject = new ChangeTrackingCollection<TestItem>(expectedItems);
            var basePath = ChangePath.Create("path");
            Assert.True(subject.Remove(expectedRemove));
            var result = subject.GetChangeSet(basePath);
            var expectedPath = basePath.AppendIndex(1);
            Assert.True(subject.HasChanges);
            Assert.NotNull(result);
            var kvp = Assert.Single(result);
            Assert.Equal(ChangeTarget.Collection, kvp.Key.Target);
            Assert.Contains(expectedPath, result as IDictionary<ChangePath, List<ChangeValue>>);
            var values = result[expectedPath];
            var value = Assert.Single(values);
            Assert.Equal(ChangeAction.Remove, value.Action);
            Assert.Null(value.NewValue);
            Assert.Same(expectedRemove, value.OldValue);
        }

        [Fact]
        public void GetChangeSet_CommitFalse_ContainsUpdates()
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
            var path = ChangePath.Create("path");
            var expectedPath = path.AppendIndex(1).AppendProperty(nameof(TestItem.StringValue));
            expectedUpdate.StringValue = expectedNewValue;
            var result = subject.GetChangeSet(path);
            Assert.True(subject.HasChanges);
            Assert.NotNull(result);
            var kvp = Assert.Single(result);
            Assert.Equal(ChangeTarget.Object, kvp.Key.Target);
            Assert.Contains(expectedPath, result as IDictionary<ChangePath, List<ChangeValue>>);
            var values = result[expectedPath];
            var value = Assert.Single(values);
            Assert.Equal(ChangeAction.Update, value.Action);
            Assert.Equal(expectedNewValue, value.NewValue);
            Assert.Equal(expectedOldValue, value.OldValue);
        }

        [Fact]
        public void GetChangeSet_CommitTrue_MakesChangesPermanent()
        {
            var removeMe = ChangeTrackingObject.CreateTrackable<TestItem>();
            var expectedUpdate = ChangeTrackingObject.CreateTrackable<TestItem>();
            var expectedItems = new List<TestItem>
            {
                removeMe,
                expectedUpdate
            };
            var subject = new ChangeTrackingCollection<TestItem>(expectedItems);
            subject.Add(new TestItem());
            subject.Remove(removeMe);
            var expectedNewValue = "New Value";
            expectedUpdate.StringValue = expectedNewValue;

            var result = subject.GetChangeSet(ChangePath.Empty, commit: true);
            Assert.Equal(3, result.Count);
            Assert.False(subject.HasChanges);
            Assert.False(expectedUpdate.HasChanges);
            Assert.Equal(expectedNewValue, expectedUpdate.StringValue);
            Assert.DoesNotContain(removeMe, subject);
            Assert.Equal(2, subject.Count);
        }

        [Fact]
        public void GetChangeSet_NullWhenNoChanges()
        {
            var subject = new ChangeTrackingCollection<TestItem>();
            Assert.Null(subject.GetChangeSet(ChangePath.Empty));
        }

        [Fact]
        public void Reset_HasChangesTrue_ResetsAllchanges()
        {
            var removeMe = new TestItem();
            var expectedOldValue = "My old value";
            var expectedNewValue = "The new value";
            var expectedUpdate = new TestItem
            {
                StringValue = expectedOldValue
            };
            var expectedItems = new List<TestItem>
            {
                removeMe,
                expectedUpdate
            };
            var subject = new ChangeTrackingCollection<TestItem>(expectedItems);
            expectedUpdate.StringValue = expectedNewValue;
            subject.Add(new TestItem());
            subject.Remove(removeMe);

            subject.Reset();

            Assert.False(subject.HasChanges);
            var changes = subject.GetChangeSet(ChangePath.Empty);
            Assert.Null(changes);
            Assert.False(expectedUpdate.HasChanges);
        }

        [Fact]
        public void Reset_HasChangesFalse_DoesNothing()
        {
            var subject = new ChangeTrackingCollection<TestItem>();
            subject.Reset();
        }

        [Fact]
        public void Add_ItemAlreadyRemovedGetsRestored()
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
            Assert.Equal(2, subject.Count);

            subject.Add(expectedItems[0]);
            Assert.False(subject.HasChanges);
            Assert.Contains(expectedItems[0], subject);
        }

        [Fact]
        public void GetChangeSet_AddAfterClear_Correct()
        {
            var expectedItems = new List<string>
            {
                "Value1",
                "Value2",
                "Value3"
            };
            var addItems = new List<string>
            {
                "Value1",
                "Value2",
                "Value3",
                "Value4"
            };
            var subject = new ChangeTrackingCollection<string>(expectedItems);
            subject.Clear();
            addItems.ForEach(item => subject.Add(item));
            var changes = subject.GetChangeSet(ChangePath.Empty);
            Assert.Single(changes);
            Assert.Single(changes.First().Value);
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
