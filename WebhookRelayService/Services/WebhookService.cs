﻿using Sentry;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using WebhookRelayService.Models;
using WebhookRelayService.Repositories;

namespace WebhookRelayService.Services
{
    public interface IWebhookService
    {
        public Task<string> HandleWebhook(int webhookId, Webhook webhook, string payload, string signature);
        public Task<int> PushNotificationAndHandleResult(WebhookUser user, string content);
    }

    public class WebhookService : IWebhookService
    {
        private IWebhookUserRepository _webhookUserRepository;
        private Settings _settings;
        private ILogger _logger;
        private IHttpService _httpService;

        public WebhookService(IWebhookUserRepository webhookUserRepository, Settings settings, ILogger<WebhookService> logger, IHttpService httpService)
        {
            _webhookUserRepository = webhookUserRepository;
            _settings = settings;
            _logger = logger;
            _httpService = httpService;
        }

        public async Task<string> HandleWebhook(int webhookId, Webhook webhook, string payload, string signature)
        {
            try
            {
                var user = await _webhookUserRepository.GetByWebhookId(webhookId);
                if (user == null)
                {
                    return "gone";
                }

                if (!_settings.SkipSignatureCheck)
                    await ValidateSignature(user.WebhookSecret, payload, signature);
                else
                    _logger.LogInformation("Skipped signature check");

                if (_settings.Logging)
                {
                    _logger.LogInformation($"Notification {webhook.GetNotificationJson()}");
                }

                await PushNotificationAndHandleResult(user, webhook.GetNotificationJson());
                return "ok";
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return "error";
            }
        }

        public async Task<int> PushNotificationAndHandleResult(WebhookUser user, string content)
        {
            var failed = false;
            try
            {
                var httpResponse = await _httpService.SendPost(user.NotificationEndpoint, content);
                if (httpResponse.StatusCode == HttpStatusCode.NotFound)
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
                    _logger.LogError("Signature verification failed.");
                    _logger.LogError($"Signature: {signature}");
                    _logger.LogError($"Calculated Signature: {signed}");
                    _logger.LogError($"Secret: {secret}");
                    _logger.LogError($"Payload: {payload}");
                }
                else
                {
                    throw new UnauthorizedAccessException();
                }
            }
        }
    }
}
