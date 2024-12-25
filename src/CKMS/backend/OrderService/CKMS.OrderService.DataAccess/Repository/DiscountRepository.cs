using CKMS.Contracts.DBModels.OrderService;
using CKMS.Interfaces.Repository;
using CKMS.Library.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CKMS.OrderService.DataAccess.Repository
{
    public class DiscountRepository : GenericRepository<Discount>, IDicountRepository
    {
        public DiscountRepository(OrderServiceDbContext context) : base(context) { }
        public async Task<Discount?> GetDiscountByCouponCodeAsync(string couponCode)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.CouponCode == couponCode);
        }
    }
    public class PersonalDiscountRepository : GenericRepository<PersonalDiscounts>, IPersonalDiscountRespository
    {
        public PersonalDiscountRepository(OrderServiceDbContext context) : base(context) { }
        public async Task<PersonalDiscounts?> GetDiscountByUserIdAndDicountId(Guid userId, Guid dicountId)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.UserId == userId && x.DiscountId == dicountId);
        }
    }
    public class DiscountUsageRepository : GenericRepository<DiscountUsage>, IDiscountUsageRepository
    {
        public DiscountUsageRepository(OrderServiceDbContext context) : base(context) { }
        public async Task<DiscountUsage?> GetDiscountUsageByOrderIdAsync(Guid orderId)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.OrderId == orderId);
        }

        public async Task<IEnumerable<DiscountUsage>> GetUsageByUserIdAndDiscountId(Guid userId, Guid dicountId)
        {
            return await _dbSet.Where(x => x.UserId == userId && x.DiscountId == dicountId).ToListAsync();
        }
    }
}
