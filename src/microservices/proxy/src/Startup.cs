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
            var _port = configuration.GetValue<string>("PORT") ?? "";
            Console.WriteLine($"PORT={_port}");

            var _monolithServiceURL = configuration.GetValue<string>("MONOLITH_URL") ?? "http://localhost:8080";
            Console.WriteLine($"MONOLITH_URL={_monolithServiceURL}");

            var _moviesServiceURL = configuration.GetValue<string>("MOVIES_SERVICE_URL") ?? "http://localhost:8081";
            Console.WriteLine($"MOVIES_SERVICE_URL={_moviesServiceURL}");

            var _eventsServiceURL = configuration.GetValue<string>("EVENTS_SERVICE_URL") ?? "http://localhost:8082";
            Console.WriteLine($"EVENTS_SERVICE_URL={_eventsServiceURL}");

            var _gradualMigration = configuration.GetValue<string>("GRADUAL_MIGRATION") ?? "false";
            Console.WriteLine($"GRADUAL_MIGRATION={_gradualMigration}");

            var _moviesMigrationPercent = configuration.GetValue<string>("MOVIES_MIGRATION_PERCENT") ?? "50";
            Console.WriteLine($"MOVIES_MIGRATION_PERCENT={_moviesMigrationPercent}");

            var parameters = new JObject();
            parameters.Add("Value", _moviesMigrationPercent);
            var percentage = new JObject();
            percentage.Add("Name", "Microsoft.Percentage");
            percentage.Add("Parameters", parameters);

            var listParams = new JArray();
            listParams.Add(percentage);
            var moviesPercentageFilter = new JObject();
            moviesPercentageFilter.Add("EnabledFor", listParams);

            JObject featureFlagsSection = new JObject();
            featureFlagsSection.Add("Movies", _gradualMigration);
            featureFlagsSection.Add("MoviesPercentageFilter", moviesPercentageFilter);

            configuration["FeatureFlags"] = featureFlagsSection.ToString();
            configuration["MONOLITH_URL"] = _monolithServiceURL;
            configuration["MOVIES_SERVICE_URL"] = _moviesServiceURL;
            configuration["EVENTS_SERVICE_URL"] = _eventsServiceURL;
            configuration["GRADUAL_MIGRATION"] = _gradualMigration;
            configuration["MOVIES_MIGRATION_PERCENT"] = _moviesMigrationPercent;
        }

        public IConfiguration Configuration { get; private set; }

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
