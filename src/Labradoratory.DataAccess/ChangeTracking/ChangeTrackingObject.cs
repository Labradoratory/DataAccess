using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Labradoratory.DataAccess.ChangeTracking
{
    /// <summary>
    /// TODO
    /// </summary>
    public class ChangeTrackingObject : ITracksChanges
    {
        /// <summary>
        /// Gets the collection of changes that are being tracked.
        /// </summary>
        private Dictionary<string, ChangeContainerValue> Changes { get; } = new Dictionary<string, ChangeContainerValue>();

        /// <summary>
        /// Gets whether or not there are changes.
        /// </summary>
        public bool HasChanges => Changes.Values.Any(v => v.HasChanges);

        /// <inheritdoc />
        public ChangeSet GetChangeSet(string path = "", bool commit = false)
        {
            if (!HasChanges)
                return null;

            var changes = new ChangeSet();
            foreach(var change in Changes)
            {
                changes.Merge(change.Value.GetChangeSet($"{path}.{change.Key}", commit));
            }

            return changes;
        }
        
        /// <inheritdoc />
        public void Reset()
        {
            foreach (var change in Changes)
                change.Value.Reset();
        }

        /// <summary>
        /// Gets the value for the specified property.
        /// </summary>
        /// <typeparam name="T">The type of the object being retrieved.</typeparam>
        /// <param name="propertyName">[Optional] Name of the property to get the value for.  This property uses <see cref="CallerMemberNameAttribute"/> if not specified.</param>
        /// <returns>The value of the specified property.</returns>
        /// <exception cref="ArgumentNullException">propertyName</exception>
        protected T GetValue<T>([CallerMemberName]string propertyName = null)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            if (Changes.TryGetValue(propertyName, out ChangeContainerValue value))
                return (T)value.CurrentValue;

            return default;
        }

        /// <summary>
        /// Sets the value for the specified property.
        /// </summary>
        /// <typeparam name="T">The type of the value being set.</typeparam>
        /// <param name="value">The value to set the property to.</param>
        /// <param name="propertyName">[Optional] Name of the property to set.  This property uses <see cref="CallerMemberNameAttribute"/> if not specified.</param>
        /// <exception cref="ArgumentNullException">propertyName</exception>
        protected void SetValue<T>(T value, [CallerMemberName]string propertyName = null)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            Changes.TryAdd(propertyName, new ChangeContainerValue());
            Changes[propertyName].CurrentValue = value;
        }
    }    
}
