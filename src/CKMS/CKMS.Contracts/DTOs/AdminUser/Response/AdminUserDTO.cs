using CKMS.Contracts.DBModels.AdminUserService;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.DTOs.AdminUser.Response
{
    public class AdminUserDTO
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public Guid KitchenId { get; set; }
    }
    public class AdminUserListDTO
    {
        public List<AdminUserDTO> Users { get; set; }
        public long TotalCount { get; set; }
    }
}
