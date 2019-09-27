using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Labradoratory.DataAccess.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TView"></typeparam>
    public abstract class EntityDataAccessController<TEntity, TView> : ControllerBase
        where TEntity : Entity
        where TView : class
    {
        protected EntityDataAccessController(IDataAccess<TEntity> dataAccess)
        {
            DataAccess = dataAccess;
        }

        protected IDataAccess<TEntity> DataAccess { get; }

        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            if(await CheckAllowGetAsync(cancellationToken))
                return Unauthorized();

            return null;
        }

        protected virtual Task<bool> CheckAllowGetAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        [HttpPost, Route("")]
        public Task<IActionResult> Add(TEntity entity, CancellationToken cancellationToken)
        {
            return null;
        }

        protected virtual Task<bool> CheckAllowAddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        [HttpPatch, Route("")]
        public Task<IActionResult> Update(JsonPatchDocument<TView> view, CancellationToken cancellationToken)
        {
            return null;
        }

        protected virtual Task<bool> CheckAllowUpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        [HttpDelete, Route("")]
        public async Task<IActionResult> Delete(object[] keys, CancellationToken cancellationToken)
        {
            var entity = await DataAccess.FindAsync(keys, cancellationToken);
            if (entity == null)
                return NotFound();

            await DataAccess.DeleteAsync(entity, cancellationToken);

            return Ok();
        }

        protected virtual Task<bool> CheckAllowDeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}
