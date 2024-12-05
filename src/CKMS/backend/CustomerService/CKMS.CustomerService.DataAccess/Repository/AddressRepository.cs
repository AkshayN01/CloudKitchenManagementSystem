using CKMS.Contracts.DBModels.CustomerService;
using CKMS.Interfaces.Repository;
using CKMS.Library.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CKMS.CustomerService.DataAccess.Repository
{
    public class AddressRepository : GenericRepository<Address>, IAddressRepository
    {
        public AddressRepository(CustomerServiceDbContext context) : base(context) { }

        public async Task<List<Address>> GetAddressByCustomerId(Guid customerId)
        {
            return await _dbSet.Where(x => x.CustomerId == customerId).ToListAsync();
        }
    }
}
