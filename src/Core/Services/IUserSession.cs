using Core.Model;

namespace Core.Services
{
	public interface IUserSession
	{
		Employee? GetCurrentUser();
		void LogIn(Employee employee);
		void LogOut();
		void PushUserMessage(FlashMessage? message);
		FlashMessage? PopUserMessage();
	}
}