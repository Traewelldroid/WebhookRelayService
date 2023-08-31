using WebhookRelayService.Repositories;
using WebhookRelayService.Services;

namespace WebhookRelayService.BackgroundServices
{
    public class ClearWebhookUsers : BackgroundService
    {
        private IServiceProvider _serviceProvider;
        private ILogger _logger;

        public ClearWebhookUsers(IServiceProvider serviceProvider, ILogger<ClearWebhookUsers> logger) 
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Clearing webhook users...");

                using var scope = _serviceProvider.CreateScope();
                var webhookUserRepository = scope.ServiceProvider.GetRequiredService<IWebhookUserRepository>();
                var webhookService = scope.ServiceProvider.GetRequiredService<IWebhookService>();

                var users = await webhookUserRepository.GetAll();
                var removedUsers = 0;
                foreach (var user in users)
                {
                    var result = await webhookService.PushNotificationAndHandleResult(user, "");
                    removedUsers += result;
                }
                _logger.LogInformation($"{removedUsers} of {users.Count} users were removed due to broken endpoints.");

                scope.Dispose();

                await Task.Delay(TimeSpan.FromHours(12), stoppingToken);
            }
        }
    }
}
