using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Auditing.Domain;
using Auditing.Infrastructure;
using Auditing.Infrastructure.Interceptors;
using Auditing.Infrastructure.Ioc;
using Auditing.Infrastructure.Repository;
using Auditing.Infrastructure.Services;
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Auditing.Api
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
            services.AddAuditLog(config =>
                config
                    .IgnoreTable<AuditLog>()
                    .IgnoreProperty<AuditLog>(x => x.CreatedDate)
                    .WithExtraData("Tags", ".NET Core")
                    .WithStorage<FileAuditStorage>()
                    .WithStorage<MongoAuditStorage>()
            );
            services.AddDbContext<CustomerContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<IDbConnection>(x => new SqlConnection(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped(typeof(IRepository), typeof(DapperRepository));
            services.AddScoped(typeof(IUnitOfWork), typeof(DapperUnitOfWork));
            services
                .AsNamedServiceProvider()
                .AddNamedService<MongoAuditStorage>("MongoAuditStorage", ServiceLifetime.Transient)
                .AddNamedService<FileAuditStorage>("FileAuditStorage", ServiceLifetime.Transient)
                .AddNamedService<ChineseSayHello>("Chinese", ServiceLifetime.Transient)
                .AddNamedService<EnglishSayHello>("English", ServiceLifetime.Transient)
                .Build();
            services.AddTransient<IFooService,FooService>();
            services.AddTransient<IBarService, BarService>();
            services.AddTransient<BarService>();
            services.AddControllers();
            services.AddControllersWithViews().AddControllersAsServices();
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, AutowiredControllerActivator>());
            services.AddTransient<ChineseSayHello>();
            services.AddTransient<EnglishSayHello>();
            services.AddTransient(implementationFactory =>
            {
                Func<string, ISayHello> sayHelloFactory = lang =>
                 {
                     switch (lang)
                     {
                         case "Chinese":
                             return implementationFactory.GetService<ChineseSayHello>();
                         case "English":
                             return implementationFactory.GetService<EnglishSayHello>();
                         default:
                             throw new NotImplementedException();
                     }
                 };

                return sayHelloFactory;
            });
        }


        //public void ConfigureContainer(ContainerBuilder builder)
        //{
        //    //builder.RegisterType<DapperRepository>().As<IRepository>()
        //    //    .InterceptedBy(typeof(AuditLogInterceptor))
        //    //    .EnableInterfaceInterceptors();
        //    //builder.RegisterType<AuditLogInterceptor>();
        //}

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
