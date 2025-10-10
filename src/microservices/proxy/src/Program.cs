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
            .ConfigureServices((builder, services) =>
                {
                    services.AddFeatureManagement(builder.Configuration.GetSection("FeatureFlags"))
                            .AddFeatureFilter<PercentageFilter>()
                            .AddFeatureFilter<TimeWindowFilter>();

                    services.AddControllers().AddNewtonsoftJson();
                    services.AddEndpointsApiExplorer();
                    services.AddSwaggerGen();
                });
    }
}