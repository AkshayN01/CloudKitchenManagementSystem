using CKMS.Contracts.DBModels.CustomerService;
using CKMS.Contracts.DBModels.InventoryService;
using CKMS.Library.SeedData.CustomerService;
using CKMS.Library.SeedData.InventoryService;
using Microsoft.EntityFrameworkCore;

namespace CKMS.InventoryService.DataAccess.Repository
{
    public class InventoryServiceDbContext : DbContext
    {
        public InventoryServiceDbContext(DbContextOptions<InventoryServiceDbContext> options) : base(options) { }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<InventoryMovement> InventoryMovements { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeItem> RecipeItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InventoryMovement>()
                .HasOne(i => i.Inventory)
                .WithMany(i => i.Movements)
                .HasForeignKey(i => i.InventoryId)
                .IsRequired();

            modelBuilder.Entity<RecipeItem>()
                .HasOne(i => i.Recipe)
                .WithMany(i => i.Items)
                .HasForeignKey(i => i.RecipeId)
                .IsRequired();

            modelBuilder.Entity<MenuItem>()
                .HasOne(m => m.Category)
                .WithMany(m => m.Items)
                .HasForeignKey(m => m.CategoryId)
                .IsRequired();
        }
        public async Task SeedTestDataAsync()
        {
            if (!Inventories.Any())
                Inventories.AddRange(await InventorySeedData.GetInventories());
            
            if (!InventoryMovements.Any())
                InventoryMovements.AddRange(await InventorySeedData.GetInventoryMovements());

            if (!Categories.Any())
                Categories.AddRange(await MenuItemSeedData.GetCategories());

            if (!MenuItems.Any())
                MenuItems.AddRange(await MenuItemSeedData.GetMenuItems());

            await SaveChangesAsync();
        }
    }
}
