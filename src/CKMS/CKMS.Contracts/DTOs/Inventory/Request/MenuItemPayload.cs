using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.DTOs.Inventory.Request
{
    public class MenuItemPayload
    {
        [Required]
        public String Name { get; set; } = String.Empty!;
        public String Description { get; set; } = String.Empty;
        public float Price { get; set; }
        public Int64 CategoryId { get; set; }
        public int IsAvalilable { get; set; }
    }
    public class MenuItemUpdatePayload
    {
        [Required]
        public Int64 MenuItemId { get; set; }
        public String Name { get; set; } = String.Empty!;
        public String Description { get; set; } = String.Empty;
        public float Price { get; set; }
        public Int64 CategoryId { get; set; }
        public int IsAvalilable { get; set; }
    }
}
