namespace WebhookRelayService.Models
{
    public class Settings
    {
        public string SentryDsn { get; } = "";
        public string PostgresConnection { get; } = "";
    }
}
