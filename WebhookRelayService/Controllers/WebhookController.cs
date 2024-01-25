using Microsoft.AspNetCore.Mvc;
using Sentry;
using System.Text;
using System.Text.Json;
using WebhookRelayService.BackgroundServices;
using WebhookRelayService.Models;
using WebhookRelayService.Services;

namespace WebhookRelayService.Controllers
{
    [ApiController]
    [Route("webhook")]
    public class WebhookController : ControllerBase
    {
        private Queue _queue;
        private ILogger _logger;
        private Settings _settings;

        public WebhookController(ILogger<WebhookController> logger, Settings settings, Queue queue)
        {
            _logger = logger;
            _settings = settings;
            _queue = queue;
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

                await _queue.Enqueue(new WebhookRequest
                {
                    WebhookId = webhookId,
                    Webhook = webhook,
                    Payload = requestBody,
                    Signature = signature
                });
            } catch (Exception ex)
            {
                _logger.LogError("Webhook error", ex);
                SentrySdk.CaptureException(ex);
                return StatusCode(500);
            }
            return Ok();
        }

        private async Task<string> GetRequestBody()
        {
            using var streamReader = new StreamReader(Request.Body, Encoding.UTF8);
            return await streamReader.ReadToEndAsync();
        }
    }
}
