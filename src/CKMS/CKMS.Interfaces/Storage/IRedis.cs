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
        Task<bool> Set(String key, Object content, bool serialize);
        Task<bool> Has(String key);
        Task<bool> Remove(String key);
    }
}
