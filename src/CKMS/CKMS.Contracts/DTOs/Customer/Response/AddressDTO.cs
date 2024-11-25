using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.DTOs.Customer.Response
{
    public class AddressDTO
    {
        public Guid AddressId { get; set; }
        public String AddressDetail { get; set; }
        public String City { get; set; }
        public String Region { get; set; }
        public String PostalCode { get; set; }
        public String Country { get; set; }
    }
}
