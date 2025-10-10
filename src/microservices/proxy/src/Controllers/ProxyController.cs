using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace proxy.Controllers
{
    [ApiController]
    [Route("")]
    public class ProxyController : Controller
    {
        private readonly ILogger<ProxyController> _logger;
        private IConfiguration _configuration;

        private string _moviesServiceURL;
        private string _monolithURL;
        private string _eventsServiceURL;

        public ProxyController(IConfiguration configuration, ILogger<ProxyController> logger)
        {
            _logger = logger;
            _configuration = configuration;

            _moviesServiceURL = _configuration.GetValue<string>("URLs:movies-service") ?? "http://localhost:8081";
            _monolithURL = _configuration.GetValue<string>("URLs:monolith") ?? "http://localhost:8080";
            _eventsServiceURL = _configuration.GetValue<string>("URLs:events-service") ?? "http://localhost:8082";
        }

        [Route("about")]
        public IResult Get()
        {
            return Results.Ok("Proxy server");
        }

        [Route("health")]
        public async Task<HttpResponseMessage> Get(IFeatureManager _featureManager)
        {
            var apiPath = "health/";
            var client = new HttpClient();
            client.BaseAddress = new System.Uri(_monolithURL);
            return await client.GetAsync($"/{apiPath}");
        }
        [HttpGet(Name = "api/movies")]
        [Route("api/movies")]
        [FeatureGate("Movies")]
        public async Task<HttpResponseMessage> GetMovies(IFeatureManager _featureManager)
        {
            var apiPath = "api/movies/";
            var client = new HttpClient();
            var newAlgorithm = await _featureManager.IsEnabledAsync("MoviesPercentageFilter");
            if (newAlgorithm)
            {
                client.BaseAddress = new System.Uri(_moviesServiceURL);
                return await client.GetAsync($"/{apiPath}");
            }

            client.BaseAddress = new System.Uri(_monolithURL);
            return await client.GetAsync($"/{apiPath}");
        }
        [HttpGet(Name = "api/users")]
        [Route("api/users")]
        public async Task<HttpResponseMessage> GetUsers(IFeatureManager _featureManager)
        {
            var apiPath = "api/users/";
            var client = new HttpClient();
            client.BaseAddress = new System.Uri(_monolithURL);
            return await client.GetAsync($"{_monolithURL}/{apiPath}");
        }
        [HttpGet(Name = "api/payments")]
        [Route("api/payments")]
        public async Task<HttpResponseMessage> GetPayments(IFeatureManager _featureManager)
        {
            var apiPath = "api/payments/";
            var client = new HttpClient();
            client.BaseAddress = new System.Uri(_monolithURL);
            return await client.GetAsync($"{_monolithURL}/{apiPath}");
        }
        [HttpGet(Name = "api/subscriptions")]
        [Route("api/subscriptions")]
        public async Task<HttpResponseMessage> GetSubscriptions(IFeatureManager _featureManager)
        {
            var apiPath = "api/subscriptions/";
            var client = new HttpClient();
            client.BaseAddress = new System.Uri(_monolithURL);
            return await client.GetAsync($"{_monolithURL}/{apiPath}");
        }
        [HttpGet(Name = "api/events")]
        [Route("api/events")]
        public async Task<HttpResponseMessage> GetEvents(IFeatureManager _featureManager)
        {
            var apiPath = "api/events/";
            var client = new HttpClient();
            client.BaseAddress = new System.Uri(_eventsServiceURL);
            return await client.GetAsync($"/{apiPath}");
        }
    }
}
