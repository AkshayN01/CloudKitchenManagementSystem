using CKMS.Contracts.DBModels.AdminUserService;
using CKMS.Library.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Library.SeedData.AdminUserService
{
    public static class KitchenSeedData
    {
        public static List<Kitchen>? kitchens;
        public static List<KitchenAudit> auditTables;
        private static String KitchenFileName = "Kitchen.json";
        public static async Task<List<Kitchen>> GetKitchenSeedData()
        {
            if(kitchens == null)
            {
                //get data from the json file
                kitchens = await Utility.ReadFromFile<List<Kitchen>>(KitchenFileName);
                if (kitchens != null && kitchens.Count > 0)
                    return kitchens;

                kitchens = new List<Kitchen>();
                auditTables = new List<KitchenAudit>();
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
                    Region = "Dublin 01",
                    IsActive = 1
                };
                kitchens.Add(kitchen);
                KitchenAudit kitchenAudit = new KitchenAudit()
                {
                    CreatedAt = DateTime.UtcNow,
                    Id = 1,
                    KitchenId = kitchen.KitchenId,
                    Payload = Utility.SerialiseData(kitchen).Result,
                };
                auditTables.Add(kitchenAudit);

                await Utility.WriteToFile<List<Kitchen>>(KitchenFileName, kitchens);
            }

            return kitchens;
        }
        public static List<KitchenAudit> GetKitchenAudits()
        {
            return auditTables;
        }
    }
}
