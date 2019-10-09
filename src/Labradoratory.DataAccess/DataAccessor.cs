﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.DataAccess.ChangeTracking;
using Labradoratory.DataAccess.Processors;
using Labradoratory.DataAccess.Processors.DataPackages;

namespace Labradoratory.DataAccess
{
    /// <summary>
    /// The base implementation of an object used to access data.  
    /// </summary>
    /// <remarks>
    /// <para>All data accessors derive from this class.</para>
    /// <para>
    /// The base implementation provides get, add, update and delete actions and 
    /// pushes the modification actions into the processing pipeline.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">The type of entity the accessor supports.</typeparam>
    /// <seealso cref="ISupportsAsyncQueryResolution{T}" />
    /// <seealso cref="ProcessorPipeline"/>
    public abstract class DataAccessor<T> : ISupportsAsyncQueryResolution<T>
        where T : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataAccessor{T}"/> class.
        /// </summary>
        /// <param name="processorPipeline">The processor pipeline.</param>
        protected DataAccessor(ProcessorPipeline processorPipeline)
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
        public abstract IQueryable<T> Get();

        /// <summary>
        /// Finds a specific entity using the identifing <paramref name="keys"/>.
        /// </summary>
        /// <param name="keys">The keys that uniquely identify the <typeparamref name="T"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task containing the result of the find.</returns>
        public abstract Task<T> FindAsync(object[] keys, CancellationToken cancellationToken);

        /// <summary>
        /// Adds an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task.</returns>
        public async Task AddAsync(T entity, CancellationToken cancellationToken)
        {
            var addingPackage = new EntityAddingPackage<T>(entity);
            await ProcessorPipeline.ProcessAsync(addingPackage, cancellationToken);

            await ExecuteAddAsync(entity, cancellationToken);

            var addedPackage = new EntityAddedPackage<T>(entity);
            await ProcessorPipeline.ProcessAsync(addedPackage, cancellationToken);
        }

        /// <summary>
        /// Executes the add.
        /// </summary>
        /// <param name="entity">To add.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task.</returns>
        protected abstract Task ExecuteAddAsync(T entity, CancellationToken cancellationToken);

        /// <summary>
        /// Updates an entity.
        /// </summary>
        /// <param name="entity">The entity with updates.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task UpdateAsync(T entity, CancellationToken cancellationToken)
        {
            var updatingPackage = new EntityUpdatingPackage<T>(entity);
            await ProcessorPipeline.ProcessAsync(updatingPackage, cancellationToken);

            var changes = entity.CommitChanges();
            await ExecuteUpdateAsync(entity, changes, cancellationToken);

            var updatedPackage = new EntityUpdatedPackage<T>(entity, changes);
            await ProcessorPipeline.ProcessAsync(updatedPackage, cancellationToken);
        }

        /// <summary>
        /// Executes the update.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="changes">The changes to the entity.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task.</returns>
        protected abstract Task<ChangeSet> ExecuteUpdateAsync(T entity, ChangeSet changes, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task.</returns>
        public async Task DeleteAsync(T entity, CancellationToken cancellationToken)
        {
            var deletingPackage = new EntityDeletingPackage<T>(entity);
            await ProcessorPipeline.ProcessAsync(deletingPackage, cancellationToken);

            await ExecuteDeleteAsync(entity, cancellationToken);

            var deletedPackage = new EntityDeletedPackage<T>(entity);
            await ProcessorPipeline.ProcessAsync(deletedPackage, cancellationToken);
        }

        /// <summary>
        /// Executes the delete.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task.</returns>
        protected abstract Task ExecuteDeleteAsync(T entity, CancellationToken cancellationToken);

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
