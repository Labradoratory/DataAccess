using System;
using System.Collections.Generic;

namespace Labradoratory.Fetch.ChangeTracking
{
    /// <summary>
    /// Contains a set of changes.
    /// </summary>
    public class ChangeSet : Dictionary<ChangePath, List<ChangeValue>>
    {
        /// <summary>
        /// Gets whether or not the <see cref="ChangeSet"/> is empty.
        /// </summary>
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// Creates a <see cref="ChangeSet"/> using the specified path and values.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static ChangeSet Create(ChangePath path, params ChangeValue[] values)
        {
            var changeSet = new ChangeSet();
            changeSet.Append(path, values);
            return changeSet;
        }

        /// <summary>
        /// Appends the path and values to the <see cref="ChangeSet"/>.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="values">The values.</param>
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
