using Labradoratory.DataAccess.ChangeTracking;

namespace Labradoratory.DataAccess
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
        /// Gets the keys that uniquely identify the entity in storage.
        /// </summary>
        /// <returns>An array of uniquely identifying values.</returns>
        public abstract object[] GetKeys();
    }
}
