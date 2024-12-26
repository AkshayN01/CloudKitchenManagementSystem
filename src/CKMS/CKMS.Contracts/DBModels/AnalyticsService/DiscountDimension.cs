using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.DBModels.AnalyticsService
{
    public class DiscountDimension
    {
        [Key]
        public Int64 Id { get; set; }
        [Required]
        public Guid DiscountId { get; set; }
        public int DiscountType { get; set; }
        [Required]
        public Guid KitchenId { get; set; } = Guid.Empty!;
        [Required]
        public String CouponCode { get; set; } = String.Empty!;
        public float DiscountValue { get; set; } //can be a percentage or amount
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int IsPersonalised { get; set; }
        public int IsActive { get; set; } = 1;
        public int UsageCount { get; set; } = 1; //how many times same discount can be applied
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
