using System;

namespace Labradoratory.Fetch.Processors.DataPackages
{
    /// <summary>
    /// The data related to updating a <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="DataPackage" />
    public class EntityUpdatingPackage<TEntity> : BaseEntityDataPackage<TEntity>
        where TEntity : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityUpdatingPackage{TEntity}"/> class.
        /// </summary>
        /// <param name="entity">The entity being updated.</param>
        public EntityUpdatingPackage(TEntity entity)
            : base(entity)
        { }
    }
}
