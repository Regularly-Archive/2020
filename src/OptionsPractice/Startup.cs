using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using OptionsPractice.Models;

namespace OptionsPractice
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //写法
            var leaningOptions = new LearningOptions();
            Configuration.GetSection("Learning").Bind(leaningOptions);

            //写法2
            leaningOptions = Configuration.GetSection("Learning").Get<LearningOptions>();

            //写法3
            services.Configure<LearningOptions>(Configuration.GetSection("Learning"));


            //写法6
            services.Configure<AppInfoOptions>(Configuration.GetSection("App"));


            services.Configure<ThemeOptions>("DarkTheme", Configuration.GetSection("Themes:Dark"));
            services.Configure<ThemeOptions>("WhiteTheme", Configuration.GetSection("Themes:White"));

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
