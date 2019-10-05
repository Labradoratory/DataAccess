using System;

namespace Labradoratory.DataAccess.ChangeTracking
{
    /// <summary>
    /// This class is used internally by the <see cref="ChangeTrackingObject"/> to track value changes.
    /// </summary>
    internal class ValueChangeContainer
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
        /// Creates <see cref="ChangeValue"/> that represents the current changes.
        /// </summary>
        /// <param name="commit">[Optional] If <c>true</c>, <see cref="HasChanges"/> will return false after this method is called.  Default is <c>false</c>.</param>
        /// <returns>The <see cref="ChangeValue"/> representing this container.</returns>
        public ChangeValue GetChangeValue(bool commit = false)
        {
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

            return value;
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
