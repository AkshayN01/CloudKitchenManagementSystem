using CKMS.Contracts.DBModels.CustomerService;
using CKMS.Interfaces.Repository;
using CKMS.Library.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CKMS.CustomerService.DataAccess.Repository
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(CustomerServiceDbContext context) : base(context) { }

        public async Task<Customer?> GetCustomerByUsername(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.UserName.ToLower() == username.ToLower());
        }
    }
}
