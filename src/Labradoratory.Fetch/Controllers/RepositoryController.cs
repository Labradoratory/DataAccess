using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    public abstract class RepositoryController<TEntity> : RepositoryController<TEntity, TEntity>
        where TEntity : Entity
    {
        /// <summary>
        /// Initializes the <see cref="RepositoryController{TEntity}"/> base class.
        /// </summary>
        /// <param name="repository">The repository to use to manipulate <typeparamref name="TEntity"/> objects.</param>
        /// <param name="mapper">The mapper to use for object conversion.</param>
        /// <param name="authorizationService"></param>
        public RepositoryController(
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
    public abstract class RepositoryController<TEntity, TView> : ControllerBase
        where TEntity : Entity
        where TView : class
    {
        /// <summary>
        /// Initializes the <see cref="RepositoryController{TEntity, TView}"/> base class.
        /// </summary>
        /// <param name="repository">The repository to use to manipulate <typeparamref name="TEntity"/> objects.</param>
        /// <param name="mapper">
        /// The mapper to use for object conversion.  The <see cref="IMapper"/> should support transformation
        /// between <typeparamref name="TEntity"/> and <typeparamref name="TView"/>, both directions.
        /// </param>
        /// <param name="authorizationService">The authorization service.</param>
        protected RepositoryController(
            Repository<TEntity> repository,
            IMapper mapper,
            IAuthorizationService authorizationService)
        {
            Repository = repository;
            Mapper = mapper;
            AuthorizationService = authorizationService;
        }

        /// <summary>
        /// Gets the created response options to use when an Add operation completes.
        /// </summary>
        protected virtual CreatedResponseOptions AddResponseOptions => CreatedResponseOptions.Location;
 
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
        /// Gets all of the <typeparamref name="TView"/> instances.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>The collection of <typeparamref name="TView"/>.</returns>
        [HttpGet, Route("")]
        public virtual async Task<ActionResult<List<TView>>> GetAll(CancellationToken cancellationToken)
        {
            var authorizationResult = await AuthorizationService.AuthorizeAsync(User, typeof(TEntity), EntityAuthorizationPolicies.GetAll.ForType<TEntity>());
            if (!authorizationResult.Succeeded)
                return AuthorizationFailed(authorizationResult);
                
            var entities = new EntityAuthorizationSet<TEntity>(
                await Repository.GetAsyncQueryResolver(FilterGetAll).ToListAsync(cancellationToken));
            authorizationResult = await AuthorizationService.AuthorizeAsync(User, entities, EntityAuthorizationPolicies.GetSome.ForType<TEntity>());
            if (!authorizationResult.Succeeded)
                return AuthorizationFailed(authorizationResult);

            return Ok(Mapper.Map<IEnumerable<TView>>(entities.GetAuthorized()));
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
        /// Gets the specified instance of <typeparamref name="TView"/>.
        /// </summary>
        /// <param name="encodedKeys">An encoded string representation of the keys to identify an instance of an entity.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>The instance of <typeparamref name="TView"/>.</returns>
        [HttpGet, Route("{encodedKeys}")]
        public virtual async Task<ActionResult<TView>> GetByKeys(string encodedKeys, CancellationToken cancellationToken)
        {
            var keys = Entity.DecodeKeys<TEntity>(encodedKeys);
            var entity = await Repository.FindAsync(keys, cancellationToken);
            if (entity == null)
                return NotFound();

            var authorizationResult = await AuthorizationService.AuthorizeAsync(User, entity, EntityAuthorizationPolicies.GetOne.ForType<TEntity>());
            if (!authorizationResult.Succeeded)
                return AuthorizationFailed(authorizationResult);

            return Ok(Mapper.Map<TView>(entity));
        }

        /// <summary>
        /// Addes the provided instance of <typeparamref name="TView"/>.
        /// </summary>
        /// <param name="view">The view of the entity being added.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>The instance of <typeparamref name="TView"/> that was added.</returns>
        [HttpPost, Route("")]
        public virtual async Task<ActionResult<TView>> Add(TView view, CancellationToken cancellationToken)
        {
            var entity = Mapper.Map<TEntity>(view);
            var authorizationResult = await AuthorizationService.AuthorizeAsync(User, entity, EntityAuthorizationPolicies.Add.ForType<TEntity>());
            if (!authorizationResult.Succeeded)
                return AuthorizationFailed(authorizationResult);

            await Repository.AddAsync(entity, cancellationToken);

            switch (AddResponseOptions)
            {
                case CreatedResponseOptions.Empty:
                    return Ok();
                case CreatedResponseOptions.Instance:
                    return Ok(Mapper.Map<TView>(entity));
                case CreatedResponseOptions.Location:
                    return CreatedAtAction(nameof(GetByKeys), new { encodedKeys = entity.EncodeKeys() }, null);
                case CreatedResponseOptions.Location | CreatedResponseOptions.Instance:
                    return CreatedAtAction(nameof(GetByKeys), new { encodedKeys = entity.EncodeKeys() }, Mapper.Map<TView>(entity));
                default:
                    throw new InvalidOperationException($"{nameof(AddResponseOptions)} value of {AddResponseOptions} is invalid.");
            }
        }

        /// <summary>
        /// Updates the specified instance of <typeparamref name="TView"/>.
        /// </summary>
        /// <param name="encodedKeys">An encoded string representation of the keys to identify an instance of an entity.</param>
        /// <param name="patch">The patch to apply to the entity.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>The instance of <typeparamref name="TView"/> that was updated.</returns>
        [HttpPatch, Route("encodedKeys")]
        public virtual async Task<ActionResult<TView>> Update(string encodedKeys, [FromBody]JsonPatchDocument<TView> patch, CancellationToken cancellationToken)
        {
            var keys = Entity.DecodeKeys<TEntity>(encodedKeys);
            var entity = await Repository.FindAsync(keys, cancellationToken);
            if (entity == null)
                return NotFound();

            var authorizationResult = await AuthorizationService.AuthorizeAsync(User, entity, EntityAuthorizationPolicies.Update.ForType<TEntity>());
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
        /// Deletes the specified instance of <typeparamref name="TView"/>.
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

            var authorizationResult = await AuthorizationService.AuthorizeAsync(User, entity, EntityAuthorizationPolicies.Delete.ForType<TEntity>());
            if (!authorizationResult.Succeeded)
                return AuthorizationFailed(authorizationResult);

            await Repository.DeleteAsync(entity, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Handles a failed authorization.
        /// </summary>
        /// <param name="authorizationResult">The authorization result.</param>
        /// <returns>An <see cref="IActionResult"/> respresenting the failure.</returns>
        protected virtual ActionResult AuthorizationFailed(AuthorizationResult authorizationResult)
        {
            // TODO: Should we pass something back?  A message?
            if (User.Identity.IsAuthenticated)
                return Forbid();

            return Unauthorized();
        }

        /// <summary>
        /// Creates a <see cref="StatusCodeResult"/> with <see cref="HttpStatusCode.MethodNotAllowed"/> code.
        /// </summary>
        /// <returns>The result.</returns>
        public StatusCodeResult MethodNotAllowed()
        {
            return StatusCode(Convert.ToInt32(HttpStatusCode.MethodNotAllowed));
        }

        /// <summary>
        /// Creates a <see cref="ObjectResult"/> with <see cref="HttpStatusCode.MethodNotAllowed"/> code and the provided <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to include in the result</param>
        /// <returns>The result.</returns>
        public ObjectResult MethodNotAllowed(object value)
        {
            return StatusCode(Convert.ToInt32(HttpStatusCode.MethodNotAllowed), value);
        }
    }
}
