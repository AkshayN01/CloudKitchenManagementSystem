using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.DBModels.AnalyticsService
{
    //when an order is placed send the payload to the audit table with MenuItemId, KitchenId
    public class BestSellingDish
    {
        public Int64 Id { get; set; }
        public Int64 MenuItemId { get; set; }
        public String MenuItemName { get; set; } //get this value from the redis
        public Guid KitchenId { get; set; }
        public DateTime OrderDay { get; set; }
        public Int64 OrderCount { get; set; }
        public Double TotalRevenue { get; set; }
        public Double ActualRevenue { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public class OrderProcessingTime
    {
        public Int64 Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid KitchenId { get; set; }
        public DateTime OrderPlaced {  get; set; }
        public DateTime CookingStartedAt { get; set; }
        public DateTime CookingCompletedAt { get; set; }
        public DateTime DeliveredAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
