using CKMS.Contracts.DBModels.OrderService;
using Microsoft.EntityFrameworkCore;

namespace CKMS.OrderService.DataAccess.Repository
{
    public class OrderServiceDbContext : DbContext
    {
        public OrderServiceDbContext(DbContextOptions<OrderServiceDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItem>()
                .HasOne(o => o.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(o => o.OrderId)
                .IsRequired();

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithOne(p => p.Payment)
                .HasForeignKey<Payment>(p => p.OrderId)
                .IsRequired();

            modelBuilder.Entity<PersonalDiscounts>()
                .HasOne(p => p.Discount)
                .WithMany(p => p.PersonalDiscounts)
                .HasForeignKey(p => p.DiscountId)
                .IsRequired();

            modelBuilder.Entity<DiscountUsage>()
                .HasOne(p => p.Discount)
                .WithMany(p => p.DiscountUsages)
                .HasForeignKey(p => p.DiscountId)
                .IsRequired();
        }
    }
}
