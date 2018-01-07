using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Beeline.AspNetCoreBenchmarks.Configuration;
using Beeline.AspNetCoreBenchmarks.Data;
using Beeline.AspNetCoreBenchmarks.Middleware;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Beeline.AspNetCoreBenchmarks
{
    [UsedImplicitly]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            DatabaseInitializer.Initialize(Configuration.GetValue<string>("ConnectionString"));
            services.Configure<AppSettings>(Configuration);
            services.AddSingleton<IRandom, DefaultRandom>();
            services.AddSingleton<DbProviderFactory>(NpgsqlFactory.Instance);
            services.AddSingleton<BeelineDb>();
            services.AddSingleton<DapperDb>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSingleQueryDapper();
            app.UseSingleQueryBeeline();
        }
    }
}
