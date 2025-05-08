using Microsoft.Extensions.Logging;
using TodoManagment.Core.ServiceContract;

namespace TodoManagment.Core.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(string subject, string message)
        {
            _logger.LogInformation("Notification - Subject: {Subject}, Message: {Message}",
                subject, message);

            return Task.CompletedTask;
        }
    }
}
