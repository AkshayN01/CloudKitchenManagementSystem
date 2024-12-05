using CKMS.Contracts.DBModels.CustomerService;
using CKMS.Library.SeedData.CustomerService;
using StackExchange.Redis;

namespace CKMS.Library.SeedData.RedisSeed
{
    public class RedisCustomerServiceSeed
    {
        IDatabase _redisDB;
        public RedisCustomerServiceSeed(IConnectionMultiplexer connectionMultiplexer) 
        {
            _redisDB = connectionMultiplexer.GetDatabase();
        }
        public async Task SeedDataAsync()
        {
            await SeedCustomerDataAsync();
        }
        public async Task SeedCustomerDataAsync()
        {
            List<Customer> customers = await CustomerSeedData.GetCustomers();
            foreach (Customer customer in customers)
            {
                string keyName = $"customer:{customer.CustomerId}";
                List<HashEntry> hashEntries = new List<HashEntry>()
                {
                    new HashEntry("emailId", customer.EmailId),
                    new HashEntry("phoneNumber", customer.PhoneNumber),
                };
                foreach(Address address in customer.AddressList)
                {
                    String addressDetail = $"{address.AddressDetail}, {address.Region}, {address.City}, {address.PostalCode}";
                    hashEntries.Add(new HashEntry("address:" + address.AddressId, addressDetail));
                }
                await _redisDB.HashSetAsync(keyName, hashEntries.ToArray());
            }
        }
    }
}
