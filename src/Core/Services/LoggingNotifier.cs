using Core.Model;
using Microsoft.Extensions.Logging;

namespace Core.Services
{
    public class LoggingNotifier : INotifier 
    {
        private readonly ILogger<LoggingNotifier> _logger;

        public LoggingNotifier(ILogger<LoggingNotifier> logger)
        {
            _logger = logger;
        }

        public void SendAssignedNotification(string message, Employee employee)
        {
            _logger.LogInformation(message);
        }

        public void SendChangeStateNotification(string message)
        {
            _logger.LogInformation(message);
        }
    }
}