using System;

namespace Labradoratory.Fetch.Processors.DataPackages
{
    /// <summary>
    /// The data related to updating a <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="DataPackage" />
    public class EntityUpdatingPackage<TEntity> : DataPackage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityUpdatingPackage{TEntity}"/> class.
        /// </summary>
        /// <param name="entity">The entity being updated.</param>
        public EntityUpdatingPackage(TEntity entity)
        {
            Entity = entity;
        }

        /// <summary>
        /// Gets the entity that is being updated.
        /// </summary>
        public TEntity Entity { get; }
    }
}
