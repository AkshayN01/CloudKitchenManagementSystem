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
        public float Quantity { get; set; }
        public int Unit { get; set; }
        public float RestockThreshold { get; set; }
        public float MaxStockLevel { get; set; }
    }
    public class InventoryUpdatePayload
    {
        [Required]
        public Int64 InventoryId { get; set; }
        public String InventoryName { get; set; }
        public float Quantity { get; set; }
        public int Unit { get; set; }
        public float RestockThreshold { get; set; }
        public float MaxStockLevel { get; set; }
    }
    public class InventoryMovementPayload
    {
        [Required]
        public Int64 InventoryId { get; set; } //Foreign Key
        public int MovementType { get; set; }
        public float Quantity { get; set; }
        public DateTime MovementDate { get; set; }
    }
    public class InventoryMovementUpdatePayload
    {
        [Required]
        public Int64 InventoryMovementId { get; set; }
        public Int64 InventoryId { get; set; }
        public int MovementType { get; set; }
        public float Quantity { get; set; }
        public DateTime MovementDate { get; set; }
    }
}
