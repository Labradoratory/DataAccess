using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Labradoratory.Fetch.Controllers
{
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
        protected EntityRepositoryController(Repository<TEntity> repository, IMapper mapper)
            : base(repository, mapper)
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
        protected EntityRepositoryController(Repository<TEntity> repository, IMapper mapper)
        {
            Repository = repository;
            Mapper = mapper;
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
        /// Gets all of the entities.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns></returns>
        [HttpGet, Route("")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            if(await CheckAllowGetAsync(cancellationToken))
                return Unauthorized();

            return Ok(Repository.GetAsyncQueryResolver().ToListAsync());
        }

        /// <summary>
        /// Checks whether or not the get operation is allowed.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>The task that will contain the results.  TRUE, the get operation is allowed; Otherwise, FALSE.</returns>
        protected virtual Task<bool> CheckAllowGetAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Handles an entity add request.
        /// </summary>
        /// <param name="view">The view of the entity being added.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns></returns>
        [HttpPost, Route("")]
        public async Task<IActionResult> Add(TView view, CancellationToken cancellationToken)
        {
            var entity = Mapper.Map<TEntity>(view);

            if (await CheckAllowAddAsync(entity, cancellationToken))
                return Unauthorized();

            await Repository.AddAsync(entity, cancellationToken);
            return Ok(Mapper.Map<TView>(entity));
        }

        /// <summary>
        /// Checks whether or not the add operation is allowed.
        /// </summary>
        /// <param name="entity">The entity being added.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>The task that will contain the results.  TRUE, the add operation is allowed; Otherwise, FALSE.</returns>
        protected virtual Task<bool> CheckAllowAddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Handles an entity update request.
        /// </summary>
        /// <param name="keys">The keys identifying the entity being updated.</param>
        /// <param name="patch">The patch to apply to the entity.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns></returns>
        [HttpPatch, Route("")]
        public async Task<IActionResult> Update(object[] keys, JsonPatchDocument<TView> patch, CancellationToken cancellationToken)
        {
            var entity = await Repository.FindAsync(keys, cancellationToken);
            if (entity == null)
                return NotFound();

            var view = Mapper.Map<TView>(entity);

            // TODO: Apply patch to view

            // Maps the patched view values back to the entity for updating.
            Mapper.Map(view, entity);

            if (await CheckAllowUpdateAsync(entity, cancellationToken))
                return Unauthorized();

            await Repository.UpdateAsync(null, cancellationToken);
            return Ok(null);
        }

        /// <summary>
        /// Checks whether or not the update operation is allowed.
        /// </summary>
        /// <param name="entity">The entity being updated.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>The task that will contain the results.  TRUE, the update operation is allowed; Otherwise, FALSE.</returns>
        protected virtual Task<bool> CheckAllowUpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Handle an entity delete request.
        /// </summary>
        /// <param name="keys">The keys that identify the entity to delete.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns></returns>
        [HttpDelete, Route("")]
        public async Task<IActionResult> Delete(object[] keys, CancellationToken cancellationToken)
        {
            var entity = await Repository.FindAsync(keys, cancellationToken);
            if (entity == null)
                return NotFound();

            if (await CheckAllowDeleteAsync(entity, cancellationToken))
                return Unauthorized();

            await Repository.DeleteAsync(entity, cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Checks whether or not the delete operation is allowed.
        /// </summary>
        /// <param name="entity">The entity being deleted.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>The task that will contain the results.  TRUE, the delete operation is allowed; Otherwise, FALSE.</returns>
        protected virtual Task<bool> CheckAllowDeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}
