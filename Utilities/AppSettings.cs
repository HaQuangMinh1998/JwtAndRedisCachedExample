using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace DVG.WIS.Utilities
{
    public class AppSettings
    {
        private static AppSettings _instance;
        private static readonly object ObjLocked = new object();
        private IConfiguration _configuration;
        protected AppSettings()
        {
        }

        public void SetConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static AppSettings Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (ObjLocked)
                    {
                        if (null == _instance)
                            _instance = new AppSettings();
                    }
                }
                return _instance;
            }
        }

        public bool GetBool(string key, bool defaultValue = false)
        {
            try
            {
                return (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[key]) && ConfigurationManager.AppSettings[key].ToBool());
            }
            catch
            {
                return defaultValue;
            }
        }

        public string GetConnection(string key, string defaultValue = "")
        {
            try
            {
                return _configuration.GetConnectionString(key);
            }
            catch
            {
                return defaultValue;
            }
        }

        public int GetInt32(string key, int defaultValue = 0)
        {
            try
            {
                return _configuration.GetSection("StringValue").GetChildren().FirstOrDefault(x => x.Key == key).Value.ToInt();
                //return (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[key]) ? ConfigurationManager.AppSettings[key].ToInt() : defaultValue);
            }
            catch
            {
                return defaultValue;
            }
        }

        public long GetInt64(string key, long defaultValue = 0L)
        {
            try
            {
                return (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[key]) ? ConfigurationManager.AppSettings[key].ToLong() : defaultValue);
            }
            catch
            {
                return defaultValue;
            }
        }

        public string GetString(string key, string defaultValue = "")
        {
            try
            {
                return _configuration.GetSection("StringValue").GetChildren().FirstOrDefault(x => x.Key == key)?.Value ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        public T Get<T>(string key = null)
        {
            if (string.IsNullOrWhiteSpace(key)) return _configuration.Get<T>();

            var section = _configuration.GetSection(key);
            return section.Get<T>();
        }

        public T Get<T>(string key, T defaultValue)
        {
            if (_configuration.GetSection(key) == null)
                return defaultValue;

            if (string.IsNullOrWhiteSpace(key))
                return _configuration.Get<T>();
            var value = _configuration.GetSection(key).Get<T>();
            return (value == null || EqualityComparer<T>.Default.Equals(value, default)) ? defaultValue : value;
        }
    }
}

