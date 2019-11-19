using System;

namespace Labradoratory.Fetch.Processors.DataPackages
{
    /// <summary>
    /// A base implementation of a <see cref="DataPackage"/> that contains <see cref="Entity"/> data.
    /// </summary>
    /// <seealso cref="Labradoratory.Fetch.Processors.DataPackages.DataPackage" />
    public abstract class BaseEntityDataPackage<TEntity> : DataPackage
        where TEntity : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEntityDataPackage{TEntity}"/> class.
        /// </summary>
        /// <param name="entity">The entity being added.</param>
        protected BaseEntityDataPackage(TEntity entity)
        {
            Entity = entity;
        }

        /// <summary>
        /// Gets the entity that was added.
        /// </summary>
        public TEntity Entity { get; }
    }
}
