using System.ComponentModel.DataAnnotations;

namespace CKMS.Contracts.DBModels.OrderService
{
    public class Order
    {
        public Guid OrderId { get; set; }
        [Required]
        public Guid CustomerId { get; set; }
        [Required]
        public Guid KitchenId { get; set; }
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
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public Payment? Payment { get; set; }
    }

    public class OrderItem
    {
        [Key]
        public Guid OrderItemId { get; set; }
        [Required]
        public Guid OrderId { get; set; } //Foreign Key
        [Required]
        public long MenuItemId { get; set; }
        [Required]
        public int Quantity { get; set; }
        public float UnitPrice { get; set; }
        public Order Order { get; set; } = null!; //Required
    }

    public enum OrderStatus
    {
        Pending = 0,
        Confirmed = 1,
        InProgress = 2,
        Delivered = 3,
        Canceled = 4,
        OutForDelivery = 5,
        Failed = 6
    }
}
