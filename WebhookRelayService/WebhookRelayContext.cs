using Microsoft.EntityFrameworkCore;
using WebhookRelayService.Models;

namespace WebhookRelayService
{
    public class WebhookRelayContext : DbContext
    {

        public DbSet<WebhookUser> WebhookUsers { get; set; }
    }
}
