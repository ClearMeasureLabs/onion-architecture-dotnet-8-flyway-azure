using Core.Model;
using System.Threading.Tasks;

namespace Core.Services
{
	public interface IUserSession
	{
		Task<Employee?> GetCurrentUserAsync();
		void LogIn(Employee employee);
		void LogOut();
		void PushUserMessage(FlashMessage? message);
		FlashMessage? PopUserMessage();
	}
}