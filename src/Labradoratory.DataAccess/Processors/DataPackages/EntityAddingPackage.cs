using System;

namespace Labradoratory.DataAccess.Processors.DataPackages
{
    /// <summary>
    /// The data related to adding a <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="DataPackage" />
    public class EntityAddingPackage<TEntity> : DataPackage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityAddingPackage{TEntity}"/> class.
        /// </summary>
        /// <param name="entity">The entity being added.</param>
        public EntityAddingPackage(TEntity entity)
        {
            Entity = entity;
        }

        /// <summary>
        /// Gets the entity that is being added.
        /// </summary>
        public TEntity Entity { get; }
    }
}
