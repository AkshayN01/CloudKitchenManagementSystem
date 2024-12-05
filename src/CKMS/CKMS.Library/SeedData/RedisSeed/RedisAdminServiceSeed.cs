using CKMS.Contracts.DBModels.AdminUserService;
using CKMS.Library.SeedData.AdminUserService;
using StackExchange.Redis;

namespace CKMS.Library.SeedData.RedisSeed
{
    public class RedisAdminServiceSeed
    {
        IDatabase _database;
        public RedisAdminServiceSeed(IConnectionMultiplexer connectionMultiplexer) 
        {
            _database = connectionMultiplexer.GetDatabase();
        }

        public async Task SeedData()
        {
            await SeedKitchenData();
        }

        private async Task SeedKitchenData()
        {
            List<Kitchen> kitchens = await KitchenSeedData.GetKitchenSeedData();
            foreach (Kitchen k in kitchens)
            {
                String address = $"{k.Address}, {k.Region}, {k.City}, {k.PostalCode}";
                String keyName = "kitchen:" + k.KitchenId;
                await _database.HashSetAsync(keyName, new HashEntry[]
                {
                    new HashEntry("name", k.KitchenName),
                    new HashEntry("address", address)
                });
            }
        }
    }
}
