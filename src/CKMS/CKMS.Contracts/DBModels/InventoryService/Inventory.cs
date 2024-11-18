using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.DBModels.InventoryService
{
    public class Inventory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 InventoryId { get; set; }
        [Required]
        public String InventoryName { get; set; } = String.Empty!;
        [Required]
        public Guid KitchenId { get; set; } = Guid.Empty!;
        public float Quantity { get; set; }
        public int Unit { get; set; }
        public float RestockThreshold { get; set; }
        public float MaxStockLevel { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public ICollection<InventoryMovement> Movements { get; set; } = new List<InventoryMovement>();
    }

    public class InventoryMovement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }
        [Required]
        public Int64 InventoryId { get; set; } //Foreign Key
        public Guid KitchenId { get; set; } = Guid.Empty!; //Required
        public int MovementType { get; set; }
        public float Quantity { get; set; }
        public DateTime MovementDate { get; set; }
        public Inventory Inventory { get; set; } = null!; //Required
        public DateTime CreatedAt { get; set; }
    }
    public enum Unit
    {
        grams = 0,
        kilograms = 1,
        litres = 2,
        millilitres = 3,
        dozen = 4
    }
}
