using System.Collections.Generic;
using System.Linq;

namespace Labradoratory.Fetch.Extensions
{
    /// <summary>
    /// Methods to make working with <see cref="IEnumerable{T}"/> a little easier.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Returns an empty enumerable if the <paramref name="target"/> enumerable is null.
        /// </summary>
        /// <typeparam name="T">The type contained in the enumerable.</typeparam>
        /// <param name="target">The target.</param>
        /// <returns>An empty enumerable if the target is null; Otherwise, returns the <paramref name="target"/>.</returns>
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> target)
        {
            return target ?? Enumerable.Empty<T>();
        }
    }
}
