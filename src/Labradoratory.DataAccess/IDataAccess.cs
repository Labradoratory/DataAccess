using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Labradoratory.DataAccess
{
    public interface IDataAccess<T>
    {
        IQueryable<T> Get();
        Task<T> FindAsync(object[] keys, CancellationToken cancellationToken);
        
        Task AddAsync(T entity, CancellationToken cancellationToken);
        Task UpdateAsync(T entity, CancellationToken cancellationToken);
        Task DeleteAsync(T entity, CancellationToken cancellationToken);
    }
}
