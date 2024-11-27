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
    public class OrderAuditTable : AuditTable
    {
        public Guid OrderId { get; set; }
    }

    public enum OrderStatus
    {
        cart = 0,
        placed = 1,
        accepted = 2,
        inprogress = 3,
        delivered = 4,
        cancelled = 5,
        outfordelivery = 6,
        failed = 7
    }
}
