using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UserCrud.Models;
using UserCrud.Repository.Interfaces;

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

        public virtual async Task<(List<T> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? search, string? sort)
        {
            var query = _dbSet.AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                var stringProperties = typeof(T).GetProperties().Where(p => p.PropertyType == typeof(string));
                foreach (var prop in stringProperties)
                {
                    var param = Expression.Parameter(typeof(T), "x");
                    var propAccess = Expression.Property(param, prop);
                    var searchExpr = Expression.Call(propAccess, typeof(string).GetMethod("Contains", new[] { typeof(string) })!, Expression.Constant(search));
                    var lambda = Expression.Lambda<Func<T, bool>>(searchExpr, param);
                    query = query.Where(lambda);
                }
            }

            // Sorting - Parse single sort parameter
            if (!string.IsNullOrWhiteSpace(sort))
            {
                var sortParts = sort.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (sortParts.Length >= 1)
                {
                    var sortField = sortParts[0];
                    var sortDirection = sortParts.Length > 1 ? sortParts[1].ToLower() : "asc";

                    var prop = typeof(T).GetProperty(sortField, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (prop != null)
                    {
                        query = (sortDirection == "desc")
                            ? query.OrderByDescending(e => EF.Property<object>(e, prop.Name))
                            : query.OrderBy(e => EF.Property<object>(e, prop.Name));
                    }
                }
            }
            else
            {
                // Default sort 
                var firstProp = typeof(T).GetProperties().FirstOrDefault();
                if (firstProp != null)
                {
                    query = query.OrderBy(e => EF.Property<object>(e, firstProp.Name));
                }
            }

            int totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, totalCount);
        }
    }
} 