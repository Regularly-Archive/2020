using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptionsPractice.Models
{
    public class RedisConfigurationOptions
    {
        public string ConnectionString { get; set; }
        public string HashCacheKey { get; set; }
        public string HashCacheChannel { get; set; }

    }
}
