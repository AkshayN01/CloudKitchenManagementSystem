using CKMS.Contracts.DBModels.CustomerService;
using Microsoft.EntityFrameworkCore;

namespace CKMS.CustomerService.DataAccess.Repository
{
    public class CustomerServiceDbContext : DbContext
    {
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
    }
}
