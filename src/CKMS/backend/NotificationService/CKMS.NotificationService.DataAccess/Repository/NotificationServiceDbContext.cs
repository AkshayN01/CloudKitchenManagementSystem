using CKMS.Contracts.DBModels.NotificationService;
using Microsoft.EntityFrameworkCore;

namespace CKMS.NotificationService.DataAccess.Repository
{
    public class NotificationServiceDbContext : DbContext
    {
        public NotificationServiceDbContext(DbContextOptions<NotificationServiceDbContext> options) : base(options) { }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationAudit> NotificationAudit { get; set; }
    }
}
