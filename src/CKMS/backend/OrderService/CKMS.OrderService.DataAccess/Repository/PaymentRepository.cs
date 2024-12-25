using CKMS.Contracts.DBModels.OrderService;
using CKMS.Interfaces.Repository;
using CKMS.Library.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CKMS.OrderService.DataAccess.Repository
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(OrderServiceDbContext context) : base(context) { }
        public async Task<Payment?> GetPaymentByOrderIdAsync(Guid orderId)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.OrderId == orderId);
        }
    }
}
