using CSRedis;
using Microsoft.Extensions.Configuration;
using OptionsPractice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptionsPractice.Extension
{
    public class RedisConfigurationSource : IConfigurationSource
    {
        private readonly RedisConfigurationOptions _options;

        public RedisConfigurationSource(RedisConfigurationOptions options)
        {
            _options = options;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new RedisConfigurationProvider(_options);
        }
    }
}
