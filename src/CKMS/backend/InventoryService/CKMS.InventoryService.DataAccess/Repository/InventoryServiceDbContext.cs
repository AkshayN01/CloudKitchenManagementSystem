using CKMS.Contracts.DBModels.InventoryService;
using Microsoft.EntityFrameworkCore;

namespace CKMS.InventoryService.DataAccess.Repository
{
    public class InventoryServiceDbContext : DbContext
    {
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<InventoryMovement> InventoryMovements { get; set; }
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
        }
    }
}
