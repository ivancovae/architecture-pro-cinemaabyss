using events.Services;
using events.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.PortableExecutable;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var bootstrapServers = builder.Configuration.GetValue<string>("Kafka:BootstrapServers");
        builder.Services.AddSingleton<IKafkaProducerService>(new KafkaProducerService(bootstrapServers));

        builder.Services.AddHostedService(service =>
                new KafkaConsumerService(
                    service.GetRequiredService<ILogger<KafkaConsumerService>>(),
                    service.GetRequiredService<IHostApplicationLifetime>(),
                    bootstrapServers
                ));
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        var app = builder.Build();
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}