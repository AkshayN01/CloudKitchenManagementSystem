using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.DBModels.InventoryService
{
    public class MenuItem
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        public Category Category { get; set; } = null!;
    }
    public class Category
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 CategoryId { get; set;}
        [Required]
        public Guid KitchenId { get; set;} = Guid.Empty!;
        [Required]
        public String Name { get; set; }
        public String Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<MenuItem> Items { get; set; }
    }
}
