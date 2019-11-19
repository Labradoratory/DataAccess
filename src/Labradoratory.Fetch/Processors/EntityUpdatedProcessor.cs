using System.Threading.Tasks;
using Labradoratory.Fetch.ChangeTracking;
using Labradoratory.Fetch.Processors.DataPackages;

namespace Labradoratory.Fetch.Processors
{
    /// <summary>
    /// Processes an <typeparamref name="TEntity"/> that was updated.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="IProcessor{TEntity}" />
    public abstract class EntityUpdatedProcessor<TEntity> : BaseEntityProcessor<TEntity, EntityUpdatedPackage<TEntity>>
        where TEntity : Entity
    { }
}
