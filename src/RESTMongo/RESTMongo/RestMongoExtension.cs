using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace RESTMongo
{
    public static class RestMongoExtension
    {
        public static void MapRestMongo(this IEndpointRouteBuilder endpoints, IConfiguration configuration)
        {
            var options = configuration.Get<RestMongoOptions>();
            endpoints.MapGet(options.RoutePrefix + "/{database}/{collection}/{id}", context =>
            {
                var database = context.Request.RouteValues["database"].ToString();
                var collection = context.Request.RouteValues["collection"].ToString();
                var id = context.Request.RouteValues["id"].ToString();
                var repository = new RepositoryBase(options.ConnectionString, database);
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
