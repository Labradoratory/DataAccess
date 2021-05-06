using System;

namespace Labradoratory.Fetch.Authorization
{
    /// <summary>
    /// Represents a entity based policy.
    /// </summary>
    public class EntityAuthorizationPolicy
    {
        /// <summary>
        /// Performs an implicit conversion from <see cref="String"/> to <see cref="EntityAuthorizationPolicy"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator EntityAuthorizationPolicy(string value) => new EntityAuthorizationPolicy(value);

        private EntityAuthorizationPolicy(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(name, "An entity authorization policy name cannot be null or whitespace.");

            Name = name;
        }

        /// <summary>
        /// Gets the base policy name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the policy name for a specified type.
        /// </summary>
        /// <typeparam name="T">The type of policy to get.</typeparam>
        /// <returns>The policy name for <typeparamref name="T"/>.</returns>
        public string ForType<T>()
        {
            return $"{Name}-{typeof(T).Name}";
        }
    }
}
