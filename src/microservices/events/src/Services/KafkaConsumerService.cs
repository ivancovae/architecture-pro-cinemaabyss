using Confluent.Kafka;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace events.Services
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly IConsumer<Null, string> _consumer;
        private readonly IHostApplicationLifetime _lifetime;
        private ILogger<KafkaConsumerService> _logger;

        private string _eventTopicUser = "user-events";
        private string _eventTopicPayment = "payment-events";
        private string _eventTopicMovie = "movie-events";
        
        public KafkaConsumerService(
            ILogger<KafkaConsumerService> logger, 
            IHostApplicationLifetime lifetime, 
            string bootstrapServers
        )
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = "events-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            _consumer = new ConsumerBuilder<Null, string>(config).Build();
            _lifetime = lifetime;
            _logger = logger;
        }
        private void ConsumeMessages(string topic)
        {
            _consumer.Subscribe(topic);
            try
            {
                while (true)
                {
                    var consumeResult = _consumer.Consume();
                    Console.WriteLine($"Consumed message: {consumeResult.Message.Value}");
                }
            }
            catch (ConsumeException e)
            {
                Console.WriteLine($"Error consuming message: {e.Error.Reason}");
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            base.StopAsync(cancellationToken);


            return Task.CompletedTask;
        }


        private void OnStarted()
        {
            
        }

        private void OnStopping()
        {
           
        }

        private void OnStopped()
        {
            
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _lifetime.ApplicationStarted.Register(OnStarted);
            _lifetime.ApplicationStopping.Register(OnStopping);
            _lifetime.ApplicationStopped.Register(OnStopped);

            if (!await WaitForAppStartup(_lifetime, stoppingToken))
                return;

            _consumer.Subscribe(_eventTopicMovie);
            try
            {
                while (true)
                {
                    var consumeResult = _consumer.Consume(stoppingToken);
                    Console.WriteLine($"Consumed message: {consumeResult.Message.Value}");
                }
            }
            catch (ConsumeException e)
            {
                Console.WriteLine($"Error consuming message: {e.Error.Reason}");
            }
        }
        static async Task<bool> WaitForAppStartup(IHostApplicationLifetime lifetime, CancellationToken stoppingToken)
        {
            var startedSource = new TaskCompletionSource();
            using var reg1 = lifetime.ApplicationStarted.Register(() => startedSource.SetResult());

            var cancelledSource = new TaskCompletionSource();
            using var reg2 = stoppingToken.Register(() => cancelledSource.SetResult());

            Task completedTask = await Task.WhenAny(startedSource.Task, cancelledSource.Task).ConfigureAwait(false);
            return completedTask == startedSource.Task;
        }
    }
}
