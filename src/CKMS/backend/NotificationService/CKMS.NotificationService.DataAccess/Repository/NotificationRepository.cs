using CKMS.Contracts.DBModels.NotificationService;
using CKMS.Interfaces.Repository;
using CKMS.Library.Interfaces;

namespace CKMS.NotificationService.DataAccess.Repository
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(NotificationServiceDbContext context) : base(context)
        {
        }
    }
    public class NotificationAuditRepository : GenericRepository<NotificationAudit>, INotificationAuditRepository
    {
        public NotificationAuditRepository(NotificationServiceDbContext context) : base(context)
        {
        }
    }
}
