using Microsoft.AspNetCore.Mvc;
using WebhookRelayService.Services;

namespace WebhookRelayService.Controllers
{
    [Route("api/[controller]")]
    public class HealthCheckController : ControllerBase
    {
        private IWebhookUserService _webhookUserService;

        public HealthCheckController(IWebhookUserService webhookUserService)
        {
            _webhookUserService = webhookUserService;
        }

        [HttpGet]
        public async Task<int> HealthCheck()
        {
            return await _webhookUserService.GetRegisteredUsersCount();
        }
    }
}
