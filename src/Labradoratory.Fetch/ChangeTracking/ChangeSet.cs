using System;
using System.Collections.Generic;
using System.Linq;

namespace Labradoratory.Fetch.ChangeTracking
{
    /// <summary>
    /// Contains a set of changes.
    /// </summary>
    public class ChangeSet : Dictionary<string, ChangeValue>
    {
        /// <summary>
        /// Combines the paths into one.
        /// </summary>
        /// <param name="paths">The paths to combine.</param>
        /// <returns>The combined paths.</returns>
        public static string CombinePaths(params string[] paths)
        {
            return string.Join('.', paths.Where(p => !string.IsNullOrWhiteSpace(p)));
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
                Add(entry.Key, entry.Value);
            }
        }
    }
}
