using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CKMS.Contracts.DBModels.CustomerService
{
    public class Customer
    {
        [Key]
        public Guid CustomerId { get; set; }
        public String Name { get; set; }
        public String PhoneNumber { get; set; }
        public String UserName { get; set; }
        public String EmailId { get; set; }
        public String PasswordHash { get; set; }
        public Int64 LoyaltyPoints { get; set; }
        public Int64 TotalOrder {  get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<Address> AddressList { get; set; } = new List<Address>();
    }

    public class Address
    {
        public Guid AddressId { get; set; }
        public String AddressDetail { get; set; }
        public String City { get; set; }
        public String Region { get; set; }
        public String PostalCode { get; set; }
        public String Country { get; set; }
        public Guid CustomerId { get; set; } //Foreign Key
        public Customer Customer { get; set; } = null!; //Required
    }
    public class CustomerAuditTable : AuditTable
    {
        public Guid CustomerId { get; set; }
    }
}
