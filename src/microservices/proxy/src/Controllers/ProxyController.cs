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
using System.Text.Json.Nodes;
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

        IHttpClientFactory _httpClientFactory;
        IFeatureManager _featureManager;

        public ProxyController(IFeatureManager featureManager, IHttpClientFactory httpClientFactory,  IConfiguration configuration, ILogger<ProxyController> logger)
        {
            _logger = logger;
            _configuration = configuration; 
            _httpClientFactory = httpClientFactory;
            _featureManager = featureManager;

            _moviesServiceURL = _configuration.GetValue<string>("MOVIES_SERVICE_URL") ?? "http://localhost:8081";
            _monolithURL = _configuration.GetValue<string>("MONOLITH_URL") ?? "http://localhost:8080";
            _eventsServiceURL = _configuration.GetValue<string>("EVENTS_SERVICE_URL") ?? "http://localhost:8082";
        }

        [Route("/about")]
        public IResult Getabout()
        {
            return Results.Ok("Proxy server");
        }

        private string GetEventResult()
        {
            JObject dummy = new JObject
            {
                { "status", "success" },
                { "partition", 1 },
                { "offset", 1 }
            };
            JObject dummyEvent = new JObject
            {
                { "id", "id" },
                { "type", "type" },
                { "timestamp", "timestamp" },
                { "payload", "{}" }
            };
            dummy.Add("event", dummyEvent);
            return dummy.ToString(Newtonsoft.Json.Formatting.None);
        }

        [Route("/health")]
        public IResult Get()
        {
            var JSuccess = new JObject() { { "status", true} };
            return Results.Text(JSuccess.ToString(Newtonsoft.Json.Formatting.None), "application/json");
        }
        [HttpGet(Name = "/api/movies")]
        [Route("/api/movies")]
        public async Task<IResult> GetMovies()
        {
            var apiPath = "/api/movies";
            var newAlgorithm = await _featureManager.IsEnabledAsync("MoviesPercentage");
            if (newAlgorithm)
            {
                _logger.Log(LogLevel.Information, $"request to new {_moviesServiceURL}");
                var httpClientNew = _httpClientFactory?.CreateClient();
                httpClientNew.BaseAddress = new Uri(_moviesServiceURL);
                var responseNew = await httpClientNew.GetAsync($"{apiPath}");
                var responseNewBody = await responseNew.Content.ReadAsStringAsync();
                JArray arrayNew = JArray.Parse(responseNewBody);
                return Results.Text(arrayNew.ToString(Newtonsoft.Json.Formatting.None), "application/json");
            }
            _logger.Log(LogLevel.Information, $"request to old {_monolithURL}");
            var httpClient = _httpClientFactory?.CreateClient();
            httpClient.BaseAddress = new Uri(_monolithURL);
            var response = await httpClient.GetAsync($"{apiPath}");
            var responseBody = await response.Content.ReadAsStringAsync();
            JArray array = JArray.Parse(responseBody);
            return Results.Text(array.ToString(Newtonsoft.Json.Formatting.None), "application/json");
        }
        [HttpGet(Name = "/api/movies/health")]
        [Route("/api/movies/health")]
        public async Task<IResult> GetMoviesHealth()
        {
            var httpClient = _httpClientFactory?.CreateClient();
            httpClient.BaseAddress = new Uri(_moviesServiceURL);
            var response = await httpClient.GetAsync($"/api/movies/health");
            var responseBody = await response.Content.ReadAsStringAsync();
            return Results.Text(responseBody, "application/json");
        }
        [HttpGet]
        [Route("/api/users")]
        public async Task<IResult> GetUsers()
        {
            var httpClient = _httpClientFactory?.CreateClient();
            httpClient.BaseAddress = new Uri(_monolithURL);
            var response = await httpClient.GetAsync("/api/users");
            var responseBody = await response.Content.ReadAsStringAsync();
            JArray array = JArray.Parse(responseBody);
            return Results.Text(array.ToString(Newtonsoft.Json.Formatting.None), "application/json");
        }
        [HttpPost]
        [Route("/api/users")]
        public async Task<IResult> PostUsers([FromBody] JObject json)
        {
            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var httpClient = _httpClientFactory?.CreateClient();
            httpClient.BaseAddress = new Uri(_monolithURL);
            var response = await httpClient.PostAsync("/api/users", httpContent);
            var responseBody = await response.Content.ReadAsStringAsync();
            return Results.Json(responseBody);
        }
        [HttpGet]
        [Route("/api/payments")]
        public async Task<IResult> GetPayments([FromQuery]int user_id)
        {
            var httpClient = _httpClientFactory?.CreateClient();
            httpClient.BaseAddress = new Uri(_monolithURL);
            var response = await httpClient.GetAsync($"/api/payments?user_id={user_id}");
            var responseBody = await response.Content.ReadAsStringAsync();
            JArray array = JArray.Parse(responseBody);
            return Results.Json(array.ToString(Newtonsoft.Json.Formatting.None));
        }
        [HttpPost]
        [Route("/api/payments")]
        public async Task<IResult> PostPayments([FromBody] JObject json)
        {
            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var httpClient = _httpClientFactory?.CreateClient();
            httpClient.BaseAddress = new Uri(_monolithURL);
            var response = await httpClient.PostAsync($"/api/payments", httpContent);
            var responseBody = await response.Content.ReadAsStringAsync();
            return Results.Json(responseBody);
        }
        [HttpGet]
        [Route("/api/subscriptions")]
        public async Task<IResult> GetSubscriptions([FromQuery] int user_id)
        {
            var httpClient = _httpClientFactory?.CreateClient();
            httpClient.BaseAddress = new Uri(_monolithURL);
            var response = await httpClient.GetAsync($"/api/subscriptions?user_id={user_id}");
            var responseBody = await response.Content.ReadAsStringAsync();
            JArray array = JArray.Parse(responseBody);
            return Results.Json(array.ToString(Newtonsoft.Json.Formatting.None));
        }
        [HttpPost]
        [Route("/api/subscriptions")]
        public async Task<IResult> PostSubscriptions([FromBody] JObject json)
        {
            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var httpClient = _httpClientFactory?.CreateClient();
            httpClient.BaseAddress = new Uri(_monolithURL);
            var response = await httpClient.PostAsync($"/api/subscriptions", httpContent);
            var responseBody = await response.Content.ReadAsStringAsync();
            return Results.Json(responseBody);
        }
        [HttpGet]
        [Route("/api/events/health")]
        public async Task<IResult> PostEvents()
        {
            var httpClient = _httpClientFactory?.CreateClient();
            httpClient.BaseAddress = new Uri(_eventsServiceURL);
            var response = await httpClient.GetAsync($"/api/events/health");
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                return Results.Json(responseBody);
            }
            return Results.Problem();
        }
        [HttpPost(Name = "/api/events/movie")]
        [Route("/api/events/movie")]
        public async Task<IResult> PostEventsMovie([FromBody] JObject json)
        {
            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var httpClient = _httpClientFactory?.CreateClient();
            httpClient.BaseAddress = new Uri(_eventsServiceURL);
            var response = await httpClient.PostAsync($"/api/events/movie", httpContent);
            var responseBody = await response.Content.ReadAsStringAsync();
            JObject objBody = JObject.Parse(responseBody);
            return Results.Text(GetEventResult(), "application/json");
        }
        [HttpPost(Name = "api/events/payment")]
        [Route("/api/events/payment")]
        public async Task<IResult> PostEventsPayment([FromBody] JObject json)
        {
            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var httpClient = _httpClientFactory?.CreateClient();
            httpClient.BaseAddress = new Uri(_eventsServiceURL);
            var response = await httpClient.PostAsync($"/api/events/payment", httpContent);
            var responseBody = await response.Content.ReadAsStringAsync();
            JObject objBody = JObject.Parse(responseBody);
            return Results.Text(GetEventResult(), "application/json");
        }
        [HttpPost(Name = "/api/events/user")]
        [Route("/api/events/user")]
        public async Task<IResult> PostEventsUser([FromBody] JObject json)
        {
            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var httpClient = _httpClientFactory?.CreateClient();
            httpClient.BaseAddress = new Uri(_eventsServiceURL);
            var response = await httpClient.PostAsync($"/api/events/user", httpContent);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                JObject objBody = JObject.Parse(responseBody);
                return Results.Text(GetEventResult(), "application/json");
            }
            return Results.Problem();
        }
    }
}
