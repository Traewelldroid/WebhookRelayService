using Microsoft.AspNetCore.Mvc;
using Sentry;
using System.Text;
using System.Text.Json;
using WebhookRelayService.Models;
using WebhookRelayService.Services;

namespace WebhookRelayService.Controllers
{
    [ApiController]
    [Route("webhook")]
    public class WebhookController : ControllerBase
    {
        private ILogger _logger;
        private IWebhookService _webhookService;
        private Settings _settings;

        public WebhookController(ILogger<WebhookController> logger, IWebhookService webhookService, Settings settings)
        {
            _logger = logger;
            _webhookService = webhookService;
            _settings = settings;
        }

        [HttpPost]
        public async Task<IActionResult> Webhook()
        {
            try
            {
                var requestBody = await GetRequestBody();
                var webhookId = int.Parse(Request.Headers["X-Trwl-Webhook-Id"].ToString() ?? "-1");
                var signature = Request.Headers["Signature"].ToString();

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                };
                var webhook = JsonSerializer.Deserialize<Webhook>(requestBody, options);

                if (webhook == null)
                {
                    if (_settings.Logging)
                    {
                        _logger.LogError($"Webhook failed! {webhookId} {requestBody}");
                    }
                    throw new InvalidDataException("Invalid Webhook");
                }

                await _webhookService.HandleWebhook(webhookId, webhook, requestBody, signature);

                return Ok();
            } catch (Exception ex)
            {
                _logger.LogError("Webhook error", ex);
                SentrySdk.CaptureException(ex);
                return StatusCode(500);
            }
        }

        private async Task<string> GetRequestBody()
        {
            using var streamReader = new StreamReader(Request.Body, Encoding.UTF8);
            return await streamReader.ReadToEndAsync();
        }
    }
}
