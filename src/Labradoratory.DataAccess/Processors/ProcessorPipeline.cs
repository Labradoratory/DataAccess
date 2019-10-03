using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Labradoratory.DataAccess.Processors
{
    public class ProcessorPipeline
    {
        public ProcessorPipeline(IProcessorProvider processorProvider)
        {

        }
    }

    public interface IProcessorProvider
    {
        IEnumerable<IProcessor<T>> GetProcessor<T>() where T : DataPackage;
    }

    public interface IProcessor<T> where T : DataPackage
    {
        uint Priority { get; }
        Task ProcessAsync(T package);
    }

    public abstract class DataPackage : Dictionary<string, object>
    {
        protected T GetValue<T>([CallerMemberName] string property = null, bool throwIfNotFound = false)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            if (TryGetValue(property, out object value))
                return (T)value;

            if (throwIfNotFound)
                throw new KeyNotFoundException(nameof(property));

            return default;
        }

        protected void SetValue<T>(T value, [CallerMemberName] string property = null)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            this[property] = value;
        }
    }
}
