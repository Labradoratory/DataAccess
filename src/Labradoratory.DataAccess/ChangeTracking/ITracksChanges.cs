using System;

namespace Labradoratory.DataAccess.ChangeTracking
{
    /// <summary>
    /// Members that change tracking objects should implement.
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
        /// <param name="path">[Optional] The path to the change set.  Default value is <see cref="string.Empty"/>.</param>
        /// <param name="commit">
        /// Whether or not to commit the changes during the get.  Commiting the changes
        /// will clear all tracking and leave the current values as-is.  Another call to
        /// <see cref="GetChangeSet(string, bool)"/> immdiately after a commit will return an
        /// empty <see cref="ChangeSet"/>.
        /// </param>
        /// <returns>A <see cref="ChangeSet"/> containing all of the changes.</returns>
        ChangeSet GetChangeSet(string path = "", bool commit = false);

        /// <summary>
        /// Resets all changes.  After calling, <see cref="HasChanges"/> will return <c>false</c>.
        /// </summary>
        void Reset();
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
