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
        IOrderItemRepository OrderItemRepository { get; }
        IDicountRepository IDicountRepository { get; }
        IPersonalDiscountRespository PersonalDiscountRespository { get; }
        IDiscountUsageRepository IDiscountUsageRepository { get; }
        IPaymentRepository PaymentRepository { get; }
        IOrderAuditRepository OrderAuditRepository { get; }
    }
    public interface IAdminUserUnitOfWork : IUnitOfWork
    {
        IAdminUserRepository AdminUserRepository { get; }
        IKitchenRepository KitchenRepository { get; }
        IKitchenAuditRespository KitchenAuditRespository { get; }
    }
    public interface IInventoryUnitOfWork : IUnitOfWork
    {
        IInventoryRepository InventoryRepository { get; }
        IInventoryMovementRepository InventoryMovementRepository { get; }
        IMenuItemRepository MenuItemRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IRecipeItemRepository RecipeItemRepository { get; }
        IRecipeRepository RecipeRepository { get; }
        IMenuItemAuditRepository MenuItemAuditRepository { get; }
    }
    public interface ICustomerUnitOfWork : IUnitOfWork
    {
        ICustomerRepository CustomerRepository { get; }
        IAddressRepository AddressRepository { get; }
        ICustomerAuditRepository CustomerAuditRepository { get; }
    }
}
