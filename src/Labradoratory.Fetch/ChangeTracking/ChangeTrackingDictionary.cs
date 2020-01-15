using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Labradoratory.Fetch.ChangeTracking
{
    /// <summary>
    /// TODO
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <seealso cref="IDictionary{TKey, TValue}" />
    /// <seealso cref="ITracksChanges" />
    public class ChangeTrackingDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ITracksChanges
    {
        /// <summary>
        /// Initializes a new, empty instance of the <see cref="ChangeTrackingDictionary{TKey, TValue}"/> class.
        /// </summary>
        public ChangeTrackingDictionary()
            : this(new Dictionary<TKey, TValue>(0))
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeTrackingDictionary{TKey, TValue}"/> class, 
        /// populated with the provided items.
        /// </summary>
        /// <param name="items">The items to populate the <see cref="ChangeTrackingDictionary{TKey, TValue}"/> with.</param>
        public ChangeTrackingDictionary(IDictionary<TKey, TValue> items)
        {
            Items = new Dictionary<TKey, ChangeContainerItem<TValue>>(
                items.Select(i => new KeyValuePair<TKey, ChangeContainerItem<TValue>>(
                    i.Key, 
                    new ChangeContainerItem<TValue>(i.Value, ChangeTarget.Dictionary, ChangeAction.None))));
            Removed = new Dictionary<TKey, ChangeContainerItem<TValue>>();
        }

        private Dictionary<TKey, ChangeContainerItem<TValue>> Items { get; }
        private Dictionary<TKey, ChangeContainerItem<TValue>> Removed { get; }
               
        /// <summary>
        /// Gets whether or not there are changes.
        /// </summary>
        public bool HasChanges => Items.Any(i => i.Value.Action != ChangeAction.None) || Removed.Any();

        /// <inheritdoc />
        public TValue this[TKey key] 
        { 
            get => Items[key].Item;
            // NOTE: I thought about making a value set like this just be a remove plus an add instead
            // of tracking as an update.  Either way would make sense, but we'll go with update for now.
            set
            {
                if (Items.TryGetValue(key, out var container))
                {
                    container.Item = value;
                }
                else
                {
                    Items[key] = new ChangeContainerItem<TValue>(value, ChangeTarget.Dictionary, ChangeAction.Add);
                }
            }
        }

        /// <inheritdoc />
        public ICollection<TKey> Keys => Items.Keys;

        /// <inheritdoc />
        public ICollection<TValue> Values => Items.Values.Select(i => i.Item).ToList();

        /// <inheritdoc />
        public int Count => Items.Count();

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <inheritdoc />
        public void Add(TKey key, TValue value)
        {
            Items.Add(key, new ChangeContainerItem<TValue>(value, ChangeTarget.Dictionary, ChangeAction.Add));
        }

        /// <inheritdoc />
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <inheritdoc />
        public void Clear()
        {
            foreach (var key in Items.Keys.ToList())
                Remove(key);
        }

        /// <inheritdoc />
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (!Items.TryGetValue(item.Key, out ChangeContainerItem<TValue> container))
                return false;

            if (!Equals(container.Item, item.Value))
                return false;

            return true;
        }

        /// <inheritdoc />
        public bool ContainsKey(TKey key)
        {
            return Items.ContainsKey(key);
        }

        /// <inheritdoc />
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (var kvp in this)
                array[arrayIndex++] = kvp;
        }

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Items.Select(i => new KeyValuePair<TKey, TValue>(i.Key, i.Value.Item)).GetEnumerator();
        }

        /// <inheritdoc />
        public bool Remove(TKey key)
        {
            if (!Items.TryGetValue(key, out ChangeContainerItem<TValue> container))
                return false;

            Items.Remove(key);
            // If the item being removed was added, that means we don't need to track it 
            // anymore, so the below does not apply.
            if (container.Action != ChangeAction.Add)
            {
                container.Action = ChangeAction.Remove;
                Removed[key] = container;
            }

            return true;
        }

        /// <inheritdoc />
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (!Items.TryGetValue(item.Key, out ChangeContainerItem<TValue> container))
                return false;

            if (!Equals(container.Item, item.Value))
                return false;

            Items.Remove(item.Key);
            // If the item being removed was added, that means we don't need to track it 
            // anymore, so the below does not apply.
            if (container.Action != ChangeAction.Add)
            {
                container.Action = ChangeAction.Remove;
                Removed.Add(item.Key, container);
            }

            return true;
        }

        /// <inheritdoc />
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            if (Items.TryGetValue(key, out ChangeContainerItem<TValue> container))
            {
                value = container.Item;
                return true;
            }

            value = default;
            return false;
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        /// <inheritdoc />
        public ChangeSet GetChangeSet(ChangePath path, bool commit = false)
        {
            if (!HasChanges)
                return null;

            var changes = new ChangeSet();

            foreach (var item in Items.Where(i => i.Value.HasChanges))
            {
                changes.Merge(
                    item.Value.GetChangeSet(
                        path.AppendKey(item.Key),
                        commit));
            }

            foreach (var removed in Removed)
            {
                changes.Merge(
                    removed.Value.GetChangeSet(
                        path.AppendKey(removed.Key),
                        commit));
            }

            if (commit)
                Removed.Clear();

            return changes;
        }

        /// <inheritdoc />
        public void Reset()
        {
            if (!HasChanges)
                return;

            // Remove all adds and reset the rest.
            foreach(var item in Items.ToList())
            {
                if (item.Value.Action == ChangeAction.Add)
                    Items.Remove(item.Key);
                else
                    item.Value.Reset();
            }

            // Move the removed back to the Items.
            foreach(var removed in Removed)
            {
                removed.Value.Reset();
                Items.Add(removed.Key, removed.Value);
            }

            Removed.Clear();
        }
    }
}
