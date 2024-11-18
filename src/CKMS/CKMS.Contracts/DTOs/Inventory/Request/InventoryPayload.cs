using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.DTOs.Inventory.Request
{
    public class InventoryPayload
    {
        [Required]
        public String InventoryName { get; set; } = String.Empty!;
        [Required]
        public Guid KitchenId { get; set; } = Guid.Empty!;
        public float Quantity { get; set; }
        public int Unit { get; set; }
        public float RestockThreshold { get; set; }
        public float MaxStockLevel { get; set; }
    }
    public class InventoryMovementPayload
    {
        [Required]
        public Int64 InventoryId { get; set; } //Foreign Key
        public Guid KitchenId { get; set; } = Guid.Empty!; //Required
        public int MovementType { get; set; }
        public float Quantity { get; set; }
        public DateTime MovementDate { get; set; }
    }
}
