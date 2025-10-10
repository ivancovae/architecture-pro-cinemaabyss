using events.Services;
using events.Services.Interfaces;
using Newtonsoft.Json.Linq;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var _port = builder.Configuration.GetValue<string>("PORT") ?? "8082";
        var _kafkaBrokers = builder.Configuration.GetValue<string>("KAFKA_BROKERS") ?? "localhost:9092";

        var jbootstrapServers = new JObject();
        jbootstrapServers.Add("BootstrapServers", _kafkaBrokers);
        builder.Configuration["Kafka"] = jbootstrapServers.ToString();

        var bootstrapServers = builder.Configuration.GetValue<string>("Kafka:BootstrapServers") ?? "localhost:9092";
        /*builder.Services.AddSingleton<IKafkaProducerService>(new KafkaProducerService(bootstrapServers));*/
        
        /*builder.Services.AddHostedService(service =>
                new KafkaConsumerService(
                    service.GetRequiredService<ILogger<KafkaConsumerService>>(),
                    service.GetRequiredService<IHostApplicationLifetime>(),
                    bootstrapServers
                ));*/
        builder.Services.AddControllers().AddNewtonsoftJson();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        var app = builder.Build();
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseRouting();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}