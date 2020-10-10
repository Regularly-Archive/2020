using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptionsPractice.Models
{
    public class RedisConfigurationOptions
    {
        /// <summary>
        /// 当配置更新时是否重新载入
        /// </summary>
        public bool AutoReload { get; set; }

        /// <summary>
        /// Redis连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Redis中缓存Hash的键
        /// </summary>
        public string HashCacheKey { get; set; }

        /// <summary>
        /// Redis中订阅Hash的Channel
        /// </summary>
        public string HashCacheChannel { get; set; }


    }
}
