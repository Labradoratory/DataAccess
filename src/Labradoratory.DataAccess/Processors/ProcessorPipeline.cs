using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Labradoratory.DataAccess.Processors
{
    public class ProcessorPipeline
    {
        public ProcessorPipeline(IProcessorProvider processorProvider)
        {
            ProcessorProvider = processorProvider;
        }

        protected IProcessorProvider ProcessorProvider { get; }

        public async Task ProcessAsync<T>(T package) where T : DataPackage
        {
            foreach(var processor in ProcessorProvider.GetProcessors<T>().OrderBy(p => p.Priority))
            {
                await processor.ProcessAsync(package);
            }
        }
    }        
}
