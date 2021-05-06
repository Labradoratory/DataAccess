using System;
using System.Collections.Generic;
using Labradoratory.Fetch.Processors;
using Labradoratory.Fetch.Processors.DataPackages;
using Microsoft.Extensions.DependencyInjection;

namespace Labradoratory.Fetch.DependencyInjection
{
    /// <summary>
    /// A default implementation of <see cref="IProcessorProvider"/> that uses the Microsoft
    /// dependency injection system to find relavent <see cref="IProcessor{T}"/> instances.
    /// </summary>
    public class DefaultProcessorProvider : IProcessorProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultProcessorProvider"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public DefaultProcessorProvider(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        private IServiceProvider ServiceProvider { get; }

        /// <inheritdoc />
        public IEnumerable<IProcessor<T>> GetProcessors<T>()
            where T : DataPackage
        {
            return ServiceProvider.GetServices<IProcessor<T>>();
        }
    }
}
