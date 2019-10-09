using System;

namespace Labradoratory.DataAccess.ChangeTracking
{
    /// <summary>
    /// This class is used internally by the <see cref="ChangeTrackingCollection{T}"/> and <see cref="ChangeTrackingDictionary{TKey, TValue}"/> to track item changes.
    /// </summary>
    /// <typeparam name="T">The type of the item in the container.</typeparam>
    internal class ChangeContainerItem<T> : ITracksChanges
    {
        private ChangeAction _action;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeTrackingCollection{T}"/> class.
        /// </summary>
        /// <param name="item">The value the container represents</param>
        /// <param name="target"></param>
        /// <param name="action">The action the container represents.</param>
        public ChangeContainerItem(T item, ChangeTarget target, ChangeAction action = ChangeAction.None)
        {
            Target = target;
            Item = item;
            _action = action;
        }

        public ChangeTarget Target { get; }

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

        private bool ItemHasChanges => (Item as ITracksChanges)?.HasChanges ?? false;

        /// <summary>
        /// Gets or sets the current value.
        /// </summary>
        public T Item { get; }

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
        public ChangeSet GetChangeSet(string path = "", bool commit = false)
        {
            switch(Action)
            {
                case ChangeAction.Add:
                    return ProcessAdd(path, commit);
                case ChangeAction.Update:
                    return ProcessUpdate(path, commit);
                case ChangeAction.Remove:
                    return ProcessRemove(path, commit);
                default:
                    return null;
            }
        }

        private ChangeSet ProcessAdd(string path, bool commit)
        {
            if(commit)
                Action = ChangeAction.None;

            path = ChangeSet.CombinePaths(path, "add");
            if (Item is ITracksChanges tc)
                return tc.GetChangeSet(path, commit);

            return new ChangeSet
            {
                {
                    path,
                    new ChangeValue
                    {
                        Target = Target,
                        Action = Action,
                        NewValue = Item
                    }
                }
            };
        }

        private ChangeSet ProcessUpdate(string path, bool commit)
        {
            if (Item is ITracksChanges tc)
                return tc.GetChangeSet(path, commit);

            throw new InvalidOperationException($"Updates are not allowed for types that don't implement {nameof(ITracksChanges)}");
        }

        private ChangeSet ProcessRemove(string path, bool commit)
        {
            if (commit)
                Action = ChangeAction.None;

            path = ChangeSet.CombinePaths(path, "remove");
            if (Item is ITracksChanges tc)
                return tc.GetChangeSet(path, commit);

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
            if (Action == ChangeAction.Update)
                (Item as ITracksChanges).Reset();

            _action = ChangeAction.None;
        }
    }
}
