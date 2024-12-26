using CKMS.Interfaces.Repository;

namespace CKMS.InventoryService.DataAccess.Repository
{
    public class InventoryUnitOfWork : IInventoryUnitOfWork
    {
        private readonly InventoryServiceDbContext _dbContext;
        private InventoryRepository? _inventoryRepository;
        private IInventoryMovementRepository? _inventoryMovementRepository;
        private IMenuItemRepository? _menuItemRepository;
        private ICategoryRepository? _categoryRepository;
        private IRecipeItemRepository? _recipeItemRepository;
        private IRecipeRepository? _recipeRepository;
        public InventoryUnitOfWork(InventoryServiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IInventoryRepository InventoryRepository => _inventoryRepository ??= new InventoryRepository(_dbContext);

        public IInventoryMovementRepository InventoryMovementRepository => _inventoryMovementRepository ??= new InventoryMovementRepository(_dbContext);

        public IMenuItemRepository MenuItemRepository => _menuItemRepository ??= new MenuItemRepository(_dbContext);

        public ICategoryRepository CategoryRepository => _categoryRepository ??= new CategoryRepository(_dbContext);

        public IRecipeItemRepository RecipeItemRepository => _recipeItemRepository ?? new RecipeItemRepository(_dbContext);

        public IRecipeRepository RecipeRepository => _recipeRepository ??= new RecipeRepository(_dbContext);

        public IMenuItemAuditRepository MenuItemAuditRepository => throw new NotImplementedException();

        public async Task CompleteAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
