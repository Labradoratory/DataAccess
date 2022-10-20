using System;
using Labradoratory.Fetch.Extensions;

namespace Labradoratory.Fetch.ChangeTracking
{
    /// <summary>
    /// A container that tracks whether or not a value has been set.
    /// </summary>
    /// <typeparam name="T">The type of value in the container.</typeparam>
    public class HasValueContainer<T>
    {
        private T? _value = default;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public T Value
        {
            get => _value.ThrowIfNull();
            set
            {
                _value = value;
                HasValue = true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has value.
        /// </summary>
        /// <remarks>
        /// <see cref="HasValue"/> is <c>true</c> if the <see cref="Value"/> property has been set.
        /// </remarks>
        public bool HasValue { get; private set; }

        /// <summary>
        /// Resets this container.
        /// </summary>
        /// <remarks>
        /// After reset, <see cref="HasValue"/> will be <c>false</c>.
        /// </remarks>
        public void Reset()
        {
            _value = default;
            HasValue = false;
        }
    }
}
