using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Labradoratory.DataAccess
{
    public abstract class DataAccessor<T> : ISupportsAsyncQueryResolution<T>
    {
        public DataAccessor()
        {

        }

        public abstract IQueryable<T> Get();

        public abstract Task<T> FindAsync(object[] keys, CancellationToken cancellationToken);
        
        public Task AddAsync(T entity, CancellationToken cancellationToken)
        {

        }

        public Task UpdateAsync(T entity, CancellationToken cancellationToken)
        {

        }

        public Task DeleteAsync(T entity, CancellationToken cancellationToken)
        {

        }

        public IAsyncQueryResolver<T> GetAsyncQueryResolver()
        {
            return GetAsyncQueryResolver(query => query);
        }

        public abstract IAsyncQueryResolver<TResult> GetAsyncQueryResolver<TResult>(System.Func<IQueryable<T>, IQueryable<TResult>> query);
    }
}
