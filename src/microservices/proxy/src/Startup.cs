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

            var _monolithServiceURL = configuration.GetValue<string>("MONOLITH_URL");
            Console.WriteLine($"MONOLITH_URL={_monolithServiceURL}");

            var _moviesServiceURL = configuration.GetValue<string>("MOVIES_SERVICE_URL");
            Console.WriteLine($"MOVIES_SERVICE_URL={_moviesServiceURL}");

            var _eventsServiceURL = configuration.GetValue<string>("EVENTS_SERVICE_URL");
            Console.WriteLine($"EVENTS_SERVICE_URL={_eventsServiceURL}");
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
