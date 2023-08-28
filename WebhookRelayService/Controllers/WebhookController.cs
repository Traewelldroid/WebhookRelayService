using Microsoft.AspNetCore.Mvc;

namespace WebhookRelayService.Controllers
{
    [ApiController]
    [Route("webhook")]
    public class WebhookController : ControllerBase
    {
        public async Task<IActionResult> Webhook()
        {
            return Ok();
        }
    }
}
