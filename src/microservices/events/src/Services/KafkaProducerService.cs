using Confluent.Kafka;
using events.Services.Interfaces;

namespace events.Services
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly IProducer<Null, string> _producer;

        public KafkaProducerService(string bootstrapServers)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers,
                ClientId = "kafka-producer"
            };
            var builder = new ProducerBuilder<Null, string>(config);

            _producer = builder.Build();
        }

        public async Task SendMessageAsync(string topic, string message)
        {
            try
            {
                await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
                Console.WriteLine($"Message '{message}' sent to topic '{topic}'.");
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                throw;
            }
        }
    }
}
