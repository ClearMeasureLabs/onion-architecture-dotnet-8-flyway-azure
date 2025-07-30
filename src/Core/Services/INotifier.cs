using Core.Model;
using ProgrammingWithPalermo.ChurchBulletin.Core.Model;

namespace Core.Services
{
    public interface INotifier
    {
        void SendAssignedNotification(string message, Employee employee);
        void SendChangeStateNotification(string message);

    }
}