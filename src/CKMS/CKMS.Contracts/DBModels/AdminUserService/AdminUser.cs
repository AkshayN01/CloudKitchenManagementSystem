using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CKMS.Contracts.DBModels.AdminUserService
{
    public class AdminUser
    {
        [Key]
        public Guid UserId { get; set; }

        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        [Required]
        public string EmailId { get; set; } = string.Empty;
        [Required]
        public string FullName { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastLogin {  get; set; }
        public int IsEmailVerified { get; set; }
        public String? VerificationToken { get; set; }
        [Required]
        public Guid KitchenId { get; set; } //Required Foreign Key
        public Kitchen Kitchen { get; set; } = null!; //Required
    }

    public enum Role
    {
        SuperAdmin = 0,
        Admin = 1,
        KitchenStaff = 2,
        DeliveryPartner = 3
    }
}
