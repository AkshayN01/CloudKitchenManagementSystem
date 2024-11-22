using CKMS.Contracts.Configuration;
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
        public static ConnectionMultiplexer _ClientManager;
        public static Contracts.Configuration.Redis _ConnectionEnvironment;

        public string KitchenKey => "kitchen";

        public string CustomerKey => "customer";

        public string orderKey => "order";

        public Redis(IOptions<Application> appSettings)
        {
            _ConnectionEnvironment = appSettings.Value.connection.redis;
        }
        public void Multiplexer()
        {
            if (_ClientManager == null || !_ClientManager.IsConnected)
                ConnectMultiplexer();
        }
        private static void ConnectMultiplexer()
        {
            EndPointCollection mEndPointCollection = new EndPointCollection();
            ConfigurationOptions mConfigurationOptions = new ConfigurationOptions();

            mConfigurationOptions = new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                KeepAlive = 60, // 60 sec to ensure connection is alive
                ConnectTimeout = 10000, // 10 sec
                SyncTimeout = 10000, // 10 sec
            };

            String mRedisConn = _ConnectionEnvironment.server;

            foreach (String mStr in mRedisConn.Split(',').ToList())
            {
                mConfigurationOptions.EndPoints.Add(mStr.Trim(), _ConnectionEnvironment.port);
            }

            _ClientManager = ConnectionMultiplexer.Connect(mConfigurationOptions);
        }
        private static void ConnectDisposer()
        {
            //_ClientManager.Dispose();
        }
        public void Disposer()
        {
            ConnectDisposer();
        }

        public string Get(string key)
        {
            throw new NotImplementedException();
        }

        public bool Has(string key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        public bool Set(string key, object content, bool serialize)
        {
            throw new NotImplementedException();
        }

        Task<string> CKMS.Interfaces.Storage.IRedis.Get(string key)
        {
            throw new NotImplementedException();
        }

        Task<bool> CKMS.Interfaces.Storage.IRedis.Set(string key, object content, bool serialize)
        {
            throw new NotImplementedException();
        }

        Task<bool> CKMS.Interfaces.Storage.IRedis.Has(string key)
        {
            throw new NotImplementedException();
        }

        Task<bool> CKMS.Interfaces.Storage.IRedis.Remove(string key)
        {
            throw new NotImplementedException();
        }

        public Task<HashEntry[]> HashGetAll(string key)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HashExist(string key, string val)
        {
            throw new NotImplementedException();
        }

        public Task<string> HashGet(string key, string field)
        {
            throw new NotImplementedException();
        }
    }
}
