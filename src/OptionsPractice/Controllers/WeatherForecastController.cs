using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
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

        private readonly IOptionsSnapshot<ThemeOptions> _themeOptionsSnapshot;

        private readonly IOptionsSnapshot<LearningOptions> _learningOptionsSnapshot;

        private readonly IOptionsMonitor<LearningOptions> _learningOptionsMonitor;

        private readonly IConfiguration _configuration;


        public WeatherForecastController(ILogger<WeatherForecastController> logger, 
            IOptions<LearningOptions> learningOptions, 
            IOptionsSnapshot<LearningOptions> learningOptionsSnapshot, 
            IOptionsMonitor<LearningOptions> learningOptionsMonitor,
            IOptionsSnapshot<ThemeOptions> themeOptionsSnapshot,
            IConfiguration configuration
            )
        {
            _logger = logger;
            _learningOptions = learningOptions;
            _learningOptionsSnapshot = learningOptionsSnapshot;
            _learningOptionsMonitor = learningOptionsMonitor;
            _configuration = configuration;
            _themeOptionsSnapshot = themeOptionsSnapshot;
            _learningOptionsMonitor.OnChange((options, value) =>
            {
                _logger.LogInformation($"OnChnage => {JsonConvert.SerializeObject(options)}");
            });

            //ChangeToken + IFileProvider 实现对文件的监听
            var filePath = @"C:\Users\admin\Downloads\德利得-三星发货订单接口测试流程.txt";
            var directory = System.IO.Path.GetDirectoryName(filePath);
            var fileProvider = new PhysicalFileProvider(directory);
            ChangeToken.OnChange(
                () => fileProvider.Watch("德利得-三星发货订单接口测试流程.txt"),
                () => {
                    _logger.LogInformation("孔乙己，你一定又偷人家书了吧");
                }
            );
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
        public ActionResult GetOptions()
        {
            var builder = new StringBuilder();
            builder.AppendLine("learningOptions:");
            builder.AppendLine(JsonConvert.SerializeObject(_learningOptions.Value));
            builder.AppendLine("learningOptionsSnapshot:");
            builder.AppendLine(JsonConvert.SerializeObject(_learningOptionsSnapshot.Value));
            builder.AppendLine("learningOptionsMonitor:");
            builder.AppendLine(JsonConvert.SerializeObject(_learningOptionsMonitor.CurrentValue));
            return Content(builder.ToString());
        }

        [HttpGet("{action}")]
        public AppInfoOptions GetConfiguration()
        {
            return _configuration.GetSection("App").Get<AppInfoOptions>();
        }
    }
}
