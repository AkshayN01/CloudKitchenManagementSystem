using CKMS.Interfaces.Repository;

namespace CKMS.NotificationService.DataAccess.Repository
{
    public class NotificationUnitOfWork : INotificationUnitOfWork
    {
        private readonly NotificationServiceDbContext _dbContext;
        private INotificationRepository? _notificationRepository;
        private INotificationAuditRepository? _notificationAuditRepository;
        public NotificationUnitOfWork(NotificationServiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public INotificationRepository NotificationRepository => _notificationRepository ??= new NotificationRepository(_dbContext);

        public INotificationAuditRepository NotificationAuditRepository => _notificationAuditRepository ??= new NotificationAuditRepository(_dbContext);

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
