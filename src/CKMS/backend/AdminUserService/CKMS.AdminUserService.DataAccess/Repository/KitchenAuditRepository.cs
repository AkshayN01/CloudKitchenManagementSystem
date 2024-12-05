using CKMS.Contracts.DBModels.AdminUserService;
using CKMS.Interfaces.Repository;
using CKMS.Library.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CKMS.AdminUserService.DataAccess.Repository
{
    public class KitchenAuditRepository : GenericRepository<KitchenAudit>, IKitchenAuditRespository
    {
        public KitchenAuditRepository(AdminUserServiceDbContext dbContext) : base(dbContext) { }
    }
}
