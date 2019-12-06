using Labradoratory.Fetch.DependencyInjection;
using Labradoratory.Fetch.Processors;
using Labradoratory.Fetch.Processors.DataPackages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Labradoratory.Fetch.Extensions
{
    /// <summary>
    /// Methods to make working with <see cref="IServiceCollection"/> a little easier.
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the dependencies for the Fetch library.
        /// </summary>
        /// <param name="serviceCollection">The serice collection.</param>
        /// <returns>The <paramref name="serviceCollection"/>.</returns>
        public static IServiceCollection AddFetch(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<ProcessorPipeline>();
            serviceCollection.TryAddSingleton<IProcessorProvider, DefaultProcessorProvider>();

            return serviceCollection;
        }

        /// <summary>
        /// Adds the <typeparamref name="TEntity"/> adding processor, as transient.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TProcessor">The type of the processor.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The <paramref name="serviceCollection"/>.</returns>
        public static IServiceCollection AddFetchAddingProcessor<TEntity, TProcessor>(this IServiceCollection serviceCollection)
            where TEntity : Entity
            where TProcessor : class, IProcessor<EntityAddingPackage<TEntity>>
        {
            serviceCollection.AddTransient<IProcessor<EntityAddingPackage<TEntity>>, TProcessor>();
            return serviceCollection;
        }

        /// <summary>
        /// Adds the <typeparamref name="TEntity"/> added processor, as transient.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TProcessor">The type of the processor.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The <paramref name="serviceCollection"/>.</returns>
        public static IServiceCollection AddFetchAddedProcessor<TEntity, TProcessor>(this IServiceCollection serviceCollection)
            where TEntity : Entity
            where TProcessor : class, IProcessor<EntityAddedPackage<TEntity>>
        {
            serviceCollection.AddTransient<IProcessor<EntityAddedPackage<TEntity>>, TProcessor>();
            return serviceCollection;
        }

        /// <summary>
        /// Adds the <typeparamref name="TEntity"/> updating processor, as transient.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TProcessor">The type of the processor.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The <paramref name="serviceCollection"/>.</returns>
        public static IServiceCollection AddFetchUpdatingProcessor<TEntity, TProcessor>(this IServiceCollection serviceCollection)
            where TEntity : Entity
            where TProcessor : class, IProcessor<EntityUpdatingPackage<TEntity>>
        {
            serviceCollection.AddTransient<IProcessor<EntityUpdatingPackage<TEntity>>, TProcessor>();
            return serviceCollection;
        }

        /// <summary>
        /// Adds the <typeparamref name="TEntity"/> updated processor, as transient.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TProcessor">The type of the processor.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The <paramref name="serviceCollection"/>.</returns>
        public static IServiceCollection AddFetchUpdatedProcessor<TEntity, TProcessor>(this IServiceCollection serviceCollection)
            where TEntity : Entity
            where TProcessor : class, IProcessor<EntityUpdatedPackage<TEntity>>
        {
            serviceCollection.AddTransient<IProcessor<EntityUpdatedPackage<TEntity>>, TProcessor>();
            return serviceCollection;
        }

        /// <summary>
        /// Adds the <typeparamref name="TEntity"/> deleting processor, as transient.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TProcessor">The type of the processor.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The <paramref name="serviceCollection"/>.</returns>
        public static IServiceCollection AddFetchDeletingProcessor<TEntity, TProcessor>(this IServiceCollection serviceCollection)
            where TEntity : Entity
            where TProcessor : class, IProcessor<EntityDeletingPackage<TEntity>>
        {
            serviceCollection.AddTransient<IProcessor<EntityDeletingPackage<TEntity>>, TProcessor>();
            return serviceCollection;
        }

        /// <summary>
        /// Adds the <typeparamref name="TEntity"/> deleted processor, as transient.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TProcessor">The type of the processor.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The <paramref name="serviceCollection"/>.</returns>
        public static IServiceCollection AddFetchDeletedProcessor<TEntity, TProcessor>(this IServiceCollection serviceCollection)
            where TEntity : Entity
            where TProcessor : class, IProcessor<EntityDeletedPackage<TEntity>>
        {
            serviceCollection.AddTransient<IProcessor<EntityDeletedPackage<TEntity>>, TProcessor>();
            return serviceCollection;
        }
    }
}
