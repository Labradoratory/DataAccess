using System.Collections.Generic;
using Labradoratory.Fetch.Processors.DataPackages;

namespace Labradoratory.Fetch.Processors
{
    /// <summary>
    /// Defines the members that a provider of processors should implement.
    /// </summary>
    public interface IProcessorProvider
    {
        /// <summary>
        /// Gets the all of the processors for the specified <see cref="DataPackage"/> type.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="DataPackage"/> to get processors for.</typeparam>
        /// <returns></returns>
        IEnumerable<IProcessor<T>> GetProcessors<T>() where T : DataPackage;
    }
}
