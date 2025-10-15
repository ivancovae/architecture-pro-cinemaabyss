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
            //Environment.SetEnvironmentVariable("GRADUAL_MIGRATION", "true");
            //Environment.SetEnvironmentVariable("MOVIES_MIGRATION_PERCENT", "100");

            var gradualMigration = Environment.GetEnvironmentVariable("GRADUAL_MIGRATION");
            if (gradualMigration == "true")
            {
                var moviesMigrationPercent = Environment.GetEnvironmentVariable("MOVIES_MIGRATION_PERCENT");
                Environment.SetEnvironmentVariable("FeatureFlags__MoviesPercentage__EnabledFor__0__Name", "Microsoft.Percentage");
                Environment.SetEnvironmentVariable("FeatureFlags__MoviesPercentage__EnabledFor__0__Parameters__Value", moviesMigrationPercent);
            }
            
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

                    Console.WriteLine($"FeatureFlags={configuration.GetSection("FeatureFlags")}");
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://0.0.0.0:8000");
                })
            .ConfigureServices((builder, services) =>
                {
                    services
                        .AddFeatureManagement(builder.Configuration.GetSection("FeatureFlags"))
                        .AddFeatureFilter<PercentageFilter>();
                    services.AddHttpClient();
                    services.AddControllers().AddNewtonsoftJson();
                    services.AddEndpointsApiExplorer();
                    services.AddSwaggerGen();
                });
    }
}