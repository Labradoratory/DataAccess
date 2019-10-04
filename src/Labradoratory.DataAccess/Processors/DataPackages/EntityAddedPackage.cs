using System;

namespace Labradoratory.DataAccess.Processors.DataPackages
{
    /// <summary>
    /// The data related a <typeparamref name="TEntity"/> that was added.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="DataPackage" />
    public class EntityAddedPackage<TEntity> : DataPackage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityAddedPackage{TEntity}"/> class.
        /// </summary>
        /// <param name="entity">The entity being added.</param>
        public EntityAddedPackage(TEntity entity)
        {
            Entity = entity;
        }

        /// <summary>
        /// Gets the entity that was added.
        /// </summary>
        public TEntity Entity { get; }
    }
}
