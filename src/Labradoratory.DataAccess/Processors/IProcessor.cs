using System.Threading.Tasks;
using Labradoratory.DataAccess.Processors.DataPackages;

namespace Labradoratory.DataAccess.Processors
{
    /// <summary>
    /// Defines the members a data processor should implement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IProcessor<T> where T : DataPackage
    {
        /// <summary>
        /// Gets the priority with which the processor should execute.
        /// </summary>
        /// <remarks>
        /// TODO
        /// </remarks>
        uint Priority { get; }

        /// <summary>
        /// Processes the provided <see cref="DataPackage"/> asynchronously.
        /// </summary>
        /// <param name="package">The package to be processed.</param>
        /// <returns>The task.</returns>
        Task ProcessAsync(T package);
    }
}
