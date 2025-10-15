using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection.PortableExecutable;

namespace proxy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(configBuilder =>
                {
                    var configuration = configBuilder.Build();

                    var _gradualMigration = configuration.GetValue<string>("GRADUAL_MIGRATION");
                    Console.WriteLine($"GRADUAL_MIGRATION={_gradualMigration}");

                    var _moviesMigrationPercent = configuration.GetValue<string>("MOVIES_MIGRATION_PERCENT");
                    Console.WriteLine($"MOVIES_MIGRATION_PERCENT={_moviesMigrationPercent}");

                    var parameters = new JObject
                            {
                                { "Value", _moviesMigrationPercent }
                            };
                    var percentage = new JObject
                            {
                                { "Name", "Microsoft.Percentage" },
                                { "Parameters", parameters }
                            };

                    var listParams = new JArray
                            {
                                percentage
                            };
                    var moviesPercentageFilter = new JObject
                            {
                                { "EnabledFor", listParams }
                            };

                    JObject featureFlagsSection = new JObject
                            {
                                { "Movies", _gradualMigration },
                                { "MoviesPercentageFilter", moviesPercentageFilter }
                            };

                    configuration["FeatureFlags"] = featureFlagsSection.ToString(Newtonsoft.Json.Formatting.None);
                    Console.WriteLine($"FeatureFlags={configuration["FeatureFlags"]}");
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://0.0.0.0:8000");
                })
            .ConfigureServices((builder, services) =>
                {
                    var featureFlags = builder.Configuration.GetSection("FeatureFlags");
                    services.AddFeatureManagement(featureFlags)
                            .AddFeatureFilter<PercentageFilter>()
                            .AddFeatureFilter<TimeWindowFilter>();
                    services.AddHttpClient();
                    services.AddControllers().AddNewtonsoftJson();
                    services.AddEndpointsApiExplorer();
                    services.AddSwaggerGen();
                });
    }
}