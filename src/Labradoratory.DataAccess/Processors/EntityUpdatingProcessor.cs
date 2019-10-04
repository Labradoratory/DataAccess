using System.Threading.Tasks;
using Labradoratory.DataAccess.Processors.DataPackages;

namespace Labradoratory.DataAccess.Processors
{
    /// <summary>
    /// Processes an <typeparamref name="TEntity"/> being updated.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="IProcessor{TEntity}" />
    public abstract class EntityUpdatingProcessor<TEntity> : IProcessor<EntityUpdatingPackage<TEntity>>
    {
        /// <summary>
        /// Gets the priority with which the processor should execute.
        /// </summary>
        /// <remarks>
        /// TODO
        /// </remarks>
        public abstract uint Priority { get; }

        /// <summary>
        /// Processes the provided <see cref="T:Labradoratory.DataAccess.Processors.DataPackage" /> asynchronously.
        /// </summary>
        /// <param name="package">The package to be processed.</param>
        /// <returns>
        /// The task.
        /// </returns>
        public abstract Task ProcessAsync(EntityUpdatingPackage<TEntity> package);
    }    
}
