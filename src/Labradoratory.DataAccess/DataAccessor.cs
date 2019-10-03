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

        /// <summary>
        /// Gets as asynchronous query resolver working on the full set of data.
        /// </summary>
        /// <returns>
        /// An instance of an async query resolver that queries <typeparamref name="T" /> and returns results as <typeparamref name="T" />.
        /// </returns>
        public IAsyncQueryResolver<T> GetAsyncQueryResolver()
        {
            return GetAsyncQueryResolver(query => query);
        }

        /// <summary>
        /// Gets as asynchronous query resolver that supports additional querying.
        /// </summary>
        /// <typeparam name="TResult">The type of the result of the query.</typeparam>
        /// <param name="query">A root query to use for additional querying.</param>
        /// <returns>
        /// An instance of an async query resolver that queries <typeparamref name="T" /> and returns results as <typeparamref name="TResult" />.
        /// </returns>
        public abstract IAsyncQueryResolver<TResult> GetAsyncQueryResolver<TResult>(System.Func<IQueryable<T>, IQueryable<TResult>> query);
    }
}
