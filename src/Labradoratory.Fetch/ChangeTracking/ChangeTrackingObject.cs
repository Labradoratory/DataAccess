using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Labradoratory.Fetch.ChangeTracking
{
    /// <summary>
    /// TODO
    /// </summary>
    public class ChangeTrackingObject : ITracksChanges
    {
        /// <summary>
        /// Creates a new instance of type <typeparamref name="T"/> and
        /// initializes its values with the default for each property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The new, intialized instance.</returns>
        /// <remarks>
        /// This method should be used to create new instances of any <see cref="ChangeTrackingObject"/>
        /// where you want to start tracking changes immediately.  If you do not use this method, 
        /// the initial values set will not be treated as changes.
        /// </remarks>
        public static T CreateTrackable<T>() where T : ChangeTrackingObject, new()
        {
            var instance = new T();

            var method = typeof(ChangeTrackingObject).GetMethod("SetDefaultValue", BindingFlags.Instance | BindingFlags.NonPublic);
            foreach(var pi in typeof(T)
                .GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite))
            {
                var gm = method.MakeGenericMethod(pi.PropertyType);
                gm.Invoke(instance, new object[] { pi.Name });
            }

            return instance;
        }

        /// <summary>
        /// Gets the collection of changes that are being tracked.
        /// </summary>
        private Dictionary<string, ChangeContainerValue> Changes { get; } = new Dictionary<string, ChangeContainerValue>();

        /// <summary>
        /// Gets whether or not there are changes.
        /// </summary>
        public bool HasChanges => Changes.Values.Any(v => v.HasChanges);

        /// <inheritdoc />
        public ChangeSet GetChangeSet(ChangePath path, bool commit = false)
        {
            if (!HasChanges)
                return null;

            var changes = new ChangeSet();
            foreach(var change in Changes)
            {
                changes.Merge(change.Value.GetChangeSet(path.AppendProperty(change.Key), commit));
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

            if(!Changes.TryAdd(propertyName, new ChangeContainerValue(value)))
                Changes[propertyName].CurrentValue = value;
        }


#pragma warning disable RCS1213 // Remove unused member declaration.  This is used via reflection.
        private void SetDefaultValue<T>(string propertyName)
        {
            SetValue<T>(default, propertyName);
        }
#pragma warning restore RCS1213 // Remove unused member declaration.
    }
}
