using CKMS.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace CKMS.Library.Interfaces
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public GenericRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<TEntity?> GetByIdAsync(Int64 id) => await _dbSet.FindAsync(id);

        public async Task<TEntity?> GetByGuidAsync(Guid id) => await _dbSet.FindAsync(id);

        public async Task<List<TEntity>> GetAllAsync(bool tracked = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (!tracked)
            {
                query = query.AsNoTracking();
            }

            return await query.ToListAsync();
        }

        public async Task AddAsync(TEntity entity) => await _dbSet.AddAsync(entity);

        public void Update(TEntity entity) => _dbSet.Update(entity);

        public void Delete(TEntity entity) => _dbSet.Remove(entity);

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<TEntity> AddAndReturnEntity(TEntity entity)
        {
           _context.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
