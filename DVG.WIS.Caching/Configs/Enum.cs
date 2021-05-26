using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DVG.WIS.Caching.Configs
{
    public enum CachedType
    {
        /// <summary>
        /// NoCache = 0
        /// </summary>
        NoCache = 0,

        /// <summary>
        /// Redis = 1
        /// </summary>
        Redis = 1,

        /// <summary>
        /// IIS = 2
        /// </summary>
        IIS = 2,

        /// <summary>
        /// Memcached = 3
        /// </summary>
        Memcached = 3,

        /// <summary>
        /// ElasticSearch = 4
        /// </summary>
        ElasticSearch = 4
    }

    public enum MessageTypeEnum
    {
        Exception = 0,
        Action = 1
    }
}
