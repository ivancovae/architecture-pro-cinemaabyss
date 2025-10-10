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

            var _kafkaBrokers = configuration.GetValue<string>("KAFKA_BROKERS") ?? "";
            Console.WriteLine($"KAFKA_BROKERS={_kafkaBrokers}");
            var jbootstrapServers = new JObject();
            jbootstrapServers.Add("BootstrapServers", _kafkaBrokers);
            configuration["Kafka"] = jbootstrapServers.ToString();
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
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Events v1");
                });
            }
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
