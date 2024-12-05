using CKMS.Contracts.Configuration;
using CKMS.Contracts.DBModels.CustomerService;
using CKMS.Interfaces.Storage;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Library.Storage
{
    public class Redis : CKMS.Interfaces.Storage.IRedis
    {
        private readonly IDatabase _redisDB;
        public string CustomerSetName => "Customers";

        public string KitchenKey => "kitchen";

        public string CustomerKey => "customer";

        public string orderKey => "order";

        public Redis(IConnectionMultiplexer muxer)
        {
            _redisDB = muxer.GetDatabase();
        }

        public async Task HashSet(string key, HashEntry[] hashEntries)
        {
            await _redisDB.HashSetAsync(key, hashEntries);
        }
        public async Task<HashEntry[]> HashGetAll(string key)
        {
            return await _redisDB.HashGetAllAsync(key);
        }

        public async Task<string> HashGet(string key, string field)
        {
            RedisValue redisValue = await _redisDB.HashGetAsync(key, field);
            return redisValue.ToString();
        }

        public async Task<bool> Has(string key)
        {
            return await _redisDB.KeyExistsAsync(key);
        }

        public async Task<bool> HashExist(string key, string val)
        {
            return await _redisDB.HashExistsAsync(key, val);
        }
    }
}
