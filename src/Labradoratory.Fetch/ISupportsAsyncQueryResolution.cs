using System;
using System.Linq;

namespace Labradoratory.Fetch
{
    /// <summary>
    /// Defines members that a object that supports async query resolution should implement.
    /// </summary>
    /// <typeparam name="T">The type of the query to resolve asynchronously.</typeparam>
    public interface ISupportsAsyncQueryResolution<T>
    {
        /// <summary>
        /// Gets as asynchronous query resolver working on the full set of data.
        /// </summary>
        /// <returns>An instance of an async query resolver that queries <typeparamref name="T"/> and returns results as <typeparamref name="T"/>.</returns>
        IAsyncQueryResolver<T> GetAsyncQueryResolver();
        /// <summary>
        /// Gets as asynchronous query resolver that supports additional querying.
        /// </summary>
        /// <typeparam name="TResult">The type of the result of the query.</typeparam>
        /// <param name="query">A root query to use for additional querying.</param>
        /// <returns>An instance of an async query resolver that queries <typeparamref name="T"/> and returns results as <typeparamref name="TResult"/>.</returns>
        IAsyncQueryResolver<TResult> GetAsyncQueryResolver<TResult>(Func<IQueryable<T>, IQueryable<TResult>> query);
    }
}
