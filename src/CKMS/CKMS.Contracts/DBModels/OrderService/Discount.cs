using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.DBModels.OrderService
{
    public class Discount
    {
        [Key]
        public Guid DiscountId { get; set; }
        public int DiscountType { get; set; }
        [Required]
        public Guid KitchenId { get; set; }  =  Guid.Empty!;
        public float DiscountValue { get; set; } //can be a percentage or amount
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int IsPersonalised { get; set; }
        public ICollection<PersonalDiscounts> PersonalDiscounts { get; set; } = new List<PersonalDiscounts>();
        public ICollection<DiscountUsage> DiscountUsages { get; set; } = new List<DiscountUsage>();
    }

    public class PersonalDiscounts
    {
        [Key]
        public Guid PersonalDiscountId { get; set; }
        [Required]
        public Guid UserId { get; set; } = Guid.Empty!;
        public Guid DiscountId { get; set; }
        public Discount Discount { get; set; } = null!;
    }
    public class DiscountUsage
    {
        [Key]
        public Guid UsageId { get; set; }
        [Required]
        public Guid UserId { get; set; } = Guid.Empty!;
        [Required]
        public Guid DiscountId { get; set; }
        public Discount Discount { get; set; } = null!;
    }
    public enum DiscountType
    {
        Percentage = 0,
        FixedAmount = 1
    }
}
