using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSRedis;
using OptionsPractice.Models;
using Microsoft.VisualBasic;

namespace OptionsPractice.Extension
{
    public class RedisConfigurationProvider : ConfigurationProvider
    {
        private CSRedisClient _redisClient;
        private readonly RedisConfigurationOptions _options;


        public RedisConfigurationProvider(RedisConfigurationOptions options )
        {
            _options = options;
            _redisClient = new CSRedisClient(_options.ConnectionString);
            if (options.AutoReload)
            {
                //利用Redis的发布-订阅重新加载配置
                _redisClient.Subscribe((_options.HashCacheChannel, msg => Load()));
            }
        }

        public override void Load()
        {
            Data = _redisClient.HGetAll<string>(_options.HashCacheKey) ?? new Dictionary<string, string>();
        }
    }
}
