using System;

namespace Labradoratory.Fetch.Extensions
{
    /// <summary>
    /// Methods to make working with <see cref="IEntityMetadata"/> a little easier.
    /// </summary>
    public static class IEntityMetadataExtensions
    {
        /// <summary>
        /// Sets the metadata.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void SetMetadata(this IEntityMetadata entity, string key, object value)
        {
            entity.Metadata[key] = value;
        }

        /// <summary>
        /// Gets the metadata.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="key">The key.</param>
        /// <returns>The metadata object.</returns>
        public static object GetMetadata(this IEntityMetadata entity, string key)
        {
            return entity.Metadata[key];
        }

        /// <summary>
        /// Gets the metadata.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="key">The key.</param>
        /// <returns>The typed metadata object.</returns>
        public static T GetMetadata<T>(this IEntityMetadata entity, string key)
        {
            return (T)entity.Metadata[key];
        }

        /// <summary>
        /// Removes the metadata.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="key">The key.</param>
        public static void RemoveMetadata(this IEntityMetadata entity, string key)
        {
            entity.Metadata.Remove(key);
        }
    }
}
