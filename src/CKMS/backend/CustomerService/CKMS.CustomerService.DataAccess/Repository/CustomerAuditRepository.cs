using CKMS.Contracts.DBModels.CustomerService;
using CKMS.Interfaces.Repository;
using CKMS.Library.Interfaces;

namespace CKMS.CustomerService.DataAccess.Repository
{
    public class CustomerAuditRepository : GenericRepository<CustomerAudit>, ICustomerAuditRepository
    {
        public CustomerAuditRepository(CustomerServiceDbContext context) : base(context) { }
    }
}
