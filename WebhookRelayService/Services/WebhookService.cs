using Sentry;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using WebhookRelayService.Models;
using WebhookRelayService.Repositories;

namespace WebhookRelayService.Services
{
    public interface IWebhookService
    {
        public Task HandleWebhook(int webhookId, Webhook webhook, string payload, string signature);
        public Task<int> PushNotificationAndHandleResult(WebhookUser user, string content);
    }

    public class WebhookService : IWebhookService
    {
        private IWebhookUserRepository _webhookUserRepository;
        private HttpClient _httpClient;
        private Settings _settings;
        private ILogger _logger;

        public WebhookService(IWebhookUserRepository webhookUserRepository, Settings settings, ILogger<WebhookService> logger)
        {
            _webhookUserRepository = webhookUserRepository;
            _httpClient = new HttpClient();
            _settings = settings;
            _logger = logger;
        }

        public async Task HandleWebhook(int webhookId, Webhook webhook, string payload, string signature)
        {
            var user = await _webhookUserRepository.GetByWebhookId(webhookId);

            await ValidateSignature(user.WebhookSecret, payload, signature);

            if (_settings.Logging)
            {
                _logger.LogInformation("Plain", webhook.Notification);
                _logger.LogInformation("Notification", webhook.GetNotificationJson());
            }

            await PushNotificationAndHandleResult(user, webhook.GetNotificationJson());
        }

        public async Task<int> PushNotificationAndHandleResult(WebhookUser user, string content)
        {
            var failed = false;
            try
            {
                var stringContent = new StringContent(content, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(user.NotificationEndpoint, stringContent);
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    failed = true;
                }
            }
            catch
            {
                failed = true;
            }

            if (failed)
            {
                if (_settings.Logging)
                {
                    _logger.LogInformation($"Error when pushing notification for webhook user {user.Id}");
                }
                await _webhookUserRepository.Delete(user);
                return 1;
            }
            return 0;
        }

        private async Task ValidateSignature(string secret, string payload, string signature)
        {
            var payloadBytes = Encoding.UTF8.GetBytes(payload);
            var secretBytes = Encoding.UTF8.GetBytes(secret);
            
            using var stream = new MemoryStream(payloadBytes);
            using var hash = new HMACSHA256(secretBytes);
            var signedBytes = await hash.ComputeHashAsync(stream);
            var signed = BitConverter.ToString(signedBytes).Replace("-", "").ToLowerInvariant();
            var verificationSucceeded = signature == signed;
            if (!verificationSucceeded)
            {
                if (_settings.Logging)
                {
                    _logger.LogInformation("Signature verification failed.");
                }
                else
                {
                    throw new UnauthorizedAccessException();
                }
            }
        }
    }
}
