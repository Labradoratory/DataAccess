using System;

namespace Labradoratory.DataAccess.ChangeTracking
{
    /// <summary>
    /// This class is used internally by the <see cref="ChangeTrackingObject"/> to track value changes.
    /// </summary>
    internal class ChangeContainerValue : ITracksChanges
    {
        private object currentValue;

        /// <summary>
        /// Gets or sets the current value.
        /// </summary>
        public object CurrentValue
        {
            get => currentValue;
            set
            {
                if (!HasChanges)
                    OldValue = currentValue;

                currentValue = value;
            }
        }

        /// <summary>
        /// Gets the old value, before any changes.
        /// </summary>
        public object OldValue { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has changes.
        /// </summary>
        public bool HasChanges => OldValue != null || ((CurrentValue as ITracksChanges)?.HasChanges ?? false);

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
            string property = "";

            if (!HasChanges)
                return null;

            var value = new ChangeValue
            {
                OldValue = OldValue,
                NewValue = CurrentValue,
                Action = ChangeAction.Update
            };

            if (commit)
                OldValue = null;

            return new ChangeSet { { property, value } };
        }

        /// <summary>
        /// Resets the changes.
        /// </summary>
        public void Reset()
        {
            CurrentValue = OldValue;
            OldValue = null;
        }
    }
}
