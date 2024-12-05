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
    public class KitchenRepository : GenericRepository<Kitchen>, IKitchenRepository
    {
        public KitchenRepository(AdminUserServiceDbContext dbContext): base(dbContext) { }
        public IQueryable<Kitchen> GetAllKitchen(bool tracking = false)
        {
            IQueryable<Kitchen> query = _dbSet;
            if (!tracking)
                query.AsNoTracking();

            return query.Where(x => x.IsActive == 1);
        }
    }
}
