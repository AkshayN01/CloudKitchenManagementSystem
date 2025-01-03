﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.DBModels.OrderService
{
    public class Payment
    {
        [Key]
        public Guid PaymentId { get; set; }
        [Required]
        public Guid OrderId { get; set; } = Guid.Empty!; //Foreign key
        [Required]
        public double Amount { get; set; }
        [Required]
        public int PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; }
        public int PaymentStatus { get; set; }
        public Order Order { get; set; } = null!; //Required
    }

    public enum PaymentMethod
    {
        CashOnDelivery = 1,
        Online = 2
    }

    public enum PaymentStatus
    {
        pending = 0,
        paid = 1,
        failed = 2,
        canceled = 3
    }
}
