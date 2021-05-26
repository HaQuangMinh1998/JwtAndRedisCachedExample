using System;
using System.Collections.Generic;
using System.Text;

namespace DVG.WIS.Caching.Configs
{
    public class RedisConfiguration
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public int Database { get; set; }
        public int Timeout { get; set; }
        public string AuthName { get; set; }
        public string AuthPassword { get; set; }
        public string SlotNameInMemory { get; set; }
        public string Name { get; set; }
    }
}
