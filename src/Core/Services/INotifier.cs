using Core.Model;

namespace Core.Services
{
    public interface INotifier
    {
        void SendAssignedNotification(string message, Employee employee);
        void SendChangeStateNotification(string message);

    }
}