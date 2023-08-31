using System;

namespace Labradoratory.Fetch.Mapping
{
    /// <summary>
    /// Defines the members for creating an instance mapper.
    /// </summary>
    public interface IMapperProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TFrom">The type mapping from.</typeparam>
        /// <typeparam name="TTo">The type mapping to.</typeparam>
        /// <returns>An instance of a mapper capable of mapping a <typeparamref name="TFrom"/> to a <typeparamref name="TTo"/>.</returns>
        IMapper<TFrom, TTo> GetMapper<TFrom, TTo>();
    }
}
