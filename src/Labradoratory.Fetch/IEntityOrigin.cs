using System.Collections.Generic;

namespace Labradoratory.Fetch
{
    /// <summary>
    /// Members associate metadata with an entity.
    /// </summary>
    public interface IEntityMetadata
    {
        /// <summary>
        /// Gets the metadata.
        /// </summary>
        IDictionary<string, object> Metadata { get; }
    }
}
