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

            _moviesServiceURL = _configuration.GetValue<string>("URLs:movies-service") ?? "";
            _monolithURL = _configuration.GetValue<string>("URLs:monolith") ?? "";
            _eventsServiceURL = _configuration.GetValue<string>("URLs:events-service") ?? "";
        }

        [Route("health")]
        public async Task<HttpResponseMessage> Get(IFeatureManager _featureManager)
        {
            var apiPath = "health/";
            var client = new HttpClient();
            return await client.GetAsync($"{_monolithURL}/{apiPath}");
        }
        [HttpGet(Name = "api/movies")]
        [Route("api/movies")]
        [FeatureGate("Movies")]
        public async Task<HttpResponseMessage> GetMovies(IFeatureManager _featureManager)
        {
            var apiPath = "api/movies/";
            var newAlgorithm = await _featureManager.IsEnabledAsync("MoviesPercentageFilter");
            if (newAlgorithm)
            {
                var clientNew = new HttpClient();
                return await clientNew.GetAsync($"{_moviesServiceURL}/{apiPath}");
            }

            var client = new HttpClient();
            return await client.GetAsync($"{_monolithURL}/{apiPath}");
        }
        [HttpGet(Name = "api/users")]
        [Route("api/users")]
        public async Task<HttpResponseMessage> GetUsers(IFeatureManager _featureManager)
        {
            var apiPath = "api/users/";
            var client = new HttpClient();
            return await client.GetAsync($"{_monolithURL}/{apiPath}");
        }
        [HttpGet(Name = "api/payments")]
        [Route("api/payments")]
        public async Task<HttpResponseMessage> GetPayments(IFeatureManager _featureManager)
        {
            var apiPath = "api/payments/";
            var client = new HttpClient();
            return await client.GetAsync($"{_monolithURL}/{apiPath}");
        }
        [HttpGet(Name = "api/subscriptions")]
        [Route("api/subscriptions")]
        public async Task<HttpResponseMessage> GetSubscriptions(IFeatureManager _featureManager)
        {
            var apiPath = "api/subscriptions/";
            var client = new HttpClient();
            return await client.GetAsync($"{_monolithURL}/{apiPath}");
        }
        [HttpGet(Name = "api/events")]
        [Route("api/events")]
        public async Task<HttpResponseMessage> GetEvents(IFeatureManager _featureManager)
        {
            var apiPath = "api/events/";
            var client = new HttpClient();
            return await client.GetAsync($"{_eventsServiceURL}/{apiPath}");
        }
    }
}
