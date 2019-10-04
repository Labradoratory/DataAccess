using System;

namespace Labradoratory.DataAccess.Processors.DataPackages
{
    /// <summary>
    /// The data related a <typeparamref name="TEntity"/> that was deleted.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="DataPackage" />
    public class EntityDeletedPackage<TEntity> : DataPackage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityDeletedPackage{TEntity}"/> class.
        /// </summary>
        /// <param name="entity">The entity was deleted.</param>
        public EntityDeletedPackage(TEntity entity)
        {
            Entity = entity;
        }

        /// <summary>
        /// Gets the entity that was deleted.
        /// </summary>
        public TEntity Entity { get; }
    }
}
