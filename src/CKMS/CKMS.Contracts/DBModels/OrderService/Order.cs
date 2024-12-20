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
        public double NetAmount { get; set; }
        public double GrossAmount { get; set; }
        [Required]
        public int Status { get; set; }
        [Required]
        public Guid Address { get; set; } = Guid.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public Payment? Payment { get; set; }
        public DiscountUsage? DiscountUsage { get; set; }
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
        public Order Order { get; set; } = null!; //Required
    }

    public enum OrderStatus
    {
        cart = 1,
        placed = 2,
        accepted = 3,
        inprogress = 4,
        outfordelivery = 5,
        delivered = 6,
        cancelled = 7,
        failed = 8
    }
}
