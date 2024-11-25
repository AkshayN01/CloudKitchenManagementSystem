using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.DTOs.Customer.Request
{
    public class CustomerUpdatePayload
    {
        public String Name { get; set; }
        public String PhoneNumber { get; set; }
        public String EmailId { get; set; }
        public String Password { get; set; }
    }
}
