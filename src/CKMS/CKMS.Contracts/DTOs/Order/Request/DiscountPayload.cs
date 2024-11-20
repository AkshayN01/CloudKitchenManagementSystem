using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.DTOs.Order.Request
{
    public class DiscountPayload
    {
        public String DiscountId { get; set; }
        public int DiscountType { get; set; }
        public String KitchenId { get; set; }
        public float DiscountValue { get; set; } //can be a percentage or amount
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int IsPersonalised { get; set; }
        public List<PersonalDiscountPayload>? PersonalDiscounts { get; set; }
    }
    public class PersonalDiscountPayload
    {
        [Required]
        public String UserId { get; set; } = String.Empty!;
        [Required]
        public String DiscountId { get; set; } = String.Empty!;
    }
    public class DiscountUsagePayload
    {
        [Required]
        public String UserId { get; set; } = String.Empty!;
        [Required]
        public String DiscountId { get; set; } = String.Empty!;
        [Required]
        public String OrderId { get; set; } = String.Empty!;
        public int IsApplied { get; set; } //set to 1 after the order is placed
        public DateTime AppliedDate { get; set; }
    }
}
