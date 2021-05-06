using System;
using Microsoft.AspNetCore.Authorization;

namespace Labradoratory.Fetch.Authorization
{
    /// <summary>
    /// The names of <see cref="AuthorizationPolicy"/> instances related to <see cref="Entity"/> access.
    /// </summary>
    public static class EntityAuthorizationPolicies
    {
        /// <summary>
        /// Resource authorize policy for getting all of a type of entity.
        /// The resource will be a <see cref="Type"/> of entity.
        /// </summary>
        public static readonly EntityAuthorizationPolicy GetAll = "EntityGetAll";

        /// <summary>
        /// Resource authorize policy for getting some of a type of entity.
        /// The resource will be an <see cref="EntityAuthorizationSet{TEntity}"/>.
        /// </summary>
        public static readonly EntityAuthorizationPolicy GetSome = "EntityGetSome";

        /// <summary>
        /// Resource authorize policy for gettings single entity.
        /// The resource is the instance of the entity.
        /// </summary>
        public static readonly EntityAuthorizationPolicy GetOne = "EntityGetOne";

        /// <summary>
        /// Resource authorize policy for updating an entity.
        /// The resource is the instance of the entity.
        /// </summary>
        public static readonly EntityAuthorizationPolicy Update = "EntityUpdate";

        /// <summary>
        /// Resource authorize policy for updating an entity.
        /// The resource is the instance of the entity.
        /// </summary>
        public static readonly EntityAuthorizationPolicy Add = "EntityAdd";

        /// <summary>
        /// Resource authorize policy for deleting an entity.
        /// The resource is the instance of the entity.
        /// </summary>
        public static readonly EntityAuthorizationPolicy Delete = "EntityDelete";
    }    
}
