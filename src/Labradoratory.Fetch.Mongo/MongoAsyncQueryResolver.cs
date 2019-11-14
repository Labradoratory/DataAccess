using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Labradoratory.Fetch.Mongo
{
    /// <summary>
    /// An implementation of <see cref="IAsyncQueryResolver{T}"/> for Mongo DB.
    /// </summary>
    /// <typeparam name="T">The type being queried.</typeparam>
    /// <seealso cref="Labradoratory.Fetch.IAsyncQueryResolver{T}" />
    public class MongoAsyncQueryResolver<T> : IAsyncQueryResolver<T>
    {
        internal MongoAsyncQueryResolver(IMongoQueryable<T> queryable)
        {
            Queryable = queryable;
        }

        private IMongoQueryable<T> Queryable { get; }

        public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
        {
            return Queryable.AnyAsync(cancellationToken);
        }

        public Task<bool> AnyAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
        {
            return Queryable.AnyAsync(expression, cancellationToken);
        }

        public Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return Queryable.CountAsync(cancellationToken);
        }

        public Task<int> CountAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
        {
            return Queryable.CountAsync(expression, cancellationToken);
        }

        public Task<T> FirstAsync(CancellationToken cancellationToken = default)
        {
            return Queryable.FirstAsync(cancellationToken);
        }

        public Task<T> FirstAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
        {
            return Queryable.FirstAsync(expression, cancellationToken);
        }

        public Task<T> FirstOrDefaultAsync(CancellationToken cancellationToken)
        {
            return Queryable.FirstOrDefaultAsync(cancellationToken);
        }

        public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
        {

            return Queryable.FirstOrDefaultAsync(expression, cancellationToken);
        }

        public Task<T> SingleAsync(CancellationToken cancellationToken = default)
        {
            return Queryable.SingleAsync(cancellationToken);
        }

        public Task<T> SingleAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
        {
            return Queryable.SingleAsync(expression, cancellationToken);
        }

        public Task<T> SingleOrDefaultAsync(CancellationToken cancellationToken)
        {
            return Queryable.SingleOrDefaultAsync(cancellationToken);
        }

        public Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
        {
            return Queryable.SingleOrDefaultAsync(expression, cancellationToken);
        }

        public async Task<IList<T>> ToListAsync(CancellationToken cancellationToken = default)
        {
            return await Queryable.ToListAsync(cancellationToken);
        }
    }
}
