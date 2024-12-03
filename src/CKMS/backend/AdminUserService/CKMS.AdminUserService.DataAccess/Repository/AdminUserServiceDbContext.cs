using CKMS.Contracts.DBModels.AdminUserService;
using CKMS.Library.SeedData.AdminUserService;
using Microsoft.EntityFrameworkCore;

namespace CKMS.AdminUserService.DataAccess.Repository
{
    public class AdminUserServiceDbContext : DbContext
    {
        public AdminUserServiceDbContext(DbContextOptions<AdminUserServiceDbContext> options) : base(options) { }
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<Kitchen> Kitchens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdminUser>()
                .HasOne(a => a.Kitchen)
                .WithMany(a => a.Users)
                .HasForeignKey(a => a.KitchenId)
                .IsRequired();

            modelBuilder.Entity<AdminUser>()
                .HasIndex(a => a.UserName)
                .IsUnique();

            modelBuilder.Entity<Kitchen>()
                .HasIndex(k => k.EmailId)
                .IsUnique();
        }
        public async Task SeedTestDataAsync()
        {
            if(!Kitchens.Any())
            {
                Kitchens.AddRange(await KitchenSeedData.GetKitchenSeedData());
            }
            if(!AdminUsers.Any())
            {
                AdminUsers.AddRange(await AdminUserSeedData.GetAdminUsers()); 
            }
            await SaveChangesAsync();
        }
    }
}
