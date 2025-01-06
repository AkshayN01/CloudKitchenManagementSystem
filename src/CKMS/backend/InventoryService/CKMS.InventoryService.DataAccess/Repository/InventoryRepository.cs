using CKMS.Contracts.DBModels.AdminUserService;
using CKMS.Contracts.DBModels.InventoryService;
using CKMS.Interfaces.Repository;
using CKMS.Library.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.InventoryService.DataAccess.Repository
{
    public class InventoryRepository : GenericRepository<Inventory>, IInventoryRepository
    {
        public InventoryRepository(InventoryServiceDbContext context) : base(context) { }

        public IQueryable<Inventory> GetAllByKitchenId(Guid KitchenId, bool tracking = false)
        {
            IQueryable<Inventory> query = _dbSet;
            if (!tracking)
                query = query.AsNoTracking();
            return query.Where(x => x.KitchenId == KitchenId);
        }
    }
    public class MenuItemRepository : GenericRepository<MenuItem>, IMenuItemRepository
    {
        public MenuItemRepository(InventoryServiceDbContext context) : base(context) { }

        public IQueryable<MenuItem> GetAllByKitchenId(Guid KitchenId, bool tracking = false)
        {
            IQueryable<MenuItem> query = _dbSet;
            if (!tracking)
                query = query.AsNoTracking();
            return query.Where(x => x.KitchenId == KitchenId);
        }
    }
    public class InventoryMovementRepository : GenericRepository<InventoryMovement>, IInventoryMovementRepository
    {
        public InventoryMovementRepository(InventoryServiceDbContext context) : base(context) { }

        public IQueryable<InventoryMovement> GetAllByInventoryId(long InventoryId, bool tracking = false)
        {
            IQueryable<InventoryMovement> query = _dbSet;
            if (!tracking)
                query = query.AsNoTracking();
            return query.Where(x => x.InventoryId == InventoryId);
        }

        public IQueryable<InventoryMovement> GetAllByKitchenId(Guid KitchenId, bool tracking = false)
        {
            IQueryable<InventoryMovement> query = _dbSet;
            if (!tracking)
                query = query.AsNoTracking();
            return query.Where(x => x.KitchenId == KitchenId);
        }
    }
    public class MenuItemAuditRepository : GenericRepository<MenuItemAudit>, IMenuItemAuditRepository
    {
        public MenuItemAuditRepository(InventoryServiceDbContext context) : base(context) { }
    }
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(InventoryServiceDbContext context) : base(context) { }
    }
    public class RecipeRepository : GenericRepository<Recipe>, IRecipeRepository
    {
        public RecipeRepository(InventoryServiceDbContext context) : base(context) { }
    }
    public class RecipeItemRepository : GenericRepository<RecipeItem>, IRecipeItemRepository
    {
        public RecipeItemRepository(InventoryServiceDbContext context) : base(context) { }
    }
}
