namespace events.Services.Interfaces
{
    public interface IKafkaProducerService
    {
        Task SendMessageAsync(string topic, string message);
    }
}
