using System.Threading.Tasks;
using Labradoratory.Fetch.Processors.DataPackages;

namespace Labradoratory.Fetch.Processors
{
    /// <summary>
    /// Processes an <typeparamref name="TEntity"/> that was added.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="IProcessor{TEntity}" />
    public abstract class EntityAddedProcessor<TEntity> : BaseEntityProcessor<TEntity, EntityAddedPackage<TEntity>>
        where TEntity : Entity
    { } 
}
