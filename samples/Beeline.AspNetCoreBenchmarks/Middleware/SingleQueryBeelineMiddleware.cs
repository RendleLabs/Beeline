using System;
using System.Buffers;
using System.Threading.Tasks;
using Beeline.AspNetCoreBenchmarks.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Beeline.AspNetCoreBenchmarks.Middleware
{
    public class SingleQueryBeelineMiddleware
    {
        private static readonly PathString Path = new PathString("/db/beeline");

        private readonly RequestDelegate _next;

        public SingleQueryBeelineMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path.StartsWithSegments(Path, StringComparison.Ordinal))
            {
                var db = httpContext.RequestServices.GetService<BeelineDb>();
                var buffer = ArrayPool<byte>.Shared.Rent(128);
                try
                {
                    var length = await db.LoadSingleQueryRow(buffer, httpContext.RequestAborted);

                    httpContext.Response.StatusCode = StatusCodes.Status200OK;
                    httpContext.Response.ContentType = "application/json";
                    httpContext.Response.ContentLength = length;
                    await httpContext.Response.Body.WriteAsync(buffer, 0, length, httpContext.RequestAborted);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }

                return;
            }

            await _next(httpContext);
        }
    }
    
    public static class SingleQueryBeelineMiddlewareExtensions
    {
        public static IApplicationBuilder UseSingleQueryBeeline(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SingleQueryBeelineMiddleware>();
        }
    }
}