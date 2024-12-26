using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.DBModels.AnalyticsService
{
    //Warehouse Analytics Table
    //Analysis that can be done ::
    //Get total revenue of the day/month/year
    //Order processing time
    //Best Selling Dish
    public class AggregatedOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }
        [Required]
        public Guid OrderId { get; set; }
        [Required] 
        public Guid KitchenId { get; set; }
        public Guid CustomerId { get; set; }
        public Int64 MenuItemId { get; set; }
        [Required]
        public String CustomerName { get; set; } //get it from the redis
        [Required]
        public String KitchenName { get; set; } //get it from the redis
        [Required]
        public String MenuItemName { get; set; } //get it from the redis
        [Required]
        public DateTime OrderDate { get; set; }
        public DateTime CookingStartedAt { get; set; }
        public DateTime CookingCompletedAt { get; set; }
        public DateTime DeliveredAt { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public Double NetAmount { get; set; }
        [Required]
        public Double GrossAmount { get; set; }
        public Guid DiscountId { get; set; }
        public int OrderStatus { get; set; }
        public int PaymentMethod { get; set; }
        public int PaymentStatus { get; set; }
    }
    public class DicountEffectiveness
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }
        public Int64 DiscountId { get; set; }
        public String CouponCode { get; set; }
        public Int64 UsageCount { get; set; }
        public Double TotalDiscountAmount { get; set; }
        public Double TotalRevenueWithDiscount { get; set; }
    }
}
