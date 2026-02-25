using Microsoft.EntityFrameworkCore;
using CustomerFluent.Data;
using System.Linq.Expressions;

namespace CustomerFluent.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;
        protected readonly ILogger<Repository<T>> _logger;

        public Repository(AppDbContext context, ILogger<Repository<T>> logger)
        {
            _context = context;
            _dbSet = context.Set<T>();
            _logger = logger;
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogDebug("Getting entity {EntityType} with ID: {Id}", typeof(T).Name, id);
                return await _dbSet.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting {EntityType} with ID: {Id}", typeof(T).Name, id);
                throw;
            }
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                _logger.LogDebug("Getting all entities of type {EntityType}", typeof(T).Name);
                return await _dbSet.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all entities of type {EntityType}", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                _logger.LogDebug("Finding entities of type {EntityType} with predicate", typeof(T).Name);
                return await _dbSet.Where(predicate).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding entities of type {EntityType}", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                _logger.LogDebug("Getting first entity of type {EntityType} with predicate", typeof(T).Name);
                return await _dbSet.FirstOrDefaultAsync(predicate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting first entity of type {EntityType}", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await _dbSet.AnyAsync(predicate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking existence of {EntityType}", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task<int> CountAsync()
        {
            try
            {
                return await _dbSet.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting entities of type {EntityType}", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await _dbSet.CountAsync(predicate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting entities of type {EntityType} with predicate", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            try
            {
                _logger.LogDebug("Adding new entity of type {EntityType}", typeof(T).Name);
                var result = await _dbSet.AddAsync(entity);
                return result.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding entity of type {EntityType}", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            try
            {
                _logger.LogDebug("Adding {Count} entities of type {EntityType}", entities.Count(), typeof(T).Name);
                await _dbSet.AddRangeAsync(entities);
                return entities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding multiple entities of type {EntityType}", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task UpdateAsync(T entity)
        {
            try
            {
                _logger.LogDebug("Updating entity of type {EntityType}", typeof(T).Name);
                _dbSet.Update(entity);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating entity of type {EntityType}", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task UpdateRangeAsync(IEnumerable<T> entities)
        {
            try
            {
                _logger.LogDebug("Updating {Count} entities of type {EntityType}", entities.Count(), typeof(T).Name);
                _dbSet.UpdateRange(entities);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating multiple entities of type {EntityType}", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task DeleteAsync(T entity)
        {
            try
            {
                _logger.LogDebug("Deleting entity of type {EntityType}", typeof(T).Name);
                _dbSet.Remove(entity);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting entity of type {EntityType}", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task DeleteAsync(int id)
        {
            try
            {
                _logger.LogDebug("Deleting entity {EntityType} with ID: {Id}", typeof(T).Name, id);
                var entity = await GetByIdAsync(id);
                if (entity != null)
                {
                    await DeleteAsync(entity);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting entity {EntityType} with ID: {Id}", typeof(T).Name, id);
                throw;
            }
        }

        public virtual async Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            try
            {
                _logger.LogDebug("Deleting {Count} entities of type {EntityType}", entities.Count(), typeof(T).Name);
                _dbSet.RemoveRange(entities);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting multiple entities of type {EntityType}", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task<bool> ExistsAsync(int id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                return entity != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking existence of {EntityType} with ID: {Id}", typeof(T).Name, id);
                throw;
            }
        }
    }
}