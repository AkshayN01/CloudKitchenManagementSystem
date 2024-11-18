using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CKMS.Contracts.DBModels.InventoryService
{
    public class Recipe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }
        [Required]
        public Int64 MenuItemId { get; set; } //Foreign Key
        public float Quantity { get; set; }
        public int Unit { get; set; }
        public MenuItem MenuItem { get; set; } = null!; //Required
        public ICollection<RecipeItem> Items { get; set; } = new List<RecipeItem>();
    }

    public class RecipeItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }
        [Required]
        public Int64 RecipeId { get; set; } //foreign key
        [Required]
        public Int64 InventoryId { get; set; } //foreign key
        public float Quantity { get; set; }
        public int Unit { get; set; }

        public Recipe Recipe { get; set; } = null!; //Required
        public Inventory Inventory { get; set; } = null!; //Required
    }
}
