using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace StorageService.Shared.DAL
{
    public class Repository<T> where T : Base
    {
        private readonly DbContext _dbContext;

        public Repository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected async Task<IEnumerable<T>> GetAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        protected async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().Where(predicate).ToListAsync();
        }

        protected async Task<T?> GetAsync(Guid id)
        {
            return await _dbContext.Set<T>().SingleOrDefaultAsync(s => s.Id == id);
        }

        protected async Task<T> InsertAsync(T entity)
        {
            var addedEntity = (await _dbContext.Set<T>().AddAsync(entity)).Entity;
            await _dbContext.SaveChangesAsync();
            return addedEntity;
        }

        protected async Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        protected async Task DeleteAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Deleted;
            await _dbContext.SaveChangesAsync();
        }
    }
}
