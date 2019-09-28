using System;

namespace Labradoratory.DataAccess.ChangeTracking
{
    /// <summary>
    /// Members that objects that track changes should implement.
    /// </summary>
    public interface ITracksChanges
    {
        /// <summary>
        /// Gets whether or not there are changes.
        /// </summary>
        bool HasChanges { get; }

        /// <summary>
        /// Gets the current changes as a <see cref="ChangeSet"/>.
        /// </summary>
        /// <param name="commit">
        /// Whether or not to commit the changes during the get.  Commiting the changes
        /// will clear all tracking and leave the current values as-is.  Another call to
        /// <see cref="GetChangeSet(bool)"/> immdiately after a commit will return an
        /// empty <see cref="ChangeSet"/>.
        /// </param>
        /// <returns>A <see cref="ChangeSet"/> containing all of the changes.</returns>
        ChangeSet GetChangeSet(bool commit = false);
    }

    /// <summary>
    /// Methods to make working with <see cref="ITracksChanges"/> a little easier.
    /// </summary>
    public static class ITracksChangesExtensions
    {
        /// <summary>
        /// Commits the changes.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>A <see cref="ChangeSet"/> containing the committed changes.</returns>
        public static ChangeSet CommitChanges(this ITracksChanges target)
        {
            return target.GetChangeSet(true);
        }
    }
}
