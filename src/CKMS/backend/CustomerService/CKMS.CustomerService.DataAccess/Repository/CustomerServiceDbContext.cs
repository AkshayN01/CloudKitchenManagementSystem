using CKMS.Contracts.DBModels.AdminUserService;
using CKMS.Contracts.DBModels.CustomerService;
using CKMS.Library.SeedData.AdminUserService;
using CKMS.Library.SeedData.CustomerService;
using Microsoft.EntityFrameworkCore;

namespace CKMS.CustomerService.DataAccess.Repository
{
    public class CustomerServiceDbContext : DbContext
    {
        public CustomerServiceDbContext(DbContextOptions<CustomerServiceDbContext> options) : base(options) { }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresss { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>()
                .HasOne(a => a.Customer)
                .WithMany(a => a.AddressList)
                .HasForeignKey(a => a.CustomerId)
                .IsRequired();

            modelBuilder.Entity<Customer>()
                .HasIndex(a => a.PhoneNumber)
                .IsUnique();

            modelBuilder.Entity<Customer>()
                .HasIndex(a => a.EmailId)
                .IsUnique();
        }
        public async Task SeedTestDataAsync()
        {
            if (!Customers.Any())
            {
                Customers.AddRange(await CustomerSeedData.GetCustomers());
            }
            if (!Addresss.Any())
            {
                Addresss.AddRange(await CustomerSeedData.GetAddresses());
            }
            await SaveChangesAsync();
        }
    }
}
