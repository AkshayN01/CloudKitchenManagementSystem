using CKMS.Contracts.DBModels.NotificationService;
using Microsoft.EntityFrameworkCore;

namespace CKMS.NotificationService.DataAccess.Repository
{
    public class NotificationServiceDbContext : DbContext
    {
        public DbSet<Notification> Notifications { get; set; }
    }
}
