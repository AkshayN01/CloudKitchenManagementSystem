using CKMS.Contracts.DBModels.AdminUserService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Library.SeedData.AdminUserService
{
    public static class KitchenSeedData
    {
        public static List<Kitchen> kitchens;

        public static List<Kitchen> GetKitchenSeedData()
        {
            if(kitchens == null)
            {
                kitchens = new List<Kitchen>();
                Kitchen kitchen = new Kitchen()
                {
                    Address = "87 Parnell St, Rotunda, Dublin, D01 AK16",
                    Country = "Ireland",
                    City = "Dublin",
                    CreatedAt = DateTime.UtcNow,
                    EmailId = "admin@enzotakeaway.com",
                    KitchenId = Guid.NewGuid(),
                    KitchenName = "Enzo's Takeaway",
                    LastUpdatedAt = DateTime.UtcNow,
                    PostalCode = "D01 AK16",
                    Region = "Dublin 01"
                };
                kitchens.Add(kitchen);
            }

            return kitchens;
        }
    }
}
