using WebhookRelayService.Repositories;

namespace WebhookRelayService.Services
{
    public interface IWebhookService
    {
        public Task HandleWebhook();
    }

    public class WebhookService : IWebhookService
    {
        private IWebhookUserRepository _webhookUserRepository;

        public WebhookService(IWebhookUserRepository webhookUserRepository)
        {
            _webhookUserRepository = webhookUserRepository;
        }

        public async Task HandleWebhook()
        {
            var user = await _webhookUserRepository.GetByWebhookId(1);
        }
    }
}
