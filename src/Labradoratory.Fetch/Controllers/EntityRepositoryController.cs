using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Labradoratory.AspNetCore.JsonPatch.Patchable;
using Labradoratory.Fetch.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Labradoratory.Fetch.Controllers
{
    // TODO: Add security hooks.

    /// <summary>
    /// A base controller implementation that provides add, update and delete functionality for an entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="EntityRepositoryController{TEntity, TEntity}" />
    public abstract class EntityRepositoryController<TEntity> : EntityRepositoryController<TEntity, TEntity>
        where TEntity : Entity
    {
        /// <summary>
        /// Initializes the <see cref="EntityRepositoryController{TEntity}"/> base class.
        /// </summary>
        /// <param name="repository">The repository to use to manipulate <typeparamref name="TEntity"/> objects.</param>
        /// <param name="mapper">The mapper to use for object conversion.</param>
        /// <param name="authorizationService"></param>
        public EntityRepositoryController(
            Repository<TEntity> repository,
            IMapper mapper,
            IAuthorizationService authorizationService)
            : base(repository, mapper, authorizationService)
        {}
    }

    /// <summary>
    /// A base controller implementation that provides add, update and delete functionality for an entity
    /// and a specialized view class to return to clients.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the controller manages.</typeparam>
    /// <typeparam name="TView">
    /// The view respresentation of the entity to provided clients.  
    /// This can be the same as the <typeparamref name="TEntity"/> if there is no special view.
    /// </typeparam>
    public abstract class EntityRepositoryController<TEntity, TView> : ControllerBase
        where TEntity : Entity
        where TView : class
    {
        /// <summary>
        /// Initializes the <see cref="EntityRepositoryController{TEntity, TView}"/> base class.
        /// </summary>
        /// <param name="repository">The repository to use to manipulate <typeparamref name="TEntity"/> objects.</param>
        /// <param name="mapper">
        /// The mapper to use for object conversion.  The <see cref="IMapper"/> should support transformation
        /// between <typeparamref name="TEntity"/> and <typeparamref name="TView"/>, both directions.
        /// </param>
        /// <param name="authorizationService">The authorization service.</param>
        protected EntityRepositoryController(
            Repository<TEntity> repository,
            IMapper mapper,
            IAuthorizationService authorizationService)
        {
            Repository = repository;
            Mapper = mapper;
            AuthorizationService = authorizationService;
        }

        /// <summary>
        /// Gets the data access instance for <typeparamref name="TEntity"/>.
        /// </summary>
        protected Repository<TEntity> Repository { get; }

        /// <summary>
        /// Gets the object conversion mapper.
        /// </summary>
        protected IMapper Mapper { get; }

        /// <summary>
        /// Gets the authorization service.
        /// </summary>
        protected IAuthorizationService AuthorizationService { get; }

        /// <summary>
        /// Gets all of the entities.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns></returns>
        [HttpGet, Route("")]
        public virtual async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var authorizationResult = await AuthorizationService.AuthorizeAsync(User, typeof(TEntity), EntityAuthorizationPolicies.GetAll);
            if (!authorizationResult.Succeeded)
                return AuthorizationFailed(authorizationResult);
                
            var entities = await Repository.GetAsyncQueryResolver(FilterGetAll).ToListAsync(cancellationToken);
            authorizationResult = await AuthorizationService.AuthorizeAsync(User, entities, EntityAuthorizationPolicies.GetSome);
            if (!authorizationResult.Succeeded)
                return AuthorizationFailed(authorizationResult);

            return Ok(Mapper.Map<IEnumerable<TView>>(entities));
        }

        /// <summary>
        /// Applies a filter to the query executed when <see cref="GetAll(CancellationToken)" /> is called.
        /// </summary>
        /// <param name="query">The query to apply the filter to.</param>
        /// <returns>The filtered query.</returns>
        protected virtual IQueryable<TEntity> FilterGetAll(IQueryable<TEntity> query)
        {
            return query;
        }

        /// <summary>
        /// Gets all of the entities.
        /// </summary>
        /// <param name="encodedKeys">An encoded string representation of the keys to identify an instance of an entity.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns></returns>
        [HttpGet, Route("{encodedKeys}")]
        public virtual async Task<IActionResult> GetByKeys(string encodedKeys, CancellationToken cancellationToken)
        {
            var keys = Entity.DecodeKeys<TEntity>(encodedKeys);
            var entity = await Repository.FindAsync(keys, cancellationToken);
            if (entity == null)
                return NotFound();

            var authorizationResult = await AuthorizationService.AuthorizeAsync(User, entity, EntityAuthorizationPolicies.GetOne);
            if (!authorizationResult.Succeeded)
                return AuthorizationFailed(authorizationResult);

            return Ok(Mapper.Map<TView>(entity));
        }

        /// <summary>
        /// Handles an entity add request.
        /// </summary>
        /// <param name="view">The view of the entity being added.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns></returns>
        [HttpPost, Route("")]
        public virtual async Task<IActionResult> Add(TView view, CancellationToken cancellationToken)
        {
            var entity = Mapper.Map<TEntity>(view);
            var authorizationResult = await AuthorizationService.AuthorizeAsync(User, entity, EntityAuthorizationPolicies.Add);
            if (!authorizationResult.Succeeded)
                return AuthorizationFailed(authorizationResult);

            await Repository.AddAsync(entity, cancellationToken);
            return Ok(Mapper.Map<TView>(entity));
        }

        /// <summary>
        /// Handles an entity update request.
        /// </summary>
        /// <param name="encodedKeys">An encoded string representation of the keys to identify an instance of an entity.</param>
        /// <param name="patch">The patch to apply to the entity.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns></returns>
        [HttpPatch, Route("encodedKeys")]
        public virtual async Task<IActionResult> Update(string encodedKeys, [FromBody]JsonPatchDocument<TView> patch, CancellationToken cancellationToken)
        {
            var keys = Entity.DecodeKeys<TEntity>(encodedKeys);
            var entity = await Repository.FindAsync(keys, cancellationToken);
            if (entity == null)
                return NotFound();

            var authorizationResult = await AuthorizationService.AuthorizeAsync(User, entity, EntityAuthorizationPolicies.Update);
            if (!authorizationResult.Succeeded)
                return AuthorizationFailed(authorizationResult);

            var view = Mapper.Map<TView>(entity);

            var errors = new List<JsonPatchError>();
            patch.ApplyToIfPatchable(view, error => errors.Add(error));

            if (errors.Count > 0)
                return BadRequest(errors);   

            // Maps the patched view values back to the entity for updating.
            Mapper.Map(view, entity);
            
            await Repository.UpdateAsync(entity, cancellationToken);
            return Ok(Mapper.Map<TView>(entity));
        }

        /// <summary>
        /// Handle an entity delete request.
        /// </summary>
        /// <param name="encodedKeys">An encoded string representation of the keys to identify an instance of an entity.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns></returns>
        [HttpDelete, Route("{encodedKeys")]
        public virtual async Task<IActionResult> Delete(string encodedKeys, CancellationToken cancellationToken)
        {
            var keys = Entity.DecodeKeys<TEntity>(encodedKeys);
            var entity = await Repository.FindAsync(keys, cancellationToken);
            if (entity == null)
                return NotFound();

            var authorizationResult = await AuthorizationService.AuthorizeAsync(User, entity, EntityAuthorizationPolicies.Delete);
            if (!authorizationResult.Succeeded)
                return AuthorizationFailed(authorizationResult);

            await Repository.DeleteAsync(entity, cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Handles a failed authorization.
        /// </summary>
        /// <param name="authorizationResult">The authorization result.</param>
        /// <returns>An <see cref="IActionResult"/> respresenting the failure.</returns>
        protected virtual IActionResult AuthorizationFailed(AuthorizationResult authorizationResult)
        {
            // TODO: Should we pass something back?  A message?
            if (User.Identity.IsAuthenticated)
                return Forbid();

            return Unauthorized();
        }
    }
}
