using CKMS.Interfaces.Repository;

namespace CKMS.OrderService.DataAccess.Repository
{
    public class OrderUnitOfWork : IOrderUnitOfWork
    {
        private readonly OrderServiceDbContext _dbContext;

        private IOrderRepository? _orderRepository;
        private IOrderItemRepository? _orderItemRepository;
        private IDicountRepository? _dicountRepository;
        private IPersonalDiscountRespository? _personalDiscountRespository;
        private IDiscountUsageRepository? _discountUsageRepository;
        private IPaymentRepository? _paymentRepository;
        private IOrderAuditRepository? _orderAuditRepository;
        public OrderUnitOfWork(OrderServiceDbContext orderServiceDbContext) 
        {
            _dbContext = orderServiceDbContext;
        }
        public IOrderRepository OrderRepository => _orderRepository ??= new OrderRepository(_dbContext);

        public IOrderItemRepository OrderItemRepository => _orderItemRepository ??= new OrderItemRepository(_dbContext);

        public IDicountRepository IDicountRepository => _dicountRepository ??= new DiscountRepository(_dbContext);

        public IPersonalDiscountRespository PersonalDiscountRespository => _personalDiscountRespository ??= new PersonalDiscountRepository(_dbContext);

        public IDiscountUsageRepository IDiscountUsageRepository => _discountUsageRepository ??= new DiscountUsageRepository(_dbContext);

        public IPaymentRepository PaymentRepository => _paymentRepository ??= new PaymentRepository(_dbContext);

        public IOrderAuditRepository OrderAuditRepository => throw new NotImplementedException();

        public async Task CompleteAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
