using CKMS.Contracts.DBModels.OrderService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.DTOs.Analytics.Request
{
    public class AnalyticsOrderPayload
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public String CustomerName { get; set; }
        public Guid KitchenId { get; set; }
        public DateTime OrderDate { get; set; }
        public double NetAmount { get; set; }
        public double GrossAmount { get; set; }
        public String Address { get; set; }
        public int OrderStatus { get; set; }
        public int PaymentStatus { get; set; }
        public int PaymentMethod { get; set; }
        public Guid DiscountId { get; set; }
        public List<AnalyticsOrderItemPayload> OrderItems { get; set; }
    }
    public class AnalyticsOrderItemPayload
    {
        public int Quantity { get; set; }
        public Int64 MenuItemId { get; set; }
        public String MenuItemName { get; set; }
        public int IsDeleted { get; set; }
    }
}
