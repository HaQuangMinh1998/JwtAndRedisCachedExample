using DVG.WIS.Caching.Configs;
using DVG.WIS.Utilities;
using DVG.WIS.Utilities.Logs;
using StackExchange.Redis;
using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DVG.WIS.Caching
{
    public class RedisClientBase : IDisposable
    {
        protected readonly RedisConfiguration _configuration;
        private readonly int _maxLengOfValueForMonitor = AppSettings.Instance.GetInt32("MaxLengOfValueForMonitor", 10000);

        protected readonly IDatabase _database;
        protected IConnectionMultiplexer _connectionMultiplexer;
        protected IConnectionMultiplexer _connectionMultiplexerForWrite;

        public RedisClientBase()
        {
            // _configuration = AppSettings.Instance.Get<RedisConfiguration>("RedisConnect");
            _configuration = new RedisConfiguration
            {
                Server = AppSettings.Instance.GetString("RedisIP"),
                Port = AppSettings.Instance.GetInt32("RedisPort"),
                Timeout = AppSettings.Instance.GetInt32("RedisTimeout"),
                Database = AppSettings.Instance.GetInt32("RedisDB"),
                SlotNameInMemory = AppSettings.Instance.GetString("RedisSlotName")
            };
        }

        public RedisClientBase(RedisConfiguration configuration)
        {
            if (configuration.Timeout > 0)
                configuration.Timeout = configuration.Timeout;

            _configuration = configuration;

        }

        //public RedisClientBase()
        //{
        //    try
        //    {
        //        _connectionMultiplexer = new RedisConnectFactory(AppSettings.Instance.GetString("RedisConnect")).GetConnection();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.WriteLog(Logger.LogType.Error, "ErrorRedisCreate: " + ex.ToString());
        //    }

        //    _database = _connectionMultiplexer?.GetDatabase();
        //}

        //public RedisClientBase(string connect)
        //{
        //    _connection = connect;

        //    try
        //    {
        //        _connectionMultiplexer = new RedisConnectFactory(_connection).GetConnection();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.WriteLog(Logger.LogType.Error, "ErrorRedisCreate: " + ex.ToString());
        //    }

        //    _database = _connectionMultiplexer?.GetDatabase();
        //}

        #region init

        protected IDatabase CreateInstanceRead()
        {
            IDatabase client = null;

            try
            {
                if (_connectionMultiplexer == null || !_connectionMultiplexer.IsConnected)
                {
                    var config = new ConfigurationOptions
                    {
                        EndPoints = { { _configuration.Server, _configuration.Port } },
                        DefaultDatabase = _configuration.Database,
                        ConnectTimeout = _configuration.Timeout,
                        AsyncTimeout = _configuration.Timeout / 2,
                        ClientName = _configuration.Name,
                        AbortOnConnectFail = false,
                        ConnectRetry = 3,
                        KeepAlive = 20
                    };

                    _connectionMultiplexer = ConnectionMultiplexer.Connect(config);
                }

                client = _connectionMultiplexer.GetDatabase(_configuration.Database);
            }
            catch (Exception ex)
            {

                Logger.ErrorLog(ex);
            }

            return client;
        }

        protected IDatabase CreateInstanceForWrite()
        {
            IDatabase client = null;

            try
            {
                if (_connectionMultiplexerForWrite == null || !_connectionMultiplexerForWrite.IsConnected)
                {
                    var config = new ConfigurationOptions
                    {
                        EndPoints = { { _configuration.Server, _configuration.Port } },
                        DefaultDatabase = _configuration.Database,
                        ConnectTimeout = _configuration.Timeout,
                        AsyncTimeout = _configuration.Timeout / 2,
                        ClientName = _configuration.Name,
                        AbortOnConnectFail = false,
                        ConnectRetry = 3,
                        KeepAlive = 20
                    };

                    _connectionMultiplexerForWrite = ConnectionMultiplexer.Connect(config);
                }

                client = _connectionMultiplexerForWrite.GetDatabase(_configuration.Database);
            }
            catch (Exception ex)
            {
                Logger.ErrorLog(ex);
            }

            return client;
        }

        protected async Task<IDatabase> CreateInstanceAsyncRead()
        {
            IDatabase client = null;
            try
            {
                if (_connectionMultiplexer == null || !_connectionMultiplexer.IsConnected)
                {
                    var config = new ConfigurationOptions
                    {
                        EndPoints = { { _configuration.Server, _configuration.Port } },
                        DefaultDatabase = _configuration.Database,
                        ConnectTimeout = _configuration.Timeout,
                        AsyncTimeout = _configuration.Timeout
                    };

                    _connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(config);
                }

                client = _connectionMultiplexer.GetDatabase(_configuration.Database);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return client;
        }

        protected async Task<IDatabase> CreateInstanceAsyncForWrite()
        {
            IDatabase client = null;
            try
            {
                if (_connectionMultiplexerForWrite == null || !_connectionMultiplexerForWrite.IsConnected)
                {
                    var config = new ConfigurationOptions
                    {
                        EndPoints = { { _configuration.Server, _configuration.Port } },
                        DefaultDatabase = _configuration.Database,
                        ConnectTimeout = _configuration.Timeout,
                        AsyncTimeout = _configuration.Timeout
                    };

                    _connectionMultiplexerForWrite = await ConnectionMultiplexer.ConnectAsync(config);
                }

                client = _connectionMultiplexerForWrite.GetDatabase(_configuration.Database);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return client;
        }

        #endregion


        #region private methods

        protected byte[] ZipToBytes<T>(T item, string key)
        {
            if (item == null || item.Equals(default(T)))
                return null;

            var bf = new BinaryFormatter();

            byte[] bytes;

            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, item);
                bytes = ms.ToArray();
            }

            //if (bytes.LongLength > _maxLengOfValueForMonitor)
            return bytes;
        }

        protected T UnZipFromBytes<T>(byte[] bytes)
        {
            if (bytes == null || bytes.Length <= 0)
                return default;

            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream(bytes))
            {
                var obj = bf.Deserialize(ms);
                return (T)obj;
            }
        }

        protected void CopyTo(Stream src, Stream dest)
        {
            var bytes = new byte[8096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0) dest.Write(bytes, 0, cnt);
        }

        protected byte[] Zip(string strInput, string key)
        {
            var bytes = Encoding.UTF8.GetBytes(strInput);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    //msi.CopyTo(gs);
                    CopyTo(msi, gs);
                }

                var newBytes = mso.ToArray();

                if (newBytes.LongLength > _maxLengOfValueForMonitor)
                    Logger.Info($"Leng of value <{key}> => {newBytes.LongLength}");

                return newBytes;
            }
        }

        protected string Unzip(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    //gs.CopyTo(mso);
                    CopyTo(gs, mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }

        #endregion private methods

        public void Dispose()
        {
            _database.Multiplexer.GetSubscriber().UnsubscribeAll();
            _database.Multiplexer.Dispose();
        }
    }
}
