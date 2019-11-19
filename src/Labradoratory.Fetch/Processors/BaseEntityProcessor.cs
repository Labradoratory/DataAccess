using System.Threading.Tasks;
using Labradoratory.Fetch.Processors.DataPackages;

namespace Labradoratory.Fetch.Processors
{
    /// <summary>
    /// The base implementation of a processor that handles an <see cref="Entity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity handled by the processor.</typeparam>
    /// <typeparam name="TDataPackage">The type of the data package.</typeparam>
    /// <seealso cref="Labradoratory.Fetch.Processors.IProcessor{TDataPackage}" />
    public abstract class BaseEntityProcessor<TEntity, TDataPackage> : IProcessor<TDataPackage>
        where TDataPackage : BaseEntityDataPackage<TEntity>
        where TEntity : Entity
    {
        /// <inheritdoc />
        public abstract uint Priority { get; }

        /// <inheritdoc />
        public abstract Task ProcessAsync(TDataPackage package);
    }
}
