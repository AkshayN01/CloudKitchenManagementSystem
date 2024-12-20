using CKMS.Contracts.DBModels;
using CKMS.Contracts.DBModels.InventoryService;
using CKMS.Contracts.DBModels.OrderService;
using CKMS.Library.SeedData.InventoryService;
using CKMS.Library.SeedData.OrderService;
using Microsoft.EntityFrameworkCore;

namespace CKMS.OrderService.DataAccess.Repository
{
    public class OrderServiceDbContext : DbContext
    {
        public OrderServiceDbContext(DbContextOptions<OrderServiceDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<PersonalDiscounts> PersonalDiscounts { get; set; }
        public DbSet<DiscountUsage> DiscountUsages { get; set; }
        public DbSet<AuditTable> Audit {  get; set; }
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

            modelBuilder.Entity<DiscountUsage>()
                .HasOne(p => p.Order)
                .WithOne(p => p.DiscountUsage)
                .HasForeignKey<DiscountUsage>(p => p.OrderId)
                .IsRequired();

            modelBuilder.Entity<Discount>()
                .HasIndex(x => x.CouponCode)
                .IsUnique();

            modelBuilder.Entity<Payment>()
                .HasIndex(x => x.OrderId);
        }
        public async Task SeedTestDataAsync()
        {
            if(!Discounts.Any())
                Discounts.AddRange(await DiscountSeedData.GetDiscounts());

            if (!Orders.Any())
                Orders.AddRange(await OrderSeedData.GetOrders());
            
            if (!OrderItems.Any())
                OrderItems.AddRange(await OrderSeedData.GetOrderItems());
            
            if (!DiscountUsages.Any())
                DiscountUsages.AddRange(await OrderSeedData.GetDiscountUsages());

            if (!Payments.Any())
                Payments.AddRange(await OrderSeedData.GetPayments());

            await SaveChangesAsync();
        }
    }
}
