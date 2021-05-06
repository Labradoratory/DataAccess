using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.Processors.DataPackages;
using Labradoratory.Fetch.Processors.Stages;

namespace Labradoratory.Fetch.Processors
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
        IStage Stage { get; }

        /// <summary>
        /// Processes the provided <see cref="DataPackage"/> asynchronously.
        /// </summary>
        /// <param name="package">The package to be processed.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>The task.</returns>
        Task ProcessAsync(T package, CancellationToken cancellationToken = default);
    }
}
