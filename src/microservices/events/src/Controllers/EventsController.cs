using Confluent.Kafka;
using events.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace events.Controllers
{
    [ApiController]
    [Route("api")]
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

        [HttpGet("/about")]
        [Route("/about")]
        public IActionResult GetAbout()
        {
            return Ok($"Event Service");
        }

        [HttpPost("/user")]
        [Route("/user")]
        public async Task<IActionResult> SendMessageUser([FromQuery] string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return BadRequest("'message' query parameters are required.");
            }
            await _producerService.SendMessageAsync(_eventTopicUser, message);
            return Ok($"Message '{message}' sent successfully to topic '{_eventTopicUser}'.");
        }

        [HttpPost("/payment")]
        [Route("/payment")]
        public async Task<IActionResult> SendMessagePayment([FromQuery] string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return BadRequest("'message' query parameters are required.");
            }
            await _producerService.SendMessageAsync(_eventTopicPayment, message);
            return Ok($"Message '{message}' sent successfully to topic '{_eventTopicPayment}'.");
        }
        [HttpPost("/movie")]
        [Route("/movie")]
        public async Task<IActionResult> SendMessageMovie([FromQuery] string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return BadRequest("'message' query parameters are required.");
            }
            await _producerService.SendMessageAsync(_eventTopicMovie, message);
            return Ok($"Message '{message}' sent successfully to topic '{_eventTopicMovie}'.");
        }
    }
}
