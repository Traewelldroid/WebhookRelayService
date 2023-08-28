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
    }

    public class WebhookService : IWebhookService
    {
        private IWebhookUserRepository _webhookUserRepository;
        private HttpClient _httpClient;

        public WebhookService(IWebhookUserRepository webhookUserRepository)
        {
            _webhookUserRepository = webhookUserRepository;
            _httpClient = new HttpClient();
        }

        public async Task HandleWebhook(int webhookId, Webhook webhook, string payload, string signature)
        {
            var user = await _webhookUserRepository.GetByWebhookId(webhookId);

            await ValidateSignature(user.WebhookSecret, payload, signature);

            var notification = new StringContent(webhook.GetNotificationJson(), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(user.NotificationEndpoint, notification);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                await _webhookUserRepository.Delete(user);
            }
        }

        private async Task ValidateSignature(string secret, string payload, string signature)
        {
            var payloadBytes = Encoding.UTF8.GetBytes(payload);
            var secretBytes = Encoding.UTF8.GetBytes(secret);
            
            using var stream = new MemoryStream(payloadBytes);
            using var hash = new HMACSHA256(secretBytes);
            var signedBytes = await hash.ComputeHashAsync(stream);
            var signed = BitConverter.ToString(signedBytes).Replace("-", "").ToLowerInvariant();
            if (signature != signed)
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}
