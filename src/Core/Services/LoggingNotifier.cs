using Core.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProgrammingWithPalermo.ChurchBulletin.Core.Model;

namespace Core.Services
{
    public class LoggingNotifier : INotifier 
    {
        private readonly ILogger<LoggingNotifier> _logger;

        public LoggingNotifier()
        {
            _logger = new NullLogger<LoggingNotifier>();
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