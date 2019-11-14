using Labradoratory.Fetch.ChangeTracking;

namespace Labradoratory.Fetch.Processors.DataPackages
{
    /// <summary>
    /// The data related a <typeparamref name="TEntity"/> that was updated.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="DataPackage" />
    public class EntityUpdatedPackage<TEntity> : DataPackage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityUpdatedPackage{TEntity}"/> class.
        /// </summary>
        /// <param name="entity">The entity that was updated.</param>
        /// <param name="changes">The changes that were made.</param>
        public EntityUpdatedPackage(TEntity entity, ChangeSet changes)
        {
            Entity = entity;
            Changes = changes;
        }

        /// <summary>
        /// Gets the entity that was updated.
        /// </summary>
        public TEntity Entity { get; }

        /// <summary>
        /// Gets the changes that were made to the entity.
        /// </summary>
        public ChangeSet Changes { get; }
    }
}
