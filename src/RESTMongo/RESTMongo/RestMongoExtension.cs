using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using MongoDB.Driver;
using Microsoft.Extensions.DependencyInjection;

namespace RESTMongo
{
    public static class RestMongoExtension
    {
        public static void MapRestMongo(this IEndpointRouteBuilder endpoints, IConfiguration configuration)
        {
            var mongoOptions = configuration.Get<RestMongoOptions>();
            var mongoRepository = 
            endpoints.Map(mongoOptions.RoutePrefix, context =>
            {
                var mongoRepository =context.RequestServices.GetRequiredService<MON>
            });
            endpoints.MapGet(mongoOptions.RoutePrefix + "/{database}/{collection}/{id}", context =>
            {
                var database = context.Request.RouteValues["database"].ToString();
                var collection = context.Request.RouteValues["collection"].ToString();
                var id = context.Request.RouteValues["id"].ToString();
                var repository = new MongoRepository(mongoOptions.ConnectionString, database);
                var repository.GetById(collection,id)
                return Task.CompletedTask;
            });

            endpoints.MapPost("/mongo/{database}/{collection}", context =>
            {
                return Task.CompletedTask;
            });

            endpoints.MapPut("/mongo/{database}/{collection}/{id}", context =>
            {
                return Task.CompletedTask;
            });

            endpoints.MapDelete("/mongo/{database}/{collection}/{id}", context =>
            {
                return Task.CompletedTask;
            });

            endpoints.MapDelete("/mongo/{database}/{collection}/query", context =>
            {
                return Task.CompletedTask;
            });
        }

        public static void MapRestMongo(this IEndpointRouteBuilder endpoints, IConfiguration configuration, Action<RestMongoOptions> action)
        {

        }
    }
}
