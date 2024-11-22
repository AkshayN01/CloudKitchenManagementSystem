using CKMS.Contracts.DBModels.AdminUserService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Interfaces.Repository
{
    public interface IAdminUserRepository : IGenericRepository<AdminUser>
    {
        IQueryable<AdminUser> GetUsersByRole(int roleId, bool tracking = false);
        IQueryable<AdminUser> GetUsersByKitchen(Guid kitchenId, bool tracking = false);
        Task<AdminUser?> GetUserByUsername(string username);
    }
    public interface IKitchenRepository : IGenericRepository<Kitchen>
    {
    }
}
