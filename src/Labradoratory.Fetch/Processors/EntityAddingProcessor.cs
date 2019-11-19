using Labradoratory.Fetch.Processors.DataPackages;

namespace Labradoratory.Fetch.Processors
{
    /// <summary>
    /// Processes an <typeparamref name="TEntity"/> being added.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="IProcessor{TEntity}" />
    public abstract class EntityAddingProcessor<TEntity> : BaseEntityProcessor<TEntity, EntityAddingPackage<TEntity>>
        where TEntity : Entity
    { }
}
