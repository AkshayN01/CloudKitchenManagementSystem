using CKMS.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.AdminUserService.DataAccess.Repository
{
    public class AdminUserUnitOfWork : IAdminUserUnitOfWork
    {
        private readonly AdminUserServiceDbContext _dbContext;

        private IKitchenAuditRespository? _KitchenAuditRespository;
        private IAdminUserRepository? _AdminUserRepository;
        private IKitchenRepository? _KitchenRepository;
        public AdminUserUnitOfWork(AdminUserServiceDbContext dbContext)  
        {
            _dbContext = dbContext;
        }
        public IAdminUserRepository AdminUserRepository => _AdminUserRepository ??= new AdminUserRepository(_dbContext);

        public IKitchenRepository KitchenRepository => _KitchenRepository ??= new KitchenRepository(_dbContext);

        public IKitchenAuditRespository KitchenAuditRespository => _KitchenAuditRespository ??= new KitchenAuditRepository(_dbContext);

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
