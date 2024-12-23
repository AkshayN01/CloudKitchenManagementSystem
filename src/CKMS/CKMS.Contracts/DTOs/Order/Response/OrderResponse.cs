﻿using CKMS.Contracts.DBModels.OrderService;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.DTOs.Order.Response
{
    public class OrderList
    {
        public List<OrderListDTO> Orders { get; set; }
        public Int64 TotalCount { get; set; }
    }
    public class OrderListDTO
    {
        public String KitchenName { get; set; }
        public Int64 ItemCount { get; set; }
        public Double NetAmount { get; set; }
        public String OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
    }
    public class OrderDTO
    {
        public Guid OrderId { get; set; }
        
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public double NetAmount { get; set; }
        public double GrossAmount { get; set; }
        public String Status { get; set; } = String.Empty;
        public String Address { get; set; } = String.Empty;
        public ICollection<OrderItemDTO> Items { get; set; } = new List<OrderItemDTO>();
        public Payment? Payment { get; set; }
    }
    public class OrderItemDTO
    {
        public Guid OrderId { get; set; } 
        
        public long MenuItemId { get; set; }
        public string ItemName { get; set; }
        
        public int Quantity { get; set; }
    }
}
