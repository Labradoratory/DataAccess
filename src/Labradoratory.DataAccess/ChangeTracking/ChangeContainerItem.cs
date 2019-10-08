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
        /// <param name="action">The action the container represents.</param>
        public ChangeContainerItem(T item, ChangeAction action = ChangeAction.None)
        {
            _action = action;
            Item = item;
        }

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
            if (!HasChanges)
                return null;

            throw new NotImplementedException();
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
