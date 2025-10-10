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
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls($"http://0.0.0.0:8000");
                    webBuilder.UseStartup<Startup>();
                })
            .ConfigureServices((services) =>
                {
                    IConfiguration configuration = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddEnvironmentVariables()
                        .Build();

                    Console.WriteLine("---=== Parameters StartUp ===---");
                    var _port = configuration.GetValue<string>("PORT") ?? "";
                    Console.WriteLine($"PORT={_port}");

                    var _monolithServiceURL = configuration.GetValue<string>("MONOLITH_URL") ?? "";
                    Console.WriteLine($"MONOLITH_URL={_monolithServiceURL}");

                    var _moviesServiceURL = configuration.GetValue<string>("MOVIES_SERVICE_URL") ?? "";
                    Console.WriteLine($"MOVIES_SERVICE_URL={_moviesServiceURL}");

                    var _eventsServiceURL = configuration.GetValue<string>("EVENTS_SERVICE_URL") ?? "";
                    Console.WriteLine($"EVENTS_SERVICE_URL={_eventsServiceURL}");

                    var _gradualMigration = configuration.GetValue<string>("GRADUAL_MIGRATION") ?? "";
                    Console.WriteLine($"GRADUAL_MIGRATION={_gradualMigration}");

                    var _moviesMigrationPercent = configuration.GetValue<string>("MOVIES_MIGRATION_PERCENT") ?? "";
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

                    services.AddFeatureManagement(configuration.GetSection("FeatureFlags"))
                            .AddFeatureFilter<PercentageFilter>()
                            .AddFeatureFilter<TimeWindowFilter>();

                    services.AddControllers();
                    services.AddEndpointsApiExplorer();
                    services.AddSwaggerGen();
                });
    }
}