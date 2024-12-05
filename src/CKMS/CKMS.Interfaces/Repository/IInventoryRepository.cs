using CKMS.Contracts.DBModels.InventoryService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Interfaces.Repository
{
    public interface IInventoryRepository : IGenericRepository<Inventory> 
    {
        IQueryable<Inventory> GetAllByKitchenId(Guid KitchenId, bool tracking = false);
    }
    public interface IInventoryMovementRepository : IGenericRepository<InventoryMovement> { }
    public interface IMenuItemRepository : IGenericRepository<MenuItem> 
    {
        IQueryable<MenuItem> GetAllByKitchenId(Guid KitchenId, bool tracking = false);
    }
    public interface ICategoryRepository : IGenericRepository<Category> { }
    public interface IRecipeRepository : IGenericRepository<Recipe> { }
    public interface IRecipeItemRepository : IGenericRepository<RecipeItem> { }
    public interface IMenuItemAuditRepository : IGenericRepository<MenuItemAudit> { }
}
