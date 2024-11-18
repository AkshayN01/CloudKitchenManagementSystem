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
        public List<InventoryDTO> Inventories { get; set;}
    }
    public class InventoryDTO
    {
        public Int64 InventoryId { get; set; }
        public String InventoryName { get; set; } = String.Empty!;
        public Guid KitchenId { get; set; } = Guid.Empty!;
        public float Quantity { get; set; }
        public int Unit { get; set; }
        public float RestockThreshold { get; set; }
        public float MaxStockLevel { get; set; }
    }
}
