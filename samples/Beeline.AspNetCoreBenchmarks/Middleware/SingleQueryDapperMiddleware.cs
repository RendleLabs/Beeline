using System;
using System.Threading.Tasks;
using Beeline.AspNetCoreBenchmarks.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Beeline.AspNetCoreBenchmarks.Middleware
{
    public class SingleQueryDapperMiddleware
    {
        private static readonly PathString _path = new PathString("/db/dapper");
        private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private readonly RequestDelegate _next;

        public SingleQueryDapperMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path.StartsWithSegments(_path, StringComparison.Ordinal))
            {
                var db = httpContext.RequestServices.GetService<DapperDb>();
                var row = await db.LoadSingleQueryRow();

                var result = JsonConvert.SerializeObject(row, _jsonSettings);

                httpContext.Response.StatusCode = StatusCodes.Status200OK;
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.ContentLength = result.Length;

                await httpContext.Response.WriteAsync(result);

                return;
            }

            await _next(httpContext);
        }
    }

    public static class SingleQueryDapperMiddlewareExtensions
    {
        public static IApplicationBuilder UseSingleQueryDapper(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SingleQueryDapperMiddleware>();
        }
    }
}