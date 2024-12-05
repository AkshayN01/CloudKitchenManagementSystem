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
        [Required]
        public String EmailId { get; set; } = String.Empty!;
        public String PasswordHash { get; set; }
        public Int64 LoyaltyPoints { get; set; }
        public Int64 TotalOrder {  get; set; }
        public String? VerificationToken { get; set; }
        public int IsVerified { get; set; }
        public int IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime LastLogin { get; set; }
        public ICollection<Address> AddressList { get; set; } = new List<Address>();
    }

    public class Address
    {
        [Key]
        public Guid AddressId { get; set; }
        public String AddressDetail { get; set; }
        public String City { get; set; }
        public String Region { get; set; }
        public String PostalCode { get; set; }
        public String Country { get; set; }
        public Guid CustomerId { get; set; } //Foreign Key
        public Customer Customer { get; set; } = null!; //Required
    }
    public class CustomerAudit : AuditTable
    {
        public Guid CustomerId { get; set; }
    }
}
