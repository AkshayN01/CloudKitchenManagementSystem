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
        Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(Guid customerId);
    }
    public interface IDicountRepository : IGenericRepository<Discount> { }
    public interface IPersonalDiscountRespository : IGenericRepository<PersonalDiscounts> 
    {
        Task<PersonalDiscounts?> GetDiscountByUserIdAndDicountId(Guid userId, Guid dicountId);
    }
    public interface IDiscountUsageRepository : IGenericRepository<DiscountUsage> { }
}
