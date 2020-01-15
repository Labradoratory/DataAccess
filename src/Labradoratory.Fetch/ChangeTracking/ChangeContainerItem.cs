using System;

namespace Labradoratory.Fetch.ChangeTracking
{
    /// <summary>
    /// This class is used internally by the <see cref="ChangeTrackingCollection{T}"/> and <see cref="ChangeTrackingDictionary{TKey, TValue}"/> to track item changes.
    /// </summary>
    /// <typeparam name="T">The type of the item in the container.</typeparam>
    internal class ChangeContainerItem<T> : ITracksChanges
    {
        private T _item;
        private ChangeAction _action;
        private readonly HasValueContainer<T> _oldItem = new HasValueContainer<T>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeTrackingCollection{T}"/> class.
        /// </summary>
        /// <param name="item">The value the container represents</param>
        /// <param name="target"></param>
        /// <param name="action">The action the container represents.</param>
        public ChangeContainerItem(T item, ChangeTarget target, ChangeAction action = ChangeAction.None)
        {
            Target = target;
            _item = item;
            _action = action;
        }

        public ChangeTarget Target { get; }

        private bool ItemHasChanges => (Item as ITracksChanges)?.HasChanges ?? false;

        /// <summary>
        /// Gets the action this change represents.
        /// </summary>
        public ChangeAction Action
        {
            get
            {
                if (_action == ChangeAction.None && ItemHasChanges)
                    return ChangeAction.Update;

                return _action;
            }
            set => _action = value;
        }

        /// <summary>
        /// Gets or sets the current value.
        /// </summary>
        public T Item
        {
            get => _item;
            set
            {
                Action = ChangeAction.Update;
                if (!_oldItem.HasValue)
                    _oldItem.Value = _item;

                _item = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has changes.
        /// </summary>
        public bool HasChanges => Action != ChangeAction.None;

        /// <summary>
        /// Gets the current changes as a <see cref="ChangeSet" />.
        /// </summary>
        /// <param name="path">[Optional] The path to the change set.  Default value is <see cref="string.Empty" />.</param>
        /// <param name="commit">Whether or not to commit the changes during the get.  Commiting the changes
        /// will clear all tracking and leave the current values as-is.  Another call to
        /// <see cref="GetChangeSet(string, bool)" /> immdiately after a commit will return an
        /// empty <see cref="ChangeSet" />.</param>
        /// <returns>
        /// A <see cref="ChangeSet" /> containing all of the changes.
        /// </returns>
        public ChangeSet GetChangeSet(ChangePath path, bool commit = false)
        {
            ChangeSet changes;
            switch (Action)
            {
                case ChangeAction.Add:
                    changes = ProcessAdd(path);
                    break;
                case ChangeAction.Update:
                    changes = ProcessUpdate(path, commit);
                    break;
                case ChangeAction.Remove:
                    changes = ProcessRemove(path);
                    break;
                default:
                    return null;
            }

            if (commit)
            {
                _oldItem.Reset();
                _action = ChangeAction.None;
            }

            return changes;
        }

        private ChangeSet ProcessAdd(ChangePath key)
        {
            key = key.WithAction(ChangeAction.Add);

            return new ChangeSet
            {
                {
                    key,
                    new ChangeValue
                    {
                        Target = Target,
                        Action = Action,
                        NewValue = Item
                    }
                }
            };
        }

        private ChangeSet ProcessUpdate(ChangePath key, bool commit)
        {
            if (_oldItem.HasValue)
            {
                return new ChangeSet
                {
                    {
                        key,
                        new ChangeValue
                        {
                            Target = Target,
                            Action = ChangeAction.Update,
                            NewValue = Item,
                            OldValue = _oldItem.Value
                        }
                    }
                };
            }

            if (Item is ITracksChanges tc)
                return tc.GetChangeSet(key, commit);

            throw new InvalidOperationException($"Updates are not allowed for types that don't implement {nameof(ITracksChanges)}");
        }

        private ChangeSet ProcessRemove(ChangePath path)
        {
            path = path.WithAction(ChangeAction.Remove);
            return new ChangeSet
            {
                {
                    path,
                    new ChangeValue
                    {
                        Target = Target,
                        Action = Action,
                        OldValue = Item
                    }
                }
            };
        }

        /// <summary>
        /// Resets the changes.
        /// </summary>
        public void Reset()
        {
            if (_oldItem.HasValue)
            {
                Item = _oldItem.Value;
                _oldItem.Reset();
            }

            (Item as ITracksChanges)?.Reset();
            _action = ChangeAction.None;
        }
    }
}
