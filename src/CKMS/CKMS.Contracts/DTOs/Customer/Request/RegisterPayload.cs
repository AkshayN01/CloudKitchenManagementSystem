using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.DTOs.Customer.Request
{
    public class RegisterPayload
    {
        [Required]
        public String Name { get; set; } = String.Empty!;
        [Required]
        public String PhoneNumber { get; set; } = String.Empty!;
        public String UserName { get; set; } //username will be emailId
        [Required]
        public String EmailId { get; set; } = String.Empty!;
        [Required] 
        public String Password { get; set; } = String.Empty!;
    }
}
