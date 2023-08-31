using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.AspNetCore.JsonPatch.Patchable;
using Labradoratory.Fetch.Authorization;
using Labradoratory.Fetch.Mapping;
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
    /// <seealso cref="RepositoryController{TEntity, TEntity}" />
    public abstract class RepositoryController<TEntity> : RepositoryController<TEntity, TEntity>
        where TEntity : Entity
    {
        /// <summary>
        /// Initializes the <see cref="RepositoryController{TEntity}"/> base class.
        /// </summary>
        /// <param name="repository">The repository to use to manipulate <typeparamref name="TEntity"/> objects.</param>
        /// <param name="mapperProvider">A provider to get <see cref="IMapper{TFrom, TTo}"/> for mapping between <typeparamref name="TEntity"/> and <typeparamref name="TEntity"/>.</param>
        /// <param name="authorizationService">The authorization service.</param>
        public RepositoryController(
            Repository<TEntity> repository,
            IMapperProvider mapperProvider,
            IAuthorizationService authorizationService)
            : base(repository, mapperProvider, authorizationService)
        { }

        /// <summary>
        /// Initializes the <see cref="RepositoryController{TEntity}"/> base class that allows for
        /// lazy creation of the <see cref="Repository{TEntity}"/> instance.
        /// </summary>
        /// <param name="getRepositoryAsync">The function to obtain an instance of <see cref="Repository{TEntity}"/> asynchronously.</param>
        /// <param name="mapperProvider">A provider to get <see cref="IMapper{TFrom, TTo}"/> for mapping between <typeparamref name="TEntity"/> and <typeparamref name="TEntity"/>.</param>
        /// <param name="authorizationService">The authorization service.</param>
        protected RepositoryController(
            Func<Task<Repository<TEntity>>> getRepositoryAsync,
            IMapperProvider mapperProvider,
            IAuthorizationService authorizationService)
            : base(getRepositoryAsync, mapperProvider, authorizationService)
        { }
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
        /// <param name="mapperProvider">A provider to get <see cref="IMapper{TFrom, TTo}"/> for mapping between <typeparamref name="TEntity"/> and <typeparamref name="TView"/>.</param>
        /// <param name="authorizationService">The authorization service.</param>
        protected RepositoryController(
            Repository<TEntity> repository,
            IMapperProvider mapperProvider,
            IAuthorizationService authorizationService)
            : this(() => Task.FromResult(repository), mapperProvider, authorizationService)
        {}

        /// <summary>
        /// Initializes the <see cref="RepositoryController{TEntity, TView}"/> base class that allows for
        /// lazy creation of the <see cref="Repository{TEntity}"/> instance.
        /// </summary>
        /// <param name="getRepositoryAsync">The function to obtain an instance of <see cref="Repository{TEntity}"/> asynchronously.</param>
        /// <param name="mapperProvider">A provider to get <see cref="IMapper{TFrom, TTo}"/> for mapping between <typeparamref name="TEntity"/> and <typeparamref name="TView"/>.</param>
        /// <param name="authorizationService">The authorization service.</param>
        protected RepositoryController(
            Func<Task<Repository<TEntity>>> getRepositoryAsync,
            IMapperProvider mapperProvider,
            IAuthorizationService authorizationService)
        {
            GetRepositoryAsync = getRepositoryAsync;
            MapperProvider = mapperProvider;
            AuthorizationService = authorizationService;
        }

        /// <summary>
        /// Gets the get repository asynchronously.
        /// </summary>
        protected Func<Task<Repository<TEntity>>> GetRepositoryAsync { get; }

        /// <summary>
        /// Gets the created response options to use when an Add operation completes.
        /// </summary>
        protected virtual CreatedResponseOptions AddResponseOptions => CreatedResponseOptions.Instance | CreatedResponseOptions.Location;
 
        /// <summary>
        /// Gets the object conversion mapper.
        /// </summary>
        protected IMapperProvider MapperProvider { get; }

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
                await (await GetRepositoryAsync()).GetAsyncQueryResolver(FilterGetAll).ToListAsync(cancellationToken));
            authorizationResult = await AuthorizationService.AuthorizeAsync(User, entities, EntityAuthorizationPolicies.GetSome.ForType<TEntity>());
            if (!authorizationResult.Succeeded)
                return AuthorizationFailed(authorizationResult);

            var mapper = MapperProvider.GetMapper<TEntity, TView>();
            return Ok(mapper.MapMany(entities.GetAuthorized()));
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
            var entity = await (await GetRepositoryAsync()).FindAsync(keys, cancellationToken);
            if (entity == null)
                return NotFound();

            var authorizationResult = await AuthorizationService.AuthorizeAsync(User, entity, EntityAuthorizationPolicies.GetOne.ForType<TEntity>());
            if (!authorizationResult.Succeeded)
                return AuthorizationFailed(authorizationResult);

            var mapper = MapperProvider.GetMapper<TEntity, TView>();
            return Ok(mapper.Map(entity));
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
            var validationFailedResult = ValidateView(view);
            if (validationFailedResult != null)
                return validationFailedResult;

            var viewMapper = MapperProvider.GetMapper<TView, TEntity>();
            var entity = viewMapper.Map(view);
            var authorizationResult = await AuthorizationService.AuthorizeAsync(User, entity, EntityAuthorizationPolicies.Add.ForType<TEntity>());
            if (!authorizationResult.Succeeded)
                return AuthorizationFailed(authorizationResult);

            await (await GetRepositoryAsync()).AddAsync(entity, cancellationToken);

            switch (AddResponseOptions)
            {
                case CreatedResponseOptions.Empty:
                    return Ok();
                case CreatedResponseOptions.Instance:
                    {
                        var entityMapper = MapperProvider.GetMapper<TEntity, TView>();
                        return Ok(entityMapper.Map(entity));
                    }
                case CreatedResponseOptions.Location:
                    return CreatedAtAction(nameof(GetByKeys), GetAddCreatedAtRouteParameters(entity), null);
                case CreatedResponseOptions.Location | CreatedResponseOptions.Instance:
                    {
                        var entityMapper = MapperProvider.GetMapper<TEntity, TView>();
                        return CreatedAtAction(nameof(GetByKeys), GetAddCreatedAtRouteParameters(entity), entityMapper.Map(entity));
                    }
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
        [HttpPatch, Route("{encodedKeys}")]
        public virtual async Task<ActionResult<TView>> Update(string encodedKeys, [FromBody]JsonPatchDocument<TView> patch, CancellationToken cancellationToken)
        {
            var repository = await GetRepositoryAsync();

            var keys = Entity.DecodeKeys<TEntity>(encodedKeys);
            var entity = await repository.FindAsync(keys, cancellationToken);
            if (entity == null)
                return NotFound();

            var authorizationResult = await AuthorizationService.AuthorizeAsync(User, entity, EntityAuthorizationPolicies.Update.ForType<TEntity>());
            if (!authorizationResult.Succeeded)
                return AuthorizationFailed(authorizationResult);

            var entityMapper = MapperProvider.GetMapper<TEntity, TView>();
            var view = entityMapper.Map(entity);

            // There are no changes, just return the existing view.
            if (patch.Operations.Count == 0)
                return Ok(view);

            var errors = new List<JsonPatchError>();
            patch.ApplyToIfPatchable(view, error => errors.Add(error));

            if (errors.Count > 0)
                return BadRequest(errors);

            var validationFailedResult = ValidateView(view);
            if (validationFailedResult != null)
                return validationFailedResult;

            // Maps the patched view values back to the entity for updating.
            var viewMapper = MapperProvider.GetMapper<TView, TEntity>();
            viewMapper.Map(view, entity);
            
            await repository.UpdateAsync(entity, cancellationToken);
            return Ok(entityMapper.Map(entity));
        }

        /// <summary>
        /// Deletes the specified instance of <typeparamref name="TView"/>.
        /// </summary>
        /// <param name="encodedKeys">An encoded string representation of the keys to identify an instance of an entity.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns></returns>
        [HttpDelete, Route("{encodedKeys}")]
        public virtual async Task<IActionResult> Delete(string encodedKeys, CancellationToken cancellationToken)
        {
            var respository = await GetRepositoryAsync();

            var keys = Entity.DecodeKeys<TEntity>(encodedKeys);
            var entity = await respository.FindAsync(keys, cancellationToken);
            if (entity == null)
                return NotFound();

            var authorizationResult = await AuthorizationService.AuthorizeAsync(User, entity, EntityAuthorizationPolicies.Delete.ForType<TEntity>());
            if (!authorizationResult.Succeeded)
                return AuthorizationFailed(authorizationResult);

            await respository.DeleteAsync(entity, cancellationToken);

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
            if (User.Identity?.IsAuthenticated ?? true)
                return Forbid();

            return Unauthorized();
        }

        /// <summary>
        /// Validates the view object's properties.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>A <see cref="BadRequestObjectResult"/> if the object is invalid; Otherwise, <c>null</c>.</returns>
        /// <remarks>Uses <see cref="Validator"/> to do the validation.</remarks>
        protected virtual BadRequestObjectResult? ValidateView(TView view)
        {
            var context = new System.ComponentModel.DataAnnotations.ValidationContext(view, serviceProvider: ControllerContext.HttpContext.RequestServices, items: null);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(view, context, validationResults, true))
            {
                return BadRequest(validationResults);
            }

            return null;
        }

        /// <summary>
        /// Gets the parameters that are required to populate the <see cref="CreatedAtActionResult"/> after an entit is added.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        protected virtual ExpandoObject GetAddCreatedAtRouteParameters(TEntity entity)
        {
            dynamic parameters = new ExpandoObject();
            parameters.encodedKeys = entity.EncodeKeys();

            return parameters;
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
