using System;

namespace Labradoratory.Fetch.Processors.DataPackages
{
    /// <summary>
    /// The data related to deleting a <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="DataPackage" />
    public class EntityDeletingPackage<TEntity> : DataPackage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityDeletingPackage{TEntity}"/> class.
        /// </summary>
        /// <param name="entity">The entity being deleted.</param>
        public EntityDeletingPackage(TEntity entity)
        {
            Entity = entity;
        }

        /// <summary>
        /// Gets the entity that is being deleted.
        /// </summary>
        public TEntity Entity { get; }
    }
}
