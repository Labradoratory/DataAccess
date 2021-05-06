using System;

namespace Labradoratory.Fetch.Processors.DataPackages
{
    /// <summary>
    /// The data related to deleting a <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="DataPackage" />
    public class EntityDeletingPackage<TEntity> : BaseEntityDataPackage<TEntity>
        where TEntity : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityDeletingPackage{TEntity}"/> class.
        /// </summary>
        /// <param name="entity">The entity being deleted.</param>
        public EntityDeletingPackage(TEntity entity)
            : base(entity)
        { }
    }
}
