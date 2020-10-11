using CSRedis;
using Microsoft.Extensions.Configuration;
using OptionsPractice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptionsPractice.Extension
{
    public static class RedisConfigurationExtensions
    {
        public static IConfigurationBuilder AddRedisConfiguration(this IConfigurationBuilder builder, RedisConfigurationOptions options)
        {
            return builder.Add(new RedisConfigurationSource(options));
        }
    }
}
