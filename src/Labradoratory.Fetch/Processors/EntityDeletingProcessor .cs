using System.Threading.Tasks;
using Labradoratory.Fetch.Processors.DataPackages;

namespace Labradoratory.Fetch.Processors
{
    /// <summary>
    /// Processes an <typeparamref name="TEntity"/> being added.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="IProcessor{TEntity}" />
    public abstract class EntityDeletingProcessor<TEntity> : BaseEntityProcessor<TEntity, EntityDeletingPackage<TEntity>>
        where TEntity : Entity
    { }
}
