namespace WebhookRelayService.Models
{
    public class Settings
    {
        public string SentryDsn { get; set; }
        public string PostgresConnection { get; set; }
        public bool Logging { get; set; }
    }
}
