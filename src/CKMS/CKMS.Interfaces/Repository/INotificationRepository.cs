using CKMS.Contracts.DBModels.NotificationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Interfaces.Repository
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
    }
    public interface INotificationAuditRepository : IGenericRepository<NotificationAudit> { }
}
