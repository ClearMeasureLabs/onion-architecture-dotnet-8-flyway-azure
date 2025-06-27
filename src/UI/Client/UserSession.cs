using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;
using Core.Model;
using Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using UI.Shared.Authentication;

namespace UI.Services
{
    public class UserSession : IUserSession
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly CustomAuthenticationStateProvider _authProvider;
        private readonly NavigationManager _navigationManager;
        private readonly Stack<FlashMessage?> _flashMessages = new();

        public UserSession(IEmployeeRepository employeeRepository, CustomAuthenticationStateProvider authProvider, NavigationManager navigationManager)
        {
            _employeeRepository = employeeRepository;
            _authProvider = authProvider;
            _navigationManager = navigationManager;
        }

        // IUserSession Members
        public async Task<Employee?> GetCurrentUserAsync()
        {
            var username = _authProvider.GetUsername();
            if (string.IsNullOrEmpty(username))
                return null;
            var currentUser = await _employeeRepository.GetByUserNameAsync(username);
            blowUpIfEmployeeCannotLogin(currentUser);
            return currentUser;
        }

        public void LogIn(Employee employee)
        {
            blowUpIfEmployeeCannotLogin(employee);
            _authProvider.Login(employee.UserName);
            _navigationManager.NavigateTo("/");
        }

        public void LogOut()
        {
            _authProvider.Logout();
            _navigationManager.NavigateTo("/login");
        }

        public void PushUserMessage(FlashMessage? message)
        {
            if (message != null)
                _flashMessages.Push(message);
        }

        public FlashMessage? PopUserMessage()
        {
            if (_flashMessages.Count == 0)
                return null;
            return _flashMessages.Pop();
        }

        private void blowUpIfEmployeeCannotLogin(Employee? employee)
        {
            if (employee == null)
            {
                throw new Exception("That user doesn't exist or is not valid.");
            }
        }
    }
}