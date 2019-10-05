using System;

namespace Labradoratory.DataAccess.ChangeTracking
{
    /// <summary>
    /// Represents the type of change.
    /// </summary>
    public enum ChangeAction
    {
        /// <summary>There was no change.</summary>
        None,
        /// <summary>Value was added.</summary>
        Add,
        /// <summary>Value was removed.</summary>
        Remove,
        /// <summary>Value was updated.</summary>
        Update
    }
}
