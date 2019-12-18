using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.ChangeTracking;
using Labradoratory.Fetch.Processors;
using Labradoratory.Fetch.Processors.DataPackages;

namespace Labradoratory.Fetch
{
    /// <summary>
    /// The base implementation of an repository used to access data.  
    /// </summary>
    /// <remarks>
    /// <para>All data accessors derive from this class.</para>
    /// <para>
    /// The base implementation provides get, add, update and delete actions and 
    /// pushes the modification actions into the processing pipeline.
    /// </para>
    /// </remarks>
    /// <typeparam name="TEntity">The type of entity the accessor supports.</typeparam>
    /// <seealso cref="ISupportsAsyncQueryResolution{TEntity}" />
    /// <seealso cref="ProcessorPipeline"/>
    public abstract class Repository<TEntity> : ISupportsAsyncQueryResolution<TEntity>
        where TEntity : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}"/> class.
        /// </summary>
        /// <param name="processorPipeline">The processor pipeline.</param>
        protected Repository(ProcessorPipeline processorPipeline)
        {
            ProcessorPipeline = processorPipeline;
        }

        /// <summary>
        /// Gets the processor pipeline.
        /// </summary>
        protected ProcessorPipeline ProcessorPipeline { get; }

        /// <summary>
        /// Gets a queryable collection of entities.
        /// </summary>
        public abstract IQueryable<TEntity> Get();

        /// <summary>
        /// Finds a specific entity using the identifing <paramref name="keys"/>.
        /// </summary>
        /// <param name="keys">The keys that uniquely identify the <typeparamref name="TEntity"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task containing the result of the find.</returns>
        public abstract Task<TEntity> FindAsync(object[] keys, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task.</returns>
        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            var addingPackage = new EntityAddingPackage<TEntity>(entity);
            await ProcessorPipeline.ProcessAsync(addingPackage, cancellationToken);

            await ExecuteAddAsync(entity, cancellationToken);

            var addedPackage = new EntityAddedPackage<TEntity>(entity);
            await ProcessorPipeline.ProcessAsync(addedPackage, cancellationToken);
        }

        /// <summary>
        /// Executes the add.
        /// </summary>
        /// <param name="entity">To add.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task.</returns>
        protected abstract Task ExecuteAddAsync(TEntity entity, CancellationToken cancellationToken);

        /// <summary>
        /// Updates an entity.
        /// </summary>
        /// <param name="entity">The entity with updates.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<ChangeSet> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (!entity.HasChanges)
                return null;

            var updatingPackage = new EntityUpdatingPackage<TEntity>(entity);
            await ProcessorPipeline.ProcessAsync(updatingPackage, cancellationToken);

            var changes = entity.CommitChanges();
            await ExecuteUpdateAsync(entity, changes, cancellationToken);

            var updatedPackage = new EntityUpdatedPackage<TEntity>(entity, changes);
            await ProcessorPipeline.ProcessAsync(updatedPackage, cancellationToken);

            return changes;
        }

        /// <summary>
        /// Executes the update.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="changes">The changes to the entity.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task.</returns>
        protected abstract Task<ChangeSet> ExecuteUpdateAsync(TEntity entity, ChangeSet changes, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task.</returns>
        public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            var deletingPackage = new EntityDeletingPackage<TEntity>(entity);
            await ProcessorPipeline.ProcessAsync(deletingPackage, cancellationToken);

            await ExecuteDeleteAsync(entity, cancellationToken);

            var deletedPackage = new EntityDeletedPackage<TEntity>(entity);
            await ProcessorPipeline.ProcessAsync(deletedPackage, cancellationToken);
        }

        /// <summary>
        /// Executes the delete.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task.</returns>
        protected abstract Task ExecuteDeleteAsync(TEntity entity, CancellationToken cancellationToken);

        /// <summary>
        /// Gets as asynchronous query resolver working on the full set of data.
        /// </summary>
        /// <returns>
        /// An instance of an async query resolver that queries <typeparamref name="TEntity" /> and returns results as <typeparamref name="TEntity" />.
        /// </returns>
        public IAsyncQueryResolver<TEntity> GetAsyncQueryResolver()
        {
            return GetAsyncQueryResolver(query => query);
        }

        /// <summary>
        /// Gets as asynchronous query resolver that supports additional querying.
        /// </summary>
        /// <typeparam name="TResult">The type of the result of the query.</typeparam>
        /// <param name="query">A root query to use for additional querying.</param>
        /// <returns>
        /// An instance of an async query resolver that queries <typeparamref name="TEntity" /> and returns results as <typeparamref name="TResult" />.
        /// </returns>
        public abstract IAsyncQueryResolver<TResult> GetAsyncQueryResolver<TResult>(Func<IQueryable<TEntity>, IQueryable<TResult>> query);
    }
}
