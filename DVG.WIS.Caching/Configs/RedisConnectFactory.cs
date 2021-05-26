using StackExchange.Redis;
using System;

namespace DVG.WIS.Caching.Configs
{
    public class RedisConnectFactory
    {
        private readonly Lazy<ConnectionMultiplexer> _connection;
        public RedisConnectFactory(string config)
        {
            var options = ConfigurationOptions.Parse(config);
            options.ConnectTimeout = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;
            options.SyncTimeout = (int)TimeSpan.FromSeconds(5).TotalMilliseconds;
            options.AbortOnConnectFail = false;
            //options.AllowAdmin = true;
            options.ClientName = "RedisDVConnection";
            options.ConnectRetry = 3;
            options.KeepAlive = 20;

            this._connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options.ToString()));
        }
        public ConnectionMultiplexer GetConnection()
        {
            return this._connection.Value;
        }
    }
}
