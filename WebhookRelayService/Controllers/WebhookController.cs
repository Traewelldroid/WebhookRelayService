using Microsoft.AspNetCore.Mvc;

namespace WebhookRelayService.Controllers
{
    [ApiController]
    [Route("webhook")]
    public class WebhookController : ControllerBase
    {
        private ILogger _logger;

        public WebhookController(ILogger<WebhookController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Webhook([FromBody] string body)
        {
            _logger.LogInformation(body);
            return Ok("Ok.");
        }
    }
}
