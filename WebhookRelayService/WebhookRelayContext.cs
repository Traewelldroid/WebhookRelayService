using Microsoft.EntityFrameworkCore;
using WebhookRelayService.Models;

namespace WebhookRelayService
{
    public class WebhookRelayContext : DbContext
    {
        public WebhookRelayContext(DbContextOptions options) : base(options) { }

        public DbSet<WebhookUser> WebhookUsers { get; set; }
    }
}
