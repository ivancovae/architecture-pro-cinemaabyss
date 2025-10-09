using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;

namespace proxy
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            configuration = builder.Build();
            Configuration = configuration;

            Console.WriteLine("---=== Parameters StartUp ===---");
            var _port = Configuration.GetValue<string>("PORT") ?? "";
            Console.WriteLine($"PORT={_port}");

            var _monolithServiceURL = Configuration.GetValue<string>("MONOLITH_URL") ?? "";
            Console.WriteLine($"MONOLITH_URL={_monolithServiceURL}");

            var _moviesServiceURL = Configuration.GetValue<string>("MOVIES_SERVICE_URL") ?? "";
            Console.WriteLine($"MOVIES_SERVICE_URL={_moviesServiceURL}");

            var _eventsServiceURL = Configuration.GetValue<string>("EVENTS_SERVICE_URL") ?? "";
            Console.WriteLine($"EVENTS_SERVICE_URL={_eventsServiceURL}");

            var _gradualMigration = Configuration.GetValue<bool>("GRADUAL_MIGRATION");
            Console.WriteLine($"GRADUAL_MIGRATION={_gradualMigration}");

            var _moviesMigrationPercent = Configuration.GetValue<string>("MOVIES_MIGRATION_PERCENT") ?? "";
            Console.WriteLine($"MOVIES_MIGRATION_PERCENT={_moviesMigrationPercent}");
        }

        public IConfiguration Configuration { get; }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Proxy v1");
                });
            }
            app.UseRouting();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
