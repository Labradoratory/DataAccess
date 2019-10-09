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
        /// Combines the paths into one.
        /// </summary>
        /// <param name="path1">The path1.</param>
        /// <param name="path2">The path2.</param>
        /// <returns>The combined paths.</returns>
        public static string CombinePaths(string path1, string path2)
        {
            if (string.IsNullOrWhiteSpace(path1))
            {
                if (string.IsNullOrWhiteSpace(path2))
                    return string.Empty;
                
                return path2;
            }
            
            return "${path1}.{path2}";
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
