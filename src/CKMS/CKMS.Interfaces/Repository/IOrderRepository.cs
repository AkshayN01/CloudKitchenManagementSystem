using CKMS.Contracts.DBModels;
using CKMS.Contracts.DBModels.OrderService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Interfaces.Repository
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        IQueryable<Order> GetOrdersByCustomerIdAsync(Guid customerId, bool includeItems = false, bool includeDiscountUsage = false, bool tracking = false);
        IQueryable<Order> GetOrdersByKitchenIdAsync(Guid kitchenId, bool includeItems = false, bool includeDiscountUsage = false, bool tracking = false);
    }
    public interface IOrderItemRepository : IGenericRepository<OrderItem>
    {
        Task<List<OrderItem>> GetOrdersByOrderIdAsync(Guid orderId);
        Task<Int64> GetOrdersCountByOrderIdAsync(Guid orderId);
        Task DeleteByMenuItemId(Int64 MenuItemId);
    }
    public interface IDicountRepository : IGenericRepository<Discount> 
    {
        Task<Discount?> GetDiscountByCouponCodeAsync(String couponCode);
    }
    public interface IPersonalDiscountRespository : IGenericRepository<PersonalDiscounts> 
    {
        Task<PersonalDiscounts?> GetDiscountByUserIdAndDicountId(Guid userId, Guid dicountId);
    }
    public interface IDiscountUsageRepository : IGenericRepository<DiscountUsage> 
    {
        Task<DiscountUsage?> GetDiscountUsageByOrderIdAsync(Guid orderId);
        Task<IEnumerable<DiscountUsage>> GetUsageByUserIdAndDiscountId(Guid userId, Guid dicountId);
    }
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task<Payment?> GetPaymentByOrderIdAsync(Guid orderId);
    }
    public interface IOrderAuditRepository: IGenericRepository<AuditTable> { }
}
