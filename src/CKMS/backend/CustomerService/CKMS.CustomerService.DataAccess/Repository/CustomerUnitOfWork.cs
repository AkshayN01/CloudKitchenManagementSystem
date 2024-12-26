using CKMS.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.CustomerService.DataAccess.Repository
{
    public class CustomerUnitOfWork : ICustomerUnitOfWork
    {
        private readonly CustomerServiceDbContext _dbContext;
        private ICustomerRepository? _customerRepository;
        private IAddressRepository? _addressRepository;
        public CustomerUnitOfWork(CustomerServiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ICustomerRepository CustomerRepository => _customerRepository ??= new CustomerRepository(_dbContext);

        public IAddressRepository AddressRepository => _addressRepository ??= new AddressRepository(_dbContext);

        public ICustomerAuditRepository CustomerAuditRepository => throw new NotImplementedException();

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
