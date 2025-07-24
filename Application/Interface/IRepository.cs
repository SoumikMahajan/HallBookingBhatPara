using System.Linq.Expressions;

namespace HallBookingBhatPara.Application.Interface
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, bool tracked = true);
        Task<T> GetAsync(Expression<Func<T, bool>> filter, bool tracked = true, string? includeProperties = null);
        Task AddAsync(T entity);
        Task RemoveAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task RemoveRangeAsync(IEnumerable<T> entities);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        Task SaveAsync();
    }
}
