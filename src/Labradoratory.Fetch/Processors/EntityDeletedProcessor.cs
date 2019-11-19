using System.Threading.Tasks;
using Labradoratory.Fetch.Processors.DataPackages;

namespace Labradoratory.Fetch.Processors
{
    /// <summary>
    /// Processes an <typeparamref name="TEntity"/> that was deleted.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="IProcessor{TEntity}" />
    public abstract class EntityDeletedProcessor<TEntity> : BaseEntityProcessor<TEntity, EntityDeletedPackage<TEntity>>
        where TEntity : Entity
    { }
}
