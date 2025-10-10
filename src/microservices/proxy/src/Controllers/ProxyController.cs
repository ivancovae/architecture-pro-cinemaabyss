using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
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

        [Route("/about")]
        public IResult Get()
        {
            return Results.Ok("Proxy server");
        }

        [Route("/health")]
        public async Task<HttpResponseMessage> Get(IFeatureManager _featureManager)
        {
            var apiPath = "/health";
            var client = new HttpClient();
            client.BaseAddress = new System.Uri(_monolithURL);
            return await client.GetAsync($"{apiPath}");
        }
        [HttpGet(Name = "/api/movies")]
        [Route("/api/movies")]
        [FeatureGate("Movies")]
        public async Task<IResult> GetMovies(IFeatureManager _featureManager)
        {
            var apiPath = "/api/movies";
            var _moviesServiceURL = _configuration.GetValue<string>("MOVIES_SERVICE_URL") ?? "";
            Console.WriteLine($"MOVIES_SERVICE_URL={_moviesServiceURL}");

            var client = new HttpClient();
            var newAlgorithm = await _featureManager.IsEnabledAsync("MoviesPercentageFilter");
            if (newAlgorithm)
            {
                client.BaseAddress = new System.Uri(_moviesServiceURL);
                var responseNew = await client.GetAsync($"{apiPath}");
                var responseNewBody = await responseNew.Content.ReadAsStringAsync();
                return Results.Json(responseNewBody);
            }

            client.BaseAddress = new System.Uri(_monolithURL);
            var response = await client.GetAsync($"{apiPath}");
            var responseBody = await response.Content.ReadAsStringAsync();
            return Results.Json(responseBody);
        }
        [HttpGet(Name = "/api/users")]
        [Route("/api/users")]
        public async Task<IResult> GetUsers(IFeatureManager _featureManager)
        {
            var apiPath = "/api/users";
            var client = new HttpClient();
            client.BaseAddress = new System.Uri(_monolithURL);
            var response = await client.GetAsync($"{apiPath}");
            var responseBody = await response.Content.ReadAsStringAsync();
            return Results.Json(responseBody);
        }
        [HttpGet(Name = "/api/payments")]
        [Route("/api/payments")]
        public async Task<IResult> GetPayments(IFeatureManager _featureManager)
        {
            var apiPath = "/api/payments";
            var client = new HttpClient();
            client.BaseAddress = new System.Uri(_monolithURL);
            var response = await client.GetAsync($"{apiPath}");
            var responseBody = await response.Content.ReadAsStringAsync();
            return Results.Json(responseBody);
        }
        [HttpGet(Name = "/api/subscriptions")]
        [Route("/api/subscriptions")]
        public async Task<IResult> GetSubscriptions(IFeatureManager _featureManager)
        {
            var apiPath = "/api/subscriptions";
            var client = new HttpClient();
            client.BaseAddress = new System.Uri(_monolithURL);
            var response = await client.GetAsync($"{apiPath}");
            var responseBody = await response.Content.ReadAsStringAsync();
            return Results.Json(responseBody);
        }
        [HttpGet(Name = "/api/events/health")]
        [Route("/api/events/health")]
        public async Task<IResult> PostEvents(IFeatureManager _featureManager)
        {
            var apiPath = "/api/events/health";
            var client = new HttpClient();
            client.BaseAddress = new System.Uri(_eventsServiceURL);
            var response = await client.GetAsync($"{apiPath}");
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                return Results.Json(responseBody);
            }
            return Results.Problem();
        }
        [HttpPost(Name = "/api/events/movie")]
        [Route("/api/events/movie")]
        public async Task<IResult> PostEventsMovie(IFeatureManager _featureManager, [FromBody] JObject json)
        {
            var apiPath = "/api/events/movie";
            var client = new HttpClient();
            client.BaseAddress = new System.Uri(_eventsServiceURL);
            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{apiPath}", httpContent);
            var responseBody = await response.Content.ReadAsStringAsync();
            return Results.Json(responseBody);
        }
        [HttpPost(Name = "api/events/payment")]
        [Route("/api/events/payment")]
        public async Task<IResult> PostEventsPayment(IFeatureManager _featureManager, [FromBody] JObject json)
        {
            var apiPath = "/api/events/payment";
            var client = new HttpClient();
            client.BaseAddress = new System.Uri(_eventsServiceURL);
            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{apiPath}", httpContent);
            var responseBody = await response.Content.ReadAsStringAsync();
            return Results.Json(responseBody);
        }
        [HttpPost(Name = "/api/events/user")]
        [Route("/api/events/user")]
        public async Task<IResult> PostEventsUser([FromBody] JObject json)
        {
            var apiPath = "/api/events/user";
            var client = new HttpClient();
            client.BaseAddress = new System.Uri(_eventsServiceURL);
            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{apiPath}", httpContent);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                return Results.Json(responseBody);
            }
            return Results.Problem();
        }
    }
}
