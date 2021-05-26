using DVG.WIS.Utilities;
using DVG.WIS.Utilities.Serialization;
using System.Linq;

namespace DVG.WIS.Caching
{
    public class KeyCacheHelper
    {
        private static string _prefixKey = AppSettings.Instance.GetString("PreCacheKey", "dvg_auto");
        private static string separator = ":";

        public struct ObjectKey
        {
            public object Input { get; set; }
            public bool IsSerialize { get; set; }
        }

        public static string GenCacheKey(string cacheName, params object[] args)
        {
            string cacheKey = string.Format("{0}{1}{2}", _prefixKey, separator, cacheName);

            if (args != null && args.Length > 0)
            {
                //return args.Aggregate(cacheKey, (current, param) => current + (separator + (param.GetType() == typeof(string) ? StringUtils.CalculateMD5Hash(param.ToString()) : param)));
                string keyComplex = args.Aggregate(cacheKey, (current, param) => current + (separator + EncodeObject(param)));
                cacheKey = string.Concat(cacheKey, separator, StringUtils.CalculateMD5Hash(keyComplex));
            }

            return cacheKey;
        }

        public static string GenCacheKeySerialize(string cacheName, params object[] args)
        {
            string cacheKey = string.Format("{0}{1}{2}", _prefixKey, separator, cacheName);

            if (args != null && args.Length > 0)
            {
                //return args.Aggregate(cacheKey, (current, param) => current + (separator + EncodeObject(param)));

                string keyComplex = args.Aggregate(cacheKey, (current, param) => current + (separator + EncodeObject(param)));
                cacheKey = string.Concat(cacheKey, separator, StringUtils.CalculateMD5Hash(keyComplex));
            }

            return cacheKey;
        }

        public static string GenCacheKeyDynamic(string cacheName, params ObjectKey[] args)
        {
            string cacheKey = string.Format("{0}{1}{2}", _prefixKey, separator, cacheName);

            if (args != null && args.Length > 0)
            {
                return args.Aggregate(cacheKey, (current, param) => current + (separator + EncodeObject(param)));
            }

            return cacheKey;
        }

        public static string GenCacheKeyStatic(string cacheName, params object[] args)
        {
            string cacheKey = string.Format("{0}{1}{2}", _prefixKey, separator, cacheName);

            if (args != null && args.Length > 0)
            {
                return args.Aggregate(cacheKey, (current, param) => current + (separator + param));
            }

            return cacheKey;
        }

        private static string EncodeObject(object obj)
        {
            if (obj == null) return string.Empty;
            return StringUtils.CalculateMD5Hash(NewtonJson.Serialize(obj));
        }

        private static object EncodeObject(ObjectKey obj)
        {
            if (obj.Input == null) return string.Empty;
            if (obj.IsSerialize)
                return StringUtils.CalculateMD5Hash(NewtonJson.Serialize(obj));

            return obj.Input;
        }
    }
}