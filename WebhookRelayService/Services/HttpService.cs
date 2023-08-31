using System.Text;

namespace WebhookRelayService.Services
{
    public interface IHttpService
    {
        public Task<HttpResponseMessage> SendGet(string endpoint);
        public Task<HttpResponseMessage> SendPost(string endpoint, string body);
    }

    public class HttpService : IHttpService
    {
        private HttpClient _httpClient;
        private ILogger _logger;

        public HttpService(ILogger<HttpService> logger)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _logger = logger;
        }

        public async Task<HttpResponseMessage> SendGet(string endpoint)
        {
            _logger.LogInformation($"[GET] {endpoint}");
            return await _httpClient.GetAsync(endpoint);
        }

        public async Task<HttpResponseMessage> SendPost(string endpoint, string body)
        {
            _logger.LogInformation($"[POST] {endpoint}");
            var stringContent = new StringContent(body, Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(endpoint, stringContent);
        }
    }
}
