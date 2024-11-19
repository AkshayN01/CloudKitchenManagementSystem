using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Interfaces.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> CompleteAsync();
    }

    public interface IOrderUnitOfWork : IUnitOfWork
    {
        IOrderRepository OrderRepository { get; }
        IDicountRepository IDicountRepository { get; }
        IPersonalDiscountRespository PersonalDiscountRespository { get; }
        IDiscountUsageRepository IDiscountUsageRepository { get; }
    }
    public interface IAdminUserUnitOfWork : IUnitOfWork
    {
        IAdminUserRepository AdminUserRepository { get; }
        IKitchenRepository KitchenRepository { get; }
    }
    public interface IInventoryUnitOfWork : IUnitOfWork
    {
        IInventoryRepository InventoryRepository { get; }
        IInventoryMovementRepository InventoryMovementRepository { get; }
        IMenuItemRepository MenuItemRepository { get; }
        IRecipeItemRepository RecipeItemRepository { get; }
        IRecipeRepository RecipeRepository { get; }
    }
}
