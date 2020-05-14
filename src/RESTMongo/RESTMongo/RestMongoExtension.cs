using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace RESTMongo
{
    public static class RestMongoExtension
    {
        public static IEndpointConventionBuilder MapRestMongo(this IEndpointRouteBuilder endpoints, IConfiguration configuration)
        {
            endpoints.MapGet("/mongo/{database}/{collection}/{id}", context =>
            {
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
    }
}
