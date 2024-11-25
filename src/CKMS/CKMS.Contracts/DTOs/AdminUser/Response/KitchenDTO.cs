using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.DTOs.AdminUser.Response
{
    public class KitchenListDTO
    {
        public Int64 TotalCount { get; set; }
        public List<KitchenDTO> KitchenList { get; set; }
    }
    public class KitchenDTO
    {
        public Guid KitchenId { get; set; }
        public string KitchenName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }
}
