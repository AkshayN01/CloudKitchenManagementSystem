using CKMS.Contracts.DBModels.AdminUserService;
using CKMS.Interfaces.Repository;
using CKMS.Library.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.AdminUserService.DataAccess.Repository
{
    public class AdminUserRepository : GenericRepository<AdminUser>, IAdminUserRepository
    {
        public AdminUserRepository(AdminUserServiceDbContext dbContext) : base(dbContext) { }
        public Task<AdminUser?> GetUserByUsername(string username)
        {
            return _dbSet.FirstOrDefaultAsync(x => x.UserName == username);
        }

        public IQueryable<AdminUser> GetUsersByKitchen(Guid kitchenId, bool tracking = false)
        {
            IQueryable<AdminUser> query = _dbSet;
            if (!tracking)
                query.AsNoTracking();
            return query.Where(x => x.KitchenId == kitchenId);
        }

        public IQueryable<AdminUser> GetUsersByRole(int roleId, Guid KitchenId, bool tracking = false)
        {
            IQueryable<AdminUser> query = _dbSet;
            if (!tracking)
                query.AsNoTracking();
            return query.Where(x => x.RoleId == roleId && x.KitchenId == KitchenId);
        }
    }
}
