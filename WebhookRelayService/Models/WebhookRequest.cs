namespace WebhookRelayService.Models
{
    public class WebhookRequest
    {
        public int WebhookId { get; set; }
        public Webhook Webhook { get; set; }
        public string Payload { get; set; }
        public string Signature { get; set; }
    }
}
