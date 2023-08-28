using Microsoft.EntityFrameworkCore;
using WebhookRelayService.Models;

namespace WebhookRelayService.Repositories
{
    public interface IWebhookUserRepository
    {
        public Task<WebhookUser> Create(WebhookUser webhookUser);
        public Task<WebhookUser> GetById(Guid id);
        public Task<WebhookUser> GetByWebhookId(int id);
    }

    public class WebhookUserRepository : IWebhookUserRepository
    {
        private readonly WebhookRelayContext _context;

        public WebhookUserRepository(WebhookRelayContext context)
        {
            _context = context;
        }

        public async Task<WebhookUser> Create(WebhookUser webhookUser)
        {
            _context.WebhookUsers.Add(webhookUser);
            await _context.SaveChangesAsync();
            return webhookUser;
        }

        public async Task<WebhookUser> GetById(Guid id)
        {
            var user = await _context.WebhookUsers.FirstAsync(u => u.Id == id);
            return user;
        }

        public async Task<WebhookUser> GetByWebhookId(int id)
        {
            var user = await _context.WebhookUsers.FirstAsync(u => u.WebhookId == id);
            return user;
        }
    }
}
