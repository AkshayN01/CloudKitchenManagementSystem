using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.DTOs.AdminUser.Request
{
    public class KitchenPayload
    {
        [Required]
        public string KitchenName { get; set; } = string.Empty!;
        [Required]
        public string Address { get; set; } = string.Empty!;
        [Required]
        public string City { get; set; } = string.Empty!;
        [Required]
        public string Region { get; set; } = string.Empty!;
        [Required]
        public string PostalCode { get; set; } = string.Empty!;
        [Required]
        public string Country { get; set; } = string.Empty!;
    }
    public class KitchenUpdatePayload
    {
        [Required]
        public String KitchenId { get; set; } = String.Empty!;
        public string KitchenName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }
}
