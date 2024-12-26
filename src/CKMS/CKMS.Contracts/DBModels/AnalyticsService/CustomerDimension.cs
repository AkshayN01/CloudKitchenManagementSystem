using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CKMS.Contracts.DBModels.AnalyticsService
{
    internal class CustomerDimension
    {
        public Guid CustomerId { get; set; }
        public String Name { get; set; }
        public String PhoneNumber { get; set; }
        public String EmailId { get; set; }
        public Int64 LoyaltyPoints { get; set; }
        public Int64 TotalOrder { get; set; }
        public int IsDeleted { get; set; }
        public DateTime JoinedDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public class CustomerPreferences
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }
        public Guid CustomerId { get; set; }
        public String CustomerName { get; set; }
        public DateTime PreferredTime { get; set; }
        public Int64 TotalOrder { get; set;}
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
