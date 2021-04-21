using System;
using System.Diagnostics.CodeAnalysis;

namespace Labradoratory.Fetch.Processors.Stages
{
    /// <summary>
    /// An implementation of <see cref="IStage"/> that uses simple <see cref="uint"/> values to determine execution.
    /// </summary>
    /// <remarks>Higher values are executed first.</remarks>
    /// <seealso cref="IStage" />
    public class NumericPriorityStage : IStage
    {
        /// <summary>
        /// A numeric stage with a value of zero.
        /// </summary>
        public readonly static IStage Zero = new NumericPriorityStage(0);

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericPriorityStage"/> class.
        /// </summary>
        /// <param name="priority">The priority.</param>
        public NumericPriorityStage(uint priority)
        {
            Priority = priority;
        }

        /// <summary>
        /// Gets the priority of execution.
        /// </summary>
        public uint Priority { get; }

        /// <inheritdoc />
        public virtual int CompareTo([AllowNull] IStage other)
        {
            if (other == null)
                return 1;

            if (other is NumericPriorityStage nps)
                return -1 * Priority.CompareTo(nps.Priority);

            throw new InvalidOperationException($"Cannot compare '{typeof(NumericPriorityStage).FullName}' to '{other?.GetType().Name ?? "null"}'");
        }
    }
}
