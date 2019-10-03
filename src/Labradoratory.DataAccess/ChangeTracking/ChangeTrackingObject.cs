﻿using System;
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
        protected Dictionary<string, ChangeValue> Changes { get; } = new Dictionary<string, ChangeValue>();

        /// <summary>
        /// Gets whether or not there are changes.
        /// </summary>
        public bool HasChanges => Changes.Values.Any(v => v.HasChanges);

        /// <summary>
        /// Gets the current changes as a <see cref="ChangeSet" />.
        /// </summary>
        /// <param name="commit">Whether or not to commit the changes during the get.  Commiting the changes
        /// will clear all tracking and leave the current values as-is.  Another call to
        /// <see cref="GetChangeSet(bool)" /> immdiately after a commit will return an
        /// empty <see cref="ChangeSet" />.</param>
        /// <returns>
        /// A <see cref="ChangeSet" /> containing all of the changes.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public ChangeSet GetChangeSet(bool commit = false)
        {
            throw new NotImplementedException();
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

            if (Changes.TryGetValue(propertyName, out ChangeValue value))
                return value.CurrentValue;

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

            Changes.TryAdd(propertyName, new ChangeValue());
            Changes[propertyName].CurrentValue = value;
        }
    }
}