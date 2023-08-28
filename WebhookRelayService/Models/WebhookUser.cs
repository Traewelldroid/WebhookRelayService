namespace WebhookRelayService.Models
{
    public class WebhookUser
    {
        public Guid Id { get; set; }
        public int WebhookId { get; set; }
        public string WebhookSecret { get; set; } = "";
        public string NotificationEndpoint { get; set; } = ""; 
    }

    public class WebhookUserCreateDTO
    {
        public int WebhookId { get; set; }
        public string WebhookSecret { get; set; } = "";
        public string NotificationEndpoint { get; set; } = "";
    }
}
