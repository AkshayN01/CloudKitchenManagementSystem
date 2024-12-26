using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.DBModels.AnalyticsService
{
    public class MenuItemDimension
    {
        [Required]
        public Int64 MenuItemId { get; set; }
        [Required]
        public Guid KitchenId { get; set; } = Guid.Empty!;
        [Required]
        public String Name { get; set; } = String.Empty!;
        public String Description { get; set; } = String.Empty;
        public float Price { get; set; }
        public Int64 CategoryId { get; set; }
        public int IsAvalilable { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public CategoryDimension Category { get; set; } = null!;
    }
    public class CategoryDimension
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 CategoryId { get; set; }
        [Required]
        public Guid KitchenId { get; set; } = Guid.Empty!;
        [Required]
        public String Name { get; set; }
        public String Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<MenuItemDimension> Items { get; set; }
    }
    public class IndeventoryTrends
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }
        [Required]
        public Int64 IndeventoryId { get; set; }
        [Required]
        public Guid KitchenId { get; set; } = Guid.Empty!;
        [Required]
        public String Name { get; set; }
        [Required]
        public Double Quantity { get; set; }
        [Required] 
        public int Unit { get;}
        [Required]
        public DateTime MovementDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
