using Labradoratory.Fetch.DependencyInjection;
using Labradoratory.Fetch.Processors;
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
        /// <param name="sericeCollection">The serice collection.</param>
        /// <returns></returns>
        public static IServiceCollection AddFetch(this IServiceCollection sericeCollection)
        {
            sericeCollection.TryAddSingleton<ProcessorPipeline>();
            sericeCollection.TryAddSingleton<IProcessorProvider, DefaultProcessorProvider>();

            return sericeCollection;
        }
    }
}
