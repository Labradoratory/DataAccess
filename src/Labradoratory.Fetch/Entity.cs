using System;
using Labradoratory.Fetch.ChangeTracking;

namespace Labradoratory.Fetch
{
    /// <summary>
    /// Represents a storable, entity withing the data access framework.
    /// </summary>
    public abstract class Entity : ChangeTrackingObject
    {
        /// <summary>
        /// Converts the provided values into a keys array.
        /// </summary>
        /// <param name="keys">The key values to convert to an array.  Order of the keys is important to an entity.</param>
        /// <returns>The provided key values as an array.</returns>
        public static object[] ToKeys(params object[] keys)
        {
            return keys;
        }

        /// <summary>
        /// Decodes the keys using the specified <see cref="Entity"/> type.
        /// </summary>
        /// <typeparam name="T">The type to use to decode the keys.</typeparam>
        /// <param name="encodedKeys">The encoded keys to decode.</param>
        /// <returns>The decoded keys.</returns>
        public static object[] DecodeKeys<T>(string encodedKeys)
            where T : Entity
        {
            var entity = Activator.CreateInstance<T>();
            return entity.DecodeKeys(encodedKeys);
        }

        /// <summary>
        /// Gets the keys that uniquely identify the entity in storage.
        /// </summary>
        /// <returns>An array of uniquely identifying values.</returns>
        public abstract object[] GetKeys();

        /// <summary>
        /// Gets the instance's keys as a URL friendly string.
        /// </summary>
        /// <returns>The string representation of the entity's keys.</returns>
        public abstract string EncodedKeys();

        /// <summary>
        /// Decodes the string encoded keys.
        /// </summary>
        /// <param name="encodedKeys">The encoded keys to decode.</param>
        /// <returns>The decoded keys.</returns>
        public abstract object[] DecodeKeys(string encodedKeys);
    }
}
