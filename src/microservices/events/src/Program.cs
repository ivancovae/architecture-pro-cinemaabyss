using events.Services;
using events.Services.Interfaces;
using Newtonsoft.Json.Linq;

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
                    webBuilder.UseUrls($"http://0.0.0.0:8082");
                    webBuilder.UseStartup<Startup>();
                })
            .ConfigureServices((builder, services) =>
            {
                var _port = builder.Configuration.GetValue<string>("PORT") ?? "8082";
                var _kafkaBrokers = builder.Configuration.GetValue<string>("KAFKA_BROKERS") ?? "localhost:9092";

                var jbootstrapServers = new JObject();
                jbootstrapServers.Add("BootstrapServers", _kafkaBrokers);
                builder.Configuration["Kafka"] = jbootstrapServers.ToString();
                builder.Configuration["Kafka:BootstrapServers"] = _kafkaBrokers;

                var bootstrapServers = builder.Configuration.GetValue<string>("Kafka:BootstrapServers") ?? "localhost:9092";
                services.AddSingleton<IKafkaProducerService>(new KafkaProducerService(bootstrapServers));

                services.AddHostedService(service =>
                        new KafkaConsumerService(
                            service.GetRequiredService<ILogger<KafkaConsumerService>>(),
                            service.GetRequiredService<IHostApplicationLifetime>(),
                            bootstrapServers
                        ));
                services.AddControllers().AddNewtonsoftJson();
                services.AddEndpointsApiExplorer();
                services.AddSwaggerGen();
            });
    }
}