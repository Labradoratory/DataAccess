using System;
using System.Collections.Generic;

namespace Labradoratory.Fetch.ChangeTracking
{
    /// <summary>
    /// Contains a set of changes.
    /// </summary>
    public class ChangeSet : Dictionary<ChangePath, List<ChangeValue>>
    {
        public static ChangeSet Create(ChangePath path, params ChangeValue[] values)
        {
            var changeSet = new ChangeSet();
            changeSet.Append(path, values);
            return changeSet;
        }

        public void Append(ChangePath path, params ChangeValue[] values)
        {
            if (!ContainsKey(path))
                this[path] = new List<ChangeValue>();

            this[path].AddRange(values);
        }

        /// <summary>
        /// Merges the specified changes into this <see cref="ChangeSet"/>.
        /// </summary>
        /// <param name="changes">The changes to merge with this <see cref="ChangeSet"/>.</param>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="ChangeSet"/>.</exception>
        public void Merge(ChangeSet changes)
        {
            if (changes == null)
                return;

            foreach (var entry in changes)
            {
                Append(entry.Key, entry.Value.ToArray());
            }
        }
    }
}
