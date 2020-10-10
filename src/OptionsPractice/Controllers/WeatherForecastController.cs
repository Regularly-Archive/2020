using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OptionsPractice.Models;

namespace OptionsPractice.Controllers
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

        private readonly IOptions<LearningOptions> _learningOptions;

        private readonly IOptionsSnapshot<LearningOptions> _learningOptionsSnapshot;

        private readonly IOptionsMonitor<LearningOptions> _learningOptionsMonitor;

        private readonly IConfiguration _configuration;


        public WeatherForecastController(ILogger<WeatherForecastController> logger, 
            IOptions<LearningOptions> learningOptions, 
            IOptionsSnapshot<LearningOptions> learningOptionsSnapshot, 
            IOptionsMonitor<LearningOptions> learningOptionsMonitor,
            IConfiguration configuration
            )
        {
            _logger = logger;
            _learningOptions = learningOptions;
            _learningOptionsSnapshot = learningOptionsSnapshot;
            _learningOptionsMonitor = learningOptionsMonitor;
            _configuration = configuration;
            _learningOptionsMonitor.OnChange((options, value) =>
            {
                _logger.LogInformation($"OnChnage => {JsonConvert.SerializeObject(options)}");
            });

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

        [HttpGet("{action}")]
        public LearningOptions GetOptions()
        {
            return _learningOptionsSnapshot.Value;
        }

        [HttpGet("{action}")]
        public AppInfoOptions GetConfiguration()
        {
            return _configuration.Get<AppInfoOptions>();
        }
    }
}
