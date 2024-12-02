using CKMS.Contracts.DBModels.AdminUserService;
using CKMS.Library.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CKMS.Library.SeedData.AdminUserService
{
    public static class AdminUserSeedData
    {
        public static List<AdminUser> Users { get; set; }
        public static List<AdminUser> GetAdminUsers()
        {
            if(Users == null)
            {
                Users = new List<AdminUser>();
                List<Kitchen> kitchens = KitchenSeedData.GetKitchenSeedData();
                foreach(Kitchen k in kitchens)
                {
                    string cleanedString = Regex.Replace(k.KitchenName, @"[^a-zA-Z0-9\s]", ""); 
                    String kitchenName = cleanedString.Replace(" ", "").ToLower();

                    foreach (Role role in Enum.GetValues(typeof(Role)))
                    {
                        AdminUser adminUser = new AdminUser()
                        {
                            EmailId = role+"@" + kitchenName,
                            CreatedAt = DateTime.UtcNow,
                            FullName = role + " "+ (int)role,
                            IsEmailVerified = 1,
                            KitchenId = k.KitchenId,
                            PasswordHash = PasswordHasher.HashPassword("admin@1234"),
                            RoleId = (int)role,
                            UserId = Guid.NewGuid(),
                            UserName = role+"@" + kitchenName,
                            LastUpdatedAt = DateTime.UtcNow,
                        };
                        Users.Add(adminUser);
                    }
                    
                }
            }
            return Users;
        }
    }
}
