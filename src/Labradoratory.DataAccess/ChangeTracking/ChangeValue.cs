using System;

namespace Labradoratory.DataAccess.ChangeTracking
{
    /// <summary>
    /// Represents a value change.
    /// </summary>
    public class ChangeValue
    {
        /// <summary>
        /// Gets or sets the action that caused the change.
        /// </summary>
        public ChangeAction Action { get; set; }

        /// <summary>
        /// Gets or sets the new value (post-change).
        /// </summary>
        public object NewValue { get; set; }

        /// <summary>
        /// Gets or sets the old value (pre-change).
        /// </summary>
        public object OldValue { get; set; }
    }
}
