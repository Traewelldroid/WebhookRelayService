using Microsoft.AspNetCore.Mvc;
using System.Text;

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
        public async Task<IActionResult> Webhook()
        {
            string rawContent = string.Empty;
            using var reader = new StreamReader(Request.Body, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false);
            rawContent = await reader.ReadToEndAsync();
            _logger.LogInformation(rawContent);
            return Ok("Ok.");
        }
    }
}
