using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.DTOs.Customer.Response
{
    public class CustomerDTO
    {
        public Guid CustomerId { get; set; }
        public String Name { get; set; }
        public String PhoneNumber { get; set; }
        public String UserName { get; set; }
        public String EmailId { get; set; }
        public Int64 LoyaltyPoints { get; set; }
        public Int64 TotalOrder { get; set; }
    }
}
