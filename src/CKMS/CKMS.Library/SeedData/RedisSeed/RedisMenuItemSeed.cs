using CKMS.Contracts.DBModels.InventoryService;
using CKMS.Library.SeedData.InventoryService;
using StackExchange.Redis;

namespace CKMS.Library.SeedData.RedisSeed
{
    public class RedisMenuItemSeed
    {
        IDatabase _redisDB;
        public RedisMenuItemSeed(IConnectionMultiplexer connectionMultiplexer)
        {
            _redisDB = connectionMultiplexer.GetDatabase();
        }
        public async Task SeedDataAsync()
        {
            await SeedMenuItemAsync();
        }

        private async Task SeedMenuItemAsync()
        {
            List<MenuItem> menuItems = await MenuItemSeedData.GetMenuItems();
            foreach (MenuItem menu in menuItems)
            {
                string keyName = $"kitchen:{menu.KitchenId}";
                HashEntry[] hashEntries = new HashEntry[] { new HashEntry($"menu:{menu.MenuItemId}", $"{menu.Name}:{menu.Price}") };
                await _redisDB.HashSetAsync(keyName, hashEntries);
            }
        }
    }
}
