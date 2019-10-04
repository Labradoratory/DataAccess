using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.DataAccess.ChangeTracking;
using Labradoratory.DataAccess.Processors;
using Labradoratory.DataAccess.Processors.DataPackages;

namespace Labradoratory.DataAccess
{
    public abstract class DataAccessor<T> : ISupportsAsyncQueryResolution<T>
    {
        public DataAccessor(ProcessorPipeline processorPipeline)
        {
            ProcessorPipeline = processorPipeline;
        }

        protected ProcessorPipeline ProcessorPipeline { get; }

        public abstract IQueryable<T> Get();

        public abstract Task<T> FindAsync(object[] keys, CancellationToken cancellationToken);
        
        public async Task AddAsync(T entity, CancellationToken cancellationToken)
        {
            var addingPackage = new EntityAddingPackage<T>(entity);
            await ProcessorPipeline.ProcessAsync(addingPackage, cancellationToken);

            await ExecuteAddAsync(addingPackage, cancellationToken);

            var addedPackage = new EntityAddedPackage<T>(entity);
            await ProcessorPipeline.ProcessAsync(addedPackage, cancellationToken);
        }

        protected abstract Task ExecuteAddAsync(EntityAddingPackage<T> package, CancellationToken cancellationToken);

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken)
        {
            var updatingPackage = new EntityUpdatingPackage<T>(entity);
            await ProcessorPipeline.ProcessAsync(updatingPackage, cancellationToken);

            var changes = await ExecuteUpdateAsync(updatingPackage, cancellationToken);

            var updatedPackage = new EntityUpdatedPackage<T>(entity, changes);
            await ProcessorPipeline.ProcessAsync(updatedPackage, cancellationToken);
        }

        protected abstract Task<ChangeSet> ExecuteUpdateAsync(EntityUpdatingPackage<T> package, CancellationToken cancellationToken);

        public async Task DeleteAsync(T entity, CancellationToken cancellationToken)
        {
            var deletingPackage = new EntityDeletingPackage<T>(entity);
            await ProcessorPipeline.ProcessAsync(deletingPackage, cancellationToken);

            await ExecuteDeleteAsync(deletingPackage, cancellationToken);

            var deletedPackage = new EntityDeletedPackage<T>(entity);
            await ProcessorPipeline.ProcessAsync(deletedPackage, cancellationToken);
        }

        protected abstract Task ExecuteDeleteAsync(EntityDeletingPackage<T> deletingPackage, CancellationToken cancellationToken);

        /// <summary>
        /// Gets as asynchronous query resolver working on the full set of data.
        /// </summary>
        /// <returns>
        /// An instance of an async query resolver that queries <typeparamref name="T" /> and returns results as <typeparamref name="T" />.
        /// </returns>
        public IAsyncQueryResolver<T> GetAsyncQueryResolver()
        {
            return GetAsyncQueryResolver(query => query);
        }

        /// <summary>
        /// Gets as asynchronous query resolver that supports additional querying.
        /// </summary>
        /// <typeparam name="TResult">The type of the result of the query.</typeparam>
        /// <param name="query">A root query to use for additional querying.</param>
        /// <returns>
        /// An instance of an async query resolver that queries <typeparamref name="T" /> and returns results as <typeparamref name="TResult" />.
        /// </returns>
        public abstract IAsyncQueryResolver<TResult> GetAsyncQueryResolver<TResult>(System.Func<IQueryable<T>, IQueryable<TResult>> query);
    }
}
