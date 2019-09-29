using System;

namespace Labradoratory.DataAccess.ChangeTracking
{
    /// <summary>
    /// Identifies the target of a change.
    /// </summary>
    public enum ChangeTarget
    {
        /// <summary>An object was the target of a change.</summary>
        Object,
        /// <summary>A collection was the target of a change.</summary>
        Collection,
        /// <summary>A dictionary was the target of a change.</summary>
        Dictionary
    }
}
