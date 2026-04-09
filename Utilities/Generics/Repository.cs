using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Utilities.Generics
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _set;

        protected Repository(DbContext context)
        {
            _context = context;
            _set = _context.Set<T>();
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>>? expression = null) => await _set.AnyAsync(expression ?? (x => true));

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? expression = null) => await _set.CountAsync(expression ?? (x => true));

        public virtual async Task CreateAsync(T entity) => await _set.AddAsync(entity);

        public virtual async Task CreateManyAsync(IEnumerable<T> entities) => await _set.AddRangeAsync(entities);

        public virtual async Task DeleteAsync(T entity) => await Task.Run(() => _set.Remove(entity));
        public virtual async Task DeleteManyAsync(IEnumerable<T> entities) => await Task.Run(() => _set.RemoveRange(entities));
        public virtual async Task DeleteManyAsync(Expression<Func<T, bool>>? expression = null)
        {
            var entities = _set.Where(expression ?? (x => true));
            await DeleteManyAsync(entities);
        }

        public virtual async Task<T?> FindFirstAsync(Expression<Func<T, bool>>? expression = null) => await _set.FirstOrDefaultAsync(expression ?? (x => true));

        public virtual async Task<T?> ReadByKeyAsync(object entityKey) => await _set.FindAsync(entityKey);

        public virtual async Task<IEnumerable<T>> ReadManyAsync(Expression<Func<T, bool>>? expression = null, params string[] includes)
        {
            var entities = _set.Where (expression ?? (x => true));
            foreach (var include in includes)
            {
                entities = entities.Include(include);
            }
            return await entities.ToListAsync();
        }

        public virtual async Task UpdateAsync(T entity) => await Task.Run(() => _set.Update(entity));

        public virtual async Task UpdateManyAsync(IEnumerable<T> entities) => await Task.Run(() => _set.UpdateRange(entities));
        public virtual async Task UpdateManyAsync(Expression<Func<T, bool>>? expression = null)
        {
            var entities = _set.Where(expression ??  (_x => true));
            await UpdateManyAsync(entities);
        }
    }
}
