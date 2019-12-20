using System.Collections.Generic;
using System.Linq;

namespace Labradoratory.Fetch.Authorization
{
    /// <summary>
    /// A set of <typeparamref name="TEntity"/> to be authorized.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class EntityAuthorizationSet<TEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityAuthorizationSet{TEntity}"/> class.
        /// </summary>
        /// <param name="entities">The entities.</param>
        public EntityAuthorizationSet(IEnumerable<TEntity> entities)
        {
            Values = entities.Select(e => new EntityAuthorizationValue(e)).ToList();
        }

        /// <summary>
        /// Gets the authorization values.
        /// </summary>
        public IEnumerable<EntityAuthorizationValue> Values { get; }

        /// <summary>
        /// Gets all of the authorized entities.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TEntity> GetAuthorized()
        {
            return Values.Where(v => v.IsAuthorized).Select(v => v.Entity);
        }

        /// <summary>
        /// A single <typeparamref name="TEntity"/> to be authorized.
        /// </summary>
        public class EntityAuthorizationValue
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="EntityAuthorizationValue"/> class.
            /// </summary>
            /// <param name="entity">The entity.</param>
            public EntityAuthorizationValue(TEntity entity)
            {
                Entity = entity;
                IsAuthorized = false;
            }

            /// <summary>
            /// Gets the entity.
            /// </summary>
            public TEntity Entity { get; }

            /// <summary>
            /// Gets a value indicating whether this instance is authorized.
            /// </summary>
            /// <value>
            ///   <c>true</c> if this instance is authorized; otherwise, <c>false</c>.
            /// </value>
            public bool IsAuthorized { get; private set; }

            /// <summary>
            /// Sets the instance as authorized.
            /// </summary>
            /// <remarks>After calling, <see cref="IsAuthorized"/> will be <c>true</c>.</remarks>
            public void Success()
            {
                IsAuthorized = true;
            }
        }
    }
}
