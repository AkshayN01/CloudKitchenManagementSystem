using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.DTOs.Inventory.Response
{
    public class MenuItemListDTO
    {
        public Int64 TotalCount { get; set; }
        public List<MenuItemDTO> MenuItems { get; set; } = new List<MenuItemDTO>();
    }
    public class MenuItemDTO
    {
        public Int64 MenuItemId { get; set; }
        public Guid KitchenId { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public float Price { get; set; }
        public Int64 CategoryId { get; set; }
        public String CategoryName { get; set; }
        public int IsAvalilable { get; set; }
    }
}
