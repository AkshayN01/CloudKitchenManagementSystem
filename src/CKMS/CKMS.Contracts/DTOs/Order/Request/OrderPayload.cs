using CKMS.Contracts.DBModels.OrderService;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.DTOs.Order.Request
{
    public class OrderPayload
    {
        public String OrderId { get; set; }
        [Required]
        public String CustomerId { get; set; } = String.Empty!;
        [Required]
        public String KitchenId { get; set; } = String.Empty!;
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        public double TotalAmount { get; set; }
        public float Discount { get; set; }
        public string CouponCode { get; set; } = string.Empty;
        [Required]
        public int Status { get; set; }
        [Required]
        public string Address { get; set; } = string.Empty;
        [Required]
        public int PaymentStatus { get; set; }
        public ICollection<OrderItemPayload> Items { get; set; } = new List<OrderItemPayload>();
    }
    public class OrderItemPayload
    {
        [Required]
        public long MenuItemId { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
    public class ConfirmOrderPayload
    {
        [Required]
        public String OrderId { get; set; } = String.Empty!;
        [Required]
        public int PaymentMethod { get; set; }
    }
}
