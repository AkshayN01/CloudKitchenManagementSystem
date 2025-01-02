using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.DTOs.Order.Response
{
    public class OrderReportSummary
    {
        public Int64 TotalOrders { get; set; }
        public Int64 TotalDiscountedOrders { get; set; }
        public Double NetRevenue { get; set; }
        public Double GrossRevenue { get; set; }
        public Double AvgOrderValue { get; set; }
        public List<CustomerOrderingPattern> OrderingPatterns { get; set; } = new List<CustomerOrderingPattern>();
    }

    public class BestSellingDish
    {
        public String MenuItemName { get; set; }
        public Int64 MenuItemId { get; set; }
        public Int64 OrderCount { get; set; }
        public Int64 TotalQuantity { get; set; }
    }

    public class TopCustomers
    {
        public String CustomerName { get; set; }
        public String CustomerId { get; set; }
        public Int64 TotalOrders { get; set; }
    }

    public class CustomerSummary
    {
        public String CustomerName { get; set; }
        public String CustomerId { get; set; }
        public Int64 TotalOrders { get; set; }
        public Int64 TotalDiscountedOrders { get; set; }
        public Double NetRevenue { get; set;}
        public Double GrossRevenue { get; set;}
        public Double AvgOrderValue { get; set; }
        public PreferredDish? PreferredDish { get; set; } = new PreferredDish();
        public List<LatestOrder> LatestOrders { get; set; } = new List<LatestOrder> ();
        public List<CustomerOrderingPattern> OrderingPatterns { get; set; } = new List<CustomerOrderingPattern>();
    }

    public class PreferredDish
    {
        public Int64 MenuItemId { get; set; }
        public String MenuItemName { get; set; }
        public Int64 TotalQuantity { get; set; }
        public String TimePeriod { get; set; }
    }

    public class CustomerOrderingPattern
    {
        public String TimePeriod { get; set; }
        public Int64 OrdersCount { get; set; }
    }

    public class LatestOrder
    {
        public String OrderId { get; set; }
        public Double NetAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public String Status { get; set; }
        public Int64 ItemCount { get; set; }
        public List<OrderItemSummary> Items { get; set; } = new List<OrderItemSummary> ();

    }
    public class OrderItemSummary
    {
        public String MenuItemName { get; set; }
        public Int64 TotalQuantity { get; set; }
    }
}
