using Sentry;
using System.Collections.Concurrent;
using WebhookRelayService.Models;
using WebhookRelayService.Services;

namespace WebhookRelayService.BackgroundServices
{
    public class Queue : BackgroundService
    {
        private ILogger _logger;
        private IWebhookService _webhookService;
        private ConcurrentQueue<WebhookRequest> _queue;

        public Queue(
            ILogger<Queue> logger,
            IWebhookService webhookService
        )
        {
            _logger = logger;
            _webhookService = webhookService;
            _queue = new ConcurrentQueue<WebhookRequest>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_queue.Count > 0)
                {
                    while (_queue.Count != 0)
                    {
                        try
                        {
                            WebhookRequest? request;
                            if (_queue.TryDequeue(out request))
                            {
                                await _webhookService.HandleWebhook(request);
                            }
                        } 
                        catch (Exception ex)
                        {
                            _logger.LogError("Webhook error", ex);
                            SentrySdk.CaptureException(ex);
                        }
                    }
                }
            }
        }

        public async Task Enqueue(WebhookRequest request)
        {
            await Task.Run(() =>
            {
                _queue.Enqueue(request);
            });
        }
    }
}
