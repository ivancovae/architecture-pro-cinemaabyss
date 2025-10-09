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
                BootstrapServers = bootstrapServers
            };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task SendMessageAsync(string topic, string message)
        {
            try
            {
                await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
                Console.WriteLine($"Message '{message}' sent to topic '{topic}'.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message to Kafka: {ex.Message}");
                throw;
            }
        }
    }
}
