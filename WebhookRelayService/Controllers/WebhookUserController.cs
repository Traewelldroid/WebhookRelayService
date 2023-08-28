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
        private IWebhookUserService _service;

        public WebhookUserController(IWebhookUserService webhookUserService)
        {
            _service = webhookUserService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WebhookUserCreateDTO dto)
        {
            try
            {
                var id = await _service.Create(dto);
                return StatusCode(201, id);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return StatusCode(500);
            }
        }

        [HttpDelete, Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute(Name = "id")] Guid id)
        {
            try
            {
                await _service.Delete(id);
                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}
