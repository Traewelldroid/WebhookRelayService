using System.Text.Json;

namespace WebhookRelayService.Models
{
    public class Webhook
    {
        public string Event { get; set; } = string.Empty;
        public JsonElement Notification { get; set; }

        public string GetNotificationJson() => Notification.ToString();
    }
}
