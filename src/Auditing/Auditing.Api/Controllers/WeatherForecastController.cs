using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auditing.Infrastructure;
using Auditing.Infrastructure.Ioc;
using Auditing.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Auditing.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IServiceProvider _serviceProvider;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet]
        [Route("GetNamedService")]
        public ActionResult GetNamedService(string serviceName)
        {
            if (!string.IsNullOrEmpty(serviceName))
            {
                var auditStorage = _serviceProvider.GetNamedService<IAuditStorage>(serviceName);
                auditStorage.SaveAuditLogs(new Domain.AuditLog[] { });
            }

            return Ok();
        }


        [HttpGet]
        [Route("Autowried")]
        public ActionResult GetAutowriedService()
        {
            var _fooService = _serviceProvider.GetService(typeof(IFooService));
            return Ok();
        }

    }
}
