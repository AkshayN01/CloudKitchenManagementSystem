using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.DBModels.InventoryService
{
    public class MenuItem
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
    }
}
