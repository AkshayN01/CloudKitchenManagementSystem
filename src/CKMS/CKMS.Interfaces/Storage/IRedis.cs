using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Interfaces.Storage
{
    public interface IRedis
    {
        String CustomerSetName { get; }
        String KitchenKey { get; } 
        String CustomerKey { get; }
        String orderKey { get; }

        Task HashSet(string key, HashEntry[] hashEntries);
        Task<HashEntry[]> HashGetAll(String key);
        Task<String> HashGet(String key, String field);
        Task<bool> Has(String key);
        Task<bool> HashExist(String key, string val);
        Task<bool> DeleteKey(String key);
    }
}
