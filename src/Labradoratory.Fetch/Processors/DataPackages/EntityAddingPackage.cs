using System;

namespace Labradoratory.Fetch.Processors.DataPackages
{
    /// <summary>
    /// The data related to adding a <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="DataPackage" />
    public class EntityAddingPackage<TEntity> : BaseEntityDataPackage<TEntity>
        where TEntity : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityAddingPackage{TEntity}"/> class.
        /// </summary>
        /// <param name="entity">The entity being added.</param>
        public EntityAddingPackage(TEntity entity)
            : base(entity)
        { }
    }
}
