namespace CKMS.Interfaces.Repository
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task AddAsync(TEntity entity);
        Task<TEntity?> GetByIdAsync(int id);
        Task<TEntity?> GetByGuidAsync(Guid id);
        Task<List<TEntity?>> GetAllAsync(bool tracked = false);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task SaveAsync();
    }
}
