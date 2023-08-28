using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sentry;
using WebhookRelayService.Models;
using WebhookRelayService.Services;

namespace WebhookRelayService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookUserController : ControllerBase
    {
        private IWebhookUserService _webhookUserService;

        public WebhookUserController(IWebhookUserService webhookUserService)
        {
            _webhookUserService = webhookUserService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WebhookUserCreateDTO dto)
        {
            try
            {
                var id = await _webhookUserService.Create(dto);
                return StatusCode(201, id);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return StatusCode(500);
            }
        }
    }
}
