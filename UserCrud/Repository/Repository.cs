using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UserCrud.Models;
using UserCrud.Repository.Interfaces;
using System.Linq.Dynamic.Core;

namespace UserCrud.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly UserdbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(UserdbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(string id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return entity;
        }

        public virtual async Task<bool> DeleteAsync(string id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
                return false;

            _dbSet.Remove(entity);
            return true;
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<(List<T> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? filter, string? sort)
        {
            var query = _dbSet.AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(filter);
            }

            // Sorting - Use the same dynamic approach as filtering
            if (!string.IsNullOrWhiteSpace(sort))
            {
                query = query.OrderBy(sort);
            }
            
            int totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, totalCount);
        }
    }
} 