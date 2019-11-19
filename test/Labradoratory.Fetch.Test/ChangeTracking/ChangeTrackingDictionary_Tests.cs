using System.Collections.Generic;
using System.Linq;
using Labradoratory.Fetch.ChangeTracking;
using Xunit;

namespace Labradoratory.Fetch.Test.ChangeTracking
{
    public class ChangeTrackingDictionary_Tests
    {
        [Fact]
        public void Ctor_NoParams_EmptyNoChanges()
        {
            var subject = new ChangeTrackingDictionary<string, TestItem>();
            Assert.Empty(subject);
            Assert.False(subject.HasChanges);
        }

        [Fact]
        public void Ctor_ItemsDictionaryParam_ContainsItemsNoChanges()
        {
            var expectedItems = new Dictionary<string, TestItem>
            {
                { "Test1", new TestItem() },
                { "Test2", new TestItem() }
            };
            var subject = new ChangeTrackingDictionary<string, TestItem>(expectedItems);
            Assert.Equal(2, subject.Count);
            Assert.Equal(expectedItems.ElementAt(0).Key, subject.ElementAt(0).Key);
            Assert.Same(expectedItems.ElementAt(0).Value, subject.ElementAt(0).Value);
            Assert.Equal(expectedItems.ElementAt(1).Key, subject.ElementAt(1).Key);
            Assert.Same(expectedItems.ElementAt(1).Value, subject.ElementAt(1).Value);
        }

        [Fact]
        public void Ctor_ItemsDictionaryParamWithChanges_HasChangesTrue()
        {
            var expectedUpdate = ChangeTrackingObject.CreateTrackable<TestItem>();
            var expectedItems = new Dictionary<string, TestItem>
            {
                { "Test1", new TestItem() },
                { "Test2", expectedUpdate }
            };
            expectedUpdate.StringValue = "new value";
            var subject = new ChangeTrackingDictionary<string, TestItem>(expectedItems);
            Assert.True(subject.HasChanges);
        }

        [Fact]
        public void Indexer_Get_ValueByKey()
        {
            var expectedKey = "Test1";
            var expectedItems = new Dictionary<string, TestItem>
            {
                { expectedKey, new TestItem() }
            };
            var subject = new ChangeTrackingDictionary<string, TestItem>(expectedItems);
            var result = subject[expectedKey];
            Assert.Same(expectedItems[expectedKey], result);
        }

        [Fact]
        public void Indexer_Get_ThrowsWhenKeyNotFound()
        {
            var subject = new ChangeTrackingDictionary<string, TestItem>();
            Assert.Throws<KeyNotFoundException>(() => subject["test"]);
        }

        [Fact]
        public void Indexer_Set_UpdateValueByKey()
        {
            var expectedKey = "Test1";
            var expectedItems = new Dictionary<string, TestItem>
            {
                { expectedKey, new TestItem() }
            };
            var subject = new ChangeTrackingDictionary<string, TestItem>(expectedItems);
            subject[expectedKey] = new TestItem();
            Assert.True(subject.HasChanges);
            var changes = subject.GetChangeSet();
            Assert.Single(changes);
            Assert.Equal(ChangeAction.Update, changes.First().Value.Action);
        }

        [Fact]
        public void Indexer_Set_AddValueByKey()
        {
            var expectedKey = "Test1";
            var subject = new ChangeTrackingDictionary<string, TestItem>();
            subject[expectedKey] = new TestItem();
            Assert.True(subject.HasChanges);
            var changes = subject.GetChangeSet();
            Assert.Single(changes);
            Assert.Equal(ChangeAction.Add, changes.First().Value.Action);
        }

        [Fact]
        public void Keys_GetsNonRemovedKeys()
        {
            var removeKey = "Test2";
            var expectedItems = new Dictionary<string, TestItem>
            {
                { "Test1", new TestItem() },
                { removeKey, new TestItem() },
                { "Test3", new TestItem() },
                { "Test4", new TestItem() }
            };
            var subject = new ChangeTrackingDictionary<string, TestItem>(expectedItems);
            subject.Remove(removeKey);

            var keys = subject.Keys;
            Assert.Equal(3, keys.Count);
            Assert.DoesNotContain(removeKey, keys);
        }

        [Fact]
        public void Values_GetsNonRemovedValues()
        {
            var removeKey = "Test3";
            var expectedItems = new Dictionary<string, TestItem>
            {
                { "Test1", new TestItem() },
                { "Test2", new TestItem() },
                { removeKey, new TestItem() },
                { "Test4", new TestItem() }
            };
            var subject = new ChangeTrackingDictionary<string, TestItem>(expectedItems);
            subject.Remove(removeKey);

            var values = subject.Values;
            Assert.Equal(3, values.Count);
            Assert.DoesNotContain(expectedItems[removeKey], values);
        }

        [Fact]
        public void IsReadOnly_False()
        {
            var subject = new ChangeTrackingDictionary<string, TestItem>();
            Assert.False(subject.IsReadOnly);
        }

        [Fact]
        public void Add_KeyAndValue_AddsNewItem()
        {
            var expectedKey = "key";
            var expectedItem = new TestItem();
            var subject = new ChangeTrackingDictionary<string, TestItem>();
            subject.Add(expectedKey, expectedItem);
            Assert.True(subject.HasChanges);
            var changes = subject.GetChangeSet();
            Assert.Single(changes);
            var change = changes.First();
            Assert.Equal(ChangeAction.Add, change.Value.Action);
            Assert.Same(expectedItem, change.Value.NewValue);
        }

        [Fact]
        public void Add_KeyValuePair_AddsNewItem()
        {
            var expectedKey = "key";
            var expectedItem = new TestItem();
            var subject = new ChangeTrackingDictionary<string, TestItem>();
            subject.Add(new KeyValuePair<string, TestItem>(expectedKey, expectedItem));
            Assert.True(subject.HasChanges);
            var changes = subject.GetChangeSet();
            Assert.Single(changes);
            var change = changes.First();
            Assert.Equal(ChangeAction.Add, change.Value.Action);
            Assert.Same(expectedItem, change.Value.NewValue);
        }

        [Fact]
        public void Clear_RemovesAllItems()
        {
            var expectedItems = new Dictionary<string, TestItem>
            {
                { "Test1", new TestItem() },
                { "Test2", new TestItem() }
            };
            var subject = new ChangeTrackingDictionary<string, TestItem>(expectedItems);
            subject.Clear();
            Assert.True(subject.HasChanges);
            Assert.Empty(subject);
            var changes = subject.GetChangeSet();
            Assert.Equal(2, changes.Count);
            Assert.All(changes, c => Assert.Equal(ChangeAction.Remove, c.Value.Action));
        }

        [Fact]
        public void Contains_KeyValuePair_TrueWhenMatchBothKeyValue()
        {
            var expectedKey = "Test1";
            var expectedItem = new TestItem();
            var expectedItems = new Dictionary<string, TestItem>
            {
                { expectedKey, expectedItem },
                { "Test2", new TestItem() }
            };
            var subject = new ChangeTrackingDictionary<string, TestItem>(expectedItems);
            var result = subject.Contains(new KeyValuePair<string, TestItem>(expectedKey, expectedItem));
            Assert.True(result);
        }

        [Fact]
        public void Contains_KeyValuePair_FalseWhenMatchKeyNotValue()
        {
            var expectedKey = "Test1";
            var expectedItem = new TestItem();
            var expectedItems = new Dictionary<string, TestItem>
            {
                { expectedKey, expectedItem },
                { "Test2", new TestItem() }
            };
            var subject = new ChangeTrackingDictionary<string, TestItem>(expectedItems);
            var result = subject.Contains(new KeyValuePair<string, TestItem>(expectedKey, new TestItem()));
            Assert.False(result);
        }

        [Fact]
        public void Contains_KeyValuePair_FalseWhenMatchValueNotKey()
        {
            var expectedKey = "Test1";
            var expectedItem = new TestItem();
            var expectedItems = new Dictionary<string, TestItem>
            {
                { expectedKey, expectedItem },
                { "Test2", new TestItem() }
            };
            var subject = new ChangeTrackingDictionary<string, TestItem>(expectedItems);
            var result = subject.Contains(new KeyValuePair<string, TestItem>("not found", expectedItem));
            Assert.False(result);
        }

        [Fact]
        public void Contains_KeyValuePair_FalseWhenNeitherKeyNorValue()
        {
            var expectedItems = new Dictionary<string, TestItem>
            {
                { "Test1", new TestItem() },
                { "Test2", new TestItem() }
            };
            var subject = new ChangeTrackingDictionary<string, TestItem>(expectedItems);
            var result = subject.Contains(new KeyValuePair<string, TestItem>("not found", new TestItem()));
            Assert.False(result);
        }

        [Fact]
        public void Contains_Key_TrueWhenMatchKey()
        {
            var expectedKey = "Test1";
            var expectedItems = new Dictionary<string, TestItem>
            {
                { expectedKey, new TestItem() },
                { "Test2", new TestItem() }
            };
            var subject = new ChangeTrackingDictionary<string, TestItem>(expectedItems);
            var result = subject.ContainsKey(expectedKey);
            Assert.True(result);
        }

        [Fact]
        public void Contains_Key_FalseWhenNoMatchKey()
        {
            var subject = new ChangeTrackingDictionary<string, TestItem>();
            var result = subject.ContainsKey("not found");
            Assert.False(result);
        }

        [Fact]
        public void CopyTo_CopiesNonRemovedItemsToArray()
        {
            var removeMe = "Test2";
            var expectedItems = new Dictionary<string, TestItem>
            {
                { "Test1", new TestItem() },
                { removeMe, new TestItem() },
                { "Test3", new TestItem() }
            };
            var subject = new ChangeTrackingDictionary<string, TestItem>(expectedItems);
            subject.Remove(removeMe);

            var array = new KeyValuePair<string, TestItem>[5];
            subject.CopyTo(array, 1);
            Assert.Null(array[0].Key);
            Assert.Null(array[0].Value);
            Assert.Equal(expectedItems.ElementAt(0).Key, array[1].Key);
            Assert.Same(expectedItems.ElementAt(0).Value, array[1].Value);
            Assert.Equal(expectedItems.ElementAt(2).Key, array[2].Key);
            Assert.Same(expectedItems.ElementAt(2).Value, array[2].Value);
            Assert.Null(array[3].Key);
            Assert.Null(array[3].Value);
            Assert.Null(array[4].Key);
            Assert.Null(array[4].Value);

        }

        [Fact]
        public void Remove_Key_WhenKeyNotFoundFalse()
        {
            var subject = new ChangeTrackingDictionary<string, TestItem>();
            Assert.False(subject.Remove("not found"));
        }

        [Fact]
        public void Remove_Key_WhenKeyFoundTrueAndRemove()
        {
            var removeKey = "Test2";
            var expectedKey = "Test1";
            var expectedItems = new Dictionary<string, TestItem>
            {
                { expectedKey, new TestItem() },
                { removeKey, new TestItem() }
            };
            var subject = new ChangeTrackingDictionary<string, TestItem>(expectedItems);
            Assert.True(subject.Remove(removeKey));
            Assert.Single(subject);
            Assert.Contains(expectedKey, subject as IDictionary<string, TestItem>);
        }

        [Fact]
        public void Remove_KeyValuePair_FalseWhenKeyNotMatch()
        {
            var expectedValue = new TestItem();
            var expectedItems = new Dictionary<string, TestItem>
            {
                { "Test1", expectedValue },
                { "Test2", new TestItem() }
            };
            var subject = new ChangeTrackingDictionary<string, TestItem>(expectedItems);
            Assert.False(subject.Remove(new KeyValuePair<string, TestItem>("not found", expectedValue)));
        }

        [Fact]
        public void Remove_KeyValuePair_FalseWhenValueNotMatch()
        {
            var expectedKey = "Test1";
            var expectedItems = new Dictionary<string, TestItem>
            {
                { expectedKey, new TestItem() },
                { "Test2", new TestItem() }
            };
            var subject = new ChangeTrackingDictionary<string, TestItem>(expectedItems);
            Assert.False(subject.Remove(new KeyValuePair<string, TestItem>(expectedKey, new TestItem())));
        }

        [Fact]
        public void Remove_KeyValuePair_TrueWhenMatch()
        {
            var expectedItems = new Dictionary<string, TestItem>
            {
                { "Test1", new TestItem() },
                { "Test2", new TestItem() }
            };
            var subject = new ChangeTrackingDictionary<string, TestItem>(expectedItems);
            Assert.True(subject.Remove(expectedItems.First()));
            Assert.True(subject.HasChanges);
        }

        [Fact]
        public void TryGetValue_TrueAndValueOutWhenFound()
        {
            var expectedKey = "Test1";
            var expectedItem = new TestItem();
            var expectedItems = new Dictionary<string, TestItem>
            {
                { expectedKey, expectedItem },
                { "Test2", new TestItem() }
            };
            var subject = new ChangeTrackingDictionary<string, TestItem>(expectedItems);
            Assert.True(subject.TryGetValue(expectedKey, out var item));
            Assert.Same(expectedItem, item);
        }

        [Fact]
        public void TryGetValue_FalseAndDefaultValueWhenNotFound()
        {
            var expectedItems = new Dictionary<string, int>
            {
                { "Test1", 100 },
                { "Test2", 50 }
            };
            var subject = new ChangeTrackingDictionary<string, int>(expectedItems);
            Assert.False(subject.TryGetValue("not found", out var item));
            Assert.Equal(default, item);
        }

        [Fact]
        public void GetChangeSet_CommitFalse_ContainsAdds()
        {
            var subject = new ChangeTrackingDictionary<string, TestItem>();
            var expectedKey = "Test1";
            var expectedItem = new TestItem();
            subject.Add(expectedKey, expectedItem);
            var path = "path";
            var expectedChangeKey = $"{path}.{expectedKey}.add";
            var result = subject.GetChangeSet(path);
            Assert.True(subject.HasChanges);
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Contains(expectedChangeKey, result as IDictionary<string, ChangeValue>);
            var change = result[expectedChangeKey];
            Assert.Equal(ChangeAction.Add, change.Action);
            Assert.Equal(ChangeTarget.Dictionary, change.Target);
            Assert.Null(change.OldValue);
            Assert.Same(expectedItem, change.NewValue);
        }

        [Fact]
        public void GetChangeSet_CommitFalse_ContainsRemoves()
        {
            var removeKey = "Test2";
            var expectedRemove = new TestItem();
            var expectedItems = new Dictionary<string, TestItem>
            {
                { "Test1",  new TestItem() },
                { removeKey, expectedRemove },
                { "Test3", new TestItem() }
            };
            var subject = new ChangeTrackingDictionary<string, TestItem>(expectedItems);
            var path = "path";
            var expectedChangeKey = $"{path}.{removeKey}.remove";
            Assert.True(subject.Remove(removeKey));
            var result = subject.GetChangeSet(path);
            Assert.True(subject.HasChanges);
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Contains(expectedChangeKey, result as IDictionary<string, ChangeValue>);
            var change = result[expectedChangeKey];
            Assert.Equal(ChangeAction.Remove, change.Action);
            Assert.Equal(ChangeTarget.Dictionary, change.Target);
            Assert.Null(change.NewValue);
            Assert.Same(expectedRemove, change.OldValue);
        }

        [Fact]
        public void GetChangeSet_CommitFalse_ContainsUpdates()
        {
            var expectedKey = "Test2";
            var expectedOldValue = "My old value";
            var expectedNewValue = "The new value";
            var expectedUpdate = new TestItem
            {
                StringValue = expectedOldValue
            };
            var expectedItems = new Dictionary<string, TestItem>
            {
                { "Test1", new TestItem() },
                { expectedKey, expectedUpdate },
                { "Test3", new TestItem() }
            };
            var subject = new ChangeTrackingDictionary<string, TestItem>(expectedItems);
            var path = "path";
            var expectedChangeKey = $"{path}.{expectedKey}.{nameof(TestItem.StringValue)}";
            expectedUpdate.StringValue = expectedNewValue;
            var result = subject.GetChangeSet(path);
            Assert.True(subject.HasChanges);
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Contains(expectedChangeKey, result as IDictionary<string, ChangeValue>);
            var change = result[expectedChangeKey];
            Assert.Equal(ChangeAction.Update, change.Action);
            Assert.Equal(ChangeTarget.Object, change.Target);
            Assert.Equal(expectedNewValue, change.NewValue);
            Assert.Equal(expectedOldValue, change.OldValue);
        }

        [Fact]
        public void GetChangeSet_CommitTrue_MakesChangesPermanent()
        {
            var removeKey = "Test1";
            var expectedUpdate = ChangeTrackingObject.CreateTrackable<TestItem>();
            var expectedNewValue = "New value";
            var expectedItems = new Dictionary<string, TestItem>
            {
                { removeKey, new TestItem() },
                { "Test2", expectedUpdate }
            };
            var subject = new ChangeTrackingDictionary<string, TestItem>(expectedItems);
            subject.Add("Test3", new TestItem());
            subject.Remove(removeKey);
            expectedUpdate.StringValue = expectedNewValue;

            var result = subject.GetChangeSet(commit: true);
            Assert.Equal(3, result.Count);
            Assert.False(subject.HasChanges);
            Assert.False(expectedUpdate.HasChanges);
            Assert.Equal(expectedNewValue, expectedUpdate.StringValue);
            Assert.DoesNotContain(removeKey, subject as IDictionary<string, TestItem>);
            Assert.Equal(2, subject.Count);
        }

        [Fact]
        public void GetChangeSet_NullWhenNoChanges()
        {
            var subject = new ChangeTrackingDictionary<string, TestItem>();
            Assert.Null(subject.GetChangeSet());
        }

        [Fact]
        public void Reset_HasChangesTrue_ResetsAllchanges()
        {
            var removeKey = "Test1";
            var expectedOldValue = "My old value";
            var expectedNewValue = "The new value";
            var expectedUpdate = new TestItem
            {
                StringValue = expectedOldValue
            };
            var expectedItems = new Dictionary<string, TestItem>
            {
                { removeKey, new TestItem() },
                { "Test2", expectedUpdate }
            };
            var subject = new ChangeTrackingDictionary<string, TestItem>(expectedItems);
            expectedUpdate.StringValue = expectedNewValue;
            subject.Add("Test3", new TestItem());
            subject.Remove(removeKey);

            subject.Reset();

            Assert.False(subject.HasChanges);
            var changes = subject.GetChangeSet();
            Assert.Null(changes);
            Assert.False(expectedUpdate.HasChanges);
        }

        [Fact]
        public void Reset_HasChangesFalse_DoesNothing()
        {
            var subject = new ChangeTrackingDictionary<string, TestItem>();
            subject.Reset();
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
