using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.DTOs.Customer.Request
{
    public class AddressPayload
    {
        [Required]
        public String AddressDetail { get; set; } = String.Empty!;
        [Required]
        public String City { get; set; } = String.Empty!;
        [Required]
        public String Region { get; set; } = String.Empty;
        [Required]
        public String PostalCode { get; set; } = String.Empty!;
        [Required]
        public String Country { get; set; } = String.Empty!;
    }
    public class AddressUpdatePayload
    {
        [Required]
        public String AddressId { get; set; } = String.Empty!;
        public String AddressDetail { get; set; }
        public String City { get; set; }
        public String Region { get; set; }
        public String PostalCode { get; set; }
        public String Country { get; set; }
    }
}
