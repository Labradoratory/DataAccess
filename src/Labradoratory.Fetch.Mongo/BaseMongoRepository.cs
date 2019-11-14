using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.Processors;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Labradoratory.Fetch.Mongo
{
    /// <summary>
    /// Provides a base implementation of <see cref="Repository{T}"/> for working with a Mongo database.
    /// </summary>
    /// <typeparam name="T">They type of document the accessor accesses.</typeparam>
    /// <seealso cref="Repository{T}" />
    public abstract class BaseMongoRepository<T> : Repository<T>
        where T : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMongoRepository{T}"/> class.
        /// </summary>
        /// <param name="collection">The Mongo database collection.</param>
        public BaseMongoRepository(ProcessorPipeline processorPipeline, IMongoCollection<T> collection)
            : base(processorPipeline)
        {
            Collection = collection;
        }

        protected IMongoCollection<T> Collection { get; }

        protected abstract FilterDefinition<T> CreateFindFilter(object[] keys);

        public override async Task<T> FindAsync(object[] keys, CancellationToken cancellationToken)
        {
            return await Collection.Find(CreateFindFilter(keys)).SingleOrDefaultAsync(cancellationToken);
        }

        public override IQueryable<T> Get()
        {
            return Collection.AsQueryable();
        }

        public override IAsyncQueryResolver<TResult> GetAsyncQueryResolver<TResult>(Func<IQueryable<T>, IQueryable<TResult>> query)
        {
            return new MongoAsyncQueryResolver<TResult>(Get() as IMongoQueryable<TResult>);
        }
    }
}
