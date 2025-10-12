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

        public ProxyController(IHttpClientFactory httpClientFactory,  IConfiguration configuration, ILogger<ProxyController> logger)
        {
            _logger = logger;
            _configuration = configuration; 
            _httpClientFactory = httpClientFactory;

            _moviesServiceURL = _configuration.GetValue<string>("MONOLITH_URL") ?? "http://localhost:8081";
            _monolithURL = _configuration.GetValue<string>("MOVIES_SERVICE_URL") ?? "http://localhost:8080";
            _eventsServiceURL = _configuration.GetValue<string>("EVENTS_SERVICE_URL") ?? "http://localhost:8082";
        }

        [Route("/about")]
        public IResult Getabout()
        {
            return Results.Ok("Proxy server");
        }

        [Route("/health")]
        public async Task<IResult> Get()
        {
            var apiPath = "/health";
            var httpClient = _httpClientFactory?.CreateClient();
            var moviesServiceURL = _configuration.GetValue<string>("MONOLITH_URL") ?? "http://localhost:8081";
            httpClient.BaseAddress = new Uri(moviesServiceURL);
            var response = await httpClient.GetAsync($"{apiPath}");
            var responseBody = await response.Content.ReadAsStringAsync();
            return Results.Json(responseBody);
        }
        [HttpGet(Name = "/api/movies")]
        [Route("/api/movies")]
        [FeatureGate("Movies")]
        public async Task<IResult> GetMovies(IFeatureManager _featureManager)
        {
            var apiPath = "/api/movies";
            var _moviesServiceURL = _configuration.GetValue<string>("MOVIES_SERVICE_URL") ?? "";
            Console.WriteLine($"MOVIES_SERVICE_URL={_moviesServiceURL}");
            var newAlgorithm = await _featureManager.IsEnabledAsync("MoviesPercentageFilter");
            if (newAlgorithm)
            {
                var httpClientNew = _httpClientFactory?.CreateClient();
                var newMoviesServiceURL = _configuration.GetValue<string>("MOVIES_SERVICE_URL") ?? "http://localhost:8081";
                httpClientNew.BaseAddress = new Uri(newMoviesServiceURL);
                var responseNew = await httpClientNew.GetAsync($"{apiPath}");
                var responseNewBody = await responseNew.Content.ReadAsStringAsync();
                JArray arrayNew = JArray.Parse(responseNewBody);
                return Results.Text(arrayNew.ToString(Newtonsoft.Json.Formatting.None), "application/json");
            }
            var httpClient = _httpClientFactory?.CreateClient();
            var moviesServiceURL = _configuration.GetValue<string>("MONOLITH_URL") ?? "http://localhost:8081";
            httpClient.BaseAddress = new Uri(moviesServiceURL);
            var response = await httpClient.GetAsync($"{apiPath}");
            var responseBody = await response.Content.ReadAsStringAsync();
            JArray array = JArray.Parse(responseBody);
            return Results.Text(array.ToString(Newtonsoft.Json.Formatting.None), "application/json");
        }
        [HttpGet(Name = "/api/movies/health")]
        [Route("/api/movies/health")]
        [FeatureGate("Movies")]
        public async Task<IResult> GetMoviesHealth(IFeatureManager _featureManager)
        {
            var apiPath = "/api/movies/health";
            var httpClient = _httpClientFactory?.CreateClient();
            var moviesServiceURL = _configuration.GetValue<string>("MONOLITH_URL") ?? "http://localhost:8081";
            httpClient.BaseAddress = new Uri(moviesServiceURL);
            var response = await httpClient.GetAsync($"{apiPath}");
            var responseBody = await response.Content.ReadAsStringAsync();
            return Results.Json(responseBody);
        }
        [HttpGet]
        [Route("/api/users")]
        public async Task<IResult> GetUsers()
        {
            var apiPath = "/api/users";
            var httpClient = _httpClientFactory?.CreateClient();
            var moviesServiceURL = _configuration.GetValue<string>("MONOLITH_URL") ?? "http://localhost:8081";
            httpClient.BaseAddress = new Uri(moviesServiceURL);
            var response = await httpClient.GetAsync($"{apiPath}");
            var responseBody = await response.Content.ReadAsStringAsync();
            JArray array = JArray.Parse(responseBody);
            return Results.Text(array.ToString(Newtonsoft.Json.Formatting.None), "application/json");
        }
        [HttpPost]
        [Route("/api/users")]
        public async Task<IResult> PostUsers([FromBody] JObject json)
        {
            var apiPath = "/api/users";
            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var httpClient = _httpClientFactory?.CreateClient();
            var moviesServiceURL = _configuration.GetValue<string>("MONOLITH_URL") ?? "http://localhost:8081";
            httpClient.BaseAddress = new Uri(moviesServiceURL);
            var response = await httpClient.PostAsync($"{apiPath}", httpContent);
            var responseBody = await response.Content.ReadAsStringAsync();
            return Results.Json(responseBody);
        }
        [HttpGet]
        [Route("/api/payments")]
        public async Task<IResult> GetPayments([FromQuery]int user_id)
        {
            var apiPath = "/api/payments";
            var httpClient = _httpClientFactory?.CreateClient();
            var moviesServiceURL = _configuration.GetValue<string>("MONOLITH_URL") ?? "http://localhost:8081";
            httpClient.BaseAddress = new Uri(moviesServiceURL);
            var response = await httpClient.GetAsync($"{apiPath}?user_id={user_id}");
            var responseBody = await response.Content.ReadAsStringAsync();
            JArray array = JArray.Parse(responseBody);
            return Results.Json(array.ToString(Newtonsoft.Json.Formatting.None));
        }
        [HttpPost]
        [Route("/api/payments")]
        public async Task<IResult> PostPayments([FromBody] JObject json)
        {
            var apiPath = "/api/payments";
            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var httpClient = _httpClientFactory?.CreateClient();
            var moviesServiceURL = _configuration.GetValue<string>("MONOLITH_URL") ?? "http://localhost:8081";
            httpClient.BaseAddress = new Uri(moviesServiceURL);
            var response = await httpClient.PostAsync($"{apiPath}", httpContent);
            var responseBody = await response.Content.ReadAsStringAsync();
            return Results.Json(responseBody);
        }
        [HttpGet]
        [Route("/api/subscriptions")]
        public async Task<IResult> GetSubscriptions([FromQuery] int user_id)
        {
            var apiPath = "/api/subscriptions";
            var httpClient = _httpClientFactory?.CreateClient();
            var moviesServiceURL = _configuration.GetValue<string>("MONOLITH_URL") ?? "http://localhost:8081";
            httpClient.BaseAddress = new Uri(moviesServiceURL);
            var response = await httpClient.GetAsync($"{apiPath}?user_id={user_id}");
            var responseBody = await response.Content.ReadAsStringAsync();
            JArray array = JArray.Parse(responseBody);
            return Results.Json(array.ToString(Newtonsoft.Json.Formatting.None));
        }
        [HttpPost]
        [Route("/api/subscriptions")]
        public async Task<IResult> PostSubscriptions([FromBody] JObject json)
        {
            var apiPath = "/api/subscriptions";
            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var httpClient = _httpClientFactory?.CreateClient();
            var moviesServiceURL = _configuration.GetValue<string>("MONOLITH_URL") ?? "http://localhost:8081";
            httpClient.BaseAddress = new Uri(moviesServiceURL);
            var response = await httpClient.PostAsync($"{apiPath}", httpContent);
            var responseBody = await response.Content.ReadAsStringAsync();
            return Results.Json(responseBody);
        }
        [HttpGet]
        [Route("/api/events/health")]
        public async Task<IResult> PostEvents()
        {
            var apiPath = "/api/events/health";
            var httpClient = _httpClientFactory?.CreateClient();
            var moviesServiceURL = _configuration.GetValue<string>("EVENTS_SERVICE_URL") ?? "http://localhost:8081";
            httpClient.BaseAddress = new Uri(moviesServiceURL);
            var response = await httpClient.GetAsync($"{apiPath}");
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
            var apiPath = "/api/events/movie";
            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var httpClient = _httpClientFactory?.CreateClient();
            var moviesServiceURL = _configuration.GetValue<string>("EVENTS_SERVICE_URL") ?? "http://localhost:8081";
            httpClient.BaseAddress = new Uri(moviesServiceURL);
            var response = await httpClient.PostAsync($"{apiPath}", httpContent);
            var responseBody = await response.Content.ReadAsStringAsync();
            JObject objBody = JObject.Parse(responseBody);

            JObject dummy = new JObject();
            JObject dummyEvent = new JObject();
            dummyEvent.Add("id", "id");
            dummyEvent.Add("type", "type");
            dummyEvent.Add("timestamp", "timestamp");
            dummyEvent.Add("payload", "{}");

            dummy.Add("status", "success");
            dummy.Add("partition", 1);
            dummy.Add("offset", 1);
            dummy.Add("event", dummyEvent.ToString(Newtonsoft.Json.Formatting.None));

            return Results.Json(dummy.ToString(Newtonsoft.Json.Formatting.None));
        }
        [HttpPost(Name = "api/events/payment")]
        [Route("/api/events/payment")]
        public async Task<IResult> PostEventsPayment([FromBody] JObject json)
        {
            var apiPath = "/api/events/payment";
            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var httpClient = _httpClientFactory?.CreateClient();
            var moviesServiceURL = _configuration.GetValue<string>("EVENTS_SERVICE_URL") ?? "http://localhost:8081";
            httpClient.BaseAddress = new Uri(moviesServiceURL);
            var response = await httpClient.PostAsync($"{apiPath}", httpContent);
            var responseBody = await response.Content.ReadAsStringAsync();
            JObject objBody = JObject.Parse(responseBody);

            JObject dummy = new JObject();
            JObject dummyEvent = new JObject();
            dummyEvent.Add("id", "id");
            dummyEvent.Add("type", "type");
            dummyEvent.Add("timestamp", "timestamp");
            dummyEvent.Add("payload", "{}");

            dummy.Add("status", "success");
            dummy.Add("partition", 1);
            dummy.Add("offset", 1);
            dummy.Add("event", dummyEvent.ToString(Newtonsoft.Json.Formatting.None));

            return Results.Json(dummy.ToString(Newtonsoft.Json.Formatting.None));
        }
        [HttpPost(Name = "/api/events/user")]
        [Route("/api/events/user")]
        public async Task<IResult> PostEventsUser([FromBody] JObject json)
        {
            var apiPath = "/api/events/user";
            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var httpClient = _httpClientFactory?.CreateClient();
            var moviesServiceURL = _configuration.GetValue<string>("EVENTS_SERVICE_URL") ?? "http://localhost:8081";
            httpClient.BaseAddress = new Uri(moviesServiceURL);
            var response = await httpClient.PostAsync($"{apiPath}", httpContent);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                JObject objBody = JObject.Parse(responseBody);
                
                JObject dummy = new JObject();
                JObject dummyEvent = new JObject();
                dummyEvent.Add("id", "id");
                dummyEvent.Add("type", "type");
                dummyEvent.Add("timestamp", "timestamp");
                dummyEvent.Add("payload", "{}");

                dummy.Add("status", "success");
                dummy.Add("partition", 1);
                dummy.Add("offset", 1);
                dummy.Add("event", dummyEvent.ToString(Newtonsoft.Json.Formatting.None));

                return Results.Json(dummy.ToString(Newtonsoft.Json.Formatting.None));
            }
            return Results.Problem();
        }
    }
}
