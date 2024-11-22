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
        Task<IEnumerable<Inventory>> GetAllByKitchenId(Guid KitchenId);
    }
    public interface IInventoryMovementRepository : IGenericRepository<InventoryMovement> { }
    public interface IMenuItemRepository : IGenericRepository<MenuItem> 
    {
        Task<IQueryable<MenuItem>> GetAllByKitchenId(Guid KitchenId);
    }
    public interface ICategoryRepository : IGenericRepository<Category> { }
    public interface IRecipeRepository : IGenericRepository<Recipe> { }
    public interface IRecipeItemRepository : IGenericRepository<RecipeItem> { }
}
