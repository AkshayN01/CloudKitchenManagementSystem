using CKMS.Contracts.DBModels.AdminUserService;
using CKMS.Contracts.DBModels.InventoryService;
using CKMS.Contracts.DBModels.OrderService;
using CKMS.Interfaces.Repository;
using CKMS.Library.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.OrderService.DataAccess.Repository
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(OrderServiceDbContext dbContext): base(dbContext) { }
        public IQueryable<Order> GetOrdersByCustomerIdAsync(Guid customerId, bool tracking = false)
        {
            IQueryable<Order> query = _dbSet;
            if (!tracking)
                query.AsNoTracking();
            return query.Where(x => x.CustomerId == customerId).OrderByDescending(x => x.OrderDate);
        }

        public IQueryable<Order> GetOrdersByKitchenIdAsync(Guid kitchenId, bool tracking = false)
        {
            IQueryable<Order> query = _dbSet;
            if (!tracking)
                query.AsNoTracking();
            return query.Where(x => x.KitchenId == kitchenId).OrderByDescending(x => x.OrderDate);
        }
    }

    public class OrderItemRepository : GenericRepository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(OrderServiceDbContext context) : base(context){}

        public async Task DeleteByMenuItemId(long MenuItemId)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(x => x.MenuItemId == MenuItemId);
            if(entity != null)
                _dbSet.Remove(entity);
        }

        public async Task<List<OrderItem>> GetOrdersByOrderIdAsync(Guid orderId)
        {
            return await _dbSet.Where(x => x.OrderId == orderId).ToListAsync();
        }

        public async Task<long> GetOrdersCountByOrderIdAsync(Guid orderId)
        {
            return await _dbSet.CountAsync(x => x.OrderId == orderId);
        }
    }
}
