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
        Task<IEnumerable<AdminUser>> GetUsersByRole(int roleId);
    }
}
