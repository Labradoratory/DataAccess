using System;

namespace Labradoratory.Fetch.Processors.Stages
{
    /// <summary>
    /// Defines that members that an object representing a stage in the processor pipeline should implement.
    /// </summary>
    /// <seealso cref="System.IComparable" />
    public interface IStage : IComparable<IStage>
    {
    }
}
