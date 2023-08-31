using System.Collections.Generic;

namespace Labradoratory.Fetch.Mapping
{
    /// <summary>
    /// Defines the members for mapping from one object type to another.
    /// </summary>
    /// <typeparam name="TFrom">The type mapping from.</typeparam>
    /// <typeparam name="TTo">The type mapping to.</typeparam>
    public interface IMapper<TFrom, TTo>
    {
        /// <summary>
        /// Maps the provided <typeparamref name="TFrom"/> to <typeparamref name="TTo"/>.
        /// </summary>
        /// <param name="from">The object to map from.</param>
        /// <returns>An instance of <typeparamref name="TTo"/> as mapped from the provided object.</returns>
        TTo Map(TFrom from);

        /// <summary>
        /// Copies the values from the provided <typeparamref name="TFrom"/> to the provided <typeparamref name="TTo"/>.
        /// </summary>
        /// <param name="from">The object to copy values from.</param>
        /// <param name="to">The object to copy values to.</param>
        /// <returns>The provided instance of <paramref name="to"/></returns>
        TTo Map(TFrom from, TTo to);

        /// <summary>
        /// Maps the provided collection of <typeparamref name="TFrom"/> to a collection of <typeparamref name="TTo"/>.
        /// </summary>
        /// <param name="from">The objects to map from.</param>
        /// <returns>An collection of instances of <typeparamref name="TTo"/> as mapped from the provided objects.</returns>
        IEnumerable<TTo> MapMany(IEnumerable<TFrom> from);
    }
}
