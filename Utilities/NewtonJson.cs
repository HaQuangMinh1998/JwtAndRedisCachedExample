using System;
using DVG.WIS.Utilities.Logs;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DVG.WIS.Utilities.Serialization
{
    public interface INewtonJson
    {
        T Deserialize<T>(string jsonString);

        T Deserialize<T>(string jsonString, string dateTimeFormat);

        string Serialize(object @object);

        string Serialize(object @object, string dateTimeFormat);
    }

    public class NewtonJson
    {
        private static readonly JsonSerializerSettings MicrosoftDateFormatSettings;

        static NewtonJson()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings {
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
            };
            MicrosoftDateFormatSettings = settings;
        }

        public static T Deserialize<T>(string jsonString)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(jsonString, MicrosoftDateFormatSettings);
            }
            catch (Exception exception)
            {
                Logger.WriteLog(Logger.LogType.Error, exception.Message);
                return default(T);
            }
        }

        public static T Deserialize<T>(string jsonString, string dateTimeFormat)
        {
            try
            {
                JsonConverter[] converters = new JsonConverter[1];
                IsoDateTimeConverter converter = new IsoDateTimeConverter {
                    DateTimeFormat = dateTimeFormat
                };
                converters[0] = converter;
                return JsonConvert.DeserializeObject<T>(jsonString, converters);
            }
            catch (Exception exception)
            {
                Logger.WriteLog(Logger.LogType.Error, exception.Message);
                return default(T);
            }
        }

        public static object DeserializeObject(string jsonString, Type type)
        {
            try
            {
                return JsonConvert.DeserializeObject(jsonString, type);
            }
            catch (Exception exception)
            {
                Logger.WriteLog(Logger.LogType.Error, exception.Message);
                return default(object);
            }
        }

        public static string Serialize(object @object)
        {
            try
            {
                return JsonConvert.SerializeObject(@object, MicrosoftDateFormatSettings);
            }
            catch (Exception exception)
            {
                Logger.WriteLog(Logger.LogType.Error, exception.Message);
                return string.Empty;
            }
        }

        public static string Serialize(object @object, string dateTimeFormat)
        {
            try
            {
                JsonConverter[] converters = new JsonConverter[1];
                IsoDateTimeConverter converter = new IsoDateTimeConverter {
                    DateTimeFormat = dateTimeFormat
                };
                converters[0] = converter;
                return JsonConvert.SerializeObject(@object, converters);
            }
            catch (Exception exception)
            {
                Logger.WriteLog(Logger.LogType.Error, exception.Message);
                return string.Empty;
            }
        }
    }
}

