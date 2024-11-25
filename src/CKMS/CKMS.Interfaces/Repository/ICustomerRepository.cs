using CKMS.Contracts.DBModels.CustomerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Interfaces.Repository
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        Task<Customer?> GetCustomerByUsername(string username); 
    }
    public interface IAddressRepository: IGenericRepository<Address> 
    {
        Task<List<Address>> GetAddressByCustomerId(Guid customerId);
    }
}
