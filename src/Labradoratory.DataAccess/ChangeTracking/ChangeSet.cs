using System;
using System.Collections.Generic;

namespace Labradoratory.DataAccess.ChangeTracking
{
    /// <summary>
    /// Contains a set of changes.
    /// </summary>
    public class ChangeSet : Dictionary<string, ChangeValue>
    {
        /// <summary>
        /// Merges the specified changes into this <see cref="ChangeSet"/>.
        /// </summary>
        /// <param name="changes">The changes to merge with this <see cref="ChangeSet"/>.</param>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="ChangeSet"/>.</exception>
        public void Merge(ChangeSet changes)
        {
            foreach (var entry in changes)
            {
                Add(entry.Key, entry.Value);
            }
        }
    }
}
