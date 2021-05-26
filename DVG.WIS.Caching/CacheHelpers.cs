using DVG.WIS.Utilities.Serialization;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DVG.WIS.Caching
{
    public class CacheHelpers
    {
        public static bool IsRequestClearCache(HttpContext context = null)
        {
            if (context == null) return false;

            if (context.Request != null && !string.IsNullOrWhiteSpace(context.Request.Headers["User-Agent"].ToString()))
            {
                return context.Request.Headers["User-Agent"].ToString().Contains("refreshcache")
                    || !string.IsNullOrWhiteSpace(context.Request.Headers["wis-refreshcache"].ToString()) && context.Request.Headers["wis-refreshcache"] == "refreshcache";
            }
            return false;
        }

        public static RedisValue Serialize(object value)
        {
            if (value == null) return RedisValue.Null;
            if (value is string s) return s;
            //if (value is byte[] b) return b;
            return NewtonJson.Serialize(value);
        }

        public static T Deserialize<T>(RedisValue value)
        {
            if (!value.HasValue || value.IsNullOrEmpty) return default(T);
            return NewtonJson.Deserialize<T>(value);
        }

        public static byte[] ToByteArray<T>(T obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T FromByteArray<T>(byte[] data)
        {
            if (data == null)
                return default(T);
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
                object obj = bf.Deserialize(ms);
                return (T)obj;
            }
        }
    }
}
