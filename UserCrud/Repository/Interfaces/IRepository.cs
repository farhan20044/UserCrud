using System.Linq.Expressions;

namespace UserCrud.Repository.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(string id);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(string id);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<(List<T> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? filter, string? sort);
    }
} 