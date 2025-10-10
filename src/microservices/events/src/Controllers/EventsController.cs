
using events.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace events.Controllers
{
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IKafkaProducerService _producerService;
        private readonly ILogger<EventsController> _logger;

        private string _eventTopicUser = "user-events";
        private string _eventTopicPayment = "payment-events";
        private string _eventTopicMovie = "movie-events";

        public EventsController(ILogger<EventsController> logger, IKafkaProducerService producerService)
        {
            _logger = logger;
            _producerService = producerService;
        }

        [HttpGet("/api/events/health")]
        [Route("/api/events/health")]
        public IActionResult GetHealth()
        {
            return Ok($"Event Service");
        }

        [HttpPost("/api/events/user")]
        [Route("/api/events/user")]
        public async Task<IActionResult> SendMessageUser([FromBody] JObject json)
        {
            await _producerService.SendMessageAsync(_eventTopicUser, $"{json.ToString()}");
            return Ok($"Message '{json.GetValue("user_id")}' sent successfully to topic '{_eventTopicUser}'.");
        }

        [HttpPost("/api/events/payment")]
        [Route("/api/events/payment")]
        public async Task<IActionResult> SendMessagePayment([FromBody] JObject json)
        {
            await _producerService.SendMessageAsync(_eventTopicPayment, $"{json.ToString()}");
            return Ok($"Message '{json.GetValue("payment_id")}' sent successfully to topic '{_eventTopicPayment}'.");
        }
        [HttpPost("/api/events/movie")]
        [Route("/api/events/movie")]
        public async Task<IActionResult> SendMessageMovie([FromBody] JObject json)
        {
            await _producerService.SendMessageAsync(_eventTopicMovie, $"{json.ToString()}");
            return Ok($"Message '{json.GetValue("movie_id")}' sent successfully to topic '{_eventTopicMovie}'.");
        }
    }
}
