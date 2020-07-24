using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BinLogConsumer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnalyseController : Controller
    {
        private readonly IDistributedCache _cache;
        public AnalyseController(IDistributedCache cache)
        {
            _cache = cache;
        }

        // Post: /<controller>/Publish
        [HttpGet]
        [Route("AnalyseLogs")]
        public Dictionary<string, int> AnalyseLogs()
        {
            var levels = new string[] { "DEBUG", "INFO", "ERROR" };
            return levels.ToDictionary(x => x, x => string.IsNullOrEmpty(_cache.GetString(x)) ? 0 : int.Parse(_cache.GetString(x)));

        }
    }
}
