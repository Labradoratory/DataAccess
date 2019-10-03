using System.Collections.Generic;

namespace Labradoratory.DataAccess.Processors
{
    public interface IProcessorProvider
    {
        IEnumerable<IProcessor<T>> GetProcessors<T>() where T : DataPackage;
    }
}
