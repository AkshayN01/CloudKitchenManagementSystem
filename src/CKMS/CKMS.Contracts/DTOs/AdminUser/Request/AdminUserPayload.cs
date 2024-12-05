using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.DTOs.AdminUser.Request
{
    //used to add a user
    public class AdminUserPayload
    {
        [Required]
        public string UserName { get; set; } = string.Empty!;
        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Email is not valid")]
        public string EmailId { get; set; } = string.Empty!;
        [Required]
        public string FullName { get; set; } = string.Empty!;
        public int RoleId { get; set; }
    }
    public class AdminUserRegisterPayload
    {
        [Required]
        public string UserName { get; set; } = string.Empty!;
        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Email is not valid")]
        public string EmailId { get; set; } = string.Empty!;
        [Required]
        public string FullName { get; set; } = string.Empty!;
        public int RoleId { get; set; }
        [Required]
        public String Password { get; set; } = String.Empty!;
    }
    //used to update the User Password
    public class AdminUserPassword 
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public string Password { get; set; } = string.Empty!;
    }
    
    //used to update the User
    public class AdminUserUpdatePayload
    {
        [Required]
        public String UserId { get; set; } = String.Empty!;
        public string Password { get; set; }
        public string EmailId { get; set; }
        public string FullName { get; set; }
        public int RoleId { get; set; }
    }
}
