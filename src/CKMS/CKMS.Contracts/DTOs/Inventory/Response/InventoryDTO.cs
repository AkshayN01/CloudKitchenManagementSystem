using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.DTOs.Inventory.Response
{
    public class InventoryListDTO
    {
        public Int64 TotalCount { get; set; }
        public List<InventoryDTO> Inventories { get; set; } = new List<InventoryDTO>();
    }
    public class InventoryDTO
    {
        public Int64 InventoryId { get; set; }
        public String InventoryName { get; set; } = String.Empty!;
        public Guid KitchenId { get; set; } = Guid.Empty!;
        public float Quantity { get; set; }
        public String Unit { get; set; }
        public float RestockThreshold { get; set; }
        public float MaxStockLevel { get; set; }
    }
    public class InventoryMovementListDTO
    {
        public Int64 TotalCount { get; set; }
        public List<InventoryMovementDTO> InventoryMovements { get; set; } = new List<InventoryMovementDTO>();
    }
    public class InventoryMovementDTO
    {
        public Int64 Id { get; set; }
        public Int64 InventoryId { get; set; } 
        public Guid KitchenId { get; set; }
        public int MovementType { get; set; }
        public float Quantity { get; set; }
        public DateTime MovementDate { get; set; }
    }
}
