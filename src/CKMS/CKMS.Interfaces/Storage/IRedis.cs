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
        String KitchenKey { get; } 
        String CustomerKey { get; }
        String orderKey { get; }
        void Multiplexer();
        void Disposer();

        Task<String> Get(String key);
        Task<HashEntry[]> HashGetAll(String key);
        Task<String> HashGet(String key, String field);
        Task<bool> Set(String key, Object content, bool serialize);
        Task<bool> Has(String key);
        Task<bool> HashExist(String key, string val);
        Task<bool> Remove(String key);
    }
}
